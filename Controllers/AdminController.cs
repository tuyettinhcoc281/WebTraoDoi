using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExchangeWebsite.Models;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeWebsite.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ExchangeWebsiteContext _context;
        private readonly UserManager<User> _userManager;

        public AdminController(ExchangeWebsiteContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Dashboard
        public IActionResult Index()
        {
            ViewBag.CategoryCount = _context.Categories.Count();
            ViewBag.UserCount = _context.Users.Count();
            ViewBag.PostCount = _context.Posts.Count();
            ViewBag.BillCount = _context.Bills.Count();
            ViewBag.Revenue = _context.Bills.Where(b => b.Status == "Paid").Sum(b => (decimal?)b.Amount) ?? 0;
            ViewBag.ReportCount = _context.Reports.Count(); // <-- Add this line
            ViewBag.ShippingCount = _context.Posts.Count(p => p.ShippingRequested);
            return View();
        }

        // CATEGORY MANAGEMENT
        [HttpGet]
        public IActionResult Categories()
        {
            var categories = _context.Categories
                .Include(c => c.SubCategories)
                .Where(c => c.ParentCategoryId == null)
                .ToList();
            return View(categories);
        }

        [HttpGet]
        public IActionResult CreateCategory()
        {
            ViewBag.Categories = _context.Categories.Where(c => c.ParentCategoryId == null).ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Categories));
            }
            // Debug: show errors if any
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            ViewBag.Errors = errors;
            ViewBag.Categories = _context.Categories.Where(c => c.ParentCategoryId == null).ToList();
            return View(category);
        }

        [HttpGet]
        public async Task<IActionResult> EditCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();
            ViewBag.Categories = _context.Categories.Where(c => c.ParentCategoryId == null && c.CategoryId != id).ToList();
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Update(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Categories));
            }
            ViewBag.Categories = _context.Categories.Where(c => c.ParentCategoryId == null && c.CategoryId != category.CategoryId).ToList();
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories
                .Include(c => c.SubCategories)
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category != null)
            {
                // Remove all subcategories first
                if (category.SubCategories != null && category.SubCategories.Any())
                {
                    _context.Categories.RemoveRange(category.SubCategories);
                }
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Categories));
        }

        // ACCOUNT CONTROL
        [HttpGet]
        public IActionResult Users()
        {
            var users = _userManager.Users.OfType<ExchangeWebsite.Models.User>().ToList();
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LockUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.LockoutEnd = DateTime.UtcNow.AddYears(100);
                await _userManager.UpdateAsync(user);
            }
            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnlockUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.LockoutEnd = null;
                await _userManager.UpdateAsync(user);
            }
            return RedirectToAction(nameof(Users));
        }

        // POST CONTROL
        [HttpGet]
        public IActionResult Posts()
        {
            var posts = _context.Posts
                .Include(p => p.User)
                .Include(p => p.Category)
                .OrderByDescending(p => p.PostedAt)
                .ToList();
            return View(posts);
        }

        [HttpGet]
        public async Task<IActionResult> PostDetail(int id)
        {
            var post = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Category)
                .Include(p => p.PostImages)
                .FirstOrDefaultAsync(p => p.PostId == id);
            if (post == null) return NotFound();
            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Posts));
        }

        // REVENUE (VIP Subscription Bills)
        [HttpGet]
        public IActionResult Revenue()
        {
            var bills = _context.Bills
                .Include(b => b.User)
                .OrderByDescending(b => b.CreatedAt)
                .ToList();
            ViewBag.TotalRevenue = bills.Where(b => b.Status == "Paid").Sum(b => b.Amount);
            return View(bills);
        }

        // REPORTS
        [HttpGet]
        public IActionResult Reports()
        {
            var reports = _context.Reports
                .Include(r => r.Post)
                .Include(r => r.Reporter)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();
            return View(reports);
        }

        [HttpGet]
        public IActionResult ShippingRequests()
        {
            var posts = _context.Posts
                .Include(p => p.User)
                .Where(p => p.ShippingRequested)
                .ToList();
            return View(posts);
        }
    }
}