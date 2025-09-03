using AnimeHubApi.Data;
using AnimeHubApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AnimeHubApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimeController : ControllerBase
    {
        private readonly AnimeDbContext _context;
        public AnimeController(AnimeDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Anime>>> GetAnime()
        {
            return Ok(await _context.Animes.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Anime>> GetAnimeById(int id)
        {
            var anime = await _context.Animes.FindAsync(id);
            if (anime is null)
                return NotFound();

            return Ok(anime);
        }

        [HttpPost]
        public async Task<ActionResult<Anime>> AddAnime(Anime newAnime)
        {
            if (newAnime is null)
                return BadRequest();

            _context.Animes.Add(newAnime);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAnimeById), new { id = newAnime.Id }, newAnime);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAnime(int id, Anime updatedAnime)
        {
            var anime = await _context.Animes.FindAsync(id);
            if (anime is null)
                return NotFound();

            anime.Id = updatedAnime.Id;
            anime.Title = updatedAnime.Title;
            anime.Genre = updatedAnime.Genre;
            anime.Episodes = updatedAnime.Episodes;
            anime.YearPublished = updatedAnime.YearPublished;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnime(int id)
        {
            var anime = await _context.Animes.FindAsync(id);
            if (anime is null)
                return NotFound();

            _context.Animes.Remove(anime);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
