using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using TaskBoard.Data;
using TaskBoard.Models;

namespace TaskBoard.Controllers
{
    public class HomeController : Controller
    {
        private readonly TaskBoardAppDbContext _dbContext;

        public HomeController(TaskBoardAppDbContext context)
        {
            _dbContext = context;
        }
        
        public async Task<IActionResult> Index()
        {

            var taskBoards = _dbContext
                .Boards
                .Select(b => b.Name)
                .Distinct();
            var tasksCounts = new List<HomeBoardModel> ();
            foreach (var boardName in taskBoards)
            {
                var tasksInBoard = _dbContext.Tasks.Where(t => t.Board.Name == boardName).Count();
                tasksCounts.Add(new HomeBoardModel()
                {
                    BoardName = boardName,
                    TasksCount = tasksInBoard
                });
            }

            var userTasksCount = -1;

            if (User.Identity.IsAuthenticated)
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                userTasksCount = _dbContext.Tasks.Where(t => t.OwnerId == currentUserId).Count();
            }

            var homeModel = new HomeViewModel()
            {
                AllTasksCount = _dbContext.Tasks.Count(),
                BoardsWithTasksCount = tasksCounts,
                UserTasksCount = userTasksCount
            };

            return View(homeModel);
        }
        
    }
}