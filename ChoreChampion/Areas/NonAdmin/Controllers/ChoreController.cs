using System;
using System.IO;
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
                .Where(m => m.DueDate >= DateTime.Now.AddDays(-2))
                .Include(m => m.Category)
                .Include(m => m.SubCategory)
                .OrderByDescending(m => m.DueDate)
                .ToListAsync()
                .ConfigureAwait(false);
            return View(chores);
        }

        [HttpGet]
        public async Task<IActionResult> CompleteChore(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            else
            {
                var chore = await _db.Chore.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);

                if (chore == null)
                {
                    return NotFound();
                }
                else
                {
                    return View(chore);
                }
            }
        }

        // POST - CompleteChore
        [HttpPost, ActionName("CompleteChore")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteChorePost(int? id)
        {
            if (ModelState.IsValid)
            {
                // Image saving below
                string webRootPath = _hostingEnvironment.WebRootPath;
                // Uploaded files from the form (will only be one)
                var files = HttpContext.Request.Form.Files;

                // The chore that was just added to the database
                var choreFromDb = await _db.Chore.FindAsync(id);

                // File has been uploaded
                if (files.Count > 0)
                {
                    var uploads = Path.Combine(webRootPath, "images\\AfterImages");
                    var extension = Path.GetExtension(files[0].FileName);

                    using (var fileStream = new FileStream(Path.Combine(uploads, id + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }
                    choreFromDb.AfterImage = @"\images\AfterImages\" + id + extension;
                }

                choreFromDb.IsComplete = true;

                await _db.SaveChangesAsync().ConfigureAwait(false);

                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(id);
            }
        }
    }
}