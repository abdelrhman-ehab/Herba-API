using HerbCare.Data;
using HerbCare.DTOs;
using HerbCare.DTOs.AuthDTOs;
using HerbCare.DTOs.ConsultationDTOs;
using HerbCare.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HerbCare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ConsultationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ConsultationController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("my-consultations")]
        public async Task<ActionResult<ApiResponseDTO<IEnumerable<ConsultationDTO>>>> GetMyConsultations()
        {
            try
            {
                var userIdClaim = User.FindFirst("UserId")?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return BadRequest(new ApiResponseDTO<IEnumerable<ConsultationDTO>>
                    {
                        Success = false,
                        Message = "Invalid user ID"
                    });
                }

                var userType = User.FindFirst("UserType")?.Value;

                IQueryable<Consultation> query = _context.Consultations
                    .Include(c => c.User)
                    .Include(c => c.Doctor);

                if (userType == UserType.Doctor.ToString())
                {
                    // Doctors see consultations where they are the doctor
                    query = query.Where(c => c.DoctorId == userId);
                }
                else
                {
                    // Regular users see their own consultations
                    query = query.Where(c => c.UserId == userId);
                }

                var consultations = await query
                    .OrderByDescending(c => c.Date)
                    .Select(c => new ConsultationDTO
                    {
                        ConId = c.ConId,
                        UserId = c.UserId,
                        UserName = c.User.FirstName + " " + c.User.LastName,
                        DoctorId = c.DoctorId,
                        DoctorName = c.Doctor.FirstName + " " + c.Doctor.LastName,
                        Message = c.Message,
                        Reply = c.Reply,
                        Date = c.Date
                    })
                    .ToListAsync();

                return Ok(new ApiResponseDTO<IEnumerable<ConsultationDTO>>
                {
                    Success = true,
                    Data = consultations
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDTO<IEnumerable<ConsultationDTO>>
                {
                    Success = false,
                    Message = "An error occurred",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponseDTO<ConsultationDTO>>> CreateConsultation(CreateConsultationDTO model)
        {
            try
            {
                var userIdClaim = User.FindFirst("UserId")?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return BadRequest(new ApiResponseDTO<ConsultationDTO>
                    {
                        Success = false,
                        Message = "Invalid user ID"
                    });
                }

                var userType = User.FindFirst("UserType")?.Value;

                // Only regular users can create consultations
                if (userType != UserType.User.ToString())
                {
                    return Forbid("Only users can create consultations");
                }

                var doctor = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == model.DoctorId && u.UserType == UserType.Doctor);

                if (doctor == null)
                {
                    return BadRequest(new ApiResponseDTO<ConsultationDTO>
                    {
                        Success = false,
                        Message = "Invalid doctor ID"
                    });
                }

                var consultation = new Consultation
                {
                    UserId = userId,
                    DoctorId = model.DoctorId,
                    Message = model.Message,
                    Date = DateTime.UtcNow
                };

                _context.Consultations.Add(consultation);
                await _context.SaveChangesAsync();

                // Load navigation properties
                await _context.Entry(consultation).Reference(c => c.User).LoadAsync();
                await _context.Entry(consultation).Reference(c => c.Doctor).LoadAsync();

                var consultationDTO = new ConsultationDTO
                {
                    ConId = consultation.ConId,
                    UserId = consultation.UserId,
                    UserName = consultation.User.FirstName + " " + consultation.User.LastName,
                    DoctorId = consultation.DoctorId,
                    DoctorName = consultation.Doctor.FirstName + " " + consultation.Doctor.LastName,
                    Message = consultation.Message,
                    Date = consultation.Date
                };

                return Ok(new ApiResponseDTO<ConsultationDTO>
                {
                    Success = true,
                    Message = "Consultation created successfully",
                    Data = consultationDTO
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDTO<ConsultationDTO>
                {
                    Success = false,
                    Message = "An error occurred",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPost("reply")]
        public async Task<ActionResult<ApiResponseDTO<ConsultationDTO>>> ReplyToConsultation(ReplyConsultationDTO model)
        {
            try
            {
                var userIdClaim = User.FindFirst("UserId")?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return BadRequest(new ApiResponseDTO<ConsultationDTO>
                    {
                        Success = false,
                        Message = "Invalid user ID"
                    });
                }

                var userType = User.FindFirst("UserType")?.Value;

                // Only doctors can reply to consultations
                if (userType != UserType.Doctor.ToString())
                {
                    return Forbid("Only doctors can reply to consultations");
                }

                var consultation = await _context.Consultations
                    .Include(c => c.User)
                    .Include(c => c.Doctor)
                    .FirstOrDefaultAsync(c => c.ConId == model.ConId && c.DoctorId == userId);

                if (consultation == null)
                {
                    return NotFound(new ApiResponseDTO<ConsultationDTO>
                    {
                        Success = false,
                        Message = "Consultation not found or you don't have permission"
                    });
                }

                consultation.Reply = model.Reply;
                await _context.SaveChangesAsync();

                var consultationDTO = new ConsultationDTO
                {
                    ConId = consultation.ConId,
                    UserId = consultation.UserId,
                    UserName = consultation.User.FirstName + " " + consultation.User.LastName,
                    DoctorId = consultation.DoctorId,
                    DoctorName = consultation.Doctor.FirstName + " " + consultation.Doctor.LastName,
                    Message = consultation.Message,
                    Reply = consultation.Reply,
                    Date = consultation.Date
                };

                return Ok(new ApiResponseDTO<ConsultationDTO>
                {
                    Success = true,
                    Message = "Reply added successfully",
                    Data = consultationDTO
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDTO<ConsultationDTO>
                {
                    Success = false,
                    Message = "An error occurred",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}