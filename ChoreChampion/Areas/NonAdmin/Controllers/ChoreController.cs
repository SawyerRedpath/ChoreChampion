using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ChoreChampion.Data;
using ChoreChampion.Models;
using ChoreChampion.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChoreChampion.Areas.NonAdmin.Controllers
{
    [Authorize(Roles = StaticDetails.RegularUser)]
    [Area("NonAdmin")]
    public class ChoreController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly UserManager<IdentityUser> _userManager;

        public ChoreController(ApplicationDbContext db, IWebHostEnvironment hostingEnvironment, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string userId = _userManager.GetUserId(User);

            var chores = await _db.Chore.Where(m => m.UserId == userId).Where(m => m.IsComplete == false)
                .Include(m => m.Category)
                .Include(m => m.SubCategory)
                .OrderByDescending(m => m.DueDate)
                .ToListAsync()
                .ConfigureAwait(false);
            return View(chores);
        }
    }
}