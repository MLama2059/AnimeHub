using AnimeHubApi.Data;
using AnimeHub.Shared.Models;
using AnimeHubApi.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AnimeHub.Shared.Models.Dtos;
using AnimeHub.Shared.Models.Dtos.Anime;
using AnimeHub.Shared.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using AnimeHub.Shared.Models.Dtos.User;
using Microsoft.AspNetCore.StaticFiles;

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

        [Authorize(Roles = RoleConstants.Admin)]
        [HttpPost]
        public async Task<IActionResult> AddAnime([FromBody] AnimeCreateDto animeDto)
        {
            var createdAnime = await _animeRepository.AddAsync(animeDto);

            if (createdAnime == null)
                return BadRequest("Failed to create anime.");

            return CreatedAtAction(nameof(GetAnimeById), new { id = createdAnime.Id }, createdAnime);
        }

        [Authorize(Roles = RoleConstants.Admin)]
        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage(IFormFile file, [FromQuery] string? oldImageUrl)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file selected.");

            // Validate that it is an Image
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(file.FileName, out var contentType) || !contentType.StartsWith("image/"))
            {
                return BadRequest("Invalid file type. Please upload an image.");
            }

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

        [Authorize(Roles = RoleConstants.Admin)]
        [HttpPost("upload-trailer-poster")]
        public async Task<IActionResult> UploadTrailerPoster(IFormFile file, [FromQuery] string? oldPosterUrl)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file selected.");

            // Validate that it is an Image
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(file.FileName, out var contentType) || !contentType.StartsWith("image/"))
            {
                return BadRequest("Invalid file type. Please upload an image.");
            }

            // Delete the old file if it exists
            if (!string.IsNullOrEmpty(oldPosterUrl))
            {
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), oldPosterUrl.Replace("/", Path.DirectorySeparatorChar.ToString()));
                if (System.IO.File.Exists(oldFilePath))
                {
                    try
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Could not delete old poster file: {ex.Message}");
                    }
                }
            }

            // Ensure folder exists
            var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "Images", "Posters");
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
            var relativePath = Path.Combine("Images", "Posters", fileName).Replace("\\", "/");
            return Ok(relativePath);
        }

        [Authorize(Roles = RoleConstants.Admin)]
        [HttpPost("upload-trailer")]
        public async Task<IActionResult> UploadTrailer(IFormFile file, [FromQuery] string? oldTrailerUrl)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file selected.");

            // Validate that it is a Video
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(file.FileName, out var contentType) || !contentType.StartsWith("video/"))
            {
                return BadRequest("Invalid file type. Please upload a video.");
            }

            // Delete the old file if it exists
            if (!string.IsNullOrEmpty(oldTrailerUrl))
            {
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), oldTrailerUrl.Replace("/", Path.DirectorySeparatorChar.ToString()));
                if (System.IO.File.Exists(oldFilePath))
                {
                    try
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Could not delete old trailer file: {ex.Message}");
                    }
                }
            }

            // Ensure folder exists
            var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "Videos", "Trailers");
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
            var relativePath = Path.Combine("Videos", "Trailers", fileName).Replace("\\", "/");
            return Ok(relativePath);
        }

        [Authorize(Roles = RoleConstants.Admin)]
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

        [Authorize(Roles = RoleConstants.Admin)]
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
