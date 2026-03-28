using AnimeHub.Shared.Models.Dtos.Anime;
using AnimeHub.Shared.Models.Dtos.Category;
using AnimeHub.Shared.Models.Dtos.Genre;

namespace AnimeHubClient.Services
{
    public class AnimeStateContainer
    {
        // Admin List State
        public string AdminSearchTerm { get; set; } = string.Empty;
        public int AdminPageIndex { get; set; } = 0;

        // Catalog State
        public string CatalogSearchTerm { get; set; } = string.Empty;
        public List<AnimeListReadDto> CatalogItems { get; set; } = new();
        public int CatalogCurrentPage { get; set; } = 1;
        public bool CatalogHasMorePages { get; set; } = true;
        public string CatalogOrderBy { get; set; } = "title_asc";

        // Filter state properties
        public int? SelectedCategoryId { get; set; }
        public int? SelectedGenreId { get; set; }
        public int? SelectedYear { get; set; }
        public string? SelectedSeason { get; set; }

        // Cached lists
        public List<CategoryReadDto> Categories { get; set; } = new();
        public List<GenreReadDto> Genres { get; set; } = new();

        // Logic to clear if needed
        public void ClearCatalogState()
        {
            CatalogSearchTerm = string.Empty;
            CatalogItems.Clear();
            CatalogCurrentPage = 1;

            // Reset the new filters
            SelectedCategoryId = null;
            SelectedGenreId = null;
            SelectedYear = null;
            SelectedSeason = null;
        }
    }
}
