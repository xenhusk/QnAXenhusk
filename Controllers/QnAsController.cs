using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QnAXenhusk.Data;
using QnAXenhusk.Models;
using PagedList;

namespace QnAXenhusk.Controllers
{
    public class QnAsController : Controller
    {
        private readonly QnAXenhuskContext _context;

        public QnAsController(QnAXenhuskContext context)
        {
            _context = context;
        }
        //pagnation
        private async Task<List<QnAs>> PaginateQuery(IQueryable<QnAs> query, int page, int pageSize)
        {
            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        // GET: QnAs
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var query = _context.QnAs.AsQueryable();
            var items = await PaginateQuery(query, page, pageSize);

            ViewData["CurrentPage"] = page;

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewData["TotalPages"] = totalPages;

            return View("QnAList", items);
        }


        // GET: QnAs/ShowSearchForm
        public async Task<IActionResult> ShowSearchForm()
        {
            return View();
        }

        // GET: QnAs/ShowSearchResults
        public async Task<IActionResult> ShowSearchResults(string searchPhrase, int? page)
        {
            const int pageSize = 10; // Set your desired page size here
            int pageNumber = page ?? 1;

            ViewBag.SearchPhrase = searchPhrase; // Pass the search phrase to the view

            var totalItems = await _context.QnAs
                .Where(j => j.Question.Contains(searchPhrase))
                .CountAsync();

            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = totalPages;

            var searchResults = await _context.QnAs
                .Where(j => j.Question.Contains(searchPhrase))
                .OrderBy(j => j.ID) // Order by some property to ensure consistent results
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            Console.WriteLine($"Search Phrase: {searchPhrase}, Page Number: {pageNumber}");
            foreach (var result in searchResults)
            {
                Console.WriteLine($"ID: {result.ID}, Question: {result.Question}");
            }

            return View("Index", searchResults);
        }


        // GET: QnAs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var qnAs = await _context.QnAs
                .FirstOrDefaultAsync(m => m.ID == id);
            if (qnAs == null)
            {
                return NotFound();
            }

            return View(qnAs);
        }

        // GET: QnAs/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: QnAs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Question,Answer")] QnAs qnAs)
        {
            if (ModelState.IsValid)
            {
                _context.Add(qnAs);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(qnAs);
        }

        // GET: QnAs/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var qnAs = await _context.QnAs.FindAsync(id);
            if (qnAs == null)
            {
                return NotFound();
            }
            return View(qnAs);
        }

        // POST: QnAs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Question,Answer")] QnAs qnAs)
        {
            if (id != qnAs.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(qnAs);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QnAsExists(qnAs.ID))
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
            return View(qnAs);
        }

        // GET: QnAs/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var qnAs = await _context.QnAs
                .FirstOrDefaultAsync(m => m.ID == id);
            if (qnAs == null)
            {
                return NotFound();
            }

            return View(qnAs);
        }

        // POST: QnAs/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var qnAs = await _context.QnAs.FindAsync(id);
            if (qnAs != null)
            {
                _context.QnAs.Remove(qnAs);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QnAsExists(int id)
        {
            return _context.QnAs.Any(e => e.ID == id);
        }
    }
}
