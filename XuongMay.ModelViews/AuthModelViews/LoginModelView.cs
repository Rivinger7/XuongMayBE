using System.ComponentModel.DataAnnotations;

namespace XuongMay.ModelViews.AuthModelViews
{
    public class LoginModelView
    {
        [Required(ErrorMessage = "*  Please enter username")]
        public required string Username { get; set; }
        [Required(ErrorMessage = "*  Please enter password")]
        public required string Password { get; set; }
    }
}
