using AnimeHub.Shared.Models;
using AnimeHub.Shared.Models.Dtos.User;
using AnimeHubApi.Data;
using AnimeHubApi.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AnimeHubApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // POST: api/Users/Registration
        [HttpPost("Registration")]
        public async Task<IActionResult> Registration(UserDto userDto)
        {
            if (userDto is null)
                return BadRequest("Invalid user data.");

            var registeredUser = await _userRepository.RegistrationAsync(userDto);

            return Ok(new
            {
                Message ="User registered successfully.",
                User = new
                {
                    registeredUser.Id,
                    registeredUser.Username,
                    registeredUser.Email,
                    registeredUser.CreatedAt,
                    registeredUser.Role
                }
            });
        }

        // POST: api/Users/Login
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var token = await _userRepository.LoginAsync(loginDto);

            if (token is null)
            {
                return Unauthorized("Invalid credentials.");
            }

            return Ok(new { Token = token });
        }

        // GET: api/Users/GetUsers
        [Authorize]
        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userRepository.GetAllAsync();
            var response = users.Select(u => new UserResponseDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                CreatedAt = u.CreatedAt,
                Role = u.Role
            }).ToList();

            return Ok(response);
        }

        // GET: api/Users/GetUser/5
        [HttpGet("GetUser/{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user is null) return NotFound();

            var response = new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                Role = user.Role
            };

            return Ok(response);
        }
    }
}
