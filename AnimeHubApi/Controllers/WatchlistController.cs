using AnimeHub.Shared.Models;
using AnimeHub.Shared.Models.Dtos;
using AnimeHub.Shared.Models.Dtos.Anime;
using AnimeHub.Shared.Models.Dtos.UserAnime;
using AnimeHub.Shared.Models.Enums;
using AnimeHubApi.Data;
using AnimeHubApi.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace AnimeHubApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WatchlistController : ControllerBase
    {
        private readonly IWatchlistRepository _watchlistRepository;
        private readonly ILogger<WatchlistController> _logger;
        private readonly IConfiguration _configuration;
        public WatchlistController(IWatchlistRepository watchlistRepository, ILogger<WatchlistController> logger, IConfiguration configuration)
        {
            _watchlistRepository = watchlistRepository;
            _logger = logger;
            _configuration = configuration;
        }

        // Helper method to get user id as int
        private int GetUserId()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(idClaim) || !int.TryParse(idClaim, out int userId))
            {
                throw new UnauthorizedAccessException("Invalid User Id in token.");
            }
            return userId;
        }

        [HttpPost("{animeId}")]
        public async Task<IActionResult> AddToWatchlist(int animeId)
        {
            try
            {
                var userId = GetUserId();
                var success = await _watchlistRepository.AddToWatchlistAsync(userId, animeId);

                if (!success)
                    return BadRequest("Anime is already in your watchlist.");

                return Ok();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpDelete("{animeId}")]
        public async Task<IActionResult> RemoveFromWatchlist(int animeId)
        {
            try
            {
                var userId = GetUserId();
                var success = await _watchlistRepository.RemoveFromWatchlistAsync(userId, animeId);

                if (!success)
                    return NotFound("Anime not found in watchlist.");

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpGet("status/{animeId}")]
        public async Task<ActionResult<WatchStatus?>> GetWatchStatus(int animeId)
        {
            try
            {
                var userId = GetUserId();
                var status = await _watchlistRepository.GetWatchStatusAsync(userId, animeId);

                if (status == null)
                    return NotFound("Anime not found in your watchlist.");

                return Ok(status);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpPut("status/{animeId}/{newStatus}")]
        public async Task<IActionResult> UpdateWatchStatus(int animeId, WatchStatus newStatus)
        {
            try
            {
                var userId = GetUserId();
                var success = await _watchlistRepository.UpdateWatchStatusAsync(userId, animeId, newStatus);

                if (!success)
                    return NotFound("Anime not found in watchlist.");

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        // Retrieves the full list of Animes in the user's watchlist.
        [HttpGet]
        public async Task <ActionResult<UserAnimeReadDto>> GetMyWatchlist([FromQuery] APIParams apiParams)
        {
            var userId = GetUserId();
            // 1. Call the new Paged Repository Method
            var pagedList = await _watchlistRepository.GetMyWatchlistAsync(userId, apiParams);
            if (!pagedList.Any())
                return NotFound();

            // 2. Add the X-Pagination Header to the HTTP Response
            var metadata = new
            {
                pagedList.TotalCount,
                pagedList.PageSize,
                pagedList.CurrentPage,
                pagedList.TotalPages,
                pagedList.HasNext,
                pagedList.HasPrevious
            };

            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(metadata));

            // 3. Return the paged items
            return Ok(pagedList);
        }
    }
}
