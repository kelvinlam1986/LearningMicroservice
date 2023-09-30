using Contracts.Identity;
using Microsoft.IdentityModel.Tokens;
using Shared.Configurations;
using Shared.DTO.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Identity
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;

        public TokenService(JwtSettings jwtSettings)
        {
            _jwtSettings = jwtSettings;
        }

        public TokenResponse GetToken(TokenRequest request)
        {
            var token = GenerateJwt();
            return new TokenResponse(token);
        }

        private string GenerateJwt()
        {
            return GenerateEncryptedToken(GetSigningCredentials());
        }

        private string GenerateEncryptedToken(SigningCredentials signingCredentials)
        {
            var claims = new[]
            {
                new Claim("Role", "Admin")
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: signingCredentials);

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }

        private SigningCredentials GetSigningCredentials()
        {
            var secrets = Encoding.UTF8.GetBytes(_jwtSettings.Key);
            return new SigningCredentials(new SymmetricSecurityKey(secrets), SecurityAlgorithms.HmacSha256);
        }
    }
}
