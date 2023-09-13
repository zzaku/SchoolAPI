using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly SchoolApiContext _context;

        public CoursesController(SchoolApiContext context)
        {
            _context = context;
        }

        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<IEnumerable<Course>>> GetCoursesForStudent(int studentId)
        {
            var student = await _context.Users
                .Include(s => s.Courses)
                .FirstOrDefaultAsync(s => s.Id == studentId);

            if (student == null)
            {
                return NotFound($"Student with Id = {studentId} not found");
            }

            // Utilisation de JsonSerializerOptions avec ReferenceHandler.Preserve
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };

            // Sérialiser les cours en JSON avec les références cycliques préservées
            var coursesJson = JsonSerializer.Serialize(student.Courses, jsonSerializerOptions);

            // Retourner les cours sous forme de contenu JSON
            return Content(coursesJson, "application/json");
        }

        [HttpPost("student/{studentId}/course/{courseId}")]

        public async Task<IActionResult> AssignCourseToStudent(int studentId, int courseId)
        {
            // Recherchez l'étudiant par ID
            var student = await _context.Users.FindAsync(studentId);
            if (student == null)
            {
                return NotFound($"Etudiant avec l'id = {studentId} pas trouvé");
            }
            
            // Recherchez le cours par ID
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
            {
                return NotFound($"Cours avec l'id = {courseId} pas trouvé");
            }

            var courseStudent = new CourseStudent
            {
                CourseId = courseId,
                UserId = studentId
            };

            _context.CourseStudents.Add(courseStudent);
            await _context.SaveChangesAsync();

            return Ok($"Assigned Course with Id = {courseId} to Student with Id = {studentId}");
        }

        [HttpPut("student/{studentId}/{courseId}")]
        public async Task<IActionResult> UpdateCourseForStudent(int studentId, int courseId, Course course)
        {
            if (!courseId.Equals(course.Id))
            {
                return BadRequest("IDs are different");
            }
            var student = await _context.Users.Include(s => s.Courses).FirstOrDefaultAsync(s => s.Id == studentId);
            if (student == null)
            {
                return NotFound($"Student with Id = {studentId} not found");
            }
            var courseToUpdate = student.Courses.FirstOrDefault(c => c.Id == courseId);
            if (courseToUpdate == null)
            {
                return NotFound($"Course with Id = {courseId} not found for student");
            }
            map(course, courseToUpdate);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("student/{studentId}/{courseId}")]
        public async Task<IActionResult> DeleteCourseForStudent(int studentId, int courseId)
        {
            var student = await _context.Users.Include(s => s.Courses).FirstOrDefaultAsync(s => s.Id == studentId);
            if (student == null)
            {
                return NotFound($"Student with Id = {studentId} not found");
            }
            var courseToDelete = student.Courses.FirstOrDefault(c => c.Id == courseId);
            if (courseToDelete == null)
            {
                return NotFound($"Cours avec l'id = {courseId} not found for student");
            }
            student.Courses.Remove(courseToDelete);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private void map(Course course, Course courseToUpdate)
        {
            courseToUpdate.Name = course.Name;
        }
    }
}
