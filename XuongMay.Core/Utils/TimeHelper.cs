namespace XuongMay.Core.Utils
{
    public static class TimeHelper
    {
        public static DateTimeOffset ConvertToUtcPlus7(DateTimeOffset dateTimeOffset)
        {
            // UTC+7 is 7 hours ahead of UTC
            TimeSpan utcPlus7Offset = new(7, 0, 0);
            return dateTimeOffset.ToOffset(utcPlus7Offset);
        }

        public static DateTimeOffset ConvertToUtcPlus7NotChanges(DateTimeOffset dateTimeOffset)
        {
            // UTC+7 is 7 hours ahead of UTC
            TimeSpan utcPlus7Offset = new(7, 0, 0);
            return dateTimeOffset.ToOffset(utcPlus7Offset).AddHours(-7);
        }


        public static DateTime GetUtcPlus7Time()
        {
            // Get the current UTC time
            DateTime utcNow = DateTime.UtcNow;

            // Define the UTC+7 time zone
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            // Convert the UTC time to UTC+7
            DateTime utcPlus7Now = TimeZoneInfo.ConvertTimeFromUtc(utcNow, timeZone);

            return utcPlus7Now;
        }

		public static DateTime ConvertToUtcPlus7(DateTime dateTime)
		{
			TimeZoneInfo utcPlus7 = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
			return TimeZoneInfo.ConvertTimeFromUtc(dateTime.ToUniversalTime(), utcPlus7);
		}
		public static DateTime? ConvertStringToDateTime(string dateTimeString)
		{
			if (DateTime.TryParseExact(dateTimeString, "HH:mm dd/MM/yyyy",
				System.Globalization.CultureInfo.InvariantCulture,
				System.Globalization.DateTimeStyles.None, out DateTime result))
			{
				return result;
			}
			return null; 
		}
	}

}
