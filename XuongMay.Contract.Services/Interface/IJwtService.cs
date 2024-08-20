using System.Security.Claims;
using XuongMay.ModelViews.JwtModelViews;

namespace XuongMay.Contract.Services.Interface
{
	public interface IJwtService
	{
		void GenerateAccessToken(IEnumerable<Claim> claims, int Id, out string accessToken, out string refreshToken);
		void RevokeToken(string Id);
		void RefreshAccessToken(out string newAccessToken, out string newRefreshToken, TokenApiModelView tokenApiModel);
	}
}
