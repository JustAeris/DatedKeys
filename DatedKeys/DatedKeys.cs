using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

// ReSharper disable once CheckNamespace
namespace Aeris
{
    public static class DatedKeys
    {
        private static Random Rand;

        private static readonly StringBuilder SB;

        static DatedKeys()
        {
            SB = new StringBuilder(30);
        }
        
        /// <summary>
        /// Gets the expiry date of a key if possible.
        /// </summary>
        /// <param name="key">Key to get the expiry date out.</param>
        /// <param name="expiryDate">Expiry date, will return <code>DateTime.MinValue</code></param>
        /// <returns></returns>
        // ReSharper disable once MemberCanBePrivate.Global
        public static bool TryParseExpiryDate(string key, out DateTime expiryDate)
        {
            try
            {
                var ts = new TimeSpan(GetTotalDays(key), 0, 0, 0);
                expiryDate = new DateTime(ts.Ticks);
                return true;
            }
            catch
            {
                expiryDate = DateTime.MinValue;
                return false;
            }
        }

        /// <summary>
        /// Check whether a ket is expired or not.
        /// </summary>
        /// <param name="key">Key to verify</param>
        /// <returns></returns>
        public static bool IsActiveKey(string key)
        {
            if (TryParseExpiryDate(key, out var expiryDate))
            {
                return expiryDate > DateTime.Now;
            }
            return false;
        }

        /// <summary>
        /// Generates a key with a expiry date.
        /// Precision up to the day.
        /// </summary>
        /// <param name="expiryDate">Expiry date of the key.</param>
        /// <param name="seed">Seed for the Random object. Leaving empty will use a random number</param>
        /// <returns></returns>
        public static string GenerateKey(DateTime expiryDate, int seed = 0)
        {
            //Since you want to allow specification of custom seeds...we can't really reuse instance of Random
            
            Rand = new Random(seed == 0 ? DateTime.UtcNow.Millisecond : seed);
            
            //We don't have to clear the string-builder...since we are pretty much guaranteed
            //to replace all chars in it!

            // key += Rand.Next(1, 5);
            // key += Rand.Next(1, 5);
            // key += Rand.Next(1, 5);
            // key += Rand.Next(1, 6);
            // key += Rand.Next(1, 6);
            // key += Rand.Next(1, 3);
            
            //Since I'm too dumb to figure out the impl of this KeyGen...I've decided to just
            //clear and append, ignoring my statement above

            SB.Append(Rand.Next(1, 5));
            
            SB.Append(Rand.Next(1, 5));
            
            SB.Append(Rand.Next(1, 5));
            
            SB.Append(Rand.Next(1, 6));
            
            SB.Append(Rand.Next(1, 6));
            
            SB.Append(Rand.Next(1, 3));
            
            var ts = new TimeSpan(expiryDate.Ticks);
            
            var totalDaysToString = ((int)ts.TotalDays).ToString(CultureInfo.InvariantCulture);

            totalDaysToString = totalDaysToString.PadLeft(7, '0');

            const char Dash = '-';
            
            SB.Append(totalDaysToString[0]);

            SB.Append(Dash);

            SB.Append(GenerateSegment(5, CharToInt(totalDaysToString[1]),
                CharToInt(SB[0])));
            
            SB.Append(Dash);
            
            SB.Append(GenerateSegment(5, CharToInt(totalDaysToString[2]),
                CharToInt(SB[1])));
            
            SB.Append(Dash);

            SB.Append(GenerateSegment(5, CharToInt(totalDaysToString[2]),
                CharToInt(SB[2])));
            
            SB.Append(Dash);

            SB.Append(GenerateSegment(5, CharToInt(totalDaysToString[4]),
                CharToInt(SB[3])));
            
            SB.Append(Dash);
            
            SB.Append(GenerateSegment(5, CharToInt(totalDaysToString[5]),
                CharToInt(SB[4])));
            
            SB.Append(Dash);

            SB.Append(GenerateSegment(5, CharToInt(totalDaysToString[6]),
                CharToInt(SB[5])));
            
            SB.Append(Dash);

            return SB.ToString();
        }

        /// <summary>
        /// Gets the total amount of days that the key contains.
        /// </summary>
        /// <param name="key">Key to process</param>
        /// <returns></returns>
        private static int GetTotalDays(string key)
        {
            // var numbers = $"{key[6]}" +
            //               $"{key[8 + (int) char.GetNumericValue(key[0])]}" +
            //               $"{key[14 + (int) char.GetNumericValue(key[1])]}" +
            //               $"{key[20 + (int) char.GetNumericValue(key[2])]}" +
            //               $"{key[26 + (int) char.GetNumericValue(key[3])]}" +
            //               $"{key[33 + (int) char.GetNumericValue(key[4])]}" +
            //               $"{key[40 + (int) char.GetNumericValue(key[5])]}";

            SB.Clear();

            SB.Append(key[6]);

            SB.Append(key[8 + CharToInt(key[0])]);
            
            SB.Append(key[14 + CharToInt(key[1])]);
            
            SB.Append(key[20 + CharToInt(key[2])]);

            SB.Append(key[26 + CharToInt(key[3])]);
            
            SB.Append(key[33 + CharToInt(key[4])]);
            
            SB.Append(key[40 + CharToInt(key[5])]);
            
            var Numbers = SB.ToString();
            
            var s = Numbers.Replace(",", "");
            
            return int.TryParse(s, out var value) ? value : 0;
        }

        /// <summary>
        /// Generates a segment of a given length and which contains a give number at a given index.
        /// </summary>
        /// <param name="Length">Length of the segment</param>
        /// <param name="NumberToStore">Number to store</param>
        /// <param name="Index">Index of the number to store</param>
        /// <returns></returns>
        private static unsafe string GenerateSegment(int Length, int NumberToStore, int Index)
        {
            var Segment = stackalloc char[Length];
            
            Segment[Index] = IntToChar(NumberToStore);
            
            //while (Segment.Length < Length) Segment.Append(Rand.Next(0, 9));

            for (int I = 0; I < Index; I++)
            {
                Segment[I] = IntToChar(Rand.Next(0, 9));
            }
            
            for (int I = Index + 1; I < Length; I++)
            {
                Segment[I] = IntToChar(Rand.Next(0, 9));
            }

            return new string(Segment, 0, Length);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static int CharToInt(char Char)
        {
            return unchecked(Unsafe.As<char, int>(ref Char) - 48);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static char IntToChar(int Int)
        {
            unchecked
            {
                Int += 48;
            }
            
            return Unsafe.As<int, char>(ref Int);
        }
    }
}