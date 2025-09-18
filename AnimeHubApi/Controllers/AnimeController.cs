using AnimeHubApi.Data;
using AnimeHub.Shared.Models;
using AnimeHubApi.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AnimeHub.Shared.Models.Dtos;

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
        public async Task<ActionResult<List<Anime>>> GetAnimes()
        {
            var animes = await _animeRepository.GetAllAsync();
            return Ok(animes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Anime>> GetAnimeById(int id)
        {
            var anime = await _animeRepository.GetByIdAsync(id);
            if (anime is null)
                return NotFound();

            return Ok(anime);
        }

        [HttpPost]
        public async Task<ActionResult<Anime>> AddAnime(AnimeDto animeDto)
        {
            if (animeDto is null)
                return BadRequest();

            var anime = new Anime
            {
                Title = animeDto.Title,
                Episodes = animeDto.Episodes,
                YearPublished = animeDto.YearPublished,
                CategoryId = animeDto.CategoryId,
                Description = animeDto.Description,
                Author = animeDto.Author,
                ImageUrl = animeDto.ImageUrl,
                Rating = animeDto.Rating
            };

            var createdAnime = await _animeRepository.AddAsync(anime, animeDto.GenreIds);
            return CreatedAtAction(nameof(GetAnimeById), new { id = createdAnime.Id }, createdAnime);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAnime(int id, [FromBody] AnimeDto animeDto)
        {
            if (!_animeRepository.Exists(id))
                return NotFound();

            var anime = new Anime
            {
                Id = id,
                Title = animeDto.Title,
                Episodes = animeDto.Episodes,
                YearPublished = animeDto.YearPublished,
                CategoryId = animeDto.CategoryId,
                Description = animeDto.Description,
                Author = animeDto.Author,
                ImageUrl = animeDto.ImageUrl,
                Rating = animeDto.Rating
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
