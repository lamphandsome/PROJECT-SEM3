using Microsoft.AspNetCore.Mvc;
using PROJECT_SEM3.Models;
using PROJECT_SEM3.Data;
using System.Security.Claims;
using PROJECT_SEM3.ViewModels;

namespace PROJECT_SEM3.Controllers
{
    public class PostController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public PostController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Posts
        public IActionResult Index()
        {
            var posts = _context.Posts
                .Select(post => new PostViewModel
                {
                    Id = post.Id,
                    FullName = post.User.FullName,  // Giả sử bạn có trường FullName trong User
                    Thumbnail = post.ImagePath,
                    CreatedAt = post.CreatedAt
                })
                .ToList();

            return View(posts);
        }


        // GET: Posts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Posts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Post post, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra UserId
                    post.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (string.IsNullOrEmpty(post.UserId))
                    {
                        ModelState.AddModelError("", "UserId không hợp lệ.");
                        return View(post);
                    }

                    // Xử lý ảnh (nếu có)
                    if (image != null)
                    {
                        var fileName = Path.GetFileName(image.FileName);
                        var path = Path.Combine(_env.WebRootPath, "img", fileName);
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            image.CopyTo(stream);
                        }
                        post.ImagePath = "/img/" + fileName;  // Lưu đường dẫn ảnh vào trong CSDL
                    }
                    else
                    {
                        // Nếu không có ảnh, bạn có thể gán một giá trị mặc định cho ImagePath
                        post.ImagePath = "/img/default-image.jpg";
                    }

                    // Khởi tạo Comments là một danh sách rỗng nếu chưa có dữ liệu
                    if (post.Comments == null)
                    {
                        post.Comments = new List<Comment>();
                    }

                    // Lưu bài viết vào cơ sở dữ liệu
                    _context.Posts.Add(post);
                    var result = _context.SaveChanges();  // Kiểm tra kết quả của SaveChanges

                    if (result > 0)  // Kiểm tra xem có lưu thành công không
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "Có lỗi xảy ra khi lưu bài viết.");
                    }
                }
                catch (Exception ex)
                {
                    // Log lỗi và hiển thị thông báo
                    Console.WriteLine($"Lỗi: {ex.Message}");
                    ModelState.AddModelError("", "Đã xảy ra lỗi khi lưu bài viết.");
                }
            }
            return View(post);
        }



        // GET: Posts/Details/5
        public IActionResult Details(int id)
        {
            var post = _context.Posts.FirstOrDefault(p => p.Id == id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }
    }
}
