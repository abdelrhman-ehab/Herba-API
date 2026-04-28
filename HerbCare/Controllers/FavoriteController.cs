using HerbCare.Data;
using HerbCare.DTOs;
using HerbCare.DTOs.FavoriteDTOs;
using HerbCare.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HerbCare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FavoriteController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FavoriteController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponseDTO<IEnumerable<FavoriteDTO>>>> GetFavorites()
        {
            try
            {
                var userIdClaim = User.FindFirst("UserId")?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return BadRequest(new ApiResponseDTO<IEnumerable<FavoriteDTO>>
                    {
                        Success = false,
                        Message = "Invalid user ID"
                    });
                }

                var favorites = await _context.Favorites
                    .Include(f => f.Herb)
                    .Where(f => f.UserId == userId && f.Add)
                    .OrderByDescending(f => f.Date)
                    .Select(f => new FavoriteDTO
                    {
                        FavId = f.FavId,
                        HerbId = f.HerbId,
                        HerbName = f.Herb.Name,
                        Date = f.Date
                    })
                    .ToListAsync();

                return Ok(new ApiResponseDTO<IEnumerable<FavoriteDTO>>
                {
                    Success = true,
                    Data = favorites
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDTO<IEnumerable<FavoriteDTO>>
                {
                    Success = false,
                    Message = "An error occurred",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponseDTO<FavoriteDTO>>> AddFavorite(AddFavoriteDTO model)
        {
            try
            {
                var userIdClaim = User.FindFirst("UserId")?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return BadRequest(new ApiResponseDTO<FavoriteDTO>
                    {
                        Success = false,
                        Message = "Invalid user ID"
                    });
                }

                var existingFavorite = await _context.Favorites
                    .FirstOrDefaultAsync(f => f.UserId == userId && f.HerbId == model.HerbId);

                if (existingFavorite != null)
                {
                    existingFavorite.Add = true;
                    existingFavorite.Date = DateTime.UtcNow;
                }
                else
                {
                    var favorite = new Favorite
                    {
                        UserId = userId,
                        HerbId = model.HerbId,
                        Add = true,
                        Date = DateTime.UtcNow
                    };
                    _context.Favorites.Add(favorite);
                }

                await _context.SaveChangesAsync();

                var herb = await _context.Herbs.FindAsync(model.HerbId);

                var favoriteDTO = new FavoriteDTO
                {
                    HerbId = model.HerbId,
                    HerbName = herb?.Name,
                    Date = DateTime.UtcNow
                };

                return Ok(new ApiResponseDTO<FavoriteDTO>
                {
                    Success = true,
                    Message = "Added to favorites",
                    Data = favoriteDTO
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDTO<FavoriteDTO>
                {
                    Success = false,
                    Message = "An error occurred",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpDelete("{herbId}")]
        public async Task<ActionResult<ApiResponseDTO<string>>> RemoveFavorite(int herbId)
        {
            try
            {
                var userIdClaim = User.FindFirst("UserId")?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return BadRequest(new ApiResponseDTO<string>
                    {
                        Success = false,
                        Message = "Invalid user ID"
                    });
                }

                var favorite = await _context.Favorites
                    .FirstOrDefaultAsync(f => f.UserId == userId && f.HerbId == herbId);

                if (favorite != null)
                {
                    favorite.Add = false;
                    await _context.SaveChangesAsync();
                }

                return Ok(new ApiResponseDTO<string>
                {
                    Success = true,
                    Message = "Removed from favorites"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDTO<string>
                {
                    Success = false,
                    Message = "An error occurred",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}