using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PROJECT_SEM3.Data;
using PROJECT_SEM3.Models;
using PROJECT_SEM3.ViewModels;
using System.Diagnostics;

namespace PROJECT_SEM3.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin")]
    [Route("admin/homeadmin")]
    public class HomeAdminController : Controller
    {
        private readonly AppDbContext _context;


        public HomeAdminController(AppDbContext context)
        {
            _context = context;
        }
        [Route("")]
        [Route("index")]
        public IActionResult Index()
        {
            var doctors = _context.Users
                .Where(u => u.Thumbnail != null && u.Thumbnail != "")
                .Select(u => new DoctorViewModel
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Thumbnail = u.Thumbnail
                })
                .ToList();

            return View(doctors);
        }
        // Create Action
        [HttpGet]
        [Route("create")]
        public IActionResult Create()
        {
            var locations = _context.Locations.ToList();  // Lấy danh sách locations từ cơ sở dữ liệu

            /// Truyền Locations vào TempData
            TempData["Locations"] = JsonConvert.SerializeObject(locations); ;

            return View();  // Không cần truyền thêm tham số, vì dữ liệu đã có trong ViewBag
        }


        // Create POST Action
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateAsync(DoctorViewModel model)
        {
            var locations = _context.Locations.ToList();  // Lấy danh sách locations từ cơ sở dữ liệu

            /// Truyền Locations vào TempData
            TempData["Locations"] = JsonConvert.SerializeObject(locations);
            model.Locations = model.LocationId.ToString();
            var thumbnailFile = Request.Form.Files.FirstOrDefault(f => f.Name == "Thumbnail");

            if (thumbnailFile != null && thumbnailFile.Length > 0)
            {
                // Xử lý lưu trữ tệp lên server
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", thumbnailFile.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await thumbnailFile.CopyToAsync(stream);
                }

                // Lưu đường dẫn tệp vào trong model (hoặc cơ sở dữ liệu)
                model.Thumbnail = "/uploads/" + thumbnailFile.FileName; // Đường dẫn tương đối đến tệp
            }

            var doctor = new Users
            {
                FullName = model.FullName,
                Thumbnail = model.Thumbnail,
                LocationId = model.LocationId
            };

            _context.Users.Add(doctor);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // Edit Action
        // Edit Action
        [HttpGet]
        [Route("edit/{id}")]
        public IActionResult Edit(string id)
        {
            // Lấy bác sĩ từ cơ sở dữ liệu theo id
            var doctor = _context.Users.FirstOrDefault(u => u.Id == id);
            if (doctor == null)
            {
                return NotFound();
            }

            // Lấy danh sách các địa điểm từ cơ sở dữ liệu
            var locations = _context.Locations.ToList();

            // Chuyển các địa điểm sang dạng JSON và lưu vào TempData
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore // Bỏ qua các tham chiếu vòng
            };
            TempData["Locations"] = Newtonsoft.Json.JsonConvert.SerializeObject(locations, settings);


            // Khởi tạo model cho trang edit
            var model = new DoctorViewModel
            {
                Id = doctor.Id,
                FullName = doctor.FullName,
                Thumbnail = doctor.Thumbnail,
                LocationId = (int)doctor.LocationId // Giả sử bác sĩ có trường LocationId
            };

            return View(model);
        }


        // Edit POST Action
        // Edit POST Action
        [HttpPost]
        [Route("edit/{id}")]
        public IActionResult Edit(string id, DoctorViewModel model, IFormFile Thumbnail)
        {
            var locations = _context.Locations.ToList();

            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore // Bỏ qua các tham chiếu vòng
            };
            TempData["Locations"] = Newtonsoft.Json.JsonConvert.SerializeObject(locations, settings);
            model.Locations = model.LocationId.ToString();

            // Kiểm tra nếu model không hợp lệ


            var doctor = _context.Users.FirstOrDefault(u => u.Id == id);
            if (doctor == null)
            {
                return NotFound();
            }

            // Cập nhật thông tin bác sĩ
            doctor.FullName = model.FullName;
            doctor.LocationId = model.LocationId;

            // Kiểm tra nếu người dùng có upload ảnh mới
            if (Thumbnail != null && Thumbnail.Length > 0)
            {
                // Lưu ảnh vào thư mục images
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", Thumbnail.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    Thumbnail.CopyTo(stream);
                }

                // Cập nhật Thumbnail
                doctor.Thumbnail = "/uploads/" + Thumbnail.FileName;
            }

            // Lưu thay đổi vào cơ sở dữ liệu
            _context.SaveChanges();

            // Chuyển hướng về danh sách bác sĩ
            return RedirectToAction("Index", "HomeAdmin");
        }


        // Delete Action
        [HttpPost]
        [Route("delete/{id}")]
        public IActionResult Delete(string id)
        {
            var doctor = _context.Users.FirstOrDefault(u => u.Id == id);
            if (doctor != null)
            {
                _context.Users.Remove(doctor);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

    }
}
