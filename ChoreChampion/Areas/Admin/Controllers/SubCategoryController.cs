using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChoreChampion.Data;
using ChoreChampion.Models;
using ChoreChampion.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ChoreChampion.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SubCategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        // TempData is great for showing status messages, after the message is closed or the page is refreshed, the message dissapears.
        [TempData]
        public string StatusMessage { get; set; }

        public SubCategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET - Index
        public async Task<IActionResult> Index()
        {
            // This line will get all subcategories from the DB, and tells the db to include each category associated with each subcategory.
            // This is eager loading
            var subCategories = await _db.SubCategory.Include(s => s.Category).ToListAsync().ConfigureAwait(false);
            // Pass this list of subcategories (with categories includes) to view
            return View(subCategories);
        }

        // GET - Create
        public async Task<IActionResult> Create()
        {
            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync().ConfigureAwait(false),
                SubCategory = new Models.SubCategory(),
                // Get subcategories ordered by name, return each subcategory name, return only distinct records
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync().ConfigureAwait(false)
            };

            return View(model);
        }

        // POST - Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Get all the records where the name matches the models name, and is contained in the same category
                var subCategoryExists = _db.SubCategory.Include(s => s.Category).Where(s => s.Name == model.SubCategory.Name && s.Category.Id == model.SubCategory.CategoryId);

                // If the subcategory already exists in this given category
                if (subCategoryExists.Count() > 0)
                {
                    // Error, not allowed
                    StatusMessage = "Error: Another Sub Category with this name exists under " + subCategoryExists.First().Category.Name + " category. Please use a different name.";
                }
                else
                {
                    _db.SubCategory.Add(model.SubCategory);
                    await _db.SaveChangesAsync().ConfigureAwait(false);
                    return RedirectToAction(nameof(Index));
                }
            }

            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync().ConfigureAwait(false),
                SubCategory = model.SubCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).ToListAsync().ConfigureAwait(false),
                StatusMessage = StatusMessage
            };
            return View(modelVM);
        }

        [ActionName("GetSubCategory")]
        public async Task<IActionResult> GetSubCategory(int id)
        {
            List<SubCategory> subCategories = new List<SubCategory>();

            // Select each subcategory in the subcategory db where the categoryid matches the id passed in
            subCategories = await (from subCategory in _db.SubCategory
                                   where subCategory.CategoryId == id
                                   select subCategory).ToListAsync().ConfigureAwait(false);

            return Json(new SelectList(subCategories, "Id", "Name"));
        }

        // GET - Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            else
            {
                var subCategory = await _db.SubCategory.SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);

                if (subCategory == null)
                {
                    return NotFound();
                }
                else
                {
                    SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
                    {
                        CategoryList = await _db.Category.ToListAsync().ConfigureAwait(false),
                        SubCategory = subCategory,
                        // Get subcategories ordered by name, return each subcategory name, return only distinct records
                        SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync().ConfigureAwait(false)
                    };
                    return View(model);
                }
            }
        }

        // POST - Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Get all the records where the name matches the models name, and is contained in the same category
                var subCategoryExists = _db.SubCategory.Include(s => s.Category).Where(s => s.Name == model.SubCategory.Name && s.Category.Id == model.SubCategory.CategoryId);

                // If the subcategory already exists in this given category
                if (subCategoryExists.Count() > 0)
                {
                    // Error, not allowed
                    StatusMessage = "Error: Another Sub Category with this name exists under " + subCategoryExists.First().Category.Name + " category. Please use a different name.";
                }
                else
                {
                    // Find the id from the database that matches the id passed in
                    var subCategoryFromDb = await _db.SubCategory.FindAsync(model.SubCategory.Id);
                    // Change the name for that subcategory to the new passed in name
                    subCategoryFromDb.Name = model.SubCategory.Name;

                    await _db.SaveChangesAsync().ConfigureAwait(false);
                    return RedirectToAction(nameof(Index));
                }
            }

            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync().ConfigureAwait(false),
                SubCategory = model.SubCategory,
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).ToListAsync().ConfigureAwait(false),
                StatusMessage = StatusMessage
            };
            return View(modelVM);
        }

        //GET Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var subCategory = await _db.SubCategory.Include(s => s.Category).SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (subCategory == null)
            {
                return NotFound();
            }

            return View(subCategory);
        }

        //GET Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var subCategory = await _db.SubCategory.Include(s => s.Category).SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            if (subCategory == null)
            {
                return NotFound();
            }

            return View(subCategory);
        }

        //POST Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subCategory = await _db.SubCategory.SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            _db.SubCategory.Remove(subCategory);
            await _db.SaveChangesAsync().ConfigureAwait(false);
            return RedirectToAction(nameof(Index));
        }
    }
}