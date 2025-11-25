using AnimeHub.Shared.Models;
using AnimeHub.Shared.Models.Dtos.Genre;
using AnimeHub.Shared.Models.Dtos.User;
using AnimeHubApi.Repository;
using AnimeHubApi.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
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
        public async Task<ActionResult<List<GenreReadDto>>> GetGenres()
        {
            var genres = await _genreRepository.GetAllAsync();

            if (genres is null)
            {
                return NotFound();
            }

            return Ok(genres);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GenreReadDto>> GetGenreById(int id)
        {
            var genre = await _genreRepository.GetByIdAsync(id);
            if (genre is null)
            {
                return NotFound();
            }

            return Ok(genre);
        }

        [Authorize(Roles = RoleConstants.Admin)]
        [HttpPost]
        public async Task<ActionResult<GenreReadDto>> AddGenre(GenreUpsertDto createDto)
        {
            if (createDto is null)
            {
                return BadRequest();
            }

            var createdGenre = await _genreRepository.AddAsync(createDto);

            return CreatedAtAction(nameof(GetGenreById), new { id = createdGenre.Id }, createdGenre);
        }

        [Authorize(Roles = RoleConstants.Admin)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGenre(int id, GenreUpsertDto updateDto)
        {
            var updatedGenre = await _genreRepository.UpdateAsync(id, updateDto);
            if (!updatedGenre)
            {
                return BadRequest("Update failed");
            }

            return NoContent();
        }

        [Authorize(Roles = RoleConstants.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGenre(int id)
        {
            var deletedGenre = await _genreRepository.DeleteAsync(id);
            if (!deletedGenre)
            {
                return BadRequest("Delete failed");
            }
            return NoContent();
        }
    }
}
