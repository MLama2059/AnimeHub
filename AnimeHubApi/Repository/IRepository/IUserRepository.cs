using AnimeHub.Shared.Models;
using AnimeHub.Shared.Models.Dtos.User;
using Microsoft.AspNetCore.Mvc;

namespace AnimeHubApi.Repository.IRepository
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<User> RegistrationAsync(RegistrationRequestDto registrationDto);
        Task<string?> LoginAsync(LoginRequestDto loginDto);
    }
}
