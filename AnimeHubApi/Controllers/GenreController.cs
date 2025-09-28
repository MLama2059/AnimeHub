using AnimeHub.Shared.Models;
using AnimeHub.Shared.Models.Dtos.Genre;
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
        public async Task<ActionResult<List<GenreReadDto>>> GetGenres()
        {
            var genres = await _genreRepository.GetAllAsync();

            var genreDtos = genres.Select(g => new GenreReadDto
            {
                Id = g.Id,
                Name = g.Name
            }).ToList();

            return Ok(genreDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GenreReadDto>> GetGenreById(int id)
        {
            var genre = await _genreRepository.GetByIdAsync(id);
            if (genre is null)
            {
                return NotFound();
            }

            var dto = new GenreReadDto { Id = genre.Id, Name = genre.Name };

            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<GenreReadDto>> AddGenre(GenreUpsertDto genreDto)
        {
            if (genreDto is null)
            {
                return BadRequest();
            }

            var genre = new Genre { Name = genreDto.Name };
            var createdGenre = await _genreRepository.AddAsync(genre);

            var dto = new GenreReadDto { Id = createdGenre.Id, Name = createdGenre.Name };
            return CreatedAtAction(nameof(GetGenreById), new { id = dto.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGenre(int id, GenreUpsertDto genreDto)
        {
            if (!_genreRepository.Exists(id))
            {
                return NotFound();
            }

            var genre = new Genre { Id = id, Name = genreDto.Name };
            var success = await _genreRepository.UpdateAsync(genre);
            if (!success)
            {
                return BadRequest("Update failed");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGenre(int id)
        {
            var success = await _genreRepository.DeleteAsync(id);
            if (!success)
            {
                return BadRequest("Delete failed");
            }
            return NoContent();
        }
    }
}
