using AnimeHubApi.Data;
using AnimeHubApi.Models;
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

        public AnimeController(IAnimeRepository animeRepository)
        {
            _animeRepository = animeRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<Anime>>> GetAnime()
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
                return BadRequest();

            var createdAnime = await _animeRepository.AddAsync(newAnime);
            return CreatedAtAction(nameof(GetAnimeById), new { id = createdAnime.Id }, createdAnime);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAnime(int id, Anime updatedAnime)
        {
            if (!_animeRepository.Exists(id))
            {
                return NotFound();
            }

            var success = await _animeRepository.UpdateAsync(id, updatedAnime);
            if (!success)
            {
                return BadRequest();
            }

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
