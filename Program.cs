using Microsoft.EntityFrameworkCore;
using Stones.Data;
using Npgsql;

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
// PostgreSQL avec Render
// -----------------
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

    if (!string.IsNullOrEmpty(databaseUrl))
    {
        // Convertir DATABASE_URL en chaîne compatible Npgsql
        var databaseUri = new Uri(databaseUrl);
        var userInfo = databaseUri.UserInfo.Split(':');

        var connStringBuilder = new NpgsqlConnectionStringBuilder
        {
            Host = databaseUri.Host,
            Port = databaseUri.Port,
            Username = userInfo[0],
            Password = userInfo[1],
            Database = databaseUri.AbsolutePath.TrimStart('/'),
            SslMode = SslMode.Prefer,
            TrustServerCertificate = true
        };

        options.UseNpgsql(connStringBuilder.ConnectionString);
    }
    else
    {
        // fallback local
        var connString = builder.Configuration.GetConnectionString("StonesConnection");
        options.UseNpgsql(connString);
    }
});

// -----------------
// Construire l'application
// -----------------
var app = builder.Build();

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
// Port dynamique Render
// -----------------
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Urls.Add($"http://0.0.0.0:{port}");


// -----------------
// Lancer l'application
// -----------------
app.Run();
