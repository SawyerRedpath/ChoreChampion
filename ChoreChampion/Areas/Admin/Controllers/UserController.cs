using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ChoreChampion.Data;
using ChoreChampion.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChoreChampion.Areas.Admin.Controllers
{
    [Authorize(Roles = StaticDetails.AdminUser)]
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;

        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            // Figure out which user is logged in
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            // Pass all users except for current user
            return View(await _db.ApplicationUser.Where(u => u.Id != claim.Value).ToListAsync().ConfigureAwait(false));
        }

        public async Task<IActionResult> Lock(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            else
            {
                var applicationUser = await _db.ApplicationUser.FirstOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);

                if (applicationUser == null)
                {
                    return NotFound();
                }
                else
                {
                    applicationUser.LockoutEnd = DateTime.Now.AddYears(150);

                    await _db.SaveChangesAsync().ConfigureAwait(false);

                    return RedirectToAction(nameof(Index));
                }
            }
        }

        public async Task<IActionResult> Unlock(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            else
            {
                var applicationUser = await _db.ApplicationUser.FirstOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);

                if (applicationUser == null)
                {
                    return NotFound();
                }
                else
                {
                    applicationUser.LockoutEnd = DateTime.Now;

                    await _db.SaveChangesAsync().ConfigureAwait(false);

                    return RedirectToAction(nameof(Index));
                }
            }
        }
    }
}