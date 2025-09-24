using Microsoft.EntityFrameworkCore;
using Stones.Data;

var builder = WebApplication.CreateBuilder(args);

// -----------------
// CORS pour React
// -----------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

// -----------------
// Ajouter les contrôleurs
// -----------------
builder.Services.AddControllersWithViews();

// -----------------
// PostgreSQL avec variables Railway
// -----------------
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var host = Environment.GetEnvironmentVariable("DB_HOST");
    var port = Environment.GetEnvironmentVariable("DB_PORT");
    var db = Environment.GetEnvironmentVariable("DB_NAME");
    var user = Environment.GetEnvironmentVariable("DB_USER");
    var pass = Environment.GetEnvironmentVariable("DB_PASS");

    var connString = $"Host={host};Port={port};Database={db};Username={user};Password={pass}";
    options.UseNpgsql(connString);
});

// -----------------
// Construire l'application
// -----------------
var app = builder.Build();

// -----------------
// Appliquer automatiquement les migrations EF Core
// -----------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate(); // Crée toutes les tables si elles n'existent pas
}

// -----------------
// Pipeline HTTP
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
// Port dynamique Railway
// -----------------
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Urls.Add($"http://0.0.0.0:{port}");

// -----------------
// Lancer l'application
// -----------------
app.Run();
