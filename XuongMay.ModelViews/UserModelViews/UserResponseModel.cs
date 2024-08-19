using System.Text.Json.Serialization;

namespace XuongMay.ModelViews.UserModelViews
{
    public class UserResponseModel
    {
        public required int Id { get; set; }
        public required string Username { get; set; }
        public required string FullName { get; set; }
        public required string Role { get; set; }

        // This field will only be included in the JSON response if it is not null
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? AssemblyLineID { get; set; }

        // This field will only be included in the JSON response if it is not null
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? AssemblyLineName { get; set; }
    }
}
