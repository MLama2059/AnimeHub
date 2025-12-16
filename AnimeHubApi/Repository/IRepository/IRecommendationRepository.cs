using AnimeHub.Shared.Models.Dtos.Anime;

namespace AnimeHubApi.Repository.IRepository
{
    public interface IRecommendationRepository
    {
        Task<List<AnimeListReadDto>> GetRecommendationsAsync(int currentAnimeId, int count);
    }
}
