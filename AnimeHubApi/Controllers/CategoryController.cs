using AnimeHub.Shared.Models;
using AnimeHub.Shared.Models.Dtos.Category;
using AnimeHubApi.Repository;
using AnimeHubApi.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AnimeHubApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<CategoryReadDto>>> GetCategories()
        {
            var categories = await _categoryRepository.GetAllAsync();
            if (categories is null)
            {
                return NotFound();
            }

            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryReadDto>> GetCategoryById(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category is null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        [HttpPost]
        public async Task<ActionResult<CategoryReadDto>> AddCategory(CategoryCreateDto createDto)
        {
            if (createDto is null)
            {
                return BadRequest();
            }

            var createdCategory = await _categoryRepository.AddAsync(createDto);

            return CreatedAtAction(nameof(GetCategoryById), new { id = createdCategory.Id }, createdCategory);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, CategoryUpdateDto updateDto)
        {
            var updatedCategory = await _categoryRepository.UpdateAsync(id, updateDto);
            if (!updatedCategory)
            {
                return BadRequest("Update failed");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var deletedCategory = await _categoryRepository.DeleteAsync(id);
            if (!deletedCategory)
            {
                return BadRequest("Delete failed");
            }
            return NoContent();
        }
    }
}
