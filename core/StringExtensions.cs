using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altimit
{
    public static class StringExtensions
    {
        public static string ToAbbreviatedString(this object value)
        {
            return value.ToString().Substring(0, Math.Min(8, value.ToString().Length));
        }

        public static string ToBracketSring(this string value)
        {
            return "[" + value + "]";
        }
        
        private static readonly Random _random = new Random();

        // Generates a random string
        public static string Random(int size = 8, bool lowerCase = false)
        {
            var builder = new StringBuilder(size);

            // Unicode/ASCII Letters are divided into two blocks
            // (Letters 65–90 / 97–122):
            // The first group containing the uppercase letters and
            // the second group containing the lowercase.  

            // char is a single Unicode character  
            char offset = lowerCase ? 'a' : 'A';
            const int lettersOffset = 26; // A...Z or a..z: length=26  

            for (var i = 0; i < size; i++)
            {
                var @char = (char)_random.Next(offset, offset + lettersOffset);
                builder.Append(@char);
            }

            return lowerCase ? builder.ToString().ToLower() : builder.ToString();
        }

        // Generates a random email--useful for testing purposes like creating a random account
        public static string RandomEmail()
        {
            return $"{Random()}@{Random()}.com";
        }
    }
}
