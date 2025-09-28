using AnimeHub.Shared.Models;
using AnimeHub.Shared.Models.Dtos.Studio;
using AnimeHubApi.Repository;
using AnimeHubApi.Repository.IRepository;
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

            var studioDtos = studios.Select(s => new StudioReadDto
            {
                Id = s.Id,
                Name = s.Name
            }).ToList();

            return Ok(studioDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StudioReadDto>> GetStudioById(int id)
        {
            var studio = await _studioRepository.GetByIdAsync(id);
            if (studio is null)
                return NotFound();

            var dto = new StudioReadDto { Id = studio.Id, Name = studio.Name };

            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<StudioReadDto>> AddStudio(StudioUpsertDto studioDto)
        {
            if (studioDto is null)
                return BadRequest();

            var studio = new Studio { Name = studioDto.Name };
            var createdStudio = await _studioRepository.AddAsync(studio);

            var dto = new StudioReadDto { Id = createdStudio.Id, Name = createdStudio.Name };
            return CreatedAtAction(nameof(GetStudioById), new { id = dto.Id }, dto);
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> UpdateStudio(int id, StudioUpsertDto studioDto)
        {
            if (studioDto is null)
                return BadRequest();

            var studio = new Studio { Id = id, Name = studioDto.Name };
            var success = await _studioRepository.UpdateAsync(studio);

            if (!success)
            {
                return BadRequest("Update failed");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudio(int id)
        {
            var success = await _studioRepository.DeleteAsync(id);
            if (!success)
                return BadRequest("Delete failed");

            return NoContent();
        }
    }
}
