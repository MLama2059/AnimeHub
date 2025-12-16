using AnimeHub.Shared.Models;
using AnimeHub.Shared.Models.Dtos.Anime;
using AnimeHubApi.Data;
using AnimeHubApi.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace AnimeHubApi.Repository
{
    public class RecommendationRepository : IRecommendationRepository
    {
        private readonly ApplicationDbContext _context;

        public RecommendationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<AnimeListReadDto>> GetRecommendationsAsync(int currentAnimeId, int count)
        {
            var allAnimes = await _context.Animes
                .Include(a => a.AnimeGenres)
                .Include(a => a.Category)
                .AsNoTracking()
                .ToListAsync();

            // Find the target anime(the one the user is looking at)
            var targetAnime = allAnimes.FirstOrDefault(a => a.Id == currentAnimeId);

            // If the anime doesn't exist or has no genres, we can't make recommendations
            if (targetAnime == null || targetAnime.AnimeGenres == null || !targetAnime.AnimeGenres.Any())
            {
                return new List<AnimeListReadDto>();
            }

            // Extract the Target's "Feature Set" (List of Genre IDs)
            // We use a HashSet for fast performance
            var targetGenreIds = targetAnime.AnimeGenres.Select(ag => ag.GenreId).ToHashSet();

            // This list will store every candidate anime and its similarity score
            var scoredCandidates = new List<(double Score, Anime anime)>();

            // THE ALGORITHM: Loop through all other animes and compare
            foreach (var candidate in allAnimes)
            {
                // Don't recommend the anime itself
                if (candidate.Id == currentAnimeId) continue;

                // Skip candidates with no genres
                if (candidate.AnimeGenres == null || !candidate.AnimeGenres.Any()) continue;

                var candidateGenreIds = candidate.AnimeGenres.Select(ag => ag.GenreId).ToHashSet();

                // --- JACCARD SIMILARITY CALCULATION ---

                // A. Intersection: How many genres do they share?
                // Example: Target has [Action, Horror], Candidate has [Action, Comedy] -> Intersection = 1 (Action)
                var intersection = targetGenreIds.Intersect(candidateGenreIds).Count();

                // B. Union: How many unique genres exist between them total?
                // Example: [Action, Horror, Comedy] -> Union = 3
                var union = targetGenreIds.Union(candidateGenreIds).Count();

                // C. Calculate Score: (Intersection / Union)
                // 1 / 3 = 0.33
                double score = (double)intersection / union;

                // Only keep it if there is at least some similarity (Score > 0)
                if (score > 0)
                {
                    scoredCandidates.Add((score, candidate));
                }
            }

            // Decision Making: Sort by Highest Score and take the top N
            var topRecommendations = scoredCandidates
                .OrderByDescending(x => x.Score)
                .ThenByDescending(x => x.anime.Rating)
                .Take(count)
                .Select(x => x.anime)
                .ToList();

            // Mapping to DTOs
            return topRecommendations.Select(a => new AnimeListReadDto
            {
                Id = a.Id,
                Title = a.Title,
                ImageUrl = a.ImageUrl,
                Rating = a.Rating,
                Episodes = a.Episodes,
                Season = a.Season.ToString(),
                PremieredYear = a.PremieredYear,
                Status = a.Status.ToString(),
                CategoryId = a.CategoryId,
                CategoryName = a.Category.Name
            }).ToList();
        }
    }
}
