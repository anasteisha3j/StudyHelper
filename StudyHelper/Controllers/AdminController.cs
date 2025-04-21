using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using StudyApp.Models;
using StudyApp.Data;
using Microsoft.EntityFrameworkCore;

namespace StudyHelper.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        private readonly UserManager<User> _userManager;  // Змінив IdentityUser на User

        public AdminController(UserManager<User> userManager, ApplicationDbContext dbContext)  // Оновлений конструктор
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var adminUsers = new List<IdentityUser>();

            foreach (var user in users)
            {
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    adminUsers.Add(user);
                }
            }

            var model = new AdminModel
            {
                TotalUsers = users.Count,
                TotalAdmins = adminUsers.Count,
                Users = users.Select(u => new AdminUserViewModel
                {
                    Id = u.Id,
                    Email = u.Email,
                    FullName = u.FullName,
                    Role = _userManager.GetRolesAsync(u).Result.FirstOrDefault(),
                    LastActivity = u.LastActivity // Replace with actual last activity from your DB
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    // Handle error
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> MakeAdmin(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.AddToRoleAsync(user, "Admin");
            }
            return RedirectToAction("Index");
        }
    


    //2
    // Додайте в AdminController.cs
    public async Task<IActionResult> StorageViolations()
    {
        var violations = await _dbContext.StorageViolations
            .OrderByDescending(v => v.ViolationDate)
            .ToListAsync();

        var model = new AdminModel
        {
            StorageViolations = violations
        };

        return View(model);
    }

    public async Task<IActionResult> UserDownloads()
    {
        var users = await _userManager.Users.ToListAsync();
        var downloadStats = new List<UserDownloadStats>();

        foreach (var user in users)
        {
            var userFiles = await _dbContext.StudyFiles
                .Where(f => f.Study.UserId == user.Id)
                .ToListAsync();

            var stats = new UserDownloadStats
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                TotalDownloadSize = userFiles.Sum(f => f.FileSize),
                TotalFiles = userFiles.Count,
                LastDownloadDate = userFiles.Any() ? userFiles.Max(f => f.UploadDate) : DateTime.MinValue
            };

            downloadStats.Add(stats);
        }

        return View(downloadStats);
    }
}}