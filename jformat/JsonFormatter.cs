
using System.Text;

namespace JFormat;

public static class JsonFormatter
{
    public static bool IsValidJson(string input)
    {
        input = RemoveWhitespace(input);
        return IsValidJsonObj(input) || IsValidArray(input);
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

    public static List<string> TokenizeJsonObj(string input)
    {
        input = RemoveWhitespace(input);
        List<string> strings = [];
        string temptoken = "";
        TokenType? next = TokenType.Key;
        int arrays_deep = 0;

        foreach (char ch in input)
        {
            bool matched = false;
            switch (ch)
            {
                case ',':
                    if (arrays_deep == 0) // ?
                    {

                        next = TokenType.Key;
                    }
                    matched = true;
                    break;
                case ':':
                    next = TokenType.Value; // ?
                    matched = true;
                    break;
                case '{' or '}': // TODO: does it break if a closing bracket is first?
                    matched = true;
                    next = TokenType.Key;
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
                        temptoken += ch;
                        strings.Add(temptoken);
                        temptoken = "";
                    }
                    break;
            }
            if (arrays_deep > 0)
            {
                temptoken += ch;
                continue;
            }
            if (matched)
            {
                if (temptoken.Length > 0)
                {
                    strings.Add(temptoken);
                    temptoken = "";
                }
                strings.Add(ch.ToString());
            }
            else
            {
                if (next is TokenType.Key or TokenType.Value)
                {
                    temptoken += ch;
                }
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

    public static bool IsValidJsonObj(string input)
    {
        if (input[0] != '{' || input.Last() != '}')
        {
            return false;
        }

        var tokens = TokenizeJsonObj(input);
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
                TokenType.OpenBracket => [TokenType.Key, TokenType.ClosingBracket],
                TokenType.ClosingBracket => [TokenType.Comma, TokenType.ClosingBracket],
                TokenType.Key => [TokenType.Colon],
                TokenType.Colon => [TokenType.Value, TokenType.OpenBracket],
                TokenType.Value => [TokenType.Comma, TokenType.ClosingBracket],
                TokenType.Comma => [TokenType.Key],
                _ => throw new NotImplementedException(),
            };
        }
        return bracketsDeep == 0;
    }

    public static bool IsValidBrackets(string input)
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
            : IsValidString(input) ? true : IsValidNumber(input) ? true : IsValidArray(input) ? true : IsValidJsonObj(input);
    }

    public static bool IsValidString(string input)
    {
        if (input.Length < 2)
        {
            return false;
        }

        char quote = '"';
        if (input[0] != quote || input.Last() != quote)
        {
            return false;
        }
        input = input[1..^1]; // chop quotes
        //  && IsValidQuotes(input); // not checking for quotes anymore? think this was useless

        bool escaping = false;
        bool hexing = true;
        string hextoken = "";
        foreach (char ch in input)
        {
            if (hexing)
            {
                if (hextoken.Length == 5) // u and 5 chars
                {
                    if (!IsValidEscape(hextoken)) { return false; }
                    // reset allat
                    hextoken = "";
                    hexing = false;
                    escaping = false;
                    break;
                }
                else
                {
                    hextoken.Append(ch);
                }
            }
            if (escaping)
            {
                if (ch == 'u') { hextoken += ch; hexing = true; continue; } // need more characters if it's a hex code thing
                if (!IsValidEscape(ch)) { return false; }
                escaping = false;
                continue;
            }
            if (ch == '\\')
            {
                escaping = true;
            }
        }

        return escaping == false && hextoken.Length == 0;
    }

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

    //public static bool IsValidQuotes(string input)
    //{
    //    int quoteCount = 0;
    //    foreach (char c in input)
    //    {
    //        if (c == '"')
    //        {
    //            quoteCount++;
    //        }
    //    }
    //    return quoteCount % 2 == 0;
    //}

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
        Func<string, bool> validWithSign = (string x) =>
        {
            return validOnlyDigits(strWithoutSign(x));
        };
        Func<string, bool> validWithNegOnly = (string x) =>
        {
            return x.Length > 0 && validOnlyDigits(x[0] == '-' ? x[1..] : x);
        };

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
        if (!IsValidJsonObj(input)) { throw new ArgumentException($"Invalid JSON provided to FormatJsonString: {input}"); }
        var no_whitespace = RemoveWhitespace(input);
        int tab_depth = 0;
        var str = new StringBuilder();

        for (int i = 0; i < no_whitespace.Length; i++)
        {
            char c = no_whitespace[i];
            str.Append(new string('\t', tab_depth)); // add tabs
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
                case '{':
                    tab_depth += 1;
                    str.Append(c);
                    str.Append(Environment.NewLine);
                    break;
                case '}':
                    tab_depth -= 1;
                    str.Append(Environment.NewLine);
                    str.Append(c);
                    str.Append(Environment.NewLine);
                    break;
                default:
                    str.Append(c);
                    break;
            }
        }

        return str.ToString();
    }

    /// <summary>
    /// Checks if every digit in the string is
    /// </summary>
    /// <param name="str">The string to be checked</param>
    /// <returns>A boolean `true` if valid, if invalid</returns>
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

    public static void FormatByFilepath(string path)
    {
        StreamReader sr = new(path);
        string text = sr.ReadToEnd();
        string formatted = FormatJsonString(text);
        sr.Close();
        File.WriteAllText(path, formatted);
    }
}
