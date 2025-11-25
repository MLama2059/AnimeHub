using AnimeHub.Shared.Models;
using AnimeHub.Shared.Models.Dtos.Studio;
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
    public class StudioController : ControllerBase
    {
        private readonly IStudioRepository _studioRepository;
        public StudioController(IStudioRepository studioRepository)
        {
            _studioRepository = studioRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<StudioReadDto>>> GetStudios()
        {
            var studios = await _studioRepository.GetAllAsync();

            if (studios is null)
            {
                return NotFound();
            }

            return Ok(studios);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StudioReadDto>> GetStudioById(int id)
        {
            var studio = await _studioRepository.GetByIdAsync(id);
            if (studio is null)
                return NotFound();

            return Ok(studio);
        }

        [Authorize(Roles = RoleConstants.Admin)]
        [HttpPost]
        public async Task<ActionResult<StudioReadDto>> AddStudio(StudioUpsertDto createDto)
        {
            if (createDto is null)
                return BadRequest();

            var createdStudio = await _studioRepository.AddAsync(createDto);

            return CreatedAtAction(nameof(GetStudioById), new { id = createdStudio.Id }, createdStudio);
        }

        [Authorize(Roles = RoleConstants.Admin)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudio(int id, StudioUpsertDto updateDto)
        {
            var updatedStudio = await _studioRepository.UpdateAsync(id, updateDto);

            if (!updatedStudio)
            {
                return BadRequest("Update failed");
            }

            return NoContent();
        }

        [Authorize(Roles = RoleConstants.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudio(int id)
        {
            var deletedStudio = await _studioRepository.DeleteAsync(id);
            if (!deletedStudio)
                return BadRequest("Delete failed");

            return NoContent();
        }
    }
}
