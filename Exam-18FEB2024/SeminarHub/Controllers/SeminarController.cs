using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeminarHub.Data;
using SeminarHub.Models;
using System.Security.Claims;

namespace SeminarHub.Controllers
{
    public class SeminarController : Controller
    {
        private readonly SeminarHubDbContext dbContext;

        public SeminarController(SeminarHubDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IActionResult> All()
        {
            return View();
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }

        private async Task<IEnumerable<CategoryViewModel>> GetCategories()
        {
            return await dbContext.Categories
                .AsNoTracking()
                .Select(t => new CategoryViewModel
                {
                    Id = t.Id,
                    Name = t.Name
                })
                .ToListAsync();
        }
    }
}
