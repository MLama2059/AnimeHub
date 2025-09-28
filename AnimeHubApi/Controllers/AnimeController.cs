using AnimeHubApi.Data;
using AnimeHub.Shared.Models;
using AnimeHubApi.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AnimeHub.Shared.Models.Dtos;
using AnimeHub.Shared.Models.Dtos.Anime;
using AnimeHub.Shared.Models.Enums;

namespace AnimeHubApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimeController : ControllerBase
    {
        private readonly IAnimeRepository _animeRepository;

        public AnimeController(IAnimeRepository animeRepository) => _animeRepository = animeRepository;

        [HttpGet]
        public async Task<ActionResult<List<AnimeReadDto>>> GetAnimes()
        {
            var animes = await _animeRepository.GetAllAsync();

            // Manual mapping to DTO
            var animeDtos = animes.Select(a => new AnimeReadDto
            {
                Id = a.Id,
                Title = a.Title,
                Episodes = a.Episodes,
                Season = a.Season.ToString(), // (Enum to String)
                PremieredYear = a.PremieredYear,
                Description = a.Description,
                ImageUrl = a.ImageUrl,
                Rating = a.Rating,
                Status = a.Status.ToString(), // (Enum to String)
                CategoryId = a.CategoryId,
                CategoryName = a.Category?.Name ?? string.Empty,
                Genres = a.AnimeGenres?.Select(ag => ag.Genre.Name).ToList() ?? new List<string>(),
                GenreIds = a.AnimeGenres?.Select(ag => ag.GenreId).ToList() ?? new List<int>(),
                Studios = a.AnimeStudios?.Select(ast => ast.Studio.Name).ToList() ?? new List<string>(),
                StudioIds = a.AnimeStudios?.Select(ast => ast.StudioId).ToList() ?? new List<int>(),
            }).ToList();

            return Ok(animeDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Anime>> GetAnimeById(int id)
        {
            var anime = await _animeRepository.GetByIdAsync(id);
            if (anime is null)
                return NotFound();

            var animeDto = new AnimeReadDto
            {
                Id = anime.Id,
                Title = anime.Title,
                Episodes = anime.Episodes,
                Season = anime.Season.ToString(), // (Enum to String)
                PremieredYear = anime.PremieredYear,
                Description = anime.Description,
                ImageUrl = anime.ImageUrl,
                Rating = anime.Rating,
                Status = anime.Status.ToString(), // (Enum to String)
                CategoryId = anime.CategoryId,
                CategoryName = anime.Category?.Name ?? string.Empty,
                Genres = anime.AnimeGenres?.Select(ag => ag.Genre.Name).ToList() ?? new List<string>(),
                GenreIds = anime.AnimeGenres?.Select(ag => ag.GenreId).ToList() ?? new List<int>(),
                Studios = anime.AnimeStudios?.Select(ast => ast.Studio.Name).ToList() ?? new List<string>(),
                StudioIds = anime.AnimeStudios?.Select(ast => ast.StudioId).ToList() ?? new List<int>(),
            };

            return Ok(animeDto);
        }

        // GET endpoint for Top Rated Anime
        [HttpGet("top")]
        public async Task<ActionResult<IEnumerable<AnimeReadDto>>> GetTopRatedAnimes()
        {
            const int count = 10; // Number of top-rated animes to return
            var animes = await _animeRepository.GetTopRatedAnimesAsync(count);

            var animeDtos = animes.Select(a => new AnimeReadDto
            {
                Id = a.Id,
                Title = a.Title,
                Episodes = a.Episodes,
                Season = a.Season.ToString(), // (Enum to String)
                PremieredYear = a.PremieredYear,
                Description = a.Description,
                ImageUrl = a.ImageUrl,
                Rating = a.Rating,
                Status = a.Status.ToString(), // (Enum to String)
                CategoryId = a.CategoryId,
                CategoryName = a.Category?.Name ?? string.Empty,
                Genres = a.AnimeGenres?.Select(ag => ag.Genre.Name).ToList() ?? new List<string>(),
                GenreIds = a.AnimeGenres?.Select(ag => ag.GenreId).ToList() ?? new List<int>(),
                Studios = a.AnimeStudios?.Select(ast => ast.Studio.Name).ToList() ?? new List<string>(),
                StudioIds = a.AnimeStudios?.Select(ast => ast.StudioId).ToList() ?? new List<int>(),
            }).ToList();

            return Ok(animeDtos);
        }

        [HttpPost]
        public async Task<ActionResult<AnimeReadDto>> AddAnime([FromBody] AnimeCreateDto animeDto)
        {
            if (animeDto is null)
                return BadRequest();

            var anime = new Anime
            {
                Title = animeDto.Title,
                Episodes = animeDto.Episodes,
                Season = (Season)animeDto.Season,
                PremieredYear = animeDto.PremieredYear,
                Description = animeDto.Description,
                ImageUrl = animeDto.ImageUrl,
                Rating = animeDto.Rating,
                Status = (Status)animeDto.Status,
                CategoryId = animeDto.CategoryId,
                // Initialize collections
                AnimeGenres = new List<AnimeGenre>(),
                AnimeStudios = new List<AnimeStudio>()
            };

            var createdAnime = await _animeRepository.AddAsync(anime, animeDto.GenreIds, animeDto.StudioIds);

            if (createdAnime == null)
                return BadRequest("Anime creation failed");

            // Map back to read DTO
            var readDto = new AnimeReadDto
            {
                Id = createdAnime.Id,
                Title = createdAnime.Title,
                Episodes = createdAnime.Episodes,
                Season = createdAnime.Season.ToString(),
                PremieredYear = createdAnime.PremieredYear,
                Description = createdAnime.Description,
                ImageUrl = createdAnime.ImageUrl,
                Rating = createdAnime.Rating,
                Status = createdAnime.Status.ToString(),
                CategoryName = createdAnime.Category?.Name ?? string.Empty,
                Genres = createdAnime.AnimeGenres?.Select(ag => ag.Genre.Name).ToList() ?? new List<string>(),
                Studios = createdAnime.AnimeStudios?.Select(ast => ast.Studio.Name).ToList() ?? new List<string>(),
            };

            return CreatedAtAction(nameof(GetAnimeById), new { id = readDto.Id }, readDto);
        }

        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage(IFormFile file, [FromQuery] string? oldImageUrl)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file selected.");

            // Delete the old file if it exists
            if (!string.IsNullOrEmpty(oldImageUrl))
            {
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), oldImageUrl.Replace("/", Path.DirectorySeparatorChar.ToString()));
                if (System.IO.File.Exists(oldFilePath))
                {
                    try
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Could not delete old image file: {ex.Message}");
                    }
                }
            }

            // Ensure folder exists
            var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "Images", "Animes");
            if (!Directory.Exists(uploadFolder))
                Directory.CreateDirectory(uploadFolder);

            // Generate unique filename
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadFolder, fileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return the relative path
            var relativePath = Path.Combine("Images", "Animes", fileName).Replace("\\", "/");
            return Ok(relativePath);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAnime(int id, [FromBody] AnimeUpdateDto animeDto)
        {
            if (!_animeRepository.Exists(id))
                return NotFound();

            var anime = new Anime
            {
                Id = id,
                Title = animeDto.Title,
                Episodes = animeDto.Episodes,
                Season = (Season)animeDto.Season,
                PremieredYear = animeDto.PremieredYear,
                Description = animeDto.Description,
                ImageUrl = animeDto.ImageUrl,
                Rating = animeDto.Rating,
                Status = (Status)animeDto.Status,
                CategoryId = animeDto.CategoryId,
                // Initialize collections
                AnimeGenres = new List<AnimeGenre>(),
                AnimeStudios = new List<AnimeStudio>()
            };

            var success = await _animeRepository.UpdateAsync(anime, animeDto.GenreIds, animeDto.StudioIds);

            if (!success)
                return BadRequest("Update failed");

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnime(int id)
        {
            var success = await _animeRepository.DeleteAsync(id);
            if (!success)
                return NotFound();

            return NoContent();
        }
    }
}
