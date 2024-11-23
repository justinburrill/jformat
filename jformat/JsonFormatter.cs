
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace JFormat
{
    static public class JsonFormatter
    {
        public static string RemoveWhitespace(string input)
        {
            int quotecount = 0;
            var outstr = new StringBuilder();
            foreach (var c in input)
            {
                if (c == '"') { quotecount++; }
                List<char> ws = [' ', '\n', '\t', '\r'];
                if (!ws.Contains(c) || quotecount % 2 != 0)
                {
                    outstr.Append(c);
                }
            }
            return outstr.ToString();
        }

        enum TokenType
        {
            Key,
            Value,
            OpenBracket,
            ClosingBracket,
            Comma,
            Colon,
            // more?
        }

        public static List<string> TokenizeJsonObj(string input)
        {
            input = RemoveWhitespace(input);
            List<string> strings = new();
            string temptoken = "";
            TokenType? next = TokenType.Key;

            foreach (char ch in input)
            {
                bool matched = false;
                switch (ch)
                {
                    case ':' or ',':
                        matched = true;
                        break;
                    case '{':
                        matched = true;
                        next = TokenType.Key;
                        break;
                    case '}':
                        matched = true;
                        next = TokenType.Key;
                        break;
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

        static bool IsValidToken(string token, TokenType tokenType)
        {
            return tokenType switch
            {
                TokenType.Key => IsValidString(token),
                TokenType.Value => IsValidValue(token),
                TokenType.OpenBracket => token == "{",
                TokenType.ClosingBracket => token == "}",
                TokenType.Comma => token == ",",
                TokenType.Colon => token == ":",
                _ => throw new NotImplementedException(),
            };
        }

        public static bool IsValidJsonObj(string input)
        {
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
            if (new List<string> { "true", "false", "null" }.Contains(input)) return true;
            if (IsValidString(input)) return true;
            if (IsValidNumber(input)) return true;
            if (IsValidJsonObj(input)) return true;
            // anything else is either an object or array

            return false;
        }

        public static bool IsValidString(string input)
        {
            if (input.Length < 2) return false;
            char quote = '"';
            return input[0] == quote && input.Last() == quote && IsValidQuotes(input);
        }

        public static bool IsValidQuotes(string input)
        {
            int quoteCount = 0;
            foreach (char c in input)
            {
                if (c == '"') quoteCount++;
            }
            return quoteCount % 2 == 0;
        }

        public static bool IsValidNumber(string input)
        {
            var signs = new List<char> { '+', '-' };
            Func<string, bool> validOnlyDigits = (string x) => x.All(Char.IsDigit) && x.Length > 0;
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
                string exponent = (split_exponent[1]);
                if (!validWithSign(exponent)) { return false; }
            }
            var split_decimal = split_exponent[0].Split('.');
            if (split_decimal.Length > 2 || split_decimal.Length < 1) { return false; } // multiple decimal points
            else if (split_decimal.Length == 1) { return validWithNegOnly(split_decimal[0]); }
            else if (split_decimal.Length == 2) { return validWithNegOnly(split_decimal[0]) && validOnlyDigits(split_decimal[1]); }
            else { throw new Exception("this wasn't supposed to happen"); }

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


        static public void FormatByFilepath(string path)
        {
            StreamReader sr = new(path);
            string text = sr.ReadToEnd();
            string formatted = FormatJsonString(text);
            sr.Close();
            File.WriteAllText(path, formatted);
        }
    }
}
