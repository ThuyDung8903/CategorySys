using CategorySys.DTO;
using CategorySys.Models;
using CategorySys.Repositories;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CategorySys.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly JwtOption _jwtOption;

        public AuthService(IAuthRepository authRepository, IOptions<JwtOption> options)
        {
            _authRepository = authRepository;
            _jwtOption = options.Value;
        }

        public string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOption.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtOption.Issuer,
                audience: _jwtOption.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string Login(UserDTO userDTO)
        {
            try
            {
                var user = _authRepository.Login(userDTO);
                var token = GenerateToken(user);
                return token;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Register(UserDTO userDTO)
        {
            try
            {
                _authRepository.Register(userDTO);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
