using Microsoft.AspNetCore.Mvc;
using PROJECT_SEM3.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using PROJECT_SEM3.Data;

namespace PROJECT_SEM3.Controllers
{
    public class MessagesController : Controller
    {
        private readonly UserManager<Users> _userManager;
        private readonly AppDbContext _context;

        public MessagesController(UserManager<Users> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // Hiển thị danh sách tin nhắn
        public async Task<IActionResult> Index(string receiverId)
        {
            var currentUserId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(currentUserId) || string.IsNullOrEmpty(receiverId))
            {
                return NotFound("Người dùng không hợp lệ.");
            }

            var messages = await _context.Messages
                .Where(m => (m.SenderId == currentUserId && m.ReceiverId == receiverId) ||
                            (m.SenderId == receiverId && m.ReceiverId == currentUserId))
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            ViewBag.ReceiverId = receiverId;
            var receiver = await _context.Users.FindAsync(receiverId);
            ViewBag.ReceiverName = receiver?.FullName;

            return View(messages);
        }

        // Gửi tin nhắn
        [HttpPost]
        public async Task<IActionResult> SendMessage(string receiverId, string content)
        {
            var currentUserId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(content))
            {
                ModelState.AddModelError("Content", "Nội dung tin nhắn không được để trống.");
                return RedirectToAction("Index", new { receiverId });
            }

            if (string.IsNullOrEmpty(receiverId))
            {
                return BadRequest("Không tìm thấy người nhận.");
            }

            var message = new Message
            {
                SenderId = currentUserId,
                ReceiverId = receiverId,
                Content = content,
                SentAt = DateTime.UtcNow
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { receiverId });
        }
    }
}