
// using Microsoft.AspNetCore.Identity;
// using Microsoft.EntityFrameworkCore;
// using StudyApp.Data;
// using StudyApp.Models;
// using Microsoft.Extensions.FileProviders;
// using Microsoft.AspNetCore.Http.Features;

// var builder = WebApplication.CreateBuilder(args);


// builder.Services.AddDbContext<ApplicationDbContext>(options =>
//     options.UseSqlite("Data Source=studyapp.db"));


// builder.Services.AddIdentity<User, IdentityRole>()
//     .AddEntityFrameworkStores<ApplicationDbContext>()
//     .AddDefaultTokenProviders();


// builder.Services.AddControllersWithViews();

// builder.Services.ConfigureApplicationCookie(options =>
// {
//     options.LoginPath = "/Account/Login";
//     options.AccessDeniedPath = "/Account/AccessDenied";
//     options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
//     options.SlidingExpiration = true;
//     options.Cookie.HttpOnly = true;
//     options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
// });
// builder.Services.Configure<FormOptions>(options =>
// {
//     options.MultipartBodyLengthLimit = 209715200; // 200MB
//     options.ValueLengthLimit = 104857600; // 100MB
//     options.MultipartHeadersLengthLimit = 104857600; // 100MB
// });
// var app = builder.Build();

// app.UseStaticFiles();
// app.UseRouting();

// app.UseAuthentication(); 
// app.UseAuthorization(); 

// app.MapControllerRoute(
//     name: "default",
//     pattern: "{controller=Home}/{action=Index}/{id?}");

// app.UseStaticFiles(new StaticFileOptions
// {
//     FileProvider = new PhysicalFileProvider(
//         Path.Combine(builder.Environment.WebRootPath, "uploads")),
//     RequestPath = "/uploads"
// });

// using (var scope = app.Services.CreateScope())
// {
//     var services = scope.ServiceProvider;
//     await SeedAdminAsync(services); 
// }

// static async Task SeedAdminAsync(IServiceProvider serviceProvider)
// {
//     var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
//     var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

//     string adminEmail = "admin@studyapp.com";
//     string adminPassword = "Admin123!";

//     if (!await roleManager.RoleExistsAsync("Admin"))
//         await roleManager.CreateAsync(new IdentityRole("Admin"));

//     var adminUser = await userManager.FindByEmailAsync(adminEmail);
//     if (adminUser == null)
//     {
//         adminUser = new User
//         {
//             UserName = adminEmail,
//             Email = adminEmail,
//             FullName = "Admin Twin"
//         };

//         var result = await userManager.CreateAsync(adminUser, adminPassword);
//         if (result.Succeeded)
//         {
//             await userManager.AddToRoleAsync(adminUser, "Admin");
//         }
//     }
//     else if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
//     {
//         await userManager.AddToRoleAsync(adminUser, "Admin");
//     }
// }

// using (var scope = app.Services.CreateScope())
// {
//     var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//     db.Database.Migrate();
// }
// using (var scope = app.Services.CreateScope())
// {
//     var services = scope.ServiceProvider;
//     await SeedAdminAsync(services); 
// }

// app.Run();


using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudyApp.Data;
using StudyApp.Models;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=studyapp.db"));

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 209715200; // 200MB
    options.ValueLengthLimit = 104857600; // 100MB
    options.MultipartHeadersLengthLimit = 104857600; // 100MB
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.WebRootPath, "uploads")),
    RequestPath = "/uploads"
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    
    try
    {
        var db = services.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();
        
        await SeedAdminAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Помилка під час міграції або ініціалізації бази даних");
    }
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

static async Task SeedAdminAsync(IServiceProvider serviceProvider)
{
    var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    const string adminEmail = "admin@studyapp.com";
    const string adminPassword = "Admin123!";
    const string adminRole = "Admin";

    if (!await roleManager.RoleExistsAsync(adminRole))
    {
        await roleManager.CreateAsync(new IdentityRole(adminRole));
    }

    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new User
        {
            UserName = adminEmail,
            Email = adminEmail,
            FullName = "Admin Twin",
            EmailConfirmed = true 
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, adminRole);
        }
    }
    else if (!await userManager.IsInRoleAsync(adminUser, adminRole))
    {
        await userManager.AddToRoleAsync(adminUser, adminRole);
    }
}