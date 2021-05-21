using System.Collections.Generic;

namespace RadLine
{
    internal static class StringTokenizer
    {
        public static IEnumerable<string> Tokenize(string text)
        {
            var buffer = string.Empty;
            foreach (var character in text)
            {
                if (char.IsLetterOrDigit(character))
                {
                    buffer += character;
                }
                else
                {
                    if (buffer.Length > 0)
                    {
                        yield return buffer;
                        buffer = string.Empty;
                    }

                    yield return new string(character, 1);
                }
            }

            if (buffer.Length > 0)
            {
                yield return buffer;
            }
        }
    }
}
