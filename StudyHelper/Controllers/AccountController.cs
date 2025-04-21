using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using StudyApp.Models;
using StudyApp.Data;

public class AccountController : Controller
{
    
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ApplicationDbContext _context;


    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }

    // GET: Fetch all users
    [HttpGet("users")]
    public IActionResult GetUsers()
    {
        var users = _userManager.Users.Select(u => new { u.Id, u.UserName, u.Email, u.FullName }).ToList();
        Console.WriteLine($"Total users found: {users.Count}");
        return Ok(users.Select(u => new { u.Id, u.UserName, u.Email, u.FullName }));
    }

    // Register (GET)
    public IActionResult Register() => View();

    // Register (POST)
   [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Register(RegisterViewModel model)
{
    if (ModelState.IsValid)
    {
        var user = new User
        {
            UserName = model.Email,
            Email = model.Email,
            FullName = model.FullName,
            LastActivity = DateTime.Now
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }

    return View(model);
}


    // Login (GET)
    public IActionResult Login() => View();

    // Login (POST)
    // [HttpPost]
    // public async Task<IActionResult> Login(LoginViewModel model)
    // {
    //     if (!ModelState.IsValid) return View(model);

    //     var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

    //     if (result.Succeeded)
    //         return RedirectToAction("Index", "Home");

    //     ModelState.AddModelError("", "Invalid login attempt.");
    //     return View(model);
    // }

    [HttpPost]
public async Task<IActionResult> Login(LoginViewModel model)
{
    if (!ModelState.IsValid)
        return View(model);

    var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

    if (result.Succeeded)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        user.LastActivity = DateTime.Now;
        await _userManager.UpdateAsync(user);

        if (await _userManager.IsInRoleAsync(user, "Admin"))
        {
            return RedirectToAction("Index", "Admin");
        }

        return RedirectToAction("Index", "Home");
    }

    ModelState.AddModelError("", "Неправильний логін або пароль.");
    return View(model);
}





    // Logout
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }

    
}
