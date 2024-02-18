using Homies.Data;
using Homies.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Claims;

namespace Homies.Controllers
{
    [Authorize]
    public class EventController : Controller
    {
        private readonly HomiesDbContext dbContext;

        public EventController(HomiesDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IActionResult> All()
        {
            var allEvents = await dbContext.Events
                .AsNoTracking()
                .Select(e => new EventAllViewModel(
                e.Id,
                e.Name,
                e.Start,
                e.Type.Name,
                e.Organiser.UserName
                ))
                .ToListAsync();

            return View(allEvents);
        }

        [HttpPost]
        public async Task<IActionResult> Join(int id)
        {
            var currentEvent = await dbContext.Events
                .Where(e => e.Id == id)
                .Include(e => e.EventsParticipants)
                .FirstOrDefaultAsync();
            
            if (currentEvent == null)
            {
                return BadRequest();
            }

            string currentUserId = GetUserId();

            if (!currentEvent.EventsParticipants.Any(p => p.HelperId == currentUserId))
            {
                currentEvent.EventsParticipants.Add(new EventParticipant()
                {
                    EventId = currentEvent.Id,
                    HelperId = currentUserId
                });

                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Joined));
        }

        [HttpGet]
        public async Task<IActionResult> Joined()
        {
            string currentUserId = GetUserId();

            var currentUserEvents = await dbContext.EventParticipants
                .Where(ep => ep.HelperId == currentUserId)
                .AsNoTracking()
                .Select(ep => new EventAllViewModel(
                    ep.EventId,
                    ep.Event.Name,
                    ep.Event.Start,
                    ep.Event.Type.Name,
                    ep.Event.Organiser.UserName
                ))
                .ToListAsync();

            return View(currentUserEvents);
        }

        public async Task<IActionResult> Leave(int id)
        {
            var currentEvent = await dbContext.Events
                .Where(e => e.Id == id)
                .Include(e => e.EventsParticipants)
                .FirstOrDefaultAsync();

            if (currentEvent == null)
            {
                return BadRequest();
            }

            string currentUserId = GetUserId();

            var eventParticipant = currentEvent.EventsParticipants
                .FirstOrDefault(ep => ep.HelperId == currentUserId);

            if (eventParticipant == null)
            {
                return BadRequest();
            }

            currentEvent.EventsParticipants.Remove(eventParticipant);

            await dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new EventAddViewModel();
            model.Types = await GetTypes();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(EventAddViewModel eventForm)
        {
            
            if (eventForm == null)
            {
                return BadRequest();
            }
            
            DateTime start = DateTime.Now;
            DateTime end = DateTime.Now;

            if (!DateTime.TryParseExact(
                eventForm.Start,
                DataConstants.DateFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out start))
            {
                ModelState
                    .AddModelError(nameof(eventForm.Start), $"Invalid date! Format must be: {DataConstants.DateFormat}");
            }

            if (!DateTime.TryParseExact(
                eventForm.End,
                DataConstants.DateFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out end))
            {
                ModelState
                    .AddModelError(nameof(eventForm.End), $"Invalid date! Format must be: {DataConstants.DateFormat}");
                   
            }

            if (!ModelState.IsValid)
            {
                //This is used to show the error messages of the ModelState
                List<string> errorMessages = new List<string>();

                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        errorMessages.Add(error.ErrorMessage);
                    }
                }

                eventForm.Types = await GetTypes();
                return View(eventForm);
            }

            var entity = new Event()
            {
                CreatedOn = DateTime.Now,
                Description = eventForm.Description,
                Name = eventForm.Name,
                OrganiserId = GetUserId(),
                TypeId = eventForm.TypeId,
                Start = start,
                End = end
            };

            await dbContext.AddAsync(entity);
            await dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var searchedEvent = await dbContext.Events
                .FindAsync(id);

            if (searchedEvent == null)
            {
                return BadRequest();
            }

            if (searchedEvent.OrganiserId != GetUserId())
            {
                return Unauthorized();
            }

            var eventToEdit = new EventAddViewModel()
            {
                Name = searchedEvent.Name,
                Description = searchedEvent.Description,
                End = searchedEvent.End.ToString(DataConstants.DateFormat),
                Start = searchedEvent.Start.ToString(DataConstants.DateFormat),
                TypeId = searchedEvent.TypeId
            };

            eventToEdit.Types = await GetTypes();

            return View(eventToEdit);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EventAddViewModel eventForm, int id)
        {

            var eventToEdit = await dbContext.Events
                .FindAsync(id);

            if (eventToEdit == null)
            {
                return BadRequest();
            }

            if (eventToEdit.OrganiserId != GetUserId())
            {
                return Unauthorized();
            }

            DateTime start = DateTime.Now;
            DateTime end = DateTime.Now;

            if (!DateTime.TryParseExact(
                eventForm.Start,
                DataConstants.DateFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out start))
            {
                ModelState
                    .AddModelError(nameof(eventForm.Start), $"Invalid date! Format must be: {DataConstants.DateFormat}");
            }

            if (!DateTime.TryParseExact(
                eventForm.End,
                DataConstants.DateFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out end))
            {
                ModelState
                    .AddModelError(nameof(eventForm.End), $"Invalid date! Format must be: {DataConstants.DateFormat}");

            }

            if (!ModelState.IsValid)
            {
                
                //This is used to show the error messages of the ModelState
                List<string> errorMessages = new List<string>();

                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        errorMessages.Add(error.ErrorMessage);
                    }
                }
                
                eventForm.Types = await GetTypes();
                return View(eventForm);
            }

            eventToEdit.Start = start;
            eventToEdit.End = end;
            eventToEdit.Description = eventForm.Description;
            eventToEdit.Name = eventForm.Name;
            eventToEdit.TypeId = eventForm.TypeId;

            await dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var currentEvent = await dbContext.Events
                .Where(e => e.Id == id)
                .FirstOrDefaultAsync();

            var eventParticipants = await dbContext.EventParticipants
                .Where(ep => ep.EventId == id)
                .ToListAsync();

            if (currentEvent == null)
            {
                return BadRequest();
            }

            if (currentEvent.OrganiserId != GetUserId())
            {
                return Unauthorized();
            }

            if (eventParticipants != null && eventParticipants.Any())
            {
                dbContext.EventParticipants.RemoveRange(eventParticipants);
            }

            dbContext.Events.Remove(currentEvent);
            await dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(All));

        }


        public async Task<IActionResult> Details(int id)
        {
            var searchedEvent = await dbContext.Events
                .Where(e => e.Id == id)
                .AsNoTracking()
                .Select(e => new EventDetailsViewModel()
                {
                    Id = e.Id,
                    CreatedOn = e.CreatedOn.ToString(DataConstants.DateFormat),
                    Description = e.Description,
                    End = e.End.ToString(DataConstants.DateFormat),
                    Name = e.Name,
                    Organiser = e.Organiser.UserName,
                    Start = e.Start.ToString(DataConstants.DateFormat),
                    Type = e.Type.Name
                })
                .FirstOrDefaultAsync();
            
            if (searchedEvent == null) 
            {
                return BadRequest();
            }

            return View(searchedEvent);
        }


        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }

        private async Task<IEnumerable<TypeViewModel>> GetTypes()
        {
            return await dbContext.Types
                .AsNoTracking()
                .Select(t => new TypeViewModel
                {
                    Id = t.Id,
                    Name = t.Name
                })
                .ToListAsync();
        }
    }
}
