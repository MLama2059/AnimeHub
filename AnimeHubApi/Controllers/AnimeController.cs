using AnimeHubApi.Data;
using AnimeHub.Shared.Models;
using AnimeHubApi.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AnimeHub.Shared.Models.Dtos;
using AnimeHub.Shared.Models.Dtos.Anime;

namespace AnimeHubApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimeController : ControllerBase
    {
        private readonly IAnimeRepository _animeRepository;

        public AnimeController(IAnimeRepository animeRepository)
        {
            _animeRepository = animeRepository;
        }

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
                YearPublished = a.YearPublished,
                Description = a.Description,
                Author = a.Author,
                ImageUrl = a.ImageUrl,
                Rating = a.Rating,
                CategoryId = a.CategoryId,
                CategoryName = a.Category?.Name ?? string.Empty,
                Genres = a.AnimeGenres?.Select(ag => ag.Genre.Name).ToList() ?? new List<string>(),
                GenreIds = a.AnimeGenres?.Select(ag => ag.GenreId).ToList() ?? new List<int>()
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
                YearPublished = anime.YearPublished,
                Description = anime.Description,
                Author = anime.Author,
                ImageUrl = anime.ImageUrl,
                Rating = anime.Rating,
                CategoryId = anime.CategoryId,
                CategoryName = anime.Category?.Name ?? string.Empty,
                Genres = anime.AnimeGenres?.Select(ag => ag.Genre.Name).ToList() ?? new List<string>(),
                GenreIds = anime.AnimeGenres?.Select(ag => ag.GenreId).ToList() ?? new List<int>()
            };

            return Ok(animeDto);
        }

        [HttpPost]
        public async Task<ActionResult<AnimeReadDto>> AddAnime([FromBody]AnimeCreateDto animeDto)
        {
            if (animeDto is null)
                return BadRequest();

            var anime = new Anime
            {
                Title = animeDto.Title,
                Episodes = animeDto.Episodes,
                YearPublished = animeDto.YearPublished,
                Description = animeDto.Description,
                Author = animeDto.Author,
                ImageUrl = animeDto.ImageUrl,
                Rating = animeDto.Rating,
                CategoryId = animeDto.CategoryId
            };

            var createdAnime = await _animeRepository.AddAsync(anime, animeDto.GenreIds);
            if (createdAnime == null)
                return BadRequest("Anime creation failed");

            // Map back to read DTO
            var readDto = new AnimeReadDto
            {
                Id = createdAnime.Id,
                Title = createdAnime.Title,
                Episodes = createdAnime.Episodes,
                YearPublished = createdAnime.YearPublished,
                Description = createdAnime.Description,
                Author = createdAnime.Author,
                ImageUrl = createdAnime.ImageUrl,
                Rating = createdAnime.Rating,
                CategoryName = createdAnime.Category?.Name ?? string.Empty,
                Genres = createdAnime.AnimeGenres?
                    .Where(ag => ag.Genre != null)
                    .Select(ag => ag.Genre.Name)
                    .ToList() ?? new List<string>()
            };

            return CreatedAtAction(nameof(GetAnimeById), new { id = readDto.Id }, readDto);
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
                YearPublished = animeDto.YearPublished,
                Description = animeDto.Description,
                Author = animeDto.Author,
                ImageUrl = animeDto.ImageUrl,
                Rating = animeDto.Rating,
                CategoryId = animeDto.CategoryId
            };

            var success = await _animeRepository.UpdateAsync(anime, animeDto.GenreIds);
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
