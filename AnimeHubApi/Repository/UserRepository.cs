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
        private readonly IJwtService _jwtService;
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context, IConfiguration configuration, IJwtService jwtService)
        {
            _context = context;
            _configuration = configuration;
            _jwtService = jwtService;
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User> RegistrationAsync(RegistrationRequestDto registrationDto)
        {
            var user = new User
            {
                Username = registrationDto.Username,
                Email = registrationDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registrationDto.Password),
                CreatedAt = DateTime.UtcNow,
                Role = RoleConstants.User
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<string?> LoginAsync(LoginRequestDto loginDto)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (existingUser == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, existingUser.PasswordHash))
                return null;

            return _jwtService.GenerateToken(existingUser);
        }
    }
}
