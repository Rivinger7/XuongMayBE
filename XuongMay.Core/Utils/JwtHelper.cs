using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace XuongMay.Core.Utils
{
	public class JwtHelper
	{
		public static string GenerateToken(string secret, string username, string role, int expireMinutes = 60)
		{
			var symmetricKey = Encoding.UTF8.GetBytes(secret);
			var tokenHandler = new JwtSecurityTokenHandler();

			var tokenDescriptor = new JwtSecurityToken(
				issuer: "https://localhost:7286/",
				audience: "https://localhost:7286/",
				claims:
				[
					new (ClaimTypes.NameIdentifier, username),
					new (ClaimTypes.Role, role)
				],
				expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(expireMinutes)),
				signingCredentials: new SigningCredentials(
									new SymmetricSecurityKey(symmetricKey),
									SecurityAlgorithms.HmacSha256Signature)
			);

			var stoken = tokenHandler.WriteToken(tokenDescriptor);
			return stoken;
		}

		//public static System.Security.Claims.ClaimsPrincipal GetPrincipal(string token, string secret)
		//{
		//	try
		//	{
		//		var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
		//		var jwtToken = (System.IdentityModel.Tokens.JwtSecurityToken)tokenHandler.ReadToken(token);
		//		if (jwtToken == null)
		//			return null;

		//		var symmetricKey = Convert.FromBase64String(secret);

		//		var validationParameters = new System.IdentityModel.Tokens.TokenValidationParameters()
		//		{
		//			RequireExpirationTime = true,
		//			ValidateIssuer = false,
		//			ValidateAudience = false,
		//			IssuerSigningKey = new System.IdentityModel.Tokens.SymmetricSecurityKey(symmetricKey)
		//		};

		//		System.Security.Claims.SecurityToken securityToken;
		//		var principal = tokenHandler.ValidateToken(token, validationParameters, out securityToken);
		//		return principal;
		//	}
		//	catch (Exception)
		//	{
		//		return null;
		//	}
		//}
	}
}
