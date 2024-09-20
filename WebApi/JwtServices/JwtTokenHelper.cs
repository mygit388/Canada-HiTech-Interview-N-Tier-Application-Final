using Dapper;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WebApi.Models;

namespace WebApi.JwtServices
{
    public class JwtTokenHelper
    {
        private static string secretKey = ConfigurationManager.AppSettings["JwtSecretKey"];
        private static readonly string Issuer = ConfigurationManager.AppSettings["JwtIssuer"];
        private static readonly string Audience = ConfigurationManager.AppSettings["JwtAudience"];

        private readonly string connectionString = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
        SqlConnection connection;
        public TokenResponse GenerateTokens(ApplicationUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                     new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(), ClaimValueTypes.Integer64),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name, user.UserName)
           // new Claim(JwtRegisteredClaimNames.Name, user.UserName)
                  
       }),
                Expires = DateTime.UtcNow.AddMinutes(30),
                Issuer = Issuer,
                Audience = Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

          
            var accessToken = tokenHandler.CreateToken(tokenDescriptor);
            var refreshToken = GenerateRefreshToken(); // Generate and store refresh token

            return new TokenResponse
            {
                AccessToken = tokenHandler.WriteToken(accessToken),
                RefreshToken = refreshToken
            };

        }
        public string GenerateRefreshToken()
        {
            // Implement your refresh token generation logic here
            return Guid.NewGuid().ToString();
        }
        public ClaimsPrincipal GetPrincipal (string token)
        {

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true, // Ignore expiration
                ValidateIssuerSigningKey = true,
                ValidIssuer = Issuer,
                ValidAudience = Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var securityToken);
            return principal;
        }

        public async Task<bool> ValidateRefreshToken(string refreshToken, ApplicationUser user)
        {
            connection = new SqlConnection(connectionString);
            var parameters = new
            {
                Email = user.Email,
                Token= refreshToken

            };
            int count = await connection.QuerySingleOrDefaultAsync<int>("sp_GetValidRefreshToken", parameters, commandType: CommandType.StoredProcedure);

            return count > 0 ;           

        }
    }
}