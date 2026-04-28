using HerbCare.Data;
using HerbCare.DTOs;
using HerbCare.DTOs.IdealWeightDTOs;
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
    public class IdealWeightController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public IdealWeightController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("calculate")]
        public async Task<ActionResult<ApiResponseDTO<IdealWeightResponseDTO>>> CalculateIdealWeight(IdealWeightRequestDTO request)
        {
            try
            {
                // Validate request
                if (request.Height <= 0 || request.Height > 300)
                {
                    return BadRequest(new ApiResponseDTO<IdealWeightResponseDTO>
                    {
                        Success = false,
                        Message = "Please enter a valid height between 1 and 300 cm"
                    });
                }

                if (request.Age <= 0 || request.Age > 150)
                {
                    return BadRequest(new ApiResponseDTO<IdealWeightResponseDTO>
                    {
                        Success = false,
                        Message = "Please enter a valid age"
                    });
                }

                if (request.Gender?.ToLower() != "male" && request.Gender?.ToLower() != "female")
                {
                    return BadRequest(new ApiResponseDTO<IdealWeightResponseDTO>
                    {
                        Success = false,
                        Message = "Gender must be 'Male' or 'Female'"
                    });
                }

                // Calculate ideal weight using different methods
                var methods = new Dictionary<string, double>();

                // 1. Devine Formula
                double devineWeight = CalculateDevineFormula(request.Gender, request.Height);
                methods.Add("Devine", Math.Round(devineWeight, 1));

                // 2. Robinson Formula
                double robinsonWeight = CalculateRobinsonFormula(request.Gender, request.Height);
                methods.Add("Robinson", Math.Round(robinsonWeight, 1));

                // 3. Miller Formula
                double millerWeight = CalculateMillerFormula(request.Gender, request.Height);
                methods.Add("Miller", Math.Round(millerWeight, 1));

                // 4. Hamwi Formula
                double hamwiWeight = CalculateHamwiFormula(request.Gender, request.Height);
                methods.Add("Hamwi", Math.Round(hamwiWeight, 1));

                // Use average of all methods for the main ideal weight
                double idealWeight = methods.Values.Average();

                var response = new IdealWeightResponseDTO
                {
                    IdealWeight = Math.Round(idealWeight, 1),
                    CurrentWeight = request.CurrentWeight,
                    DifferentMethods = methods
                };

                // Calculate weight difference if current weight is provided
                if (request.CurrentWeight.HasValue)
                {
                    response.WeightDifference = Math.Round(request.CurrentWeight.Value - idealWeight, 1);
                    response.WeightStatus = GetWeightStatus(idealWeight, request.CurrentWeight);
                    response.Recommendation = GetRecommendation(response.WeightStatus, response.WeightDifference);
                }

                // Save calculation to database if user is authenticated
                var userIdClaim = User.FindFirst("UserId")?.Value;
                if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int userId))
                {
                    var calculation = new IdealWeightCalculation
                    {
                        UserId = userId,
                        Gender = request.Gender,
                        Height = request.Height,
                        Age = request.Age,
                        CurrentWeight = request.CurrentWeight,
                        IdealWeight = response.IdealWeight,
                        CalculationMethod = "Average of multiple formulas",
                        CalculationDate = DateTime.UtcNow
                    };

                    _context.IdealWeightCalculations.Add(calculation);
                    await _context.SaveChangesAsync();
                }

                return Ok(new ApiResponseDTO<IdealWeightResponseDTO>
                {
                    Success = true,
                    Message = "Ideal weight calculated successfully",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDTO<IdealWeightResponseDTO>
                {
                    Success = false,
                    Message = "An error occurred while calculating ideal weight",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpGet("history")]
        public async Task<ActionResult<ApiResponseDTO<IEnumerable<WeightCalculatorHistoryDTO>>>> GetCalculationHistory()
        {
            try
            {
                var userIdClaim = User.FindFirst("UserId")?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return BadRequest(new ApiResponseDTO<IEnumerable<WeightCalculatorHistoryDTO>>
                    {
                        Success = false,
                        Message = "Invalid user ID"
                    });
                }

                var history = await _context.IdealWeightCalculations
                    .Where(c => c.UserId == userId)
                    .OrderByDescending(c => c.CalculationDate)
                    .Select(c => new WeightCalculatorHistoryDTO
                    {
                        Id = c.Id,
                        CalculationDate = c.CalculationDate,
                        Gender = c.Gender,
                        Height = c.Height,
                        Age = c.Age,
                        IdealWeight = c.IdealWeight,
                        CurrentWeight = c.CurrentWeight
                    })
                    .Take(20)
                    .ToListAsync();

                return Ok(new ApiResponseDTO<IEnumerable<WeightCalculatorHistoryDTO>>
                {
                    Success = true,
                    Data = history
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDTO<IEnumerable<WeightCalculatorHistoryDTO>>
                {
                    Success = false,
                    Message = "An error occurred",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpGet("bmi")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponseDTO<BMICalculationDTO>>> CalculateBMI(double weight, double height)
        {
            try
            {
                if (height <= 0 || weight <= 0)
                {
                    return BadRequest(new ApiResponseDTO<BMICalculationDTO>
                    {
                        Success = false,
                        Message = "Height and weight must be positive numbers"
                    });
                }

                if (height > 300 || weight > 500)
                {
                    return BadRequest(new ApiResponseDTO<BMICalculationDTO>
                    {
                        Success = false,
                        Message = "Please enter realistic values"
                    });
                }

                // Calculate BMI
                double heightInMeters = height / 100;
                double bmi = weight / (heightInMeters * heightInMeters);
                bmi = Math.Round(bmi, 1);

                string category;
                string healthRisk;

                if (bmi < 18.5)
                {
                    category = "Underweight";
                    healthRisk = "Increased risk of nutritional deficiencies and osteoporosis";
                }
                else if (bmi < 25)
                {
                    category = "Normal weight";
                    healthRisk = "Lowest risk";
                }
                else if (bmi < 30)
                {
                    category = "Overweight";
                    healthRisk = "Increased risk of heart disease, high blood pressure, and diabetes";
                }
                else if (bmi < 35)
                {
                    category = "Obese Class I";
                    healthRisk = "High risk of health problems";
                }
                else if (bmi < 40)
                {
                    category = "Obese Class II";
                    healthRisk = "Very high risk of health problems";
                }
                else
                {
                    category = "Obese Class III";
                    healthRisk = "Extremely high risk of health problems";
                }

                double minHealthyWeight = Math.Round(18.5 * heightInMeters * heightInMeters, 1);
                double maxHealthyWeight = Math.Round(24.9 * heightInMeters * heightInMeters, 1);

                var result = new BMICalculationDTO
                {
                    BMI = bmi,
                    Category = category,
                    HealthRisk = healthRisk,
                    HealthyWeightRange = $"{minHealthyWeight} - {maxHealthyWeight} kg"
                };

                return Ok(new ApiResponseDTO<BMICalculationDTO>
                {
                    Success = true,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDTO<BMICalculationDTO>
                {
                    Success = false,
                    Message = "An error occurred",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        #region Private Helper Methods

        private double CalculateDevineFormula(string gender, double heightCm)
        {
            // Convert cm to inches
            double heightInches = heightCm / 2.54;

            if (gender.ToLower() == "male")
            {
                // Devine formula for men: 50 kg + 2.3 kg per inch over 5 feet
                return 50 + 2.3 * (heightInches - 60);
            }
            else
            {
                // Devine formula for women: 45.5 kg + 2.3 kg per inch over 5 feet
                return 45.5 + 2.3 * (heightInches - 60);
            }
        }

        private double CalculateRobinsonFormula(string gender, double heightCm)
        {
            double heightInches = heightCm / 2.54;

            if (gender.ToLower() == "male")
            {
                // Robinson formula for men: 52 kg + 1.9 kg per inch over 5 feet
                return 52 + 1.9 * (heightInches - 60);
            }
            else
            {
                // Robinson formula for women: 49 kg + 1.7 kg per inch over 5 feet
                return 49 + 1.7 * (heightInches - 60);
            }
        }

        private double CalculateMillerFormula(string gender, double heightCm)
        {
            double heightInches = heightCm / 2.54;

            if (gender.ToLower() == "male")
            {
                // Miller formula for men: 56.2 kg + 1.41 kg per inch over 5 feet
                return 56.2 + 1.41 * (heightInches - 60);
            }
            else
            {
                // Miller formula for women: 53.1 kg + 1.36 kg per inch over 5 feet
                return 53.1 + 1.36 * (heightInches - 60);
            }
        }

        private double CalculateHamwiFormula(string gender, double heightCm)
        {
            double heightInches = heightCm / 2.54;

            if (gender.ToLower() == "male")
            {
                // Hamwi formula for men: 48 kg + 2.7 kg per inch over 5 feet
                return 48 + 2.7 * (heightInches - 60);
            }
            else
            {
                // Hamwi formula for women: 45.5 kg + 2.2 kg per inch over 5 feet
                return 45.5 + 2.2 * (heightInches - 60);
            }
        }

        private string GetWeightStatus(double idealWeight, double? currentWeight)
        {
            if (!currentWeight.HasValue) return "Unknown";

            double ratio = currentWeight.Value / idealWeight;

            if (ratio < 0.85) return "Severely Underweight";
            if (ratio < 0.95) return "Underweight";
            if (ratio <= 1.05) return "Normal Weight";
            if (ratio <= 1.15) return "Overweight";
            if (ratio <= 1.25) return "Obese Class I";
            if (ratio <= 1.35) return "Obese Class II";
            return "Obese Class III";
        }

        private string GetRecommendation(string weightStatus, double? weightDifference)
        {
            if (!weightDifference.HasValue) return "Enter your current weight for personalized recommendations.";

            return weightStatus switch
            {
                "Severely Underweight" => "You are significantly underweight. Please consult with a healthcare provider and consider a nutrition plan to gain weight safely.",
                "Underweight" => "You are underweight. Consider increasing your caloric intake with nutritious foods and strength training exercises.",
                "Normal Weight" => "Great! You are at a healthy weight. Maintain your current lifestyle with balanced nutrition and regular exercise.",
                "Overweight" => "You are slightly overweight. Consider increasing physical activity and monitoring your calorie intake.",
                "Obese Class I" => "You are in the obese range. We recommend consulting with a healthcare provider and starting a structured weight loss program.",
                "Obese Class II" => "You are significantly overweight. Please consult with a healthcare provider for a personalized weight management plan.",
                "Obese Class III" => "You are in the severely obese range. It's important to consult with a healthcare provider for comprehensive medical advice.",
                _ => "Maintain a healthy lifestyle with balanced diet and regular exercise."
            };
        }

        #endregion
    }
}