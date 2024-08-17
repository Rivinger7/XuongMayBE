using System.ComponentModel.DataAnnotations;

namespace XuongMay.ModelViews.AuthModelViews
{
    public class RegisterModelView
    {
        [Required(ErrorMessage = "*  Please enter fullname")]
        public required string FullName { get; set; }
        [Required(ErrorMessage = "*  Please enter username")]
        public required string Username { get; set; }
        [Required(ErrorMessage = "*  Please enter password")]
        public required string Password { get; set; }
        [Required(ErrorMessage = "* Please confirm your password")]
        public required string ConfirmedPassword { get; set; }
    }
}
