using System;
using System.Globalization;
using System.Text;

namespace DatedKeys
{
    public static class DatedKeys
    {
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

        public static bool IsActiveKey(string key)
        {
            if (TryParseExpiryDate(key, out var expiryDate))
            {
                return expiryDate > DateTime.Now;
            }
            return false;
        }

        public static string GenerateKey(DateTime expiryDate)
        {
            var key = "";
            key += new Random().Next(1, 5);
            key += new Random().Next(1, 5);
            key += new Random().Next(1, 5);
            key += new Random().Next(1, 6);
            key += new Random().Next(1, 6);
            key += new Random().Next(1, 3);

            var ts = new TimeSpan(expiryDate.Ticks);
            var totalDaysToString = ts.TotalDays.ToString(CultureInfo.InvariantCulture);

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

        private static string GenerateSegment(int length, int numberToStore, int index)
        {
            var segment = new StringBuilder();

            while (segment.Length < length) segment.Append(new Random().Next(0, 9));

            segment[index] = numberToStore.ToString()[0];
 
            return segment.ToString();
        }
    }
}