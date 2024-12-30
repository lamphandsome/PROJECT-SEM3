using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROJECT_SEM3.Data;
using PROJECT_SEM3.Models;
using PROJECT_SEM3.Services;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using PROJECT_SEM3.Dtos;

namespace PROJECT_SEM3.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly UserManager<Users> _userManager;
        private readonly IMessageService _messageService;
        private readonly AppDbContext _context;

        public ChatController(
            UserManager<Users> userManager,
            IMessageService messageService,
            AppDbContext context)
        {
            _userManager = userManager;
            _messageService = messageService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return NotFound();


            var allUsers = await _context.Users
                .Where(u => u.Id != currentUser.Id)
                .OrderBy(u => u.FullName)
                .ToListAsync();

            ViewBag.AllUsers = allUsers;
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> LoadMessages(string userId, int page = 1, int pageSize = 20)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest("User ID is required.");

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            var messages = await _messageService.GetChatHistoryAsync(currentUser.Id, userId, page, pageSize);

            // Map to DTOs to avoid circular references
            var messageDtos = messages.Select(m => new MessageDto
            {
                Id = m.Id,
                Content = m.Content,
                SentAt = m.SentAt,
                SenderId = m.SenderId,
                SenderUserName = m.Sender?.UserName,
                ReceiverId = m.ReceiverId,
                ReceiverUserName = m.Receiver?.UserName
            }).ToList();

            return Json(messageDtos);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserDetails(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest("User ID is required.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            return Json(new
            {
                user.Id,
                user.FullName,
                user.Thumbnail,
                Location = user.Location?.City ?? "Unknown"
            });
        }

        [HttpGet]
        public async Task<IActionResult> SearchUsers(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return Json(new List<object>());

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            var users = await _context.Users
                .Where(u => u.Id != currentUser.Id &&
                           (u.FullName.Contains(searchTerm) ||
                            u.Email.Contains(searchTerm) ||
                            u.UserName.Contains(searchTerm)))
                .Take(10)
                .Select(u => new
                {
                    u.Id,
                    u.FullName,
                    u.Thumbnail,
                    Location = u.Location.City ?? "Unknown",
                })
                .ToListAsync();

            return Json(users);
        }

        [HttpGet]
        public async Task<IActionResult> GetUnreadCount()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            var unreadCount = await _context.Messages
                .Where(m => m.ReceiverId == currentUser.Id && !m.IsRead)
                .CountAsync();

            return Json(new { unreadCount });
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsRead(string senderId)
        {
            if (string.IsNullOrWhiteSpace(senderId))
                return BadRequest("Sender ID is required.");

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            await _context.Messages
                .Where(m => m.SenderId == senderId &&
                           m.ReceiverId == currentUser.Id &&
                           !m.IsRead)
                .ExecuteUpdateAsync(s => s.SetProperty(b => b.IsRead, true));

            return Ok();
        }
    }
}
