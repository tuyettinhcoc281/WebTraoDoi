using Microsoft.AspNetCore.Mvc;
using ExchangeWebsite.Models;
using ExchangeWebsite.Controllers;
using Microsoft.EntityFrameworkCore;

namespace ExchangeWebsite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ExchangeWebsiteContext _context;

        public HomeController(ExchangeWebsiteContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Fetch all categories and include their subcategories
            var categories = _context.Categories
                .Include(c => c.SubCategories)
                .Where(c => c.ParentCategoryId == null) // Main categories only
                .OrderBy(c => c.DisplayOrder)
                .ToList();

            ViewData["Title"] = "Home Page";
            return View(categories);
        }
    }
}