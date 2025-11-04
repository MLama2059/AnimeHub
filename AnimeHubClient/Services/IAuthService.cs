using AnimeHub.Shared.Models.Dtos.User;

namespace AnimeHubClient.Services
{
    public interface IAuthService
    {
        Task<AuthenticationResponseDto> LoginAsync(LoginRequestDto loginDto);
        Task<bool> RegisterAsync(RegistrationRequestDto registerDto);
        Task Logout();
    }
}
