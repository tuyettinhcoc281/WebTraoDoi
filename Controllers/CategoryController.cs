﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using ExchangeWebsite.Models;

namespace ExchangeWebsite.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ExchangeWebsiteContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CategoryController(ExchangeWebsiteContext context, UserManager<IdentityUser> userManager)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public IActionResult Index(int id)
        {
            var category = _context.Categories
                .Include(c => c.SubCategories)
                .FirstOrDefault(c => c.CategoryId == id);

            if (category == null)
                return NotFound();

            // Get posts for this sub-category
            var posts = _context.Posts
                .Where(p => p.CategoryId == id)
                .Include(p => p.PostImages)
                .OrderByDescending(p => p.PostId)
                .ToList();

            ViewBag.Posts = posts;
            return View(category);
        }

        public IActionResult Post(int id)
        {
            var post = _context.Posts
                .Include(p => p.PostImages)
                .Include(p => p.Category)
                .FirstOrDefault(p => p.PostId == id);

            if (post == null)
                return NotFound();

            return View(post);
        }

        [HttpGet]
        public IActionResult Index(int id, int? subCategoryId, string sort)
        {
            var category = _context.Categories
                .Include(c => c.SubCategories)
                .FirstOrDefault(c => c.CategoryId == id);

            if (category == null)
                return NotFound();

            // Start with posts in this category
            var postsQuery = _context.Posts
                .Include(p => p.PostImages)
                .Where(p => p.CategoryId == id);

            // If a sub-category is selected, filter to it only
            if (subCategoryId.HasValue)
            {
                postsQuery = postsQuery.Where(p => p.CategoryId == subCategoryId.Value);
            }

            // Sorting
            postsQuery = sort switch
            {
                "date_asc" => postsQuery.OrderBy(p => p.PostedAt),
                "date_desc" => postsQuery.OrderByDescending(p => p.PostedAt),
                "price_asc" => postsQuery.OrderBy(p => p.Price),
                "price_desc" => postsQuery.OrderByDescending(p => p.Price),
                _ => postsQuery.OrderByDescending(p => p.PostId)
            };

            var posts = postsQuery.ToList();
            ViewBag.Posts = posts;
            return View(category);
        }
        
        [HttpGet]
        public IActionResult CreatePost()
        {
            // Only sub-categories (categories with a parent)
            var subCategories = _context.Categories
                .Where(c => c.ParentCategoryId != null)
                .OrderBy(c => c.CategoryName)
                .ToList();

            ViewBag.Categories = subCategories;
            ViewBag.Conditions = new List<string> { "New", "Like New", "Good", "Fair", "Salvage" };
            ViewBag.Languages = new List<string> { "english", "spanish", "french", "german" };
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost([Bind("Title,Price,City,ZipCode,Description,Make,ModelNumber,Condition,CryptocurrencyAccepted,DeliveryAvailable,ContactEmail,PhoneNumber,ShowAddress,CategoryId,Language")] Post post, List<IFormFile> images)
        {
            var userId = _userManager.GetUserId(User);
            if (!string.IsNullOrEmpty(userId))
            {
                post.UserId = userId;
            }
            post.PostedAt = DateTime.UtcNow;

            // Only sub-categories (categories with a parent)
            ViewBag.Categories = _context.Categories
                .Where(c => c.ParentCategoryId != null)
                .OrderBy(c => c.CategoryName)
                .ToList();
            ViewBag.Conditions = new List<string> { "New", "Like New", "Good", "Fair", "Salvage" };
            ViewBag.Languages = new List<string> { "english", "spanish", "french", "german" };

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                if (errors.Count > 0)
                {
                    // For debugging: set a breakpoint here or log the errors
                    ViewBag.Errors = errors;
                }
                return View(post);
            }

            _context.Add(post);
            await _context.SaveChangesAsync();

            if (images != null && images.Count > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/posts");
                Directory.CreateDirectory(uploadsFolder);

                foreach (var image in images)
                {
                    if (image.Length > 0)
                    {
                        string uniqueFileName = $"{post.PostId}_{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(fileStream);
                        }

                        var postImage = new PostImage
                        {
                            PostId = post.PostId,
                            ImagePath = $"/uploads/posts/{uniqueFileName}"
                        };
                        _context.PostImages.Add(postImage);
                    }
                }
                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "Post created successfully!";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult MyPost()
        {
            var userId = _userManager.GetUserId(User);
            var posts = _context.Posts
                .Where(p => p.UserId == userId)
                .Include(p => p.PostImages)
                .OrderByDescending(p => p.PostedAt)
                .ToList();

            ViewBag.Posts = posts;
            return View();
        }
    }
}
