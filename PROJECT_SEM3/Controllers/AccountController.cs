using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROJECT_SEM3.Data;
using PROJECT_SEM3.Models;
using PROJECT_SEM3.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PROJECT_SEM3.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<Users> signInManager;
        private readonly UserManager<Users> userManager;
        private readonly AppDbContext _context; //location
        private readonly IWebHostEnvironment _environment;

        public AccountController(SignInManager<Users> signInManager, UserManager<Users> userManager, AppDbContext context, IWebHostEnvironment environment)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this._context = context; //location
            this._environment = environment;
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe,false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Email or Password is incorrect");
                    return View(model);
                }
            }
            return View(model);
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                Users users = new Users
                {
                    FullName = model.Name,
                    Email = model.Email,
                    UserName = model.Email,
                };

                var result = await userManager.CreateAsync(users, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    foreach(var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    return View(model);
                }
            }
            return View(model); 
        }

        public IActionResult VerifyEmail()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel model)
        {
            if(ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(model.Email);

                if (user == null)
                {
                    ModelState.AddModelError("", "Something is wrong!");
                    return View(model);
                }
                else
                {
                    return RedirectToAction("ChangePassword", "Account", new { username = user.UserName });
                }
            }
            return View(model);
        }
        public IActionResult ChangePassword(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("VerifyEmail", "Account");
            }
            return View(new ChangePasswordViewModel { Email = username});
        }

        [HttpPost]

        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(model.Email);
                if(user != null)
                {
                    var result = await userManager.RemovePasswordAsync(user);
                    if (result.Succeeded)
                    {
                        result = await userManager.AddPasswordAsync(user, model.NewPassword);
                        return RedirectToAction("Login", "Account");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }

                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Email not found!");
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError("", "Something went wrong. Try Again.");
                return View(model);
            }
        }
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        [Authorize]
        public async Task<IActionResult> UserDetail()
        {
            var user = await userManager.GetUserAsync(User); // Lấy thông tin người dùng hiện tại
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Lấy danh sách Locations từ bảng Locations
            var locations = _context.Locations.ToList();

            // Truyền Locations vào View thông qua ViewBag
            ViewBag.Countries = locations
                .GroupBy(l => l.Country)
                .Select(g => new LocationViewModel
                {
                    Country = g.Key,
                    Cities = g.Select(c => c.City).ToList()
                }).ToList();

            return View("UserDetail", user);
        }

        [Authorize]
        [HttpPost]
        /*public async Task<IActionResult> UpdateUserInformation(string? fullname, string? country, string? city)
        {
            var user = await userManager.GetUserAsync(User);
            var locations = _context.Locations.ToList();
            ViewBag.Countries = locations
                .GroupBy(l => l.Country)
                .Select(g => new LocationViewModel
                {
                    Country = g.Key,
                    Cities = g.Select(c => c.City).ToList()
                }).ToList();
            if (fullname == null)
            {
                return View("UserDetail", user);
            }
            else
            {
                user.FullName = fullname;
            }

            if (country == null || city == null)
            {
                return View("UserDetail", user);
            }
            var locationInDB = await _context.Locations.FirstOrDefaultAsync(lo => lo.Country.Equals(country) && lo.City.Equals(city));
            user.LocationId = locationInDB?.Id;
            var updated = await userManager.UpdateAsync(user);
            return View("UserDetail", user);
        }*/
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateUserInformation(
    string? fullname, string? country, string? city,
    DateTime? dateOfBirth, int? yearOfExperience, IFormFile? thumbnail)
        {
            // Lấy thông tin người dùng hiện tại
            var user = await userManager.GetUserAsync(User);

            if (user == null)
            {
                ViewData["Message"] = "Người dùng không tồn tại!";
                return RedirectToAction("Login", "Account");
            }

            // Lấy danh sách quốc gia và thành phố để hiển thị
            var locations = _context.Locations.ToList();
            ViewBag.Countries = locations
                .GroupBy(l => l.Country)
                .Select(g => new LocationViewModel
                {
                    Country = g.Key,
                    Cities = g.Select(c => c.City).ToList()
                }).ToList();

            // Cập nhật thông tin người dùng
            if (fullname != null)
            {
                user.FullName = fullname;
            }

            if (dateOfBirth.HasValue)
            {
                user.DateOfBirth = dateOfBirth.Value;
            }

            if (yearOfExperience.HasValue)
            {
                user.YearsOfExperience = yearOfExperience.Value;
            }

            if (country != null && city != null)
            {
                var locationInDB = await _context.Locations
                    .FirstOrDefaultAsync(lo => lo.Country.Equals(country) && lo.City.Equals(city));
                user.LocationId = locationInDB?.Id;
            }

            // Xử lý upload thumbnail (nếu có)
            if (thumbnail != null)
            {
                // Tạo đường dẫn thư mục lưu ảnh
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder); // Tạo thư mục nếu chưa tồn tại
                }

                // Đặt tên file ảnh duy nhất
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + thumbnail.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Lưu file ảnh vào thư mục
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await thumbnail.CopyToAsync(fileStream);
                }

                // Cập nhật đường dẫn file vào thuộc tính Thumbnail của user
                user.Thumbnail = "/uploads/" + uniqueFileName;
            }

            // Lưu thay đổi vào cơ sở dữ liệu
            var updated = await userManager.UpdateAsync(user);

            if (updated.Succeeded)
            {
                ViewData["Message"] = "Cập nhật thông tin thành công!";
            }
            else
            {
                ViewData["Message"] = "Cập nhật thất bại!";
            }

            return View("UserDetail", user);
        }

    }
}
