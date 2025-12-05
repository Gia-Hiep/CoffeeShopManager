using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CoffeeShopManager.Data;
using Rotativa.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Thêm DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection") ??
        throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")));

// 2. Thêm ASP.NET Core Identity (có Role)
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    // Tùy chỉnh chính sách mật khẩu (có thể nới lỏng cho demo)
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequiredLength = 6;
})
    .AddRoles<IdentityRole>()                     // Thêm dòng này để dùng Role
    .AddEntityFrameworkStores<ApplicationDbContext>();

// 3. Thêm Razor Pages
builder.Services.AddRazorPages();

// 4. Cấu hình Cookie (để hiển thị tên người dùng + redirect khi chưa login)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";      // trang login
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 5. Hai dòng QUAN TRỌNG – phải đúng thứ tự
app.UseAuthentication();   // PHẢI đứng TRƯỚC UseAuthorization
app.UseAuthorization();

app.UseRotativa();         // Rotativa để in PDF

app.MapRazorPages();

// 6. Tạo Role + User Admin ngay khi khởi động (seed data)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate(); // DÒNG NÀY LÀ "THẦN DƯỢC" – TẠO DB + BẢNG IDENTITY TRƯỚC KHI SEED

        await SeedData.InitializeAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Lỗi khi tạo database hoặc seed data.");
    }
}

app.Run();