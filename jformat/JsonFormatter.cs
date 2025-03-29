
using System.Data;
using System.Text;

using jformat.extensions;

namespace jformat;

public static class JsonFormatter
{
    /// <summary>
    /// Check if input string is valid json
    /// </summary>
    /// <param name="input">JSON with root as an object or an array</param>
    /// <returns>True if valid, false if invalid</returns>
    public static bool IsValidJson(string input)
    {
        input = RemoveWhitespace(input);
        return IsValidObject(input) || IsValidArray(input);
    }

    private enum TokenType
    {
        Key,
        Value,
        OpenBracket,
        ClosingBracket,
        Comma,
        Colon,
        // more?
    }

    public static string RemoveWhitespace(string input)
    {
        int quotecount = 0;
        var outstr = new StringBuilder();
        char prev = input.Length > 0 ? input[0] : '\0';
        List<char> ws = [' ', '\n', '\t', '\r'];
        foreach (var c in input)
        {
            if (c == '"' && prev != '\\') { quotecount++; }
            if (!ws.Contains(c) || quotecount % 2 != 0)
            {
                outstr.Append(c);
            }
            prev = c;
        }
        return outstr.ToString();
    }

    /// <summary>
    /// Note: assumes valid json is passed in.
    /// Will split the input into strings where each represents a "token"
    /// </summary>
    /// <param name="input">Valid JSON string</param>
    /// <returns>List of <c>string</c> tokens</returns>
    /// <exception cref="ArgumentException">Thrown when there are uneven brackets or duplicate keys in the provided json</exception>
    public static List<string> TokenizeJsonObj(string input)
    {
        input = RemoveWhitespace(input);
        List<string> strings = [];
        StringBuilder temptoken = new();
        TokenType? next = TokenType.OpenBracket; // next doesn't always have meaning and cannot be relied on
        int arrays_deep = 0;
        int objs_deep = 0;
        char last = input[0];
        bool in_string = false;
        Dictionary<int, List<string>> object_keys = [];

        foreach (char ch in input)
        {
            bool end_token_due_to_close_bracket = false;
            bool end_token_and_keep_char = false;
            if (ch == '"')
            {
                if (in_string && last != '\\')
                {
                    in_string = false;
                    end_token_and_keep_char = true;
                }
                else if (!in_string)
                {
                    in_string = true;
                }
            }

            if (!in_string)
            {
                switch (ch)
                {
                    case ',':
                        if (arrays_deep == 0)
                        {
                            next = TokenType.Key;
                            end_token_due_to_close_bracket = true;
                        }
                        break;
                    case ':':
                        next = TokenType.Value;
                        end_token_due_to_close_bracket = true;
                        break;
                    case '{':
                        objs_deep++;
                        if (!object_keys.ContainsKey(objs_deep))
                        {
                            // make sure list exists
                            object_keys[objs_deep] = [];
                        }
                        end_token_due_to_close_bracket = true;
                        next = TokenType.Key;
                        break;
                    case '}':
                        objs_deep--;
                        end_token_due_to_close_bracket = true;
                        next = TokenType.Comma;
                        break;
                    case '[':
                        arrays_deep++;
                        next = TokenType.Value;
                        break;
                    case ']':
                        arrays_deep--;
                        next = TokenType.ClosingBracket;
                        if (arrays_deep == 0)
                        {
                            temptoken.Append(ch);
                            strings.Add(temptoken.ToString());
                            temptoken.Clear();
                        }
                        break;
                    default:
                        break;
                }
            }
            if (arrays_deep > 0)
            {
                temptoken.Append(ch);
                continue;
            }
            if (end_token_due_to_close_bracket)
            {
                if (temptoken.Length > 0)
                {
                    strings.Add(temptoken.ToString());
                    temptoken.Clear();
                }
                strings.Add(ch.ToString());
            }
            else if (end_token_and_keep_char)
            {
                temptoken.Append(ch);
                strings.Add(temptoken.ToString());
                if (next is TokenType.Key) // add key to list for duplicate checking
                {
                    object_keys[objs_deep].Add(temptoken.ToString());
                }
                temptoken.Clear();
            }
            else if (next is TokenType.Key or TokenType.Value)
            {
                temptoken.Append(ch);
            }

            last = ch;
        }

        if (objs_deep != 0 || arrays_deep != 0)
        {
            throw new ArgumentException("Tokenizer was passed invalid json with unmatched brackets");
        }
        foreach (List<string> keys in object_keys.Values)
        {
            if (keys.Count != keys.Distinct().Count())
            {
                throw new ArgumentException("Duplicate keys in provided json");
            }
        }

        return strings;
    }

    private static bool IsValidToken(string token, TokenType tokenType)
    {
        return tokenType switch
        {
            TokenType.Key => IsValidString(token),
            TokenType.Value => IsValidValue(token),
            TokenType.OpenBracket => token == "{",
            TokenType.ClosingBracket => token == "}",
            TokenType.Comma => token == ",",
            TokenType.Colon => token == ":",
            _ => throw new NotImplementedException($"Don't know what that is: tokenType \"{tokenType}\" token: \"{token}\""),
        };
    }

    /// <summary>
    /// Check if the input is a valid JSON object, i.e. not an array or value
    /// </summary>
    /// <param name="input">String representing a JSON object, inside { curly braces }</param>
    /// <returns>True if valid, false if invalid</returns>
    public static bool IsValidObject(string input)
    {
        input = RemoveWhitespace(input);
        if (input[0] != '{' || input[^1] != '}')
        {
            return false;
        }

        var tokens = new List<string>();
        try
        {
            tokens = TokenizeJsonObj(input);
        }
        catch (ArgumentException)
        {
            return false;
        }
        int bracketsDeep = 0;
        List<TokenType> validNextTypes = [TokenType.OpenBracket, TokenType.Key];
        foreach (string token in tokens)
        {
            TokenType? type = null;
            foreach (TokenType tt in validNextTypes)
            {
                if (IsValidToken(token, tt)) { type = tt; break; }
            }
            if (type == TokenType.OpenBracket) { bracketsDeep++; }
            if (type == TokenType.ClosingBracket) { bracketsDeep--; }
            if (type is null)
            {
                return false;
            }
            validNextTypes = type switch
            {
                TokenType.OpenBracket => [TokenType.ClosingBracket, TokenType.Key],
                TokenType.ClosingBracket => [TokenType.Comma, TokenType.ClosingBracket],
                TokenType.Key => [TokenType.Colon],
                TokenType.Colon => [TokenType.OpenBracket, TokenType.Value],
                TokenType.Value => [TokenType.Comma, TokenType.ClosingBracket],
                TokenType.Comma => [TokenType.Key],
                _ => throw new NotImplementedException(),
            };
        }
        return bracketsDeep == 0;
    }

    public static bool HasValidBrackets(string input)
    {
        Dictionary<char, char> charPairTypes = new()
        {
            { ')', '(' },
            { ']', '[' },
            { '}', '{' },
            { '>', '<' },
        };
        var currently_open_pairs = new List<char>();
        foreach (char c in input)
        {
            // opening
            if (charPairTypes.ContainsValue(c)) { currently_open_pairs.Add(c); }
            // closing bracket
            else if (charPairTypes.ContainsKey(c))
            {
                // if no open pairs
                if (currently_open_pairs.Count == 0) { return false; }
                if (currently_open_pairs.Last() == charPairTypes[c]) { currently_open_pairs.RemoveAt(currently_open_pairs.Count - 1); }
                else { return false; }
            }
        }
        return currently_open_pairs.Count == 0;
    }

    public static bool IsValidValue(string input)
    {
        return input.Length == 0
            ? false
            : new List<string> { "true", "false", "null" }.Contains(input)
            ? true
            : IsValidString(input) ? true : IsValidNumber(input) ? true : IsValidArray(input) ? true : IsValidObject(input);
    }

    public static bool IsValidString(string input)
    {
        char quote = '"';
        if (input.Length < 2 || input[0] != quote || input.Last() != quote)
        {
            return false;
        }
        input = input[1..^1]; // chop quotes
        //  && IsValidQuotes(input); // not checking for quotes anymore? think this was useless

        bool escaping = false;
        bool readingHexEscape = false;
        StringBuilder hextoken = new();
        foreach (char ch in input)
        {
            // these control characters are not allowed
            if ((int)ch is < 32 or >= 127)
            {
                return false;
            }
            // these characters must be escaped
            if (ch == quote && !escaping)
            {
                return false;
            }
            // check for /uXXXX type escapes
            if (readingHexEscape)
            {
                if (hextoken.Length == 5) // u and 4 chars
                {
                    if (!IsValidEscape(hextoken.ToString()))
                    {
                        return false;
                    }
                    // reset allat
                    hextoken.Clear();
                    readingHexEscape = false;
                    escaping = false;
                    break;
                }
                else
                {
                    hextoken.Append(ch);
                    continue;
                }
            }

            if (escaping)
            {
                if (ch == 'u') { hextoken.Append(ch); readingHexEscape = true; continue; } // need more characters if it's a hex code thing
                if (!IsValidEscape(ch))
                {
                    return false;
                }
                escaping = false;
                continue;
            }
            if (ch == '\\')
            {
                escaping = true;
            }
        }

        // escaping should be true here if the last character is a backslash
        return escaping == false && hextoken.Length == 0;
    }

    /// <summary>
    /// Pass in a character to see if it is "escapable"
    /// </summary>
    /// <param name="ch"></param>
    public static bool IsValidEscape(char ch)
    {
        var options = new List<char> { '/', '\\', 'b', 'f', 'n', 'r', 't', '"' };
        return options.Contains(ch);
    }

    public static bool IsValidEscape(string str)
    {
        if (str.Length == 0) { return false; }
        if (str.Length == 1 && IsValidEscape(str[0]))
        {
            return true;
        }
        if (str.Length == 5 && str[0] == 'u')
        {
            if (IsValidHexDigits(str[1..]))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Check if the input string represents a valid decimal number in JSON, e.g.:
    /// 7 is valid;
    /// -85.3 is valid;
    /// 1.23e5 is valid;
    /// -13232.1231E4321 is valid.
    /// </summary>
    /// <param name="input">String representing a number, without any whitespace</param>
    public static bool IsValidNumber(string input)
    {
        var signs = new List<char> { '+', '-' };
        Func<string, bool> validOnlyDigits = (string x) => x.All(char.IsDigit) && x.Length > 0;
        Func<string, string> strWithoutSign = (string x) =>
        {
            if (x.Length == 0) { return x; }
            bool hasSign = signs.Contains(x[0]);
            return hasSign ? x[1..] : x;
        };
        Func<string, bool> validWithSign = (string x) => validOnlyDigits(strWithoutSign(x));
        Func<string, bool> validWithNegOnly = (string x) => x.Length > 0 && validOnlyDigits(x[0] == '-' ? x[1..] : x);

        var split_exponent = input.ToLower().Split('e');
        if (split_exponent.Length > 2) { return false; } // multiple exponents
        if (split_exponent.Length > 1)
        {
            string exponent = split_exponent[1];
            if (!validWithSign(exponent)) { return false; }
        }
        var split_decimal = split_exponent[0].Split('.');
        return split_decimal.Length is not > 2 and not < 1 && (split_decimal.Length == 1
            ? validWithNegOnly(split_decimal[0])
            : split_decimal.Length == 2
            ? validWithNegOnly(split_decimal[0]) && validOnlyDigits(split_decimal[1])
            : throw new Exception("this wasn't supposed to happen"));
    }

    public static List<string> TokenizeArray(string input)
    {
        input = RemoveWhitespace(input);
        List<string> items = [];
        int openBracketCount = 0;
        List<char> openBrackets = ['{', '[', '('];
        List<char> closeBrackets = ['}', ']', ')'];
        StringBuilder temp = new();
        foreach (char ch in input)
        {
            if (openBrackets.Contains(ch))
            {
                openBracketCount++;
                if (openBracketCount == 1) { continue; }
            }
            else if (closeBrackets.Contains(ch))
            {
                openBracketCount--;
                if (openBracketCount == 0) { continue; }
            }

            if (openBracketCount == 1)
            {
                if (ch == ',')
                {
                    items.Add(temp.ToString());
                    temp.Clear();
                }
                else
                {
                    temp.Append(ch);
                }
            }
            else
            {
                temp.Append(ch);
            }
        }
        if (items.Count > 0)
        {
            items.Add(temp.ToString());
        }
        return items;
    }

    public static bool IsValidArray(string input)
    {
        input = input.Trim();
        if (input.Length < 2) { return false; }
        if (input[0] != '[' || input[^1] != ']')
        {
            return false;
        }
        List<string> tokens = TokenizeArray(input);

        foreach (string item in tokens)
        {
            if (!IsValidValue(item))
            {
                return false;
            }
        }
        return true;
    }

    public static string FormatJsonString(string input)
    {
        if (!IsValidObject(input)) { throw new ArgumentException($"Invalid JSON provided to FormatJsonString: {input}"); }
        var no_whitespace = RemoveWhitespace(input);
        int tab_depth = 0;
        var str = new StringBuilder();
        var get_tabs = () => new string('\t', tab_depth);

        for (int i = 0; i < no_whitespace.Length; i++)
        {
            char c = no_whitespace[i];
            if (str.Length > 0 && str.ToString()[^1] == '\n')
            {
                str.Append(get_tabs()); // add tabs
            }
            switch (c)
            {
                case ',':
                    str.Append(c);
                    str.Append(Environment.NewLine);
                    break;
                case ':':
                    str.Append(c);
                    str.Append(' ');
                    break;
                case '{' or '[':
                    tab_depth += 1;
                    str.Append(c);
                    str.Append(Environment.NewLine);
                    break;
                case '}' or ']':
                    tab_depth -= 1;
                    str.Append(Environment.NewLine);
                    str.Append(get_tabs());
                    str.Append(c);
                    //str.Append(Environment.NewLine);
                    break;
                default:
                    str.Append(c);
                    break;
            }
        }

        return str.ToString();
    }

    /// <summary>
    /// Checks if every digit in the string is a letter A-Z/a-z or a digit 0-9
    /// </summary>
    /// <param name="str">The string to be checked</param>
    /// <returns>A boolean <c>true</c> if valid, <c>false</c> if invalid</returns>
    public static bool IsValidHexDigits(string str)
    {
        char[] chars = "0123456789abcdef".ToCharArray();
        foreach (char ch in str)
        {
            if (!chars.Contains(ch.ToLower()))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Format file in place
    /// </summary>
    /// <param name="path">Relative path of the .json file</param>
    public static void FormatByFilepath(string path, bool overrwrite = true)
    {
        StreamReader sr = new(path);
        string text = sr.ReadToEnd();
        if (!overrwrite)
        {
            path = path.RemovedSuffix(".json") + "-formatted.json";
        }
        try
        {
            string formatted = FormatJsonString(text);
            sr.Close();
            Console.WriteLine($"writing to path: {path}");
            File.WriteAllText(path, formatted);
        }
        catch (ArgumentException)
        {
            Console.WriteLine($"Couldn't format {path} invalid JSON");
        }
    }
}
