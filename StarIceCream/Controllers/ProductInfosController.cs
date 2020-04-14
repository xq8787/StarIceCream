using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StarIceCream.Data;
using StarIceCream.Models;

namespace StarIceCream.Controllers
{
    [Authorize]
    public class ProductInfosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        public ProductInfosController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _environment = env;
        }


        // GET: ProductInfos
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Products.Include(p => p.Category);
            return View(await applicationDbContext.ToListAsync());
        }

        [AllowAnonymous]
        public async Task<IActionResult> ViewProducts()
        {
            var applicationDbContext = _context.Products.Include(p => p.Category);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ProductInfos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productInfo = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (productInfo == null)
            {
                return NotFound();
            }

            return View(productInfo);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ProductDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productInfo = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (productInfo == null)
            {
                return NotFound();
            }

            return View(productInfo);
        }

        // GET: ProductInfos/Create
        public IActionResult Create()
        {
            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName");
            return View();
        }

        // POST: ProductInfos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductID,ProductTitle,FileUpload,Description,Price,CategoryID")] ProductInfo productInfo)
        {
            using (var memoryStream = new MemoryStream())
            {
                await productInfo.FileUpload.FormFile.CopyToAsync(memoryStream);

                string photoname = productInfo.FileUpload.FormFile.FileName;
                int photosize = (int)productInfo.FileUpload.FormFile.Length;
                string phototype = productInfo.FileUpload.FormFile.ContentType;
                productInfo.PhotoName = photoname;
                productInfo.PhotoType = phototype;
                productInfo.ExtName = Path.GetExtension(photoname);
                if(!".jpg.jpeg.png.gif.bmp".Contains(productInfo.ExtName.ToLower()))
                {
                    ModelState.AddModelError("FileUpload.FormFile", "Invalid Format of Image Given.");
                }
                else
                {
                    ModelState.Remove("PhotoName");
                    ModelState.Remove("PhotoType");
                    ModelState.Remove("ExtName");
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(productInfo);
                await _context.SaveChangesAsync();
                var uploadsRootFolder = Path.Combine(_environment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsRootFolder))
                {
                    Directory.CreateDirectory(uploadsRootFolder);
                }
                string filename = productInfo.ProductID + productInfo.ExtName;
                var filePath = Path.Combine(uploadsRootFolder, filename);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await productInfo.FileUpload.FormFile.CopyToAsync(fileStream).ConfigureAwait(false);
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName", productInfo.CategoryID);
            return View(productInfo);
        }

        // GET: ProductInfos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productInfo = await _context.Products.FindAsync(id);
            if (productInfo == null)
            {
                return NotFound();
            }
            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName", productInfo.CategoryID);
            return View(productInfo);
        }

        // POST: ProductInfos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductID,ProductTitle,PhotoName,PhotoType,ExtName,Description,Price,CategoryID")] ProductInfo productInfo)
        {
            if (id != productInfo.ProductID)
            {
                return NotFound();
            }
            
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productInfo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductInfoExists(productInfo.ProductID))
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
            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "CategoryName", productInfo.CategoryID);
            return View(productInfo);
        }

        // GET: ProductInfos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productInfo = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (productInfo == null)
            {
                return NotFound();
            }

            return View(productInfo);
        }

        // POST: ProductInfos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productInfo = await _context.Products.FindAsync(id);
            _context.Products.Remove(productInfo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductInfoExists(int id)
        {
            return _context.Products.Any(e => e.ProductID == id);
        }
    }

    
}
