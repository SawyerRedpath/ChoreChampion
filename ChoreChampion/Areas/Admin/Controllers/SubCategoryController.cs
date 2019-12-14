using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChoreChampion.Data;
using ChoreChampion.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChoreChampion.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SubCategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        public SubCategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET - Index
        public async Task<IActionResult> Index()
        {
            // This line will get all subcategories from the DB, and tells the db to include each category associated with each subcategory.
            // This is eager loading
            var subCategories = await _db.SubCategory.Include(s => s.Category).ToListAsync();
            // Pass this list of subcategories (with categories includes) to view
            return View(subCategories);
        }

        // GET - Create
        public async Task<IActionResult> Create()
        {
            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategory = new Models.SubCategory(),
                // Get subcategories ordered by name, return each subcategory name, return only distinct records
                SubCategoryList = await _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync()
            };

            return View(model);
        }
    }
}