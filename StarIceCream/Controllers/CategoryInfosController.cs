using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StarIceCream.Data;
using StarIceCream.Models;


namespace StarIceCream.Controllers
{
    [Authorize]
    public class CategoryInfosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryInfosController(ApplicationDbContext context)
        {
            _context = context;
        }

        
        // GET: CategoryInfos
        public async Task<IActionResult> Index(string sortOrder)
        {
            /* Older Code 
            return View(await _context.Categories.ToListAsync());
            */
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";        
            var categories = from s in _context.Categories
                           select s;
            switch (sortOrder)
            {
                case "name_desc":
                    categories = categories.OrderByDescending(s => s.CategoryName);
                    break;               
                default:
                    categories = categories.OrderBy(s => s.CategoryName);
                    break;
            }
            return View(await categories.AsNoTracking().ToListAsync());

        }

        // GET: CategoryInfos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoryInfo = await _context.Categories
                .FirstOrDefaultAsync(m => m.CategoryID == id);
            if (categoryInfo == null)
            {
                return NotFound();
            }

            return View(categoryInfo);
        }

        // GET: CategoryInfos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CategoryInfos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryID,CategoryName")] CategoryInfo categoryInfo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(categoryInfo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(categoryInfo);
        }

        // GET: CategoryInfos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoryInfo = await _context.Categories.FindAsync(id);
            if (categoryInfo == null)
            {
                return NotFound();
            }
            return View(categoryInfo);
        }

        // POST: CategoryInfos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryID,CategoryName")] CategoryInfo categoryInfo)
        {
            if (id != categoryInfo.CategoryID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categoryInfo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryInfoExists(categoryInfo.CategoryID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(categoryInfo);
        }

        // GET: CategoryInfos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoryInfo = await _context.Categories
                .FirstOrDefaultAsync(m => m.CategoryID == id);
            if (categoryInfo == null)
            {
                return NotFound();
            }

            return View(categoryInfo);
        }

        // POST: CategoryInfos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categoryInfo = await _context.Categories.FindAsync(id);
            _context.Categories.Remove(categoryInfo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryInfoExists(int id)
        {
            return _context.Categories.Any(e => e.CategoryID == id);
        }
    }
}
