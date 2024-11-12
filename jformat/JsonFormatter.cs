
namespace JFormat
{
    public class JsonFormatter
    {
        static string RemoveWhitespace(string input)
        {
            String outstr = "";
            foreach (var c in input)
            {
                List<char> ws = [' ', '\n'];
                if (!ws.Contains(c))
                {
                    outstr.Append(c);
                }
            }
            return outstr;
        }

        public static bool IsValidJson(string input)
        {
            return IsValidBrackets(input);
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

        public static string FormatString(string input)
        {
            var no_whitespace = RemoveWhitespace(input);
            int tab_depth = 0;
            string out_str = "";
            for (int i = 0; i<no_whitespace.Length;i++)
            {
                char c = no_whitespace[i];
                switch (c)
                {
                    case ':':
                        out_str.Append(c);
                        out_str.Append(' ');
                        break;
                    case '{':
                        out_str.Append('\n');
                        tab_depth += 1;
                        out_str.Concat(new string('\t', tab_depth)); // add tabs
                        out_str.Append(c);
                        break;
                    default:
                        out_str.Append(c);
                        break;
                }
            }

            return out_str;
        }
    }
}
