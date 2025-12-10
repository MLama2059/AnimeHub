using AnimeHub.Shared.Models;
using AnimeHub.Shared.Models.Dtos;
using AnimeHub.Shared.Models.Dtos.Anime;
using AnimeHub.Shared.Models.Dtos.User;
using AnimeHub.Shared.Models.Enums;
using AnimeHubApi.Data;
using AnimeHubApi.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

namespace AnimeHubApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimeController : ControllerBase
    {
        private readonly IAnimeRepository _animeRepository;
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AnimeController(IAnimeRepository animeRepository, IFileService fileService, IWebHostEnvironment webHostEnvironment)
        {
            _animeRepository = animeRepository;
            _fileService = fileService;
            _webHostEnvironment = webHostEnvironment;
        }

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
            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metadata));

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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdAnime = await _animeRepository.AddAsync(animeDto);

            if (createdAnime == null)
                return BadRequest("Failed to create anime.");

            return CreatedAtAction(nameof(GetAnimeById), new { id = createdAnime.Id }, createdAnime);
        }

        // Helper method to encapsulate upload logic
        private async Task<IActionResult> UploadFileInternal(IFormFile file, string? oldFileUrl, string folderName, string[] allowedMimePrefixes)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file selected.");

            // Type Validation
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(file.FileName, out var contentType) || !allowedMimePrefixes.Any(prefix => contentType.StartsWith(prefix)))
            {
                return BadRequest($"Invalid file type. Allowed types: {string.Join(", ", allowedMimePrefixes)}");
            }

            // 1. Delete the old file using the service
            _fileService.DeleteFile(oldFileUrl);

            // 2. Save the new file
            var relativeFolder = Path.Combine(folderName);
            var uploadFolder = Path.Combine(_webHostEnvironment.ContentRootPath, relativeFolder);

            if (!Directory.Exists(uploadFolder))
                Directory.CreateDirectory(uploadFolder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // 3. Return the relative path
            var relativePath = Path.Combine(relativeFolder, fileName).Replace("\\", "/");
            return Ok(relativePath);
        }

        [Authorize(Roles = RoleConstants.Admin)]
        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage(IFormFile file, [FromQuery] string? oldImageUrl)
        {
            return await UploadFileInternal(file, oldImageUrl, Path.Combine("Images", "Animes"), new[] { "image/" });
        }

        [Authorize(Roles = RoleConstants.Admin)]
        [HttpPost("upload-trailer-poster")]
        public async Task<IActionResult> UploadTrailerPoster(IFormFile file, [FromQuery] string? oldPosterUrl)
        {
            return await UploadFileInternal(file, oldPosterUrl, Path.Combine("Images", "Posters"), new[] { "image/" });
        }

        [Authorize(Roles = RoleConstants.Admin)]
        [DisableRequestSizeLimit]
        [RequestSizeLimit(104857600)]
        [HttpPost("upload-trailer")]
        public async Task<IActionResult> UploadTrailer(IFormFile file, [FromQuery] string? oldTrailerUrl)
        {
            return await UploadFileInternal(file, oldTrailerUrl, Path.Combine("Videos", "Trailers"), new[] { "video/" });
        }

        [Authorize(Roles = RoleConstants.Admin)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAnime(int id, [FromBody] AnimeUpdateDto animeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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
