using Lexicon_LMS.Data;
using Lexicon_LMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Lexicon_LMS
{
    public class ActivityModelsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ActivityModelsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ActivityModels
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ActivityModel.Include(a => a.ActivityType);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ActivityModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activityModel = await _context.ActivityModel
                .Include(a => a.ActivityType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (activityModel == null)
            {
                return NotFound();
            }

            return View(activityModel);
        }

        // GET: ActivityModels/Create
        public IActionResult Create()
        {
            ViewData["ActivityTypeId"] = new SelectList(_context.Set<ActivityType>(), "Id", "Name");

            var @activityModel = new ActivityModel
            { ModuleId = int.Parse(TempData.Peek("LastModuleId").ToString()),
                StartDate = DateTime.Parse(TempData.Peek("LastModuleStartDate").ToString().Replace("00:00:00","09:00:00")),
                StopDate = DateTime.Parse(TempData.Peek("LastModuleStartDate").ToString().Replace("00:00:00", "12:15:00"))
            };
            return View(@activityModel);
        }

        // POST: ActivityModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ActivityTypeId,Name,StartDate,StopDate,Description,ModuleId")] ActivityModel activityModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(activityModel);
                await _context.SaveChangesAsync();
		
				var url = "~/Modules/Details/" + activityModel.ModuleId;
                return LocalRedirect(url);
//                return RedirectToAction(nameof(Index));
            }
            ViewData["ActivityTypeId"] = new SelectList(_context.Set<ActivityType>(), "Id", "Name", activityModel.ActivityTypeId);
            return View(activityModel);
        }

        // GET: ActivityModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activityModel = await _context.ActivityModel.FindAsync(id);
            if (activityModel == null)
            {
                return NotFound();
            }
            ViewData["ActivityTypeId"] = new SelectList(_context.Set<ActivityType>(), "Id", "Name", activityModel.ActivityTypeId);
            return View(activityModel);
        }

        // POST: ActivityModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ActivityTypeId,Name,StartDate,StopDate,Description")] ActivityModel activityModel)
        {
            if (id != activityModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(activityModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActivityModelExists(activityModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ActivityTypeId"] = new SelectList(_context.Set<ActivityType>(), "Id", "Name", activityModel.ActivityTypeId);
            return View(activityModel);
        }

        // GET: ActivityModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activityModel = await _context.ActivityModel
                .Include(a => a.ActivityType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (activityModel == null)
            {
                return NotFound();
            }

            return View(activityModel);
        }

        // POST: ActivityModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var activityModel = await _context.ActivityModel.FindAsync(id);
            _context.ActivityModel.Remove(activityModel);
            await _context.SaveChangesAsync();
//            return RedirectToAction(nameof(Index));
            var url = "~/Modules/Details/" + TempData.Peek("LastModuleId");
            return LocalRedirect(url);
        }

        private bool ActivityModelExists(int id)
        {
            return _context.ActivityModel.Any(e => e.Id == id);
        }
    }
}
