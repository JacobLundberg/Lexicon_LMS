using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Lexicon_LMS.Data;
using Lexicon_LMS.Models;

namespace Lexicon_LMS.Controllers
{
	//[Authorize(Roles = "Teacher")]
	public class ModuleController : Controller
    {
		private readonly ApplicationDbContext _context;
		private readonly UserManager<ApplicationUser> userManager;

		public IActionResult Index()
        {
            return View();
        }

		//public IActionResult GetAllModules(int id)
		//{

		//}
    }
}