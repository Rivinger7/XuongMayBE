namespace XuongMay.Core.Utils
{
    public class CoreHelper
    {
        public static DateTimeOffset SystemTimeNow => TimeHelper.ConvertToUtcPlus7(DateTimeOffset.Now);

		public static DateTime SystemTimeNows => TimeHelper.ConvertToUtcPlus7(DateTimeOffset.Now).DateTime;

	}
}
