using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Dreamland.Json
{
    public static class MiniJson
    {
        public static object Deserialize(string json)
        {
            if (json == null) return null;
            using (var reader = new StringReader(json))
            {
                return ParseValue(reader);
            }
        }

        static object ParseValue(StringReader reader)
        {
            EatWhitespace(reader);
            int next = reader.Peek();
            if (next == -1) return null;
            char c = (char)next;
            if (c == '"') return ParseString(reader);
            if (c == '{') return ParseObject(reader);
            if (c == '[') return ParseArray(reader);
            if (char.IsDigit(c) || c == '-') return ParseNumber(reader);
            return ParseLiteral(reader);
        }

        static IDictionary<string, object> ParseObject(StringReader reader)
        {
            var table = new Dictionary<string, object>();
            reader.Read();
            while (true)
            {
                EatWhitespace(reader);
                int next = reader.Peek();
                if (next == -1) return table;
                if ((char)next == '}')
                {
                    reader.Read();
                    return table;
                }

                var key = ParseString(reader);
                EatWhitespace(reader);
                reader.Read();
                var value = ParseValue(reader);
                table[key] = value;

                EatWhitespace(reader);
                next = reader.Peek();
                if ((char)next == ',')
                {
                    reader.Read();
                    continue;
                }
                if ((char)next == '}')
                {
                    reader.Read();
                    return table;
                }
            }
        }

        static IList ParseArray(StringReader reader)
        {
            var list = new List<object>();
            reader.Read();
            while (true)
            {
                EatWhitespace(reader);
                int next = reader.Peek();
                if (next == -1) return list;
                if ((char)next == ']')
                {
                    reader.Read();
                    return list;
                }

                var value = ParseValue(reader);
                list.Add(value);

                EatWhitespace(reader);
                next = reader.Peek();
                if ((char)next == ',')
                {
                    reader.Read();
                    continue;
                }
                if ((char)next == ']')
                {
                    reader.Read();
                    return list;
                }
            }
        }

        static string ParseString(StringReader reader)
        {
            var sb = new StringBuilder();
            reader.Read();
            while (true)
            {
                int next = reader.Read();
                if (next == -1) break;
                char c = (char)next;
                if (c == '"') break;
                if (c == '\\')
                {
                    int esc = reader.Read();
                    if (esc == -1) break;
                    switch ((char)esc)
                    {
                        case '"': sb.Append('"'); break;
                        case '\\': sb.Append('\\'); break;
                        case '/': sb.Append('/'); break;
                        case 'b': sb.Append('\b'); break;
                        case 'f': sb.Append('\f'); break;
                        case 'n': sb.Append('\n'); break;
                        case 'r': sb.Append('\r'); break;
                        case 't': sb.Append('\t'); break;
                        case 'u':
                            var hex = new char[4];
                            reader.Read(hex, 0, 4);
                            sb.Append((char)Convert.ToInt32(new string(hex), 16));
                            break;
                    }
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        static object ParseNumber(StringReader reader)
        {
            var sb = new StringBuilder();
            while (true)
            {
                int next = reader.Peek();
                if (next == -1) break;
                char c = (char)next;
                if (!char.IsDigit(c) && c != '-' && c != '+' && c != '.' && c != 'e' && c != 'E') break;
                sb.Append(c);
                reader.Read();
            }

            var s = sb.ToString();
            if (s.IndexOf('.') != -1 || s.IndexOf('e') != -1 || s.IndexOf('E') != -1)
            {
                double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var d);
                return d;
            }

            long.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var l);
            return l;
        }

        static object ParseLiteral(StringReader reader)
        {
            var sb = new StringBuilder();
            while (true)
            {
                int next = reader.Peek();
                if (next == -1) break;
                char c = (char)next;
                if (!char.IsLetter(c)) break;
                sb.Append(c);
                reader.Read();
            }

            var word = sb.ToString();
            if (word == "true") return true;
            if (word == "false") return false;
            if (word == "null") return null;
            return word;
        }

        static void EatWhitespace(StringReader reader)
        {
            while (true)
            {
                int next = reader.Peek();
                if (next == -1) return;
                if (!char.IsWhiteSpace((char)next)) return;
                reader.Read();
            }
        }
    }
}
