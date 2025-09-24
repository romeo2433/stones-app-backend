using Microsoft.EntityFrameworkCore;
using Stones.Data;

var builder = WebApplication.CreateBuilder(args);

// -----------------
// 1️⃣ Ajouter les services AVANT builder.Build()
// -----------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("StonesConnection")));

// -----------------
// 2️⃣ Construire l'application
// -----------------
var app = builder.Build();

// -----------------
// 3️⃣ Configurer le pipeline HTTP
// -----------------
app.UseCors("AllowAll");

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Routes
app.MapControllerRoute(
    name: "auth",
    pattern: "login",
    defaults: new { controller = "Auth", action = "Login" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

// -----------------
// 4️⃣ Lancer l'application
// -----------------
app.Run();
