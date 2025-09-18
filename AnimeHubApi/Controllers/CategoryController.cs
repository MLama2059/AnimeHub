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

            var categoryDtos = categories.Select(c => new CategoryReadDto
            {
                Id = c.Id,
                Name = c.Name
            }).ToList();

            return Ok(categoryDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryReadDto>> GetCategoryById(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category is null)
            {
                return NotFound();
            }

            var categoryDto = new CategoryReadDto
            {
                Id = category.Id,
                Name = category.Name
            };

            return Ok(categoryDto);
        }

        [HttpPost]
        public async Task<ActionResult<CategoryReadDto>> AddCategory(CategoryCreateDto categoryDto)
        {
            if (categoryDto is null)
            {
                return BadRequest();
            }

            var category = new Category
            {
                Name = categoryDto.Name
            };

            var createdCategory = await _categoryRepository.AddAsync(category);

            var readDto = new CategoryReadDto
            {
                Id = createdCategory.Id,
                Name = createdCategory.Name
            };

            return CreatedAtAction(nameof(GetCategoryById), new { id = readDto.Id }, readDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, CategoryUpdateDto categoryDto)
        {
            if (!_categoryRepository.Exists(id))
            {
                return NotFound();
            }

            var category = new Category
            {
                Id = id,
                Name = categoryDto.Name
            };

            var success = await _categoryRepository.UpdateAsync(category);
            if (!success)
            {
                return BadRequest("Update failed");
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var success = await _categoryRepository.DeleteAsync(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
