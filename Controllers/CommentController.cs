using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExchangeWebsite.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeWebsite.Controllers
{
    [Authorize]
    public class CommentController : Controller
    {
        private readonly ExchangeWebsiteContext _context;
        private readonly UserManager<User> _userManager;

        public CommentController(ExchangeWebsiteContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int postId, string content, int? parentCommentId)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return RedirectToAction("Post", "Category", new { id = postId });
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Forbid();
            }

            var comment = new Comment
            {
                PostId = postId,
                UserId = userId,
                Content = content,
                ParentCommentId = parentCommentId,
                CreatedAt = DateTime.Now
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Post", "Category", new { id = postId });
        }
    }
}