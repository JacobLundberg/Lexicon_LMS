using Lexicon_LMS.Data;
using Lexicon_LMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Lexicon_LMS
{
    public class ModulesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ModulesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Modules
        public async Task<IActionResult> Index()
        {
            return View(await _context.Module.ToListAsync());
        }

        // GET: Modules/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @module = await _context.Module
                .Include(ma => ma.ModuleActivities)
                .ThenInclude(ac => ac.ActivityType)
                .FirstOrDefaultAsync(m => m.Id == id)
                ;
            if (@module == null)
            {
                return NotFound();
            }
            return View(@module);
        }

        // GET: Modules/Create
        [HttpGet]
        public IActionResult Create(int courseId)
        {
            var @module = new Module {  CourseId = courseId,
          StartTime = DateTime.Parse(TempData.Peek("LastCourseStartDate").ToString()),
          EndTime = DateTime.Parse(TempData.Peek("LastCourseStartDate").ToString())
            };

            ViewData["courseTimeStart"] = "";
            ViewData["startTime"] = "";

            if (module.CourseId > 0)
            {
                var course = _context.Course.Where(c => c.Id == module.CourseId).SingleOrDefault();
                if (course != null)
                {
                    CultureInfo culture = CultureInfo.CreateSpecificCulture("sv-SE");  // en-US
                    CultureInfo ci = CultureInfo.InvariantCulture;
                    
                    ViewData["courseTimeStart"] = course.StartDate.ToString("dd/MM-yy", ci);
                    ViewData["startTime"] = course.StartDate.ToString("yyyy-dd-MM");
                }
            }

            return View(@module);
        }

        // POST: Modules/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,StartTime,EndTime,CourseId")] Module @module)
        {
            Boolean timeError = false;

            ViewData["errorTimeStart"] = "";
            ViewData["errorTimeEnd"] = "";

            var course = _context.Course.Where(m => m.Id == module.CourseId).SingleOrDefault();

            if (ModelState.IsValid)
            {
                if (course != null)
                {
                    if (module.StartTime < course.StartDate)  // Före kurs
                    {
                        timeError = true;
                        ViewData["errorTimeStart"] = "Starttid kan inte vara före kursens starttid!";
                    }

                    if (module.EndTime < module.StartTime)  // Före starttid
                    {
                        timeError = true;
                        ViewData["errorTimeEnd"] = "Sluttid kan inte vara före starttid!";
                    }

                    if (timeError == false)
                    {
                        _context.Add(@module);
                        await _context.SaveChangesAsync();
                        var url = "~/Courses/Details/" + @module.CourseId;
                        return LocalRedirect(url);
                    }
                }
            }
            
            // Skapa data vid fel
            // CultureInfo culture = CultureInfo.CreateSpecificCulture("sv-SE");  // en-US
            CultureInfo ci = CultureInfo.InvariantCulture;

            ViewData["courseTimeStart"] = course.StartDate.ToString("dd/MM-yy", ci);
            ViewData["startTime"] = course.StartDate.ToString("yyyy-dd-MM");

            return View(@module);
        }

        // GET: Modules/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @module = await _context.Module.FindAsync(id);
            if (@module == null)
            {
                return NotFound();
            }
            return View(@module);
        }

        // POST: Modules/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,StartTime,EndTime, CourseId")] Module @module)
        {
            if (id != @module.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@module);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModuleExists(@module.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                var url = "~/Courses/Details/" + @module.CourseId;
                return LocalRedirect(url);
//                return RedirectToAction(nameof(Index));
            }
            return View(@module);
        }

        // GET: Modules/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @module = await _context.Module
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@module == null)
            {
                return NotFound();
            }

            return View(@module);
        }

        // POST: Modules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @module = await _context.Module.FindAsync(id);
            _context.Module.Remove(@module);
            await _context.SaveChangesAsync();
//            return RedirectToAction(nameof(Index));
            var url = "~/Courses/Details/" + TempData.Peek("LastCourseId");
            return LocalRedirect(url);
        }

        private bool ModuleExists(int id)
        {
            return _context.Module.Any(e => e.Id == id);
        }
    }
}
