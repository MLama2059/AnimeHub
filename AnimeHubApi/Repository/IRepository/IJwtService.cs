using AnimeHub.Shared.Models;

namespace AnimeHubApi.Repository.IRepository
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
