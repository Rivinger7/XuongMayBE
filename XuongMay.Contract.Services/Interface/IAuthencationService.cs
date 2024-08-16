using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XuongMay.ModelViews.AuthModelViews;

namespace XuongMay.Contract.Services.Interface
{
    public interface IAuthencationService
    {
        Task RegisterUserAsync(RegisterModelView registerModelView); // RegisterModel
        Task AuthenticateUserAsync(LoginModelView loginModelView); // LoginModel
    }
}
