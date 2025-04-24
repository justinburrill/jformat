using System.Text;

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

    /// <summary>
    /// Check if the input represents any valid JSON value, be it a string, number, array, object, or true/false/null.
    /// </summary>
    /// <param name="input">String representing JSON value. If it is supposed to be a JSON string, it should be surrounded in quotes.</param>
    /// <returns>True if valid, false if invalid</returns>
    public static bool IsValidValue(string input) => input.Length != 0 && (new List<string> { "true", "false", "null" }.Contains(input) || IsValidString(input) || IsValidNumber(input) || IsValidArray(input) || IsValidObject(input));

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
        foreach (char c in input)
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
        TokenType? next = TokenType.OpenBracket; // note: next doesn't always have meaning and cannot be relied on
        int arrays_deep = 0;
        int objs_deep = 0;
        char last = input[0];
        bool in_string = false;
        Dictionary<int, List<string>> object_keys = [];
        bool escaping = false;

        foreach (char ch in input)
        {
            bool this_char_is_escaped = escaping;
            bool end_token_and_end_new_token = false;
            bool end_token_and_keep_char = false;
            if (in_string && ch == '\\')
            {
                escaping = !escaping; // this should be toggled, not set to false
                // if we have a \\ then one is escaped and the next char afterwards is not
            }
            if (ch == '"' && arrays_deep == 0)
            {
                if (in_string && !escaping)
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
                            end_token_and_end_new_token = true;
                        }
                        break;
                    case ':':
                        next = TokenType.Value;
                        end_token_and_end_new_token = true;
                        break;
                    case '{':
                        objs_deep++;
                        if (!object_keys.ContainsKey(objs_deep))
                        {
                            // make sure list exists
                            object_keys[objs_deep] = [];
                        }
                        end_token_and_end_new_token = true;
                        next = TokenType.Key;
                        break;
                    case '}':
                        object_keys.Remove(objs_deep, out List<string>? keys_for_that_level);
                        // check for duplicate keys
                        if (keys_for_that_level is not null && keys_for_that_level.HasDuplicates(out IEnumerable<string> dupes))

                        {
                            throw new ArgumentException($"Duplicate keys in provided json: {dupes}");
                        }
                        objs_deep--;
                        end_token_and_end_new_token = true;
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
            if (end_token_and_end_new_token)
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
            if (this_char_is_escaped) { escaping = false; }
        }
        // check for unclosed brackets
        return objs_deep != 0 || arrays_deep != 0
            ? throw new ArgumentException("Tokenizer was passed invalid json with unmatched brackets.") // bad json!!
            : strings; // success!
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
    public static bool IsValidObject(string input, out string? error)
    {

        if (input.Length == 0)
        {
            error = "Empty input.";
            return false;
        }

        input = RemoveWhitespace(input);
        if (input[0] != '{' || input[^1] != '}')
        {
            error = "Input does not have surrounding { curly brackets }.";
            return false;
        }

        List<string> tokens;
        try { tokens = TokenizeJsonObj(input); }
        catch (ArgumentException err)
        {
            error = err.Message;
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
                error = $"Unrecognized or invalid token \"{token}\"";
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
        if (bracketsDeep == 0)
        {
            error = null;
            return true;
        }
        else
        {
            error = "Unmatched brackets.";
            return false;
        }
    }

    public static bool IsValidObject(string input) => IsValidObject(input, out _);

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
            // the following ranges of characters are not allowed:
            // u0000 to u001f, as well as u007f, u0080 to u009f, u00ad, u0600 to u0605, u200b, u200b to u200d, u2028 to u2029, u2060, and ufeff

            if ((int)ch is < 32 or 127 or (>= 0x80 and <= 0x9f))
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
                if (!IsValidEscapeChar(ch))
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
    public static bool IsValidEscapeChar(char ch)
    {
        var options = new List<char> { '/', '\\', 'b', 'f', 'n', 'r', 't', '"' };
        return options.Contains(ch);
    }

    /// <summary>
    /// Check if a string is valid to be escaped, that is, to be placed after a backslash
    /// </summary>
    /// <param name="str">Something like \ or " or u1234 </param>
    /// <returns><c>true</c> if valid, <c>false</c> if invalid</returns>
    public static bool IsValidEscape(string str)
    {

        if (str.Length == 0) { return false; }
        if (str.Length == 1 && IsValidEscapeChar(str[0]))
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
        bool notEmpty(string x) => x.Length > 0;
        bool startsWithZero(string x) => x.Length > 0 && x[0] == '0' && x.Length > 1;
        bool validOnlyDigits(string x) => x.All(char.IsDigit) && x.Length > 0;
        string strWithoutSign(string x)
        {
            if (x.Length == 0) { return x; }
            bool hasSign = signs.Contains(x[0]);
            return hasSign ? x[1..] : x;
        }
        bool validWithSign(string x) => validOnlyDigits(strWithoutSign(x));
        // check if x is numeric and allows starting with '-'
        bool validWithNegOnly(string x) => x.Length > 0 && validOnlyDigits(x[0] == '-' ? x[1..] : x);

        var split_exponent = input.ToLower().Split('e');
        if (split_exponent.Length > 2) { return false; } // multiple exponents
        if (split_exponent.Length > 1)
        {
            string exponent = split_exponent[1];
            if (!validWithSign(exponent)) { return false; }
        }
        var split_decimal = split_exponent[0].Split('.');
        return notEmpty(input) && !startsWithZero(split_decimal[0]) && split_decimal.Length is not > 2 and not < 1 && (split_decimal.Length == 1
            ? validWithNegOnly(split_decimal[0])
            : split_decimal.Length == 2
            ? validWithNegOnly(split_decimal[0]) && validOnlyDigits(split_decimal[1])
            : throw new Exception("Error in parsing number."));
    }

    public static List<string> ExtractArrayElements(string input)
    {
        input = RemoveWhitespace(input);
        List<string> items = [];
        int openBracketCount = 0;
        int commaCount = 0;
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
                    commaCount++;
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
        if (temp.Length > 0)
        {
            items.Add(temp.ToString());
        }
        return items.Count > 0 && commaCount != items.Count - 1
            ? throw new ArgumentException("invalid commas in provided json array")
            : items;
    }

    public static bool IsValidArray(string input)
    {
        input = input.Trim();
        if (input.Length < 2) { return false; }
        if (input[0] != '[' || input[^1] != ']')
        {
            return false;
        }
        List<string> tokens;
        try
        {
            tokens = ExtractArrayElements(input);
        }
        catch (ArgumentException)
        {
            return false;
        }

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
        if (!IsValidObject(input, out string? error)) { throw new ArgumentException($"Invalid JSON provided to FormatJsonString: {error}", paramName: nameof(input)); }
        var no_whitespace = RemoveWhitespace(input);
        int tab_depth = 0;
        bool in_string = false;
        char last = input[0];
        var str = new StringBuilder();
        string get_tabs() => new('\t', tab_depth);

        for (int i = 0; i < no_whitespace.Length; i++)
        {
            char c = no_whitespace[i];
            if (str.Length > 0 && str.ToString()[^1] == '\n')
            {
                str.Append(get_tabs()); // add tabs
            }
            switch (c)
            {
                case '"':
                    if (!in_string) { in_string = true; }
                    else if (in_string && last != '\\') { in_string = false; }
                    str.Append(c);
                    break;
                case ',':
                    str.Append(c);
                    if (!in_string) { str.Append(Environment.NewLine); }
                    break;
                case ':':
                    str.Append(c);
                    if (!in_string) { str.Append(' '); }
                    break;
                case '{' or '[':
                    str.Append(c);
                    if (!in_string) { tab_depth += 1; str.Append(Environment.NewLine); }
                    break;
                case '}' or ']':
                    if (!in_string) { tab_depth -= 1; str.Append(Environment.NewLine); str.Append(get_tabs()); }
                    str.Append(c);
                    break;
                default:
                    str.Append(c);
                    break;
            }
            last = c;
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
    public static void FormatByFilepath(string path, FormatConfig config)
    {
        StreamReader sr = new(path);
        string text = sr.ReadToEnd();
        sr.Close();
        if (config.ValidateOnly)
        {
            var result = IsValidJson(text);
            if (result)
            {
                Console.WriteLine("The provided JSON is valid.");
            }
            else
            {
                Console.WriteLine("The provided JSON is NOT valid.");
            }
            return;
        }
        try
        {
            string formatted = FormatJsonString(text);
            if (config.Overwrite)
            {
                Console.WriteLine($"Writing to path: {path}");
                File.WriteAllText(path, formatted);
            }
            else if (config.OutputToFile)
            {
                string outpath = path.RemovedSuffix(".json") + "-formatted.json";
                Console.WriteLine($"Writing to path: {outpath}");
                File.WriteAllText(outpath, formatted);
            }
            else
            {
                Console.Write(formatted + "\n"); // print result by default
            }
        }
        catch (ArgumentException err)
        {
            Console.WriteLine($"Couldn't format {path} because it is invalid JSON: {err.Message}");
        }
    }
}
