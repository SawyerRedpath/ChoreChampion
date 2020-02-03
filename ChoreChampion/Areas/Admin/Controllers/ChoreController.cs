using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ChoreChampion.Data;
using ChoreChampion.Models;
using ChoreChampion.Models.ViewModels;
using ChoreChampion.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChoreChampion.Areas.Admin.Controllers
{
    [Authorize(Roles = StaticDetails.AdminUser)]
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
                Chore = new Chore(),
                User = _db.ApplicationUser
            };
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // .Include to use eager loading to include associated category and subcategory.
            var chores = await _db.Chore.Where(m => m.IsComplete == false).Include(m => m.Category).Include(m => m.SubCategory).Include(m => m.User).OrderByDescending(m => m.DueDate).ToListAsync().ConfigureAwait(false);
            return View(chores);
        }

        // GET - CompletedChores
        [HttpGet]
        public async Task<IActionResult> CompletedChores()
        {
            // .Include to use eager loading to include associated category and subcategory.
            var chores = await _db.Chore.Where(m => m.IsComplete == true).Include(m => m.Category).Include(m => m.SubCategory).Include(m => m.User).OrderByDescending(m => m.DueDate).ToListAsync().ConfigureAwait(false);
            return View(chores);
        }

        // GET - CREATE
        [HttpGet]
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
            ChoreVM.User = await _db.ApplicationUser.ToListAsync().ConfigureAwait(false);

            if (ModelState.IsValid)
            {
                _db.Chore.Add(ChoreVM.Chore);
                await _db.SaveChangesAsync().ConfigureAwait(false);

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

                await _db.SaveChangesAsync().ConfigureAwait(false);

                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(ChoreVM);
            }
        }

        // GET - Edit
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ChoreVM.Chore = await _db.Chore.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);
            ChoreVM.SubCategory = await _db.SubCategory.Where(s => s.CategoryId == ChoreVM.Chore.CategoryId).ToListAsync().ConfigureAwait(false);

            if (ChoreVM.Chore == null)
            {
                return NotFound();
            }
            else
            {
                return View(ChoreVM);
            }
        }

        // POST - Edit
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        // The menu item viewmodel does not need to be passed to the method because I have used the BindProperty above
        public async Task<IActionResult> EditPOST(int? id)
        {
            ChoreVM.Chore.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

            if (ModelState.IsValid)
            {
                // Image saving below
                string webRootPath = _hostingEnvironment.WebRootPath;
                // Uploaded files from the form (will only be one)
                var files = HttpContext.Request.Form.Files;

                // The chore that was just added to the database
                var choreFromDb = await _db.Chore.FindAsync(ChoreVM.Chore.Id);

                // New image has been uploaded
                if (files.Count > 0)
                {
                    var uploads = Path.Combine(webRootPath, "images");
                    var extension_new = Path.GetExtension(files[0].FileName);

                    // Delete the original image
                    var imagePath = Path.Combine(webRootPath, choreFromDb.BeforeImage.TrimStart('\\'));

                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }

                    // Upload new image
                    using (var fileStream = new FileStream(Path.Combine(uploads, ChoreVM.Chore.Id + extension_new), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }
                    choreFromDb.BeforeImage = @"\images\" + ChoreVM.Chore.Id + extension_new;
                }

                // Incase user has changed any of these fields, make the changes
                choreFromDb.Name = ChoreVM.Chore.Name;
                choreFromDb.Description = ChoreVM.Chore.Description;
                choreFromDb.DueDate = ChoreVM.Chore.DueDate;
                choreFromDb.CategoryId = ChoreVM.Chore.CategoryId;
                choreFromDb.SubCategoryId = ChoreVM.Chore.SubCategoryId;
                choreFromDb.Name = ChoreVM.Chore.Name;

                await _db.SaveChangesAsync().ConfigureAwait(false);

                return RedirectToAction(nameof(Index));
            }
            else
            {
                ChoreVM.SubCategory = await _db.SubCategory.Where(s => s.CategoryId == ChoreVM.Chore.CategoryId).ToListAsync().ConfigureAwait(false);
                return View(ChoreVM);
            }
        }

        // GET - Details
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            else
            {
                // Find the chore with id matching the one passed in
                ChoreVM.Chore = await _db.Chore.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);

                if (ChoreVM.Chore == null)
                {
                    return NotFound();
                }
                else
                {
                    return View(ChoreVM);
                }
            }
        }

        // GET - Delete
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            else
            {
                ChoreVM.Chore = await _db.Chore.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.Id == id).ConfigureAwait(false);

                if (ChoreVM.Chore == null)
                {
                    return NotFound();
                }
                else
                {
                    return View(ChoreVM);
                }
            }
        }

        // Post - Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string webRootPath = _hostingEnvironment.WebRootPath;
            Chore chore = await _db.Chore.FindAsync(id);

            if (chore != null)
            {
                var imagePath = Path.Combine(webRootPath, chore.BeforeImage.TrimStart('\\'));

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                _db.Chore.Remove(chore);
                await _db.SaveChangesAsync().ConfigureAwait(false);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}