using HerbCare.Data;
using HerbCare.DTOs;
using HerbCare.DTOs.QuizDTOs;
using HerbCare.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HerbCare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class QuizController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public QuizController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponseDTO<IEnumerable<QuizDTO>>>> GetQuizzes()
        {
            try
            {
                var quizzes = await _context.Quizzes
                    .Include(q => q.QuestionQuizzes)
                    .ThenInclude(qq => qq.Question)
                    .ToListAsync();

                var quizDTOs = quizzes.Select(q => new QuizDTO
                {
                    QuizId = q.QuizId,
                    Title = q.Title,
                    TotalPoints = q.TotalPoints,
                    Questions = q.QuestionQuizzes.Select(qq => new QuestionDTO
                    {
                        QuestionId = qq.Question.QuestionId,
                        Text = qq.Question.Text,
                        OptionA = qq.Question.OptionA,
                        OptionB = qq.Question.OptionB,
                        OptionC = qq.Question.OptionC,
                        OptionD = qq.Question.OptionD
                    }).ToList()
                }).ToList();

                return Ok(new ApiResponseDTO<IEnumerable<QuizDTO>>
                {
                    Success = true,
                    Data = quizDTOs
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDTO<IEnumerable<QuizDTO>>
                {
                    Success = false,
                    Message = "An error occurred",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponseDTO<QuizDTO>>> GetQuiz(int id)
        {
            try
            {
                var quiz = await _context.Quizzes
                    .Include(q => q.QuestionQuizzes)
                        .ThenInclude(qq => qq.Question)
                    .FirstOrDefaultAsync(q => q.QuizId == id);

                if (quiz == null)
                {
                    return NotFound(new ApiResponseDTO<QuizDTO>
                    {
                        Success = false,
                        Message = "Quiz not found"
                    });
                }

                var quizDTO = new QuizDTO
                {
                    QuizId = quiz.QuizId,
                    Title = quiz.Title,
                    TotalPoints = quiz.TotalPoints,
                    Questions = quiz.QuestionQuizzes.Select(qq => new QuestionDTO
                    {
                        QuestionId = qq.Question.QuestionId,
                        Text = qq.Question.Text,
                        OptionA = qq.Question.OptionA,
                        OptionB = qq.Question.OptionB,
                        OptionC = qq.Question.OptionC,
                        OptionD = qq.Question.OptionD
                    }).ToList()
                };

                return Ok(new ApiResponseDTO<QuizDTO>
                {
                    Success = true,
                    Data = quizDTO
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDTO<QuizDTO>
                {
                    Success = false,
                    Message = "An error occurred",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPost("{id}/submit")]
        public async Task<ActionResult<ApiResponseDTO<QuizResultDTO>>> SubmitQuiz(int id, SubmitQuizDTO model)
        {
            try
            {
                var userIdClaim = User.FindFirst("UserId")?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return BadRequest(new ApiResponseDTO<QuizResultDTO>
                    {
                        Success = false,
                        Message = "Invalid user ID"
                    });
                }

                var quiz = await _context.Quizzes
                    .Include(q => q.QuestionQuizzes)
                        .ThenInclude(qq => qq.Question)
                    .FirstOrDefaultAsync(q => q.QuizId == id);

                if (quiz == null)
                {
                    return NotFound(new ApiResponseDTO<QuizResultDTO>
                    {
                        Success = false,
                        Message = "Quiz not found"
                    });
                }

                var existing = await _context.UserQuizzes
                    .FirstOrDefaultAsync(uq => uq.UserId == userId && uq.QuizId == id);

                if (existing != null)
                {
                    return BadRequest(new ApiResponseDTO<QuizResultDTO>
                    {
                        Success = false,
                        Message = "You have already taken this quiz"
                    });
                }
                int score = 0;
                int totalQuestions = quiz.QuestionQuizzes.Count;
                int pointsPerQuestion = totalQuestions > 0 ? quiz.TotalPoints / totalQuestions : 0;

                var answers = new List<AnswerResultDTO>();

                foreach (var answer in model.Answers)
                {
                    var question = quiz.QuestionQuizzes
                        .Select(qq => qq.Question)
                        .FirstOrDefault(q => q.QuestionId == answer.QuestionId);

                    if (question != null)
                    {
                        bool isCorrect = question.Correct == answer.SelectedAnswer;

                        if (isCorrect)
                        {
                            score += pointsPerQuestion;
                        }

                        answers.Add(new AnswerResultDTO
                        {
                            QuestionId = question.QuestionId,
                            QuestionText = question.Text,
                            SelectedAnswer = answer.SelectedAnswer,
                            IsCorrect = isCorrect,
                            CorrectAnswer = isCorrect ? null : question.Correct
                        });
                    }
                }

                var userQuiz = new UserQuiz
                {
                    UserId = userId,
                    QuizId = id
                };

                _context.UserQuizzes.Add(userQuiz);
                await _context.SaveChangesAsync();

                var result = new QuizResultDTO
                {
                    QuizId = quiz.QuizId,
                    QuizTitle = quiz.Title,
                    Score = score,
                    TotalPoints = quiz.TotalPoints,
                    Percentage = quiz.TotalPoints > 0 ? (score * 100) / quiz.TotalPoints : 0,
                    Answers = answers
                };

                return Ok(new ApiResponseDTO<QuizResultDTO>
                {
                    Success = true,
                    Message = "Quiz submitted successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDTO<QuizResultDTO>
                {
                    Success = false,
                    Message = "An error occurred",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpGet("my-results")]
        public async Task<ActionResult<ApiResponseDTO<IEnumerable<UserQuizResultDTO>>>> GetMyQuizResults()
        {
            try
            {
                var userIdClaim = User.FindFirst("UserId")?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return BadRequest(new ApiResponseDTO<IEnumerable<UserQuizResultDTO>>
                    {
                        Success = false,
                        Message = "Invalid user ID"
                    });
                }

                var results = await _context.UserQuizzes
                    .Where(uq => uq.UserId == userId)
                    .Include(uq => uq.Quiz)
                    .Select(uq => new UserQuizResultDTO
                    {
                        QuizId = uq.QuizId,
                        QuizTitle = uq.Quiz.Title,
                        CompletedDate = DateTime.UtcNow
                    })
                    .ToListAsync();

                return Ok(new ApiResponseDTO<IEnumerable<UserQuizResultDTO>>
                {
                    Success = true,
                    Data = results
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDTO<IEnumerable<UserQuizResultDTO>>
                {
                    Success = false,
                    Message = "An error occurred",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        //[HttpPost]
        //[Authorize(Roles = "Doctor")]
        //public async Task<ActionResult<ApiResponseDTO<QuizDTO>>> CreateQuiz(QuizDTO model)
        //{
        //    try
        //    {
        //        var quiz = new Quiz
        //        {
        //            Title = model.Title,
        //            TotalPoints = model.TotalPoints
        //        };

        //        _context.Quizzes.Add(quiz);
        //        await _context.SaveChangesAsync();

        //        // Add questions
        //        foreach (var q in model.Questions)
        //        {
        //            var question = new Question
        //            {
        //                Text = q.Text,
        //                OptionA = q.OptionA,
        //                OptionB = q.OptionB,
        //                OptionC = q.OptionC,
        //                OptionD = q.OptionD,
        //                Correct = q.Correct
        //            };

        //            _context.Questions.Add(question);
        //            await _context.SaveChangesAsync();

        //            _context.QuestionQuizzes.Add(new QuestionQuiz
        //            {
        //                QuestionId = question.QuestionId,
        //                QuizId = quiz.QuizId
        //            });
        //        }

        //        await _context.SaveChangesAsync();

        //        return await GetQuiz(quiz.QuizId);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new ApiResponseDTO<QuizDTO>
        //        {
        //            Success = false,
        //            Message = "An error occurred",
        //            Errors = new List<string> { ex.Message }
        //        });
        //    }
        //}
    }
}