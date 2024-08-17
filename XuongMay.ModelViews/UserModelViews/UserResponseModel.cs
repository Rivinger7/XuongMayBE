namespace XuongMay.ModelViews.UserModelViews
{
    public class UserResponseModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string? FullName { get; set; }
        public string Role { get; set; } = null!;
    }
}
