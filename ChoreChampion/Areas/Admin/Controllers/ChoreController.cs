using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ChoreChampion.Data;
using ChoreChampion.Models;
using ChoreChampion.Models.ViewModels;
using ChoreChampion.Utility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChoreChampion.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ChoreController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostingEnvironment;

        [BindProperty]
        public ChoreViewModel ChoreVM { get; set; }

        public ChoreController(ApplicationDbContext db, IWebHostEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
            ChoreVM = new ChoreViewModel()
            {
                Category = _db.Category,
                Chore = new Chore()
            };
        }

        public async Task<IActionResult> Index()
        {
            // .Include to use eager loading to include associated category and subcategory.
            var chores = await _db.Chore.Include(m => m.Category).Include(m => m.SubCategory).ToListAsync();
            return View(chores);
        }

        // GET - CREATE
        public IActionResult Create()
        {
            return View(ChoreVM);
        }

        // POST - CREATE
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        // The menu item viewmodel does not need to be passed to the method because I have used the BindProperty above
        public async Task<IActionResult> CreatePOST()
        {
            ChoreVM.Chore.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

            if (ModelState.IsValid)
            {
                _db.Chore.Add(ChoreVM.Chore);
                await _db.SaveChangesAsync();

                // Image saving below
                string webRootPath = _hostingEnvironment.WebRootPath;
                // Uploaded files from the form (will only be one)
                var files = HttpContext.Request.Form.Files;

                // The chore that was just added to the database
                var choreFromDb = await _db.Chore.FindAsync(ChoreVM.Chore.Id);

                // File has been uploaded
                if (files.Count > 0)
                {
                    var uploads = Path.Combine(webRootPath, "images");
                    var extension = Path.GetExtension(files[0].FileName);

                    using (var fileStream = new FileStream(Path.Combine(uploads, ChoreVM.Chore.Id + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }
                    choreFromDb.BeforeImage = @"\images\" + ChoreVM.Chore.Id + extension;
                }

                // No file uploaded, use default
                else
                {
                    var uploads = Path.Combine(webRootPath, @"images\" + StaticDetails.PlaceHolderImage);
                    System.IO.File.Copy(uploads, webRootPath + @"\images\" + ChoreVM.Chore.Id + ".png");
                    choreFromDb.BeforeImage = @"\images\" + ChoreVM.Chore.Id + ".png";
                }

                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(ChoreVM);
            }
        }
    }
}