using AnimeHub.Shared.Models.Dtos.Rating;
using AnimeHubApi.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AnimeHubApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly IRatingRepository _ratingRepository;

        public RatingController(IRatingRepository ratingRepository)
        {
            _ratingRepository = ratingRepository;
        }

        // Public endpoint: Anyone can see reviews for an anime
        [HttpGet("{animeId}")]
        public async Task<IActionResult> GetRatingsForAnime(int animeId)
        {
            var ratings = await _ratingRepository.GetRatingsAsync(animeId);
            return Ok(ratings);
        }

        // Protected endpoint: Gets the logged-in user's specific rating for this anime
        // (Used to fill in the stars if they revisited the page)
        [Authorize]
        [HttpGet("user/{animeId}")]
        public async Task<IActionResult> GetUserRating(int animeId)
        {
            // Extract current User ID from the JWT Token
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return Unauthorized();
            }

            var rating = await _ratingRepository.GetUserRatingsAsync(userId, animeId);

            if (rating == null)
            {
                return NoContent();
            }

            return Ok(rating);
        }

        // Protected endpoint: Submit or Update a rating
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpsertRating([FromBody] RatingCreateDto ratingDto)
        {
            // Security: We get the ID from the token, NOT the request body
            var userId = GetCurrentUserId();
            if (userId == 0)
            {
                return Unauthorized();
            }

            try
            {
                var result = await _ratingRepository.AddOrUpdateRatingAsync(userId, ratingDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error saving rating: {ex.Message}");
            }
        }

        // Helper Method to get User ID from Token
        private int GetCurrentUserId()
        {
            // "User" is a built-in property in the Controller that holds the parsed JWT claims
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (idClaim != null && int.TryParse(idClaim.Value, out int userId))
            {
                return userId;
            }
            return 0; // Failed to find ID
        }
    }
}
