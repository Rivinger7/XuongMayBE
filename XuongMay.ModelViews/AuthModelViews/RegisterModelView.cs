using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XuongMay.ModelViews.AuthModelViews
{
    public class RegisterModelView
    {
        [Required(ErrorMessage = "*  Please enter username")]
        public required string Username { get; set; }
        [Required(ErrorMessage = "*  Please enter password")]
        public required string Password { get; set; }
        [Required(ErrorMessage = "* Please confirm your password")]
        public required string ConfirmedPassword { get; set; }
    }
}
