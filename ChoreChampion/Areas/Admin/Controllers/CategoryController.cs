using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChoreChampion.Data;
using ChoreChampion.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChoreChampion.Areas.Admin
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        // ApplicationDBContext will automatically look in startup.cs to get connection string and store it in db which we
        // will assign to _db
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET - INDEX
        public async Task<IActionResult> Index()
        {
            return View(await _db.Category.ToListAsync().ConfigureAwait(false));
        }

        // no async because we are not passing anything to the view
        // GET - CREATE
        public IActionResult Create()
        {
            return View();
        }

        // POST - CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                // if everything is valid
                _db.Category.Add(category);
                await _db.SaveChangesAsync().ConfigureAwait(false);

                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(category);
            }
        }

        // GET - EDIT
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            else
            {
                // Find the category with the PK id matching the id passed in and store it in category
                var category = await _db.Category.FindAsync(id);

                if (category == null)
                {
                    return NotFound();
                }
                else
                {
                    return View(category);
                }
            }
        }

        // POST - EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _db.Update(category);
                await _db.SaveChangesAsync().ConfigureAwait(false);

                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(category);
            }
        }

        // GET - DELETE
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            else
            {
                // Find the category with the PK id matching the id passed in and store it in category
                var category = await _db.Category.FindAsync(id);

                if (category == null)
                {
                    return NotFound();
                }
                else
                {
                    return View(category);
                }
            }
        }

        // POST - DELETE
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _db.Category.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }
            else
            {
                _db.Category.Remove(category);
                await _db.SaveChangesAsync().ConfigureAwait(false);
                return RedirectToAction(nameof(Index));
            }
        }

        // GET - DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            else
            {
                // Find the category with the PK id matching the id passed in and store it in category
                var category = await _db.Category.FindAsync(id);

                if (category == null)
                {
                    return NotFound();
                }
                else
                {
                    return View(category);
                }
            }
        }
    }
}