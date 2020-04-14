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
    public class EnquiryInfosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EnquiryInfosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EnquiryInfos
        public async Task<IActionResult> Index()
        {
            return View(await _context.Enquiries.ToListAsync());
        }

        // GET: EnquiryInfos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enquiryInfo = await _context.Enquiries
                .FirstOrDefaultAsync(m => m.EnquiryID == id);
            if (enquiryInfo == null)
            {
                return NotFound();
            }

            return View(enquiryInfo);
        }

        [AllowAnonymous]
        // GET: EnquiryInfos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EnquiryInfos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EnquiryID,Name,Email,Mobile,Subject,Message")] EnquiryInfo enquiryInfo)
        {
            if (ModelState.IsValid)
            {
                enquiryInfo.EnquiryDate = DateTime.Now;
                _context.Add(enquiryInfo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Create));
            }
            return View(enquiryInfo);
        }

        // GET: EnquiryInfos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enquiryInfo = await _context.Enquiries.FindAsync(id);
            if (enquiryInfo == null)
            {
                return NotFound();
            }
            return View(enquiryInfo);
        }

        // POST: EnquiryInfos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EnquiryID,Name,Email,Mobile,Subject,Message,EnquiryDate")] EnquiryInfo enquiryInfo)
        {
            if (id != enquiryInfo.EnquiryID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(enquiryInfo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnquiryInfoExists(enquiryInfo.EnquiryID))
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
            return View(enquiryInfo);
        }

        // GET: EnquiryInfos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enquiryInfo = await _context.Enquiries
                .FirstOrDefaultAsync(m => m.EnquiryID == id);
            if (enquiryInfo == null)
            {
                return NotFound();
            }

            return View(enquiryInfo);
        }

        // POST: EnquiryInfos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var enquiryInfo = await _context.Enquiries.FindAsync(id);
            _context.Enquiries.Remove(enquiryInfo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EnquiryInfoExists(int id)
        {
            return _context.Enquiries.Any(e => e.EnquiryID == id);
        }
    }
}
