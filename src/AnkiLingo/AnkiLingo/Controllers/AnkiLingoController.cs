using AnkiLingoExcelService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AnkiLingo.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AnkiLingoController : ControllerBase
    {
        private readonly ILogger<CourseController> _logger;

        public AnkiLingoController(ILogger<CourseController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetCoursesFromFolder")]
        public async Task<IActionResult> Get()
        {
            var courses = ExcelService.GetCourseNames("C:\\temp\\AnkiLingo");
            if (courses == null || !courses.Any())
            {
                return NotFound("No courses found.");
            }

            return Ok(courses);
        }

        [HttpGet("GetCourseDetails/{courseName}")]
        public async Task<IActionResult> Get(string courseName)
        {
            var filePath = Path.Combine("C:\\temp\\AnkiLingo", $"{courseName}.xlsx");
            var courseDetails = ExcelService.GetCourseDetails(filePath);
            if (courseDetails == null || string.IsNullOrEmpty(courseDetails.Name))
            {
                return NotFound($"Course {courseName} not found.");
            }
            return Ok(courseDetails);
        }

        [HttpPut("UpdateCourseDetails/{courseName}")]
        public async Task<IActionResult> UpdateCourseDetails(string courseName, string sectionName, string unitName, string value1, string value2, int levelOfKnowledge)
        {
            var filePath = Path.Combine("C:\\temp\\AnkiLingo", $"{courseName}.xlsx");
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound($"Course {courseName} not found.");
            }
            var result = ExcelService.UpdateEntry(filePath, sectionName, unitName, value1, value2, levelOfKnowledge);
            if (!result)
            {
                return BadRequest("Failed to update course details.");
            }
            return Ok("Course details updated successfully.");
        }
    }
}
