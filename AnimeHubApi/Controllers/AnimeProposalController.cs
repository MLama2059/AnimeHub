using AnimeHub.Shared.Models.Dtos.AnimeProposal;
using AnimeHubApi.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AnimeHubApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AnimeProposalController : ControllerBase
    {
        private readonly IAnimeProposalRepository _proposalRepository;
        private readonly IFileService _fileService;

        public AnimeProposalController(IAnimeProposalRepository proposalRepository, IFileService fileService)
        {
            _proposalRepository = proposalRepository;
            _fileService = fileService;
        }

        [HttpPost("upload-temp")]
        public async Task<IActionResult> UploadTempFile(IFormFile file)
        {
            try
            {
                // Allowed formats for both Images and Videos
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".mp4", ".mkv" };

                var path = await _fileService.SaveFileAsync(file, allowedExtensions);

                // Return the path so the Frontend can put it into the ProposalDTO
                return Ok(new { filePath = path });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error while uploading file.");
            }
        }

        // USER ACTIONS
        [HttpPost]
        public async Task<IActionResult> CreateProposal([FromBody] AnimeProposalCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Securely get the User ID from the JWT Token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("Invalid User ID in token.");
            }

            try
            {
                var createdProposal = await _proposalRepository.CreateProposalAsync(userId, dto);
                return Ok(createdProposal);
            }
            catch (Exception)
            {
                // Log exception in real app
                return StatusCode(500, "Error creating proposal.");
            }
        }

        // ADMIN ACTIONS
        [HttpGet("pending")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPendingProposals()
        {
            var proposals = await _proposalRepository.GetPendingProposalsAsync();
            return Ok(proposals);
        }

        [HttpPost("{id}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveProposal(int id)
        {
            var result = await _proposalRepository.ApproveProposalAsync(id);

            if (!result)
            {
                return BadRequest("Proposal could not be approved. It might not exist, or is not in 'Pending' status.");
            }

            return Ok(new { message = "Proposal approved and Anime created/updated successfully." });
        }

        [HttpPost("{id}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RejectProposal(int id, [FromBody] RejectProposalDto dto)
        {
            var result = await _proposalRepository.RejectProposalAsync(id, dto.Feedback);

            if (!result)
            {
                return NotFound("Proposal not found.");
            }

            return Ok(new { message = "Proposal rejected successfully." });
        }

    }
}
