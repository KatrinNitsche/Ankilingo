using AnkiLingo.Data;
using AnkiLingo.Dtos;
using AnkiLingo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AnkiLingo.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class SectionController : ControllerBase
    {
        private ISectionService SectionService;
        private readonly ILogger<SectionController> _logger;

        public SectionController(ILogger<SectionController> logger, ISectionService SectionService)
        {
            _logger = logger;
            this.SectionService = SectionService;
        }

        /// <summary>
        /// Gets a list of all Section entries
        /// </summary>
        /// <returns>list of Section entries</returns>
        [HttpGet("{courseId}")]
        public async Task<IEnumerable<GetSectionDto>> GetSectionsForCourse(int courseId)
        {
            var data = (await SectionService.GetAll()).Where(x => x.CourseId == courseId);
            return data.Select(x => new GetSectionDto()
            {
                Created = x.Created,
                description = x.Description,
                id = x.Id,
                name = x.Name,
                Updated = x.Updated
            });
        }

        /// <summary>
        /// Add a Section entry
        /// </summary>
        /// <param name="Section">Section object</param>
        /// <returns>added Section object</returns>
        [HttpPost(Name = "AddSection")]
        public async Task<Section> Add([FromBody] AddSectionDto SectionDto)
        {
            Section Section = new Section()
            {
                Description = SectionDto.description,
                Name = SectionDto.name,              
                Created = DateTime.Now,
                Updated = DateTime.Now,
                CourseId = SectionDto.courseId
            };

            var result = SectionService.Add(Section);
            return await result;
        }

        /// <summary>
        /// Updates a Section entry
        /// </summary>
        /// <param name="Section">Section object</param>
        /// <returns>Updated Section object</returns>
        [HttpPut("{id}", Name = "UpdateSection")]
        public async Task<Section> Update([FromRoute] int id, [FromBody] EditSectionDto SectionDto)
        {
            Section Section = new Section()
            {
                Id = SectionDto.id,
                Description = SectionDto.description,
                Name = SectionDto.name,
                Updated = DateTime.Now,
                CourseId = SectionDto.courseId
            };

            var result = SectionService.Update(Section);
            return await result;
        }

        /// <summary>
        /// Deletes a Section entry and all sub-entries permamently
        /// </summary>
        /// <param name="id">ID of the Section</param>
        /// <returns>true if deleted successfully</returns>
        [HttpDelete("{id}", Name = "DeleteSection")]
        public async Task<bool> Delete([FromRoute] int id)
        {
            var result = SectionService.Remove(id);
            return await result;
        }        
    }
}
