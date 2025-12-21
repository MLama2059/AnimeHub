using AnimeHub.Shared.Models.Dtos.Anime;

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

        // Logic to clear if needed
        public void ClearCatalogState()
        {
            CatalogSearchTerm = string.Empty;
            CatalogItems.Clear();
            CatalogCurrentPage = 1;
        }
    }
}
