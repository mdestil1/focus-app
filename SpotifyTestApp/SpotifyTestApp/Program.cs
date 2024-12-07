using Microsoft.EntityFrameworkCore;
using SpotifyTestApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Addition: Added chunk from ChatGPT
// Add session services
builder.Services.AddDistributedMemoryCache(); // Use in-memory cache for sessions
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout
    options.Cookie.HttpOnly = true; // Secure session cookies
    options.Cookie.IsEssential = true; // Ensure session cookie is essential for GDPR compliance
});

// Register the DbContext with the connection string
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); // Custom error page for production
    app.UseHsts(); // Add HTTP Strict Transport Security for secure connections
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

//Addition: Add session middleware (from ChatGPT suggestion)
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    //Comment: Might be a problem with "controller=Home" instead of "controller=Spotify"
    pattern: "{controller=Spotify}/{action=Index}/{id?}");

app.Run();
