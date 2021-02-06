using System;
using System.Globalization;
using System.Text;

// ReSharper disable once CheckNamespace
namespace Aeris
{
    public static class DatedKeys
    {
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
            var random = new Random(seed == 0 ? DateTime.UtcNow.Millisecond : seed);
            
            var key = "";
            key += random.Next(1, 5);
            key += random.Next(1, 5);
            key += random.Next(1, 5);
            key += random.Next(1, 6);
            key += random.Next(1, 6);
            key += random.Next(1, 3);

            var ts = new TimeSpan(expiryDate.Ticks);
            var totalDaysToString = ((int)ts.TotalDays).ToString(CultureInfo.InvariantCulture);

            totalDaysToString = totalDaysToString.PadLeft(7, '0');

            key += totalDaysToString[0] + "-";

            key += GenerateSegment(5, (int) char.GetNumericValue(totalDaysToString[1]),
                (int) char.GetNumericValue(key[0])) + "-";

            key += GenerateSegment(5, (int) char.GetNumericValue(totalDaysToString[2]),
                (int) char.GetNumericValue(key[1])) + "-";

            key += GenerateSegment(5, (int) char.GetNumericValue(totalDaysToString[3]),
                (int) char.GetNumericValue(key[2])) + "-";

            key += GenerateSegment(6, (int) char.GetNumericValue(totalDaysToString[4]),
                (int) char.GetNumericValue(key[3])) + "-";

            key += GenerateSegment(6, (int) char.GetNumericValue(totalDaysToString[5]),
                (int) char.GetNumericValue(key[4])) + "-";

            key += GenerateSegment(3, (int) char.GetNumericValue(totalDaysToString[6]),
                (int) char.GetNumericValue(key[5]));

            return key;
        }

        /// <summary>
        /// Gets the total amount of days that the key contains.
        /// </summary>
        /// <param name="key">Key to process</param>
        /// <returns></returns>
        private static int GetTotalDays(string key)
        {
            var numbers = $"{key[6]}" +
                          $"{key[8 + (int) char.GetNumericValue(key[0])]}" +
                          $"{key[14 + (int) char.GetNumericValue(key[1])]}" +
                          $"{key[20 + (int) char.GetNumericValue(key[2])]}" +
                          $"{key[26 + (int) char.GetNumericValue(key[3])]}" +
                          $"{key[33 + (int) char.GetNumericValue(key[4])]}" +
                          $"{key[40 + (int) char.GetNumericValue(key[5])]}";
            var s = numbers.Replace(",", "");
            return int.TryParse(s, out var value) ? value : 0;
        }

        /// <summary>
        /// Generates a segment of a given length and which contains a give number at a given index.
        /// </summary>
        /// <param name="length">Length of the segment</param>
        /// <param name="numberToStore">Number to store</param>
        /// <param name="index">Index of the number to store</param>
        /// <returns></returns>
        private static string GenerateSegment(int length, int numberToStore, int index)
        {
            var segment = new StringBuilder();

            while (segment.Length < length) segment.Append(new Random().Next(0, 9));

            segment[index] = numberToStore.ToString()[0];
 
            return segment.ToString();
        }
    }
}