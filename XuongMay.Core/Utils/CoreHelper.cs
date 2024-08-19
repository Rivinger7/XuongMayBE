using System.Globalization;
using System.Text;

namespace XuongMay.Core.Utils
{
    public class CoreHelper
    {
        public static DateTimeOffset SystemTimeNow => TimeHelper.ConvertToUtcPlus7(DateTimeOffset.Now);

		public static DateTime SystemTimeNows => TimeHelper.ConvertToUtcPlus7(DateTimeOffset.Now).DateTime;

		public static string ConvertVnString(string input)
		{
			if (string.IsNullOrWhiteSpace(input))
				return string.Empty;

			// Chuẩn hóa chuỗi Unicode về dạng chuẩn (dấu tách biệt)
			var normalizedString = input.Normalize(NormalizationForm.FormD);
			var stringBuilder = new StringBuilder();

			// Loại bỏ các ký tự có dấu
			foreach (var c in normalizedString)
			{
				var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
				if (unicodeCategory != UnicodeCategory.NonSpacingMark)
				{
					stringBuilder.Append(c);
				}
			}

			// Chuyển đổi về dạng không dấu, chuyển thường và loại bỏ khoảng trắng thừa
			var result = stringBuilder
				.ToString()
				.Normalize(NormalizationForm.FormC)
				.ToLowerInvariant()
				.Trim()
				.Replace(" ", "");

			return result;
		}

	}
}
