using AnkiLingo.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AnkiLingo.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class CourseController : ControllerBase
    {
        private readonly ILogger<CourseController> _logger;

        public CourseController(ILogger<CourseController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetCourses")]
        public IEnumerable<Course> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new Course
            {
                Created = DateTime.Now.AddMonths(-3),
                Updated = DateTime.Now.AddDays(index),
                Id = Guid.NewGuid(),
                Name = $"Course - {Random.Shared.Next(-20, 55)}",
                Description = $"A course description comes here.",
                Icon = "path to icon"
            }).ToArray();
        }
    }
}
