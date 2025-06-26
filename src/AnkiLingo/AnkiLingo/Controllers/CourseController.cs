using AnkiLingo.Data;
using AnkiLingo.Dtos;
using AnkiLingo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace AnkiLingo.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class CourseController : ControllerBase
    {
        private ICourseService courseService;
        private readonly ILogger<CourseController> _logger;

        public CourseController(ILogger<CourseController> logger, ICourseService courseService)
        {
            _logger = logger;
            this.courseService = courseService;
        }

        /// <summary>
        /// Gets a list of all course entries
        /// </summary>
        /// <returns>list of course entries</returns>
        [HttpGet(Name = "GetCourses")]
        public async Task<IEnumerable<Course>> Get()
        {
            var data = courseService.GetAll();
            return await data;
        }


        /// <summary>
        /// Gets a single course
        /// </summary>
        /// <param name="id">ID of the requested course</param>
        /// <returns>requested course object</returns>
        [HttpGet("{id}", Name = "GetCourse")]
        public async Task<DetailedCourseDto> GetCourse([FromRoute] int id)
        {
            var course = await courseService.GetById(id);
            DetailedCourseDto courseDetails = new DetailedCourseDto()
            {
                id = course.Id,
                Created = course.Created,
                description = course.Description,
                name = course.Name,
                Sections = course.Sections.Select(x => new GetSectionDto()
                {
                    Updated = x.Updated,
                    Created = x.Created,
                    description = x.Description,
                    id = x.Id,
                    name = x.Name,
                    Units = x.Units.Select(u => new GetUnitsDto()
                    {
                        Created = u.Created,
                        description = u.Description,
                        id = u.Id,
                        name = u.Name,
                        Updated = u.Updated,
                        Entries = u.Entries.Select(e => new GetEntryDto()
                        {
                            description = e.Description,
                            id = e.Id,
                            name = e.Name,
                            LastReviewed = e.LastReviewed,
                            LevelOnKnowledge = e.LevelOnKnowledge,
                            Value1 = e.Value1,
                            Value2 = e.Value2
                        }).AsEnumerable<GetEntryDto>()
                    }).AsEnumerable<GetUnitsDto>()
                }).AsEnumerable<GetSectionDto>(),
                Updated = course.Updated
            };

            return courseDetails;
        }

        /// <summary>
        /// Add a course entry
        /// </summary>
        /// <param name="course">course object</param>
        /// <returns>added course object</returns>
        [HttpPost(Name = "AddCourse")]
        public async Task<Course> Add([FromBody] AddCourseDto courseDto)
        {
            Course course = new Course()
            {
                Description = courseDto.description,
                Name = courseDto.name,
                Icon = courseDto.icon,                
                Created = DateTime.Now,
                Updated = DateTime.Now
            };

            var result = courseService.Add(course);
            return await result;
        }

        /// <summary>
        /// Updates a course entry
        /// </summary>
        /// <param name="course">course object</param>
        /// <returns>Updated course object</returns>
        [HttpPut("{id}", Name = "UpdateCourse")]
        public async Task<Course> Update([FromRoute] int id, [FromBody] EditCourseDto courseDto)
        {
            Course course = new Course()
            {
                Id = courseDto.id,
                Description = courseDto.description,
                Name = courseDto.name,
                Icon = courseDto.icon,
                Updated = DateTime.Now
            };

            var result = courseService.Update(course);
            return await result;
        }

        /// <summary>
        /// Deletes a course entry and all sub-entries permamently
        /// </summary>
        /// <param name="id">ID of the course</param>
        /// <returns>true if deleted successfully</returns>
        [HttpDelete("{id}", Name = "DeleteCourse")]
        public async Task<bool> Delete([FromRoute] int id)
        {
            var result = courseService.Remove(id);
            return await result;
        }        
    }
}
