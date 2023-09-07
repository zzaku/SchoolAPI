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

namespace SchoolApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly SchoolApiContext _context;

        public CoursesController(SchoolApiContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Course>>> GetCourses()
        {
            return await _context.Courses.ToListAsync();
        }


        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Course>> GetCourseById(int id)
        {
            var course = await _context.Courses.Where(c => c.Id.Equals(id)).FirstOrDefaultAsync();
            if (course == null)
            {
                return NotFound();
            }
            return course;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Course>> CreateCourse(Course course)
        {
            // valider les données
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCourseById), new { id = course.Id }, course);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateCourse(int id, Course course)
        {
            if (!id.Equals(course.Id))
            {
                return BadRequest("IDs are different");
            }
            var courseToUpdate = await _context.Courses.FindAsync(id);
            if (courseToUpdate == null)
            {
                return NotFound($"Course with Id = {id} not found");
            }
            //courseToUpdate.Name = course.Name;
            map(course, courseToUpdate);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("student/{studentId}")]
        [Authorize]
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
                return NotFound($"Student with Id = {studentId} not found");
            }

            // Recherchez le cours par ID
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
            {
                return NotFound($"Course with Id = {courseId} not found");
            }

            // Créez une nouvelle entrée dans la table CourseStudent pour associer le cours à l'étudiant
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
        [Authorize]
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
        [Authorize]
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
