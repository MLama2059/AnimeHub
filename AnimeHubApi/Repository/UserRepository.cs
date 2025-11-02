using AnimeHub.Shared.Models;
using AnimeHub.Shared.Models.Dtos.User;
using AnimeHubApi.Data;
using AnimeHubApi.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AnimeHubApi.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User> RegistrationAsync(UserDto userDto)
        {
            var user = new User
            {
                Username = userDto.Username,
                Email = userDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password),
                CreatedAt = DateTime.UtcNow,
                Role = "User"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<string?> LoginAsync(LoginDto loginDto)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (existingUser == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, existingUser.PasswordHash))
                return null;

            return GenerateJwtToken(existingUser);
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
