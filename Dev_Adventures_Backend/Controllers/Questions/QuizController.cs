using Dev_Db.Data;
using Dev_Models.DTOs.QuizDTO;
using Dev_Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Dev_Adventures_Backend.Controllers.Questions
{

    [ApiController]
    [Route("api/[controller]")]
    public class QuizController : ControllerBase
    {

        private readonly Dev_DbContext _context;

        public QuizController(Dev_DbContext context)
        {
            _context = context;
        }

        [HttpGet("lesson/{lessonId}")]
        public async Task<ActionResult<QuizDTO>> GetQuizByLesson([FromRoute] int lessonId)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                .ThenInclude(q => q.Answers)
                .FirstOrDefaultAsync(q => q.LessonId == lessonId);

            if (quiz == null)
                return NotFound();

            return new QuizDTO
            {
                Id = quiz.Id,
                Title = quiz.Title,
                LessonId = quiz.LessonId,
                Questions = quiz.Questions.Select(q => new QuizQuestionDTO
                {
                    Id = q.Id,
                    Text = q.Text,
                    Answers = q.Answers.Select(a => new QuizAnswerDTO
                    {
                        Id = a.Id,
                        Text = a.Text
                    }).ToList()
                }).ToList()
            };
        }


        [HttpPost("submit")]
        public async Task<ActionResult<int>> SubmitQuiz([FromBody] QuizSubmissionDTO submission)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                .ThenInclude(q => q.Answers)
                .FirstOrDefaultAsync(q => q.Id == submission.QuizId);

            if (quiz == null)
                return NotFound();

            int totalQuestions = quiz.Questions.Count;
            int correctAnswers = 0;

            foreach (var question in quiz.Questions)
            {
                if (submission.Answers.TryGetValue(question.Id, out int answerId))
                {
                    var selectedAnswer = question.Answers.FirstOrDefault(a => a.Id == answerId);
                    if (selectedAnswer?.IsCorrect == true)
                        correctAnswers++;
                }
            }

            int score = (int)((double)correctAnswers / totalQuestions * 100);

            var result = new UserQuizResult
            {
                UserId = userId,
                QuizId = submission.QuizId,
                Score = score,
                CompletedAt = DateTime.UtcNow
            };

            _context.UserQuizResults.Add(result);
            await _context.SaveChangesAsync();

            return Ok(score);
        }


        [HttpPost]
        public async Task<ActionResult<Quiz>> CreateQuiz([FromBody] QuizDTO quizDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var quiz = new Quiz
                {
                    Title = quizDto.Title,
                    LessonId = quizDto.LessonId,
                    Questions = quizDto.Questions.Select(q => new QuizQuestion
                    {
                        Text = q.Text,
                        Answers = q.Answers.Select(a => new QuizAnswer
                        {
                            Text = a.Text,
                            IsCorrect = a.IsCorrect ?? false
                        }).ToList()
                    }).ToList()
                };

                _context.Quizzes.Add(quiz);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetQuizByLesson), new { lessonId = quiz.LessonId }, quiz);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteQuiz(int id)
        {
            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz == null)
                return NotFound();

            _context.Quizzes.Remove(quiz);
            await _context.SaveChangesAsync();
            return NoContent();
        }


        [HttpPut("{id}")]
        public async Task<ActionResult<QuizDTO>> UpdateQuiz(int id, [FromBody] QuizDTO quizDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != quizDto.Id)
                return BadRequest("Quiz ID mismatch");

            try
            {
                var existingQuiz = await _context.Quizzes
                    .Include(q => q.Questions)
                    .ThenInclude(q => q.Answers)
                    .FirstOrDefaultAsync(q => q.Id == id);

                if (existingQuiz == null)
                    return NotFound($"Quiz with ID {id} not found");

                existingQuiz.Title = quizDto.Title;
                existingQuiz.LessonId = quizDto.LessonId;

                _context.QuizAnswers.RemoveRange(
                    existingQuiz.Questions.SelectMany(q => q.Answers));
                _context.QuizQuestions.RemoveRange(existingQuiz.Questions);

                existingQuiz.Questions = quizDto.Questions.Select(q => new QuizQuestion
                {
                    Text = q.Text,
                    Answers = q.Answers.Select(a => new QuizAnswer
                    {
                        Text = a.Text,
                        IsCorrect = a.IsCorrect ?? false
                    }).ToList()
                }).ToList();

                await _context.SaveChangesAsync();

                return await GetQuizByLesson(existingQuiz.LessonId);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await QuizExists(id))
                    return NotFound();
                throw;
            }
            catch (Exception ex)
            {
                // Log the exception here
                return StatusCode(500, "An error occurred while updating the quiz");
            }
        }

        private async Task<bool> QuizExists(int id)
        {
            return await _context.Quizzes.AnyAsync(q => q.Id == id);
        }

    }





}

