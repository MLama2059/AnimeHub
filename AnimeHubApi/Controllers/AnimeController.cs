using AnimeHubApi.Data;
using AnimeHub.Shared.Models;
using AnimeHubApi.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AnimeHubApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimeController : ControllerBase
    {
        private readonly IAnimeRepository _animeRepository;
        private readonly ICategoryRepository _categoryRepository;

        public AnimeController(IAnimeRepository animeRepository, ICategoryRepository categoryRepository)
        {
            _animeRepository = animeRepository;
            _categoryRepository = categoryRepository;
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
        public async Task<ActionResult<Anime>> AddAnime(Anime newAnime)
        {
            if (newAnime is null)
                return BadRequest("Anime data is required");

            // Ensure category exists
            var category = await _categoryRepository.GetByIdAsync(newAnime.CategoryId);
            if (category == null)
                return BadRequest($"Category with Id {newAnime.CategoryId} does not exist");

            var createdAnime = await _animeRepository.AddAsync(newAnime);
            return CreatedAtAction(nameof(GetAnimeById), new { id = createdAnime.Id }, createdAnime);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAnime(int id, Anime updatedAnime)
        {
            if (updatedAnime == null || id != updatedAnime.Id)
                return BadRequest("Invalid anime data");

            if (!_animeRepository.Exists(id))
                return NotFound();

            // Ensure category exists
            var category = await _categoryRepository.GetByIdAsync(updatedAnime.CategoryId);
            if (category == null)
                return BadRequest($"Category with Id {updatedAnime.CategoryId} does not exist");

            var success = await _animeRepository.UpdateAsync(id, updatedAnime);
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
