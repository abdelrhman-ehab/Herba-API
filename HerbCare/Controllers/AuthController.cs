using HerbCare.DTOs;
using HerbCare.DTOs.AuthDTOs;
using HerbCare.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HerbCare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponseDTO<AuthResponseDTO>>> Register(RegisterDTO model)
        {
            try
            {
                // Check if user exists
                var userExists = await _userManager.FindByEmailAsync(model.Email);
                if (userExists != null)
                {
                    return BadRequest(new ApiResponseDTO<AuthResponseDTO>
                    {
                        Success = false,
                        Message = "User already exists!"
                    });
                }

                // Create user based on UserType
                ApplicationUser user = new()
                {
                    Email = model.Email,
                    UserName = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserType = model.UserType,
                    Address= model.Address,
                    Gender = model.Gender,
                };

                // Set properties based on UserType
                if (model.UserType == UserType.Doctor)
                {
                    // Doctor-specific validation
                    if (string.IsNullOrEmpty(model.Phone))
                    {
                        return BadRequest(new ApiResponseDTO<AuthResponseDTO>
                        {
                            Success = false,
                            Message = "Phone number is required for doctors"
                        });
                    }

                    if (string.IsNullOrEmpty(model.Specialty))
                    {
                        return BadRequest(new ApiResponseDTO<AuthResponseDTO>
                        {
                            Success = false,
                            Message = "Specialty is required for doctors"
                        });
                    }

                    // Set doctor properties
                    user.Phone = model.Phone;
                    user.Specialty = model.Specialty;
                    user.Rating = 0;

                    // User properties not needed for doctors
                    user.BirthDate = null;                    

                }
                else // Regular User
                {
                    // Set user properties
                    user.BirthDate = model.BirthDate;
                    user.Gender = model.Gender;
                    user.Address = model.Address;

                    // Doctor properties not needed for users
                    user.Phone = null;
                    user.Specialty = null;
                    user.Rating = 3;
                }

                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    return BadRequest(new ApiResponseDTO<AuthResponseDTO>
                    {
                        Success = false,
                        Message = "User creation failed!",
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    });
                }

                // Create token
                var token = await CreateToken(user);

                return Ok(new ApiResponseDTO<AuthResponseDTO>
                {
                    Success = true,
                    Message = $"{model.UserType} registered successfully!",
                    Data = token
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDTO<AuthResponseDTO>
                {
                    Success = false,
                    Message = "An error occurred while registering",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponseDTO<AuthResponseDTO>>> Login(LoginDTO model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    var token = await CreateToken(user);

                    return Ok(new ApiResponseDTO<AuthResponseDTO>
                    {
                        Success = true,
                        Message = "Login successful",
                        Data = token
                    });
                }

                return Unauthorized(new ApiResponseDTO<AuthResponseDTO>
                {
                    Success = false,
                    Message = "Invalid email or password"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDTO<AuthResponseDTO>
                {
                    Success = false,
                    Message = "An error occurred while logging in",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        private async Task<AuthResponseDTO> CreateToken(ApplicationUser user)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserId", user.Id.ToString()),  // Convert to string for claims
                new Claim("UserType", user.UserType.ToString()),
                new Claim(ClaimTypes.Role, user.UserType.ToString())
            };

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["JWT:TokenValidityInMinutes"])),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new AuthResponseDTO
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo,
                UserId = user.Id,
                Email = user.Email,
                UserType = user.UserType,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsSuccess = true
            };
        }
    }
}