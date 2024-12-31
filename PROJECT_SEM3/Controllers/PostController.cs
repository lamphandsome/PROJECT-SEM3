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

        // PostController.cs - Thêm logic để chỉ hiển thị bài viết từ những người user đang theo dõi
        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // ID user hiện tại
            if (userId == null)
            {
                return RedirectToAction("Login", "Account"); // Chuyển hướng nếu chưa đăng nhập
            }

            // Lấy danh sách ID người mà user đang follow
            var followingIds = _context.Follows
                .Where(f => f.FollowerId == userId)
                .Select(f => f.FollowingId)
                .ToList();

            // Lấy bài viết của những người đang follow
            var posts = _context.Posts
                .Where(p => followingIds.Contains(p.UserId))
                .Select(post => new PostViewModel
                {
                    Id = post.Id,
                    FullName = post.User.FullName,
                    Thumbnail = post.ImagePath,
                    Title = post.Title,
                    CreatedAt = post.CreatedAt,
                    Content = post.Content
                })
                .ToList();

            return View(posts);
        }

        // PostController.cs - Thêm action để hiển thị chi tiết bài viết cùng phần bình luận
        public IActionResult Details(int id)
        {
            var post = _context.Posts
                .Where(p => p.Id == id)
                .Select(p => new PostDetailViewModel
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    ImagePath = p.ImagePath,
                    CreatedAt = p.CreatedAt,
                    AuthorName = p.User.FullName,
                    Comments = p.Comments.Select(c => new CommentViewModel
                    {
                        Content = c.Content,
                        UserName = c.User.FullName,
                        CreatedAt = c.CreatedAt
                    }).ToList()
                })
                .FirstOrDefault();

            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // PostController.cs - Thêm action để xử lý việc thêm bình luận
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddComment(int postId, string content)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var comment = new Comment
            {
                Content = content,
                CreatedAt = DateTime.Now,
                PostId = postId,
                UserId = userId
            };

            _context.Comments.Add(comment);
            _context.SaveChanges();

            return RedirectToAction("Details", new { id = postId });
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
    }
}
