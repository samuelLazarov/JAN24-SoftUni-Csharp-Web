using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskBoard.Data;
using TaskBoard.Models;

namespace TaskBoard.Controllers
{
    [Authorize]
    public class TaskController : Controller
    {
        private readonly TaskBoardAppDbContext data;

        public TaskController(TaskBoardAppDbContext context)
        {
            data = context;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new TaskFormViewModel();
            model.Boards = await GetBoards();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TaskFormViewModel model)
        {
            if (!(await GetBoards()).Any(b => b.Id == model.BoardId))
            {
                ModelState.AddModelError(nameof(model.BoardId), "Board does not exist");
            }
            if (!ModelState.IsValid) 
            {
                model.Boards = await GetBoards();

                return View(model);
            }

            var entity = new Data.Task()
            {
                BoardId = model.BoardId,
                CreatedOn = DateTime.Now,
                Description = model.Description,
                OwnerId = GetUserId(),
                Title = model.Title

            };

            await data.AddAsync(entity);
            await data.SaveChangesAsync();

            var boards = data.Boards;

            return RedirectToAction("Index", "Board");
        }

        private string GetUserId()
            => User.FindFirstValue(ClaimTypes.NameIdentifier);
        

        private async Task<IEnumerable<TaskBoardModel>> GetBoards()
        {
            return await data.Boards
             .Select(b => new TaskBoardModel
            {
                Id= b.Id,
                Name= b.Name,
            })
            .ToListAsync();
        }
    }
}
