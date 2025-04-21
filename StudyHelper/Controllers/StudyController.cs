// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using StudyApp.Data;
// using StudyApp.Models;
// using StudyApp.Models.ViewModels;
// using System;
// using System.IO;
// using System.Linq;
// using System.Threading.Tasks;

// namespace StudyApp.Controllers
// {
//     [Authorize]
//     public class StudyController : Controller
//     {
//         private readonly ApplicationDbContext _context;
//         private readonly UserManager<User> _userManager;
//         private readonly IWebHostEnvironment _env;

//         public StudyController(
//             ApplicationDbContext context,
//             UserManager<User> userManager,
//             IWebHostEnvironment env)
//         {
//             _context = context;
//             _userManager = userManager;
//             _env = env;
//         }

//         public async Task<IActionResult> Index()
//         {
//             var user = await _userManager.GetUserAsync(User);
//             if (user == null) return Challenge();

//             var studies = await _context.Studies
//                 .Include(s => s.Files)
//                 .Where(s => s.UserId == user.Id)
//                 .OrderByDescending(s => s.CreatedAt)
//                 .ToListAsync();

//             return View(studies);
//         }

// [HttpGet]
// public IActionResult Create()
// {
//     return View(new StudyUploadViewModel());
// }

// // [HttpPost]
// // [ValidateAntiForgeryToken]
// // public async Task<IActionResult> Create(StudyUploadViewModel viewModel)
// // {
// //     if (ModelState.IsValid)
// //     {
// //         var study = new Study
// //         {
// //             Title = viewModel.Title,
// //             Category = viewModel.Category,
// //             Tags = viewModel.Tags,
// //             CreatedAt = DateTime.Now,
// //             UserId = _userManager.GetUserId(User)
// //         };

// //         // Обробка файлів
// //  if (viewModel.Files != null && viewModel.Files.Count > 0)
// //                 {
// //                     foreach (var file in viewModel.Files)
// //                     {
// //                         if (file.Length > 0)
// //                         {
// //                             var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
// //                             Directory.CreateDirectory(uploadsDir);

// //                             var uniqueName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
// //                             var filePath = Path.Combine(uploadsDir, uniqueName);

// //                             using (var stream = new FileStream(filePath, FileMode.Create))
// //                             {
// //                                 await file.CopyToAsync(stream);
// //                             }

// //                             study.Files.Add(new StudyFile
// //                             {
// //                                 OriginalName = file.FileName,
// //                                 StoragePath = $"/uploads/{uniqueName}",
// //                                 FileType = Path.GetExtension(file.FileName),
// //                                 FileSize = file.Length
// //                             });
// //                         }
// //                     }
// //                 }


// //         _context.Studies.Add(study);
// //         await _context.SaveChangesAsync();
// //         return RedirectToAction(nameof(Index));
// //     }
// //     return View(viewModel);
// // }



// [HttpPost]
// [ValidateAntiForgeryToken]
// public async Task<IActionResult> Create(StudyUploadViewModel viewModel)
// {
//     if (ModelState.IsValid)
//     {
//         var user = await _userManager.GetUserAsync(User);
//         if (user == null) return Challenge();

//         // Перевірка ліміту зберігання
//         const long maxFileSize = 10 * 1024 * 1024; // 10MB
//         long totalSize = 0;

//         if (viewModel.Files != null)
//         {
//             foreach (var file in viewModel.Files)
//             {
//                 if (file.Length > maxFileSize)
//                 {
//                     // Логування для адміна
//                     var violation = new StorageViolation
//                     {
//                         UserId = user.Id,
//                         UserEmail = user.Email,
//                         ViolationDate = DateTime.Now,
//                         AttemptedFileType = Path.GetExtension(file.FileName),
//                         AttemptedSize = file.Length,
//                         MaxAllowed = maxFileSize
//                     };
//                     _context.StorageViolations.Add(violation);
//                     await _context.SaveChangesAsync();

//                     // Повідомлення для користувача
//                     TempData["Error"] = $"Файл '{file.FileName}' перевищує ліміт у {maxFileSize / 1024 / 1024}MB";
//                     return View(viewModel);
//                 }
//                 totalSize += file.Length;
//             }

//             // Перевірка загального ліміту (наприклад 100MB на користувача)
//             var currentUsage = await _context.StudyFiles
//                 .Where(f => f.Study.UserId == user.Id)
//                 .SumAsync(f => f.FileSize);

//             if (currentUsage + totalSize > 100 * 1024 * 1024) // 100MB
//             {
//                 TempData["Error"] = "Перевищено загальний ліміт зберігання. Будь ласка, видаліть деякі файли.";
//                 return View(viewModel);
//             }
//         }

//         var study = new Study
//         {
//             Title = viewModel.Title,
//             Category = viewModel.Category,
//             Tags = viewModel.Tags,
//             CreatedAt = DateTime.Now,
//             UserId = user.Id
//         };

//         // Обробка файлів
//         if (viewModel.Files != null)
//         {
//             foreach (var file in viewModel.Files.Where(f => f.Length > 0))
//             {
//                 var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
//                 Directory.CreateDirectory(uploadsDir);

//                 var uniqueName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
//                 var filePath = Path.Combine(uploadsDir, uniqueName);

//                 using (var stream = new FileStream(filePath, FileMode.Create))
//                 {
//                     await file.CopyToAsync(stream);
//                 }

//                 study.Files.Add(new StudyFile
//                 {
//                     OriginalName = file.FileName,
//                     StoragePath = $"/uploads/{uniqueName}",
//                     FileType = Path.GetExtension(file.FileName),
//                     FileSize = file.Length,
//                     UploadDate = DateTime.Now
//                 });
//             }
//         }

//         _context.Studies.Add(study);
//         await _context.SaveChangesAsync();
//         TempData["Success"] = "Конспект успішно створено!";
//         return RedirectToAction(nameof(Index));
//     }
//     return View(viewModel);
// }



//    [HttpPost, ActionName("Delete")]
// [ValidateAntiForgeryToken]
// public async Task<IActionResult> DeleteConfirmed(int id)
// {
//     var study = await _context.Studies
//         .Include(s => s.Files)
//         .FirstOrDefaultAsync(s => s.Id == id);

//     if (study == null)
//     {
//         return NotFound();
//     }

//     foreach (var file in study.Files)
//     {
//         var filePath = Path.Combine(_env.WebRootPath, file.StoragePath.TrimStart('/'));
//         if (System.IO.File.Exists(filePath))
//         {
//             System.IO.File.Delete(filePath);
//         }
//     }

//     _context.Studies.Remove(study);
//     await _context.SaveChangesAsync();

//     return RedirectToAction(nameof(Index));
// }


// [HttpGet]
// public async Task<IActionResult> Delete(int? id)
// {
//     if (id == null)
//     {
//         return NotFound();
//     }

//     var study = await _context.Studies
//         .Include(s => s.Files)
//         .FirstOrDefaultAsync(m => m.Id == id);

//     if (study == null)
//     {
//         return NotFound();
//     }

//     return View(study);
// }

// [HttpGet] 
// public async Task<IActionResult> Edit(int? id)
// {
//     if (id == null)
//     {
//         return NotFound();
//     }

//     var study = await _context.Studies
//         .Include(s => s.Files)
//         .FirstOrDefaultAsync(s => s.Id == id);

//     if (study == null)
//     {
//         return NotFound();
//     }

//     return View(study);
// }


// [HttpPost]
// [ValidateAntiForgeryToken]
// public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Category,Tags")] Study study, List<IFormFile> newFiles)
// {
//     var user = await _userManager.GetUserAsync(User);
//     if (user == null) return Challenge();

//     var existingStudy = await _context.Studies
//         .Include(s => s.Files)
//         .FirstOrDefaultAsync(s => s.Id == id);

//     if (existingStudy == null)
//     {
//         return NotFound();
//     }

//     if (ModelState.IsValid)
//     {
//         try
//         {
//             // Оновлюємо основні поля
//             existingStudy.Title = study.Title;
//             existingStudy.Category = study.Category;
//             existingStudy.Tags = study.Tags;

//             // Перевірка лімітів для нових файлів
//             const long maxFileSize = 10 * 1024 * 1024; // 10MB
//             long totalNewSize = 0;

//             if (newFiles != null && newFiles.Count > 0)
//             {
//                 // Перевіряємо розмір кожного нового файлу
//                 foreach (var file in newFiles)
//                 {
//                     if (file.Length > maxFileSize)
//                     {
//                         // Логування порушення
//                         _context.StorageViolations.Add(new StorageViolation
//                         {
//                             UserId = user.Id,
//                             UserEmail = user.Email,
//                             ViolationDate = DateTime.Now,
//                             AttemptedFileType = Path.GetExtension(file.FileName),
//                             AttemptedSize = file.Length,
//                             MaxAllowed = maxFileSize
//                         });
//                         await _context.SaveChangesAsync();

//                         TempData["Error"] = $"Файл '{file.FileName}' перевищує ліміт у {maxFileSize / 1024 / 1024}MB";
//                         return View(existingStudy);
//                     }
//                     totalNewSize += file.Length;
//                 }

//                 // Перевірка загального ліміту (100MB)
//                 var currentUsage = await _context.StudyFiles
//                     .Where(f => f.Study.UserId == user.Id)
//                     .SumAsync(f => f.FileSize);

//                 if (currentUsage + totalNewSize > 100 * 1024 * 1024)
//                 {
//                     TempData["Error"] = "Перевищено загальний ліміт зберігання. Видаліть деякі файли.";
//                     return View(existingStudy);
//                 }

//                 // Додаємо нові файли
//                 var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
//                 if (!Directory.Exists(uploadsDir))
//                 {
//                     Directory.CreateDirectory(uploadsDir);
//                 }

//                 foreach (var file in newFiles.Where(f => f.Length > 0))
//                 {
//                     var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
//                     var filePath = Path.Combine(uploadsDir, uniqueFileName);

//                     using (var fileStream = new FileStream(filePath, FileMode.Create))
//                     {
//                         await file.CopyToAsync(fileStream);
//                     }

//                     existingStudy.Files.Add(new StudyFile
//                     {
//                         OriginalName = file.FileName,
//                         StoragePath = $"/uploads/{uniqueFileName}",
//                         FileType = Path.GetExtension(file.FileName).ToLower(),
//                         FileSize = file.Length,
//                         UploadDate = DateTime.Now
//                     });
//                 }
//             }

//             _context.Update(existingStudy);
//             await _context.SaveChangesAsync();
            
//             TempData["Success"] = "Конспект успішно оновлено!";
//             return RedirectToAction(nameof(Index));
//         }
//         catch (DbUpdateConcurrencyException ex)
//         {
//             if (!StudyExists(study.Id))
//             {
//                 return NotFound();
//             }
//             else
//             {
//                 ModelState.AddModelError("", "Помилка при оновленні: " + ex.Message);
//             }
//         }
//     }

//     return View(existingStudy);
// }



// [HttpPost]
// public async Task<IActionResult> DeleteFile(int id)
// {
//     var file = await _context.StudyFiles
//         .Include(f => f.Study)
//         .FirstOrDefaultAsync(f => f.Id == id);

//     if (file == null)
//     {
//         return NotFound();
//     }

//     var physicalPath = Path.Combine(_env.WebRootPath, file.StoragePath.TrimStart('/'));
//     if (System.IO.File.Exists(physicalPath))
//     {
//         System.IO.File.Delete(physicalPath);
//     }

//     _context.StudyFiles.Remove(file);
//     await _context.SaveChangesAsync();

//     return Ok();
// }
// private bool StudyExists(int id)
// {
//     return _context.Studies.Any(e => e.Id == id);
// }
// }}


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudyApp.Data;
using StudyApp.Models;
using StudyApp.Models.ViewModels;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StudyApp.Controllers
{
    [Authorize]
    public class StudyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _env;

        public StudyController(
            ApplicationDbContext context,
            UserManager<User> userManager,
            IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var studies = await _context.Studies
                .Include(s => s.Files)
                .Where(s => s.UserId == user.Id)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            return View(studies);
        }

[HttpGet]
public IActionResult Create()
{
    return View(new StudyUploadViewModel());
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(StudyUploadViewModel viewModel)
{
    if (ModelState.IsValid)
    {
        var study = new Study
        {
            Title = viewModel.Title,
            Category = viewModel.Category,
            Tags = viewModel.Tags,
            CreatedAt = DateTime.Now,
            UserId = _userManager.GetUserId(User)
        };

        // Обробка файлів
 if (viewModel.Files != null && viewModel.Files.Count > 0)
                {
                    foreach (var file in viewModel.Files)
                    {
                        if (file.Length > 0)
                        {
                            var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
                            Directory.CreateDirectory(uploadsDir);

                            var uniqueName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                            var filePath = Path.Combine(uploadsDir, uniqueName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            study.Files.Add(new StudyFile
                            {
                                OriginalName = file.FileName,
                                StoragePath = $"/uploads/{uniqueName}",
                                FileType = Path.GetExtension(file.FileName),
                                FileSize = file.Length
                            });
                        }
                    }
                }


        _context.Studies.Add(study);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    return View(viewModel);
}
// [HttpPost]
// [ValidateAntiForgeryToken]
// public async Task<IActionResult> Create(Study study, List<IFormFile> files)
// {
//     if (ModelState.IsValid)
//     {
//         // Обробка файлів та збереження
//         study.CreatedAt = DateTime.Now;
//         _context.Studies.Add(study);
//         await _context.SaveChangesAsync();
//         return RedirectToAction(nameof(Index));
//     }
//     return View(study);
// }

        // [HttpGet]
        // public IActionResult Create()
        // {
        //     return View(new StudyUploadViewModel());
        // }

        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> Create(StudyUploadViewModel model)
        // {
        //     var user = await _userManager.GetUserAsync(User);
        //     if (user == null) return Challenge();

        //     if (ModelState.IsValid)
        //     {
        //         var study = new Study
        //         {
        //             Title = model.Title,
        //             Category = model.Category,
        //             Tags = model.Tags,
        //             UserId = user.Id,
        //             User = user
        //         };

        //         if (model.Files != null && model.Files.Count > 0)
        //         {
        //             foreach (var file in model.Files)
        //             {
        //                 if (file.Length > 0)
        //                 {
        //                     var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
        //                     Directory.CreateDirectory(uploadsDir);

        //                     var uniqueName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        //                     var filePath = Path.Combine(uploadsDir, uniqueName);

        //                     using (var stream = new FileStream(filePath, FileMode.Create))
        //                     {
        //                         await file.CopyToAsync(stream);
        //                     }

        //                     study.Files.Add(new StudyFile
        //                     {
        //                         OriginalName = file.FileName,
        //                         StoragePath = $"/uploads/{uniqueName}",
        //                         FileType = Path.GetExtension(file.FileName),
        //                         FileSize = file.Length
        //                     });
        //                 }
        //             }
        //         }

        //         _context.Studies.Add(study);
        //         await _context.SaveChangesAsync();

        //         return RedirectToAction(nameof(Index));
        //     }

        //     return View(model);
        // }

   [HttpPost, ActionName("Delete")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> DeleteConfirmed(int id)
{
    var study = await _context.Studies
        .Include(s => s.Files)
        .FirstOrDefaultAsync(s => s.Id == id);

    if (study == null)
    {
        return NotFound();
    }

    foreach (var file in study.Files)
    {
        var filePath = Path.Combine(_env.WebRootPath, file.StoragePath.TrimStart('/'));
        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
        }
    }

    _context.Studies.Remove(study);
    await _context.SaveChangesAsync();

    return RedirectToAction(nameof(Index));
}


[HttpGet]
public async Task<IActionResult> Delete(int? id)
{
    if (id == null)
    {
        return NotFound();
    }

    var study = await _context.Studies
        .Include(s => s.Files)
        .FirstOrDefaultAsync(m => m.Id == id);

    if (study == null)
    {
        return NotFound();
    }

    return View(study);
}

[HttpGet] 
public async Task<IActionResult> Edit(int? id)
{
    if (id == null)
    {
        return NotFound();
    }

    var study = await _context.Studies
        .Include(s => s.Files)
        .FirstOrDefaultAsync(s => s.Id == id);

    if (study == null)
    {
        return NotFound();
    }

    return View(study);
}

   [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Study study, List<IFormFile> newFiles)
    {
        try
        {
            // Оновлюємо основні дані конспекту
            var existingStudy = await _context.Studies
                .Include(s => s.Files)
                .FirstOrDefaultAsync(s => s.Id == study.Id);

            if (existingStudy == null)
            {
                return NotFound();
            }

            existingStudy.Title = study.Title;
            existingStudy.Category = study.Category;
            existingStudy.Tags = study.Tags;

            // Обробка нових файлів
            if (newFiles != null && newFiles.Any())
            {
                foreach (var file in newFiles.Where(f => f.Length > 0))
                {
                    var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    if (!Directory.Exists(uploadsDir))
                    {
                        Directory.CreateDirectory(uploadsDir);
                    }

                    var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    var filePath = Path.Combine(uploadsDir, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var newFile = new StudyFile
                    {
                        OriginalName = file.FileName,
                        StoragePath = $"/uploads/{uniqueFileName}",
                        FileType = Path.GetExtension(file.FileName),
                        FileSize = file.Length,
                        UploadDate = DateTime.UtcNow,
                        StudyId = study.Id
                    };

                    _context.StudyFiles.Add(newFile);
                }
            }

            await _context.SaveChangesAsync();

            // Повертаємо JSON для AJAX
            return Json(new { redirect = Url.Action("Index") });
        }
        catch (Exception ex)
        {
            // Логування або додаткові дії
            return BadRequest("Сталася помилка при оновленні.");
        }
    }

    [HttpPost]
    public IActionResult DeleteFile(int id, int studyId)
    {
        var file = _context.StudyFiles.Find(id);
        if (file != null)
        {
            _context.StudyFiles.Remove(file);
            _context.SaveChanges();
        }

        return Ok(); // Повертаємо успішну відповідь
    }
private bool StudyExists(int id)
{
    return _context.Studies.Any(e => e.Id == id);
}
}}