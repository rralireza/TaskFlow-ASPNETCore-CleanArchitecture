using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;
using TaskFlow.Application.DTO.TaskItem;
using TaskFlow.Application.DTO.Filters;
using TaskFlow.Application.Intefaces.Services.TaskItem;

namespace TaskFlow.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskItemAdderService _taskItemAdderService;
        private readonly ITaskItemGetterService _taskItemGetterService;

        public TaskController(ITaskItemAdderService taskItemAdderService, ITaskItemGetterService taskItemGetterService)
        {
            _taskItemAdderService = taskItemAdderService;
            _taskItemGetterService = taskItemGetterService;
        }

        [HttpGet(nameof(GetAllCurrentUserTasks))]
        [Authorize(Policy = "TaskCreators")]
        public async Task<IActionResult> GetAllCurrentUserTasks([FromQuery] TaskFilterDto filter)
        {
            try
            {
                return Ok(await _taskItemGetterService.GetAllTasksForCurrentUser(filter));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost(nameof(CreateTask))]
        [Authorize(Policy = "TaskCreators")]
        public async Task<IActionResult> CreateTask([FromBody] AddTaskItemRequestDto request)
        {
            try
            {
                var response = await _taskItemAdderService.AddTaskItem(request);

                if (response == null)
                    return BadRequest();

                return Ok(response);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Join(", ", ex.Message));
            }
        }
    }
}
