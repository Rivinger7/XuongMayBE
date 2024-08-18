using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using XuongMay.ModelViews.JwtModelViews;
using XuongMay.ModelViews.UserModelViews;

namespace XuongMay.Contract.Services.Interface
{
	public interface IJwtService
	{
		void GenerateAccessToken(IEnumerable<Claim> claims, int Id, out string accessToken, out string refreshToken);
		void RevokeToken(string Id);
		void RefreshAccessToken(out string newAccessToken, out string newRefreshToken, TokenApiModelView tokenApiModel);
	}
}
