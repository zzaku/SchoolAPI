using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolApi.Models;
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
    public class CoursesControllers : ControllerBase
    {
        private readonly SchoolApiContext _context;

        public CoursesControllers(SchoolApiContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Course>>> GetCourses()
        {
            return await _context.Courses.ToListAsync();
        }


        [HttpGet("{id}")]
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
        public async Task<ActionResult<Course>> CreateCourse(Course course)
        {
            // valider les données
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCourseById), new { id = course.Id }, course);
        }

        [HttpDelete("{id}")]
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

            map(course, courseToUpdate);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private void map(Course course, Course courseToUpdate)
        {
            courseToUpdate.Name = course.Name;
        }
    }
}
