using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;
using TaskFlow.Application.DTO.TaskItem;
using TaskFlow.Application.Intefaces.Services.TaskItem;

namespace TaskFlow.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskItemAdderService _taskItemAdderService;

        public TaskController(ITaskItemAdderService taskItemAdderService)
        {
            _taskItemAdderService = taskItemAdderService;
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
