namespace AnimeHubApi.Models
{
    public class Anime
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Genre { get; set; }
        public int Episodes { get; set; }
        public int YearPublished { get; set; }
    }
}
