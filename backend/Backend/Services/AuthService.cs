using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Backend.Services
{
    public class AuthService(AppDbContext context, IConfiguration config)
    {
        private readonly IConfiguration _config = config;

        private readonly AppDbContext _dbContext = context;

        public async Task<bool> RegisterUser(string username, string password)
        {
            if (_dbContext.Users.Any(u => u.Username == username))
                return false;

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            var user = new User { Username = username, PasswordHash = hashedPassword };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<User?> AuthenticateUser(string username, string password)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;

            string refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _dbContext.SaveChangesAsync();

            return user;
        }

        public async Task<User?> CheckRefreshToken(string refreshToken)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return null;

            return user;
        }

        public string GenerateAccessToken(User user)
        {
            string jwtKey = _config["JWT:Secret"] ?? throw new InvalidOperationException("JWT Key is missing in configuration.");
            string jwtIssuer = _config["JWT:Issuer"] ?? throw new InvalidOperationException("JWT Issuer is missing in configuration.");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes);
        }
    }
}
