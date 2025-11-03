using AnimeHub.Shared.Models.Dtos.User;

namespace AnimeHubClient.Services
{
    public interface IAuthService
    {
        Task<AuthenticationResponseDto> Login(LoginRequestDto loginDto);
        Task<bool> Register(RegistrationRequestDto registerDto);
        Task Logout();
    }
}
