using AnimeHub.Shared.Models.Dtos.Anime;
using AnimeHub.Shared.Models.Dtos.AnimeProposal;
using AnimeHub.Shared.Models.Enums;
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

        // 1. ENDPOINT: Upload Proposal File (Saves to Temp)
        // Client calls this when user selects a file in the Proposal Form
        [HttpPost("upload-temp")]
        public async Task<IActionResult> UploadTempFile(IFormFile file, [FromQuery] string type)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            try
            {
                string[] allowedExtensions;
                if (type == "video") allowedExtensions = new[] { ".mp4", ".mkv" };
                else allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };

                // Saves to Images/Temp or Videos/Temp
                var path = await _fileService.SaveProposalFileAsync(file, allowedExtensions);

                // Return the path so the Frontend can put it into the ProposalDTO
                return Ok(path);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 2. ENDPOINT: Create Proposal (Saves DTO with Temp paths to DB)
        [HttpPost]
        public async Task<IActionResult> CreateProposal([FromBody] AnimeProposalCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                // Securely get the User ID from the JWT Token
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized("Invalid User ID in token.");
                }

                // Just save the data. The paths in dto (ImageUrl, etc.) point to /Temp/
                var createdProposal = await _proposalRepository.CreateProposalAsync(userId, dto);
                return CreatedAtAction(nameof(GetProposalById), new { id = createdProposal.Id }, createdProposal);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // 3. GET ALL (Pending Proposals)
        [HttpGet]
        // [Authorize(Roles = "Admin")] // Only admins should see the list
        public async Task<IActionResult> GetAllProposals()
        {
            var proposals = await _proposalRepository.GetAllProposalsAsync();
            return Ok(proposals);
        }

        // 4. GET BY ID
        [HttpGet("{id}")]
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetProposalById(int id)
        {
            var proposal = await _proposalRepository.GetProposalByIdAsync(id);
            if (proposal == null) return NotFound();
            return Ok(proposal);
        }

        // 5. ENDPOINT: Approve Proposal
        // This is the CRITICAL step. It moves files and creates the actual Anime.
        [HttpPost("{id}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveProposal(int id)
        {
            var success = await _proposalRepository.ApproveProposalAsync(id);

            if (!success)
            {
                return BadRequest("Could not approve proposal. It may not exist, is not 'Pending', or the target anime was not found.");
            }

            return Ok(new { Message = "Proposal approved and Anime created/updated successfully." });
        }

        [HttpPost("{id}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RejectProposal(int id, [FromBody] string feedback)
        {
            var success = await _proposalRepository.RejectProposalAsync(id, feedback);

            if (!success)
            {
                return NotFound("Proposal not found.");
            }

            return Ok(new { message = "Proposal rejected successfully." });
        }

    }
}
