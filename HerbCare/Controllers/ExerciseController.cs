using HerbCare.Data;
using HerbCare.DTOs;
using HerbCare.DTOs.AuthDTOs;
using HerbCare.DTOs.ExerciseDTOs;
using HerbCare.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace HerbCare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExerciseController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ExerciseController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponseDTO<IEnumerable<ExerciseDTO>>>> GetExercises()
        {
            try
            {
                var exercises = await _context.Exercises.ToListAsync();

                var result = exercises.Select(e => new ExerciseDTO
                {
                    ExerciseId = e.ExerciseId,
                    Title = e.Title,
                    Description = e.Description,
                    Duration = e.Duration,
                    Steps = e.Steps,
                    GifUrl = e.GifUrl,
                    IsAssigned = false
                }).ToList();

                return Ok(new ApiResponseDTO<IEnumerable<ExerciseDTO>>
                {
                    Success = true,
                    Data = result // ✅ FIX HERE
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDTO<IEnumerable<ExerciseDTO>>
                {
                    Success = false,
                    Message = "An error occurred",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpGet("my-exercises")]
        public async Task<ActionResult<ApiResponseDTO<IEnumerable<ExerciseDTO>>>> GetMyExercises()
        {
            try
            {
                var userIdClaim = User.FindFirst("UserId")?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return BadRequest(new ApiResponseDTO<IEnumerable<ExerciseDTO>>
                    {
                        Success = false,
                        Message = "Invalid user ID"
                    });
                }

                var exercises = await _context.UserExercises
                    .Where(ue => ue.UserId == userId)
                    .Include(ue => ue.Exercise)
                    .Select(ue => new ExerciseDTO
                    {
                        ExerciseId = ue.Exercise.ExerciseId,
                        Title = ue.Exercise.Title,
                        Description = ue.Exercise.Description,
                        Duration = ue.Exercise.Duration,
                        IsAssigned = true
                    })
                    .ToListAsync();

                return Ok(new ApiResponseDTO<IEnumerable<ExerciseDTO>>
                {
                    Success = true,
                    Data = exercises
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDTO<IEnumerable<ExerciseDTO>>
                {
                    Success = false,
                    Message = "An error occurred",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPost("assign")]
      
        public async Task<ActionResult<ApiResponseDTO<string>>> AssignExercise(AssignExerciseDTO model)
        {
            try
            {
                var doctorIdClaim = User.FindFirst("UserId")?.Value;

                if (!int.TryParse(doctorIdClaim, out int doctorId))
                {
                    return BadRequest(new ApiResponseDTO<string>
                    {
                        Success = false,
                        Message = "Invalid doctor ID"
                    });
                }

                // Check user
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == model.UserId && u.UserType == UserType.User);

                if (user == null)
                {
                    return BadRequest(new ApiResponseDTO<string>
                    {
                        Success = false,
                        Message = "Invalid user ID"
                    });
                }

                // Check exercise
                var exercise = await _context.Exercises
                    .FirstOrDefaultAsync(e => e.ExerciseId == model.ExerciseId);

                if (exercise == null)
                {
                    return BadRequest(new ApiResponseDTO<string>
                    {
                        Success = false,
                        Message = "Invalid exercise ID"
                    });
                }

                // Prevent duplicate assignment
                var exists = await _context.UserExercises
                    .AnyAsync(ue => ue.UserId == model.UserId && ue.ExerciseId == model.ExerciseId);

                if (exists)
                {
                    return BadRequest(new ApiResponseDTO<string>
                    {
                        Success = false,
                        Message = "Exercise already assigned to this user"
                    });
                }

                var userExercise = new UserExercise
                {
                    UserId = model.UserId,
                    ExerciseId = model.ExerciseId
                };

                _context.UserExercises.Add(userExercise);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponseDTO<string>
                {
                    Success = true,
                    Message = "Exercise assigned successfully",
                    Data = "Assigned" // ✔ better response
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

        //[HttpPost]
        //[Authorize(Roles = "Doctor")]
        //public async Task<ActionResult<ApiResponseDTO<ExerciseDTO>>> CreateExercise(CreateExerciseDTO model)
        //{
        //    try
        //    {
        //        var exercise = new Exercise
        //        {
        //            Title = model.Title,
        //            Description = model.Description,
        //            Duration = model.Duration,
        //            AssignmentExercise = model.AssignmentExercise
        //        };

        //        _context.Exercises.Add(exercise);
        //        await _context.SaveChangesAsync();

        //        var exerciseDTO = new ExerciseDTO
        //        {
        //            ExerciseId = exercise.ExerciseId,
        //            Title = exercise.Title,
        //            Description = exercise.Description,
        //            Duration = exercise.Duration,
        //            AssignmentExercise = exercise.AssignmentExercise,
        //            IsAssigned = false
        //        };

        //        return Ok(new ApiResponseDTO<ExerciseDTO>
        //        {
        //            Success = true,
        //            Message = "Exercise created successfully",
        //            Data = exerciseDTO
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new ApiResponseDTO<ExerciseDTO>
        //        {
        //            Success = false,
        //            Message = "An error occurred",
        //            Errors = new List<string> { ex.Message }
        //        });
        //    }
        //}
    }
}