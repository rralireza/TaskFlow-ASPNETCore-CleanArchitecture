using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.DTO.Filters;
using TaskFlow.Application.DTO.Project;
using TaskFlow.Application.Intefaces.Services.Projects;
using TaskFlow.Application.Services.Projects;

namespace TaskFlow.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectAdderService _projectAdderService;

        private readonly IProjectGetterService _projectGetterService;

        private readonly IProjectUpdaterService _projectUpdaterService;

        public ProjectController(IProjectAdderService projectAdderService, IProjectGetterService projectGetterService, IProjectUpdaterService projectUpdaterService)
        {
            _projectAdderService = projectAdderService;
            _projectGetterService = projectGetterService;
            _projectUpdaterService = projectUpdaterService;
        }

        [HttpPost("CreateProject")]
        [Authorize(Policy = "ProjectCreators")]
        public async Task<IActionResult> CreateProject([FromBody] AddProjectRequestDto project)
        {
            try
            {
                var result = await _projectAdderService.CreateProject(project);

                if (result == null)
                    throw new ArgumentNullException(nameof(result));

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetAllCurrentUserProjects")]
        [Authorize(Policy = "ProjectCreators")]
        public async Task<IActionResult> GetAllCurrentUserProjects([FromQuery] ProjectFilterDto filter)
        {
            try
            {
                var result = await _projectGetterService.GetAllProjectsForCurrentUser(filter);

                if (result == null)
                    throw new ArgumentNullException(nameof(result));

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("UpdateProject")]
        [Authorize(Policy = "ProjectCreators")]
        public async Task<IActionResult> UpdateProject([FromBody] UpdateProjectRequestDto request)
        {
            try
            {
                var response = await _projectUpdaterService.UpdateProject(request);

                if (response is null)
                    return BadRequest("Ooops! there's someting went wrong...");

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(string.Join(", ", ex.Message));
            }
        }
    }
}
