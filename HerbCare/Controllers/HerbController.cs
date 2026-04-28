using HerbCare.Data;
using HerbCare.DTOs;
using HerbCare.DTOs.HerbDTOs;
using HerbCare.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HerbCare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HerbController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public HerbController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponseDTO<IEnumerable<HerbDTO>>>> GetHerbs(
            [FromQuery] int? categoryId,
            [FromQuery] string? searchTerm)
        {
            try
            {
                var query = _context.Herbs
                    .Include(h => h.Category)
                    .AsQueryable();

                if (categoryId.HasValue)
                {
                    query = query.Where(h => h.CategoryId == categoryId);
                }

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(h =>
                        h.Name.Contains(searchTerm) ||
                        h.Benefits.Contains(searchTerm));
                }

                var herbs = await query
                .Select(h => new HerbDTO
                {
                    HerbId = h.HerbId,
                    Name = h.Name,
                    ScientificName = h.ScientificName, 
                    ImageLink = h.ImageLink,            
                    Benefits = h.Benefits,
                    Price = h.Price,
                    Description = h.Description,
                    SideEffects = h.SideEffects,
                    CategoryId = h.CategoryId,
                    CategoryName = h.Category.Name
                })
                .ToListAsync();

                return Ok(new ApiResponseDTO<IEnumerable<HerbDTO>>
                {
                    Success = true,
                    Data = herbs
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDTO<IEnumerable<HerbDTO>>
                {
                    Success = false,
                    Message = "An error occurred",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDTO<HerbDTO>>> GetHerb(int id)
        {
            try
            {
                var herb = await _context.Herbs
                    .Include(h => h.Category)
                    .FirstOrDefaultAsync(h => h.HerbId == id);

                if (herb == null)
                {
                    return NotFound(new ApiResponseDTO<HerbDTO>
                    {
                        Success = false,
                        Message = "Herb not found"
                    });
                }

                var herbDTO = new HerbDTO
                {
                    HerbId = herb.HerbId,
                    Name = herb.Name,
                    ScientificName = herb.ScientificName,
                    ImageLink = herb.ImageLink,           
                    Benefits = herb.Benefits,
                    Price = herb.Price,
                    Description = herb.Description,
                    SideEffects = herb.SideEffects,
                    CategoryId = herb.CategoryId,
                    CategoryName = herb.Category?.Name
                };

                return Ok(new ApiResponseDTO<HerbDTO>
                {
                    Success = true,
                    Data = herbDTO
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDTO<HerbDTO>
                {
                    Success = false,
                    Message = "An error occurred",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpGet("categories")]
        public async Task<ActionResult<ApiResponseDTO<IEnumerable<CategoryDTO>>>> GetCategories()
        {
            try
            {
                var categories = await _context.Categories
                    .Select(c => new CategoryDTO
                    {
                        CategoryId = c.CategoryId,
                        Name = c.Name,
                        Description = c.Description
                    })
                    .ToListAsync();

                return Ok(new ApiResponseDTO<IEnumerable<CategoryDTO>>
                {
                    Success = true,
                    Data = categories
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDTO<IEnumerable<CategoryDTO>>
                {
                    Success = false,
                    Message = "An error occurred",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}