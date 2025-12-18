using eClaims.Core.Interfaces;
using eClaims.Infrastructure.Data;
using eClaims.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

// builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddSingleton(typeof(IRepository<>), typeof(InMemoryRepository<>));
builder.Services.AddScoped<eClaims.Core.Interfaces.INotificationService, eClaims.Infrastructure.Services.EmailNotificationService>();

builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
    });

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Seed Data for Reporting Engine
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var claimRepo = services.GetRequiredService<IRepository<eClaims.Core.Entities.Claim>>();
    var userRepo = services.GetRequiredService<IRepository<eClaims.Core.Entities.User>>();
    eClaims.Web.Services.DataSeeder.SeedData(claimRepo, userRepo).Wait();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
