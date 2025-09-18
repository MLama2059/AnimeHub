using AnimeHub.Shared.Models;
using AnimeHubApi.Repository;
using AnimeHubApi.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AnimeHubApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly IGenreRepository _genreRepository;

        public GenreController(IGenreRepository genreRepository)
        {
            _genreRepository = genreRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<Genre>>> GetCategory()
        {
            var genres = await _genreRepository.GetAllAsync();
            return Ok(genres);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Genre>> GetCategoryById(int id)
        {
            var genre = await _genreRepository.GetByIdAsync(id);
            if (genre is null)
            {
                return NotFound();
            }

            return Ok(genre);
        }

        [HttpPost]
        public async Task<ActionResult<Genre>> AddCategory(Genre newGenre)
        {
            if (newGenre is null)
            {
                return BadRequest();
            }

            var createdGenre = await _genreRepository.AddAsync(newGenre);
            return CreatedAtAction(nameof(GetCategoryById), new { id = createdGenre.Id }, createdGenre);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, Genre updatedGenre)
        {
            if (!_genreRepository.Exists(id))
            {
                return NotFound();
            }

            updatedGenre.Id = id;
            var success = await _genreRepository.UpdateAsync(updatedGenre);
            if (!success)
            {
                return BadRequest();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var success = await _genreRepository.DeleteAsync(id);
            if (!success)
            {
                return BadRequest();
            }
            return NoContent();
        }
    }
}
