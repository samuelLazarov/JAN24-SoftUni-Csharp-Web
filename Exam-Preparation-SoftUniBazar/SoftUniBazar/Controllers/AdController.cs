using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoftUniBazar.Data;
using SoftUniBazar.Data.Models;
using SoftUniBazar.Models.Ad;
using SoftUniBazar.Models.Category;
using System.Security.Claims;
using static SoftUniBazar.Data.Common.Constraints;

namespace SoftUniBazar.Controllers
{
    [Authorize]
    public class AdController : Controller
    {

        private BazarDbContext dbContext;
        public AdController(BazarDbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        [HttpGet]
        public async Task<IActionResult> All()
        {
            //if no ads were created yet
            if (!dbContext.Ads.Any())
            {
                return View(new List<AdAllViewModel>());
            }

            //All created ads
            var allAds = await dbContext.Ads
                .AsNoTracking()
                .Select(a => new AdAllViewModel()
                {
                    Id = a.Id,
                    Name = a.Name,
                    ImageUrl = a.ImageUrl,
                    CreatedOn = a.CreatedOn.ToString(dateTimeFormat),
                    Category = a.Category.Name.ToString(),
                    Description = a.Description,
                    Price = a.Price,
                    Owner = a.Owner.UserName
                })
                .ToListAsync();

            return View(allAds);
        }

        //Adding new Ad
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var newAdForm = new AdAddViewModel();
            newAdForm.Categories = await GetCategories();
            return View(newAdForm);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AdAddViewModel newAdForm)
        {
            var allCategories = await GetCategories();

            if (!allCategories.Any(c => c.Id == newAdForm.CategoryId))
            {
                ModelState.AddModelError(nameof(newAdForm.CategoryId), "Category does not exist!");
            }

            string currentUser = GetUserById();

            if (!ModelState.IsValid)
            {
                return View(newAdForm);
            }

            var newAd = new Ad()
            {
                Name = newAdForm.Name,
                Description = newAdForm.Description,
                ImageUrl = newAdForm.ImageUrl,
                Price = newAdForm.Price,
                OwnerId = currentUser,
                CreatedOn = DateTime.Now,
                CategoryId = newAdForm.CategoryId
            };
            await dbContext.Ads.AddAsync(newAd);
            await dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        //Cart buttons
        [HttpGet]
        public async Task<IActionResult> Cart()
        {
            var currentUserCart = await dbContext.AdsBuyers
                .Where(ab => ab.BuyerId == GetUserById())
                .Select(ab => new AdCartViewModel()
                {
                    Id = ab.Ad.Id,
                    Name = ab.Ad.Name,
                    ImageUrl = ab.Ad.ImageUrl,
                    CreatedOn = ab.Ad.CreatedOn.ToString(dateTimeFormat),
                    Category = ab.Ad.Category.Name,
                    Description = ab.Ad.Description,
                    Price = ab.Ad.Price,
                    Owner = ab.Ad.Owner.UserName
                }).ToListAsync();

            return View(currentUserCart);
        }

        //Adding an Ad to the User's Cart through the "Add to Cart" button
        [HttpPost]
        public async Task<IActionResult> AddToCart(int id)
        {
            var userId = GetUserById();
            var adAndBuyer = await dbContext.AdsBuyers.FirstOrDefaultAsync(ab => ab.AdId == id && ab.BuyerId == userId);
            if (adAndBuyer == null)
            {
                var newAdAndBuyer = new AdBuyer()
                {
                    AdId = id,
                    BuyerId = userId,
                };

                await dbContext.AdsBuyers.AddAsync(newAdAndBuyer);
                await dbContext.SaveChangesAsync();
            }
            else
            {
                return RedirectToAction(nameof(All));
            }

            return RedirectToAction(nameof(Cart));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var userId = GetUserById();
            var adAndBuyer = await dbContext.AdsBuyers.FirstOrDefaultAsync(ab => ab.AdId == id && ab.BuyerId == userId);

            if (adAndBuyer != null)
            {
                dbContext.AdsBuyers.Remove(adAndBuyer);
                await dbContext.SaveChangesAsync();
            }
            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            //Checking if the User tries to edit an Ad which is not his
            var searchedAd = await dbContext.Ads
                .AsNoTracking()
                .Where(e => e.Id == id)
                .FirstOrDefaultAsync();

            if (!ModelState.IsValid || searchedAd == null || searchedAd.OwnerId != GetUserById())
            {
                return Unauthorized();
            }

            var adEditInfo = await dbContext.Ads
                .Where(a => a.Id == id)
                .Select(a => new AdEditViewModel()
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                    ImageUrl = a.ImageUrl,
                    Price = a.Price,
                    CategoryId = a.CategoryId,
                }).FirstOrDefaultAsync();
            adEditInfo.Categories = await GetCategories();

            return View(adEditInfo);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(AdEditViewModel editedAdForm)
        {
            //Validation of the Event that the User wants to Edit
            if (!ModelState.IsValid)
            {
                editedAdForm.Categories = await GetCategories();
                return View(editedAdForm);
            }

            //Editing the Model
            Ad editedAd = await dbContext.Ads.FindAsync(editedAdForm.Id);
            editedAd.Name = editedAdForm.Name;
            editedAd.Description = editedAdForm.Description;
            editedAd.ImageUrl = editedAdForm.ImageUrl;
            editedAd.Price = editedAdForm.Price;
            editedAd.CategoryId = editedAdForm.CategoryId;

            await dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        private string GetUserById()
        {
            string id = string.Empty;

            if (User != null)
            {
                id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            return id;
        }

        private async Task<IEnumerable<CategoryViewModel>> GetCategories()
            => await dbContext.Categories
            .Select(c => new CategoryViewModel()
            {
                Id = c.Id,
                Name = c.Name
            })
            .ToListAsync();
    }
}
