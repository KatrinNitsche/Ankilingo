using AnkiLingo.Data;
using AnkiLingo.Dtos;
using AnkiLingo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace AnkiLingo.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class EntryController : ControllerBase
    {
        private IEntryService entryService;
        private readonly ILogger<CourseController> _logger;

        public EntryController(ILogger<CourseController> logger, IEntryService entryService)
        {
            _logger = logger;
            this.entryService = entryService;
        }

        /// <summary>  
        /// Updates a unit entry  
        /// </summary>  
        /// <param name="course">course object</param>  
        /// <returns>Updated entry object</returns>  
        [HttpPut("{id}", Name = "UpdateEntry")]
        public async Task<Entry> Update([FromRoute] int id, [FromBody] EditEntryDto entryDto)
        {
            var entryData = await entryService.GetById(id);
            var course = new Entry()
            {
                Id = entryDto.id,
                LevelOnKnowledge = entryDto.LevelOnKnowledge,
                Name = entryData.Name,
                Created = entryData.Created,
                Description= entryData.Description,
                LastReviewed = DateTime.Now,
                Unit = entryData.Unit,
                UnitId = entryData.UnitId,
                Updated = entryData.Updated,
                Value1 = entryData.Value1,
                Value2 = entryData.Value2
            };

            var result = entryService.Update(course);
            return await result;
        }
    }
}
