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
        public async Task<IActionResult> GetAnimes([FromQuery] APIParams apiParams)
        {
            var pagedList = await _animeRepository.GetAllAsync(apiParams);

            if (!pagedList.Any())
                return NotFound();

            // CRITICAL: Add Pagination Metadata to the Response Headers
            // The client needs this info (TotalPages, TotalCount) to build the UI.
            var metadata = new
            {
                pagedList.TotalCount,
                pagedList.PageSize,
                pagedList.CurrentPage,
                pagedList.TotalPages,
                pagedList.HasNext,
                pagedList.HasPrevious
            };

            // Must serialize the metadata and add it to a custom response header
            Response.Headers.Append("X-Pagination", System.Text.Json.JsonSerializer.Serialize(metadata));

            return Ok(pagedList);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAnimeById(int id)
        {
            var anime = await _animeRepository.GetByIdAsync(id);
            if (anime == null)
                return NotFound();

            return Ok(anime);
        }

        // GET endpoint for Top Rated Anime
        [HttpGet("top")]
        public async Task<IActionResult> GetTopRatedAnimes()
        {
            var animes = await _animeRepository.GetTopRatedAnimesAsync(10);

            return Ok(animes);
        }

        [HttpPost]
        public async Task<IActionResult> AddAnime([FromBody] AnimeCreateDto animeDto)
        {
            var createdAnime = await _animeRepository.AddAsync(animeDto);

            if (createdAnime == null)
                return BadRequest("Failed to create anime.");

            return CreatedAtAction(nameof(GetAnimeById), new { id = createdAnime.Id }, createdAnime);
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

            var success = await _animeRepository.UpdateAsync(id, animeDto);

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
