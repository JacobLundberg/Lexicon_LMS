using Lexicon_LMS.Data;
using Lexicon_LMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
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


            if (activityModel.ModuleId > 0)
            {
                var module = _context.Module.Where(m => m.Id == activityModel.ModuleId).SingleOrDefault();
                if (module != null)
                {
                    var startTime = module.StartTime;  //.ToString("yyyy-dd-MM hh:mm");
                    var endTime = module.StartTime;  //.ToString("yyyy-dd-MM hh:mm");

                    // method
                    CultureInfo culture = CultureInfo.CreateSpecificCulture("sv-SE");  // en-US
                    CultureInfo ci = CultureInfo.InvariantCulture;
                    
                    // 2018-11-26T09:00:00.000
                    
                    // 24/12 12:30
                    ViewData["modTimeStart"] = module.StartTime.ToString("dd/MM hh:mm", ci); 
                    ViewData["modTimeEnd"] = module.EndTime.ToString("dd/MM hh:mm", ci);
                    // 1999-12-24 12:30
                    ViewData["startTime"] = startTime.ToString("yyyy-dd-MM hh:mm");
                    ViewData["endTime"] = endTime.ToString("yyyy-dd-MM hh:mm");

                }

            }


            return View(@activityModel);
        }

        // POST: ActivityModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ActivityTypeId,Name,StartDate,StopDate,Description,ModuleId")] ActivityModel activityModel)
        {
            Boolean timeError = false;

            ViewData["errorTimeStart"] = "";
            ViewData["errorTimeEnd"] = "";

            if (ModelState.IsValid)
            {
                var module = _context.Module.Where(m => m.Id == activityModel.ModuleId).SingleOrDefault();
                if (module != null)
                {
                    if (activityModel.StartDate < module.StartTime)  // Före modulen
                    {
                        timeError = true;
                        ViewData["errorTimeStart"] = "Starttid kan inte vara före modulens starttid!";
                    }
                    else if (activityModel.StartDate > module.EndTime)  // Efter modulen
                    {
                        timeError = true;
                        ViewData["errorTimeStart"] = "Starttid kan inte vara efter modulens sluttid!";
                    }

                    if (activityModel.StopDate > module.EndTime)  // Efter modulen
                    {
                        timeError = true;
                        ViewData["errorTimeEnd"] = "Sluttid kan inte vara efter modulens sluttid!";
                    }
                    else if (activityModel.StopDate < activityModel.StartDate)  // Före starttid
                    {
                        timeError = true;
                        ViewData["errorTimeEnd"] = "Sluttid kan inte vara före starttid!";
                    }


                    if (timeError == false) { 
                        _context.Add(activityModel);
                        await _context.SaveChangesAsync();
		
				        var url = "~/Modules/Details/" + activityModel.ModuleId;
                        return LocalRedirect(url);
                        //                return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        var startTime = module.StartTime;
                        var endTime = module.StartTime;

                        // För att få t.ex: 24/12 12:30
                        CultureInfo culture = CultureInfo.CreateSpecificCulture("sv-SE");  // en-US
                        CultureInfo ci = CultureInfo.InvariantCulture;

                        // 24/12 12:30
                        ViewData["modTimeStart"] = module.StartTime.ToString("dd/MM hh:mm", ci);
                        ViewData["modTimeEnd"] = module.EndTime.ToString("dd/MM hh:mm", ci);
                        // 1999-12-24 12:30
                        ViewData["startTime"] = startTime.ToString("yyyy-dd-MM hh:mm");
                        ViewData["endTime"] = endTime.ToString("yyyy-dd-MM hh:mm");
                    }
                }
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,ActivityTypeId,Name,StartDate,StopDate,Description,ModuleId")] ActivityModel activityModel)
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
                //                return RedirectToAction(nameof(Index));
                var url = "~/Modules/Details/" + TempData.Peek("LastModuleId");
                return LocalRedirect(url);

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
