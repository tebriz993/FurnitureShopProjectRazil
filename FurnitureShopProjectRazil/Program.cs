using FurnitureShopProjectRazil.Data;
using FurnitureShopProjectRazil.Interfaces;
using FurnitureShopProjectRazil.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Servisl?rin konteyner? ?lav? edilm?si
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// PasswordService qeydiyyat?
builder.Services.AddScoped<IPasswordService, PasswordService>();

// === YEN? E-PO�T SERV?S? QEYD?YYATI BA?LAYIR ===
// EmailSettings-i konfiqurasiyadan oxumaq ���n
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
// EmailSender servisini qeydiyyatdan ke�irm?k
// AddTransient istifad? edirik, ��nki EmailSender h?r e-po�t g�nd?ri?i ���n yeni SmtpClient yarad?r.
builder.Services.AddTransient<IEmailSender, EmailSender>();
// === YEN? E-PO�T SERV?S? QEYD?YYATI B?T?R ===

// HttpContextAccessor qeydiyyat?
builder.Services.AddHttpContextAccessor();

// Authentication konfiqurasiyas?
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });

builder.Services.AddDistributedMemoryCache(); // Session ���n z?ruridir
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// HTTP sor?u pipeline-n?n konfiqurasiyas?
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

// app.UseHttpsRedirection(); // HTTPS ���n aktivl??dirin
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Vacibdir! UseAuthorization-dan ?VV?L g?lm?lidir.
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Account}/{action=Login}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();