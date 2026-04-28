using HerbCare.Data;
using HerbCare.DTOs;
using HerbCare.DTOs.AuthDTOs;
using HerbCare.DTOs.UserDTOs;
using HerbCare.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HerbCare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager; 

        public UserController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager; 
        }

        [HttpGet("profile")]
        public async Task<ActionResult<ApiResponseDTO<object>>> GetProfile()
        {
            try
            {
                var userIdClaim = User.FindFirst("UserId")?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return BadRequest(new ApiResponseDTO<object>
                    {
                        Success = false,
                        Message = "Invalid user ID"
                    });
                }

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    return NotFound(new ApiResponseDTO<object>
                    {
                        Success = false,
                        Message = "User not found"
                    });
                }

                if (user.UserType == UserType.Doctor)
                {
                    var doctorProfile = new
                    {
                        user.Id,
                        user.FirstName,
                        user.LastName,
                        user.Email,
                        user.Phone,
                        user.Specialty,
                        user.Rating,
                        user.UserType
                    };

                    return Ok(new ApiResponseDTO<object>
                    {
                        Success = true,
                        Data = doctorProfile
                    });
                }
                else
                {
                    var userProfile = new
                    {
                        user.Id,
                        user.FirstName,
                        user.LastName,
                        user.Email,
                        user.BirthDate,
                        user.Gender,
                        user.Address,
                        user.UserType
                    };

                    return Ok(new ApiResponseDTO<object>
                    {
                        Success = true,
                        Data = userProfile
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDTO<object>
                {
                    Success = false,
                    Message = "An error occurred",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpGet("doctors")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponseDTO<IEnumerable<object>>>> GetDoctors()
        {
            try
            {
                var doctors = await _context.Users
                    .Where(u => u.UserType == UserType.Doctor)
                    .Select(u => new
                    {
                        u.Id,
                        u.FirstName,
                        u.LastName,
                        u.Email,
                        u.Phone,
                        u.Specialty,
                        u.Rating
                    })
                    .ToListAsync();

                return Ok(new ApiResponseDTO<IEnumerable<object>>
                {
                    Success = true,
                    Data = doctors
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDTO<IEnumerable<object>>
                {
                    Success = false,
                    Message = "An error occurred",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpGet("doctor/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponseDTO<object>>> GetDoctor(int id)
        {
            try
            {
                var doctor = await _context.Users
                    .Where(u => u.UserType == UserType.Doctor && u.Id == id)
                    .Select(u => new
                    {
                        u.Id,
                        u.FirstName,
                        u.LastName,
                        u.Email,
                        u.Phone,
                        u.Specialty,
                        u.Rating
                    })
                    .FirstOrDefaultAsync();

                if (doctor == null)
                {
                    return NotFound(new ApiResponseDTO<object>
                    {
                        Success = false,
                        Message = "Doctor not found"
                    });
                }

                return Ok(new ApiResponseDTO<object>
                {
                    Success = true,
                    Data = doctor
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDTO<object>
                {
                    Success = false,
                    Message = "An error occurred",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPut("profile")]
        public async Task<ActionResult<ApiResponseDTO<UserDTO>>> UpdateProfile(UpdateUserDTO model)
        {
            try
            {
                var userIdClaim = User.FindFirst("UserId")?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return BadRequest(new ApiResponseDTO<UserDTO>
                    {
                        Success = false,
                        Message = "Invalid user ID"
                    });
                }

                var user = await _userManager.FindByIdAsync(userId.ToString());

                if (user == null)
                {
                    return NotFound(new ApiResponseDTO<UserDTO>
                    {
                        Success = false,
                        Message = "User not found"
                    });
                }

                // Only allow updating if user is a regular user (not doctor)
                if (user.UserType != UserType.User)
                {
                    return BadRequest(new ApiResponseDTO<UserDTO>
                    {
                        Success = false,
                        Message = "Doctors cannot update these profile fields"
                    });
                }

                user.FirstName = model.FirstName ?? user.FirstName;
                user.LastName = model.LastName ?? user.LastName;
                user.BirthDate = model.BirthDate ?? user.BirthDate;
                user.Gender = model.Gender ?? user.Gender;
                user.Address = model.Address ?? user.Address;

                var result = await _userManager.UpdateAsync(user); 

                if (!result.Succeeded)
                {
                    return BadRequest(new ApiResponseDTO<UserDTO>
                    {
                        Success = false,
                        Message = "Update failed",
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    });
                }

                var userDTO = new UserDTO
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    BirthDate = user.BirthDate,
                    Gender = user.Gender,
                    Address = user.Address,
                };

                return Ok(new ApiResponseDTO<UserDTO>
                {
                    Success = true,
                    Message = "Profile updated successfully",
                    Data = userDTO
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDTO<UserDTO>
                {
                    Success = false,
                    Message = "An error occurred",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}