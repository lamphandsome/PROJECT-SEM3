using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROJECT_SEM3.Data;
using PROJECT_SEM3.Models;
using PROJECT_SEM3.ViewModels;
using System.Linq;
using System.Security.Claims;

namespace PROJECT_SEM3.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly AppDbContext _context;

        public DoctorsController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Profile(string id)
        {
            // tim info bacsi
            var doctor = _context.Users
                .Where(u => u.Id == id)
                .Select(u => new DoctorViewModel
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Thumbnail = u.Thumbnail,
                })
                .FirstOrDefault();

            if (doctor == null)
            {
                return NotFound();
            }

            // lay so luong theo doi
            var followersCount = _context.Follows.Count(f => f.FollowingId == id);
            var followingCount = _context.Follows.Count(f => f.FollowerId == id);
            var posts = _context.Posts
                .Where(p => p.UserId == id)
                .Select(p => new { p.ImagePath })
                .ToList();

            
            ViewBag.FollowersCount = followersCount;
            ViewBag.FollowingCount = followingCount;
            ViewBag.PostsCount = posts.Count(); 
            ViewBag.Posts = posts;

            // ktra co dang theo doi hay k
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.IsFollowing = _context.Follows.Any(f => f.FollowerId == userId && f.FollowingId == id);

            return View(doctor);
        }

        // folow
        [HttpPost]
        [Authorize]
        public IActionResult Follow(string doctorId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 

            // Nếu chưa follow thì thêm vào bảng Follows
            if (!_context.Follows.Any(f => f.FollowerId == userId && f.FollowingId == doctorId))
            {
                _context.Follows.Add(new Follow
                {
                    FollowerId = userId,
                    FollowingId = doctorId
                });
                _context.SaveChanges(); 
            }

            return RedirectToAction("Profile", new { id = doctorId });
        }

        // unfolow
        [HttpPost]
        [Authorize]
        public IActionResult Unfollow(string doctorId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 

            // tim` de xoa
            var follow = _context.Follows
                .FirstOrDefault(f => f.FollowerId == userId && f.FollowingId == doctorId);

            if (follow != null)
            {
                _context.Follows.Remove(follow); 
                _context.SaveChanges(); 
            }

            return RedirectToAction("Profile", new { id = doctorId });
        }

        public IActionResult Index()
        {
            // Lấy danh sách bác sĩ từ bảng Users
            var doctors = _context.Users
                .Where(u => u.Thumbnail != null && u.Thumbnail != "") // Lọc những bác sĩ có hình ảnh
                .Select(u => new DoctorViewModel
                {
                    Id = u.Id, // Gán Id từ User.Id
                    FullName = u.FullName,
                    Thumbnail = u.Thumbnail
                })
                .ToList();

            return View(doctors); // Trả về danh sách bác sĩ cho view Index
        }


        [HttpGet]
        public IActionResult Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                // Nếu không có từ khóa, trả về danh sách trống
                return View("SearchResults", new List<DoctorViewModel>());
            }

            // Tìm kiếm theo tên hoặc địa chỉ
            var results = _context.Users
                .Include(u => u.Location)
                .Where(u => u.FullName.Contains(query) ||
                            (u.Location != null &&
                             (u.Location.City.Contains(query) || u.Location.Country.Contains(query))))
                .Select(u => new DoctorViewModel
                {
                    FullName = u.FullName,
                    Thumbnail = u.Thumbnail,
                    Locations = u.Location != null ? $"{u.Location.City}, {u.Location.Country}" : "N/A"
                })
                .ToList();

            return View("SearchResults", results);
        }
    }
}
