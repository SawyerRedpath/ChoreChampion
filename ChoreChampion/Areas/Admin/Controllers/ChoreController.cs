using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChoreChampion.Data;
using ChoreChampion.Models;
using ChoreChampion.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace ChoreChampion.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ChoreController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IHostEnvironment _hostingEnvironment;

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
    }
}