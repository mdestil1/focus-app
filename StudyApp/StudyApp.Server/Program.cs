using StudyApp.Server;
using Microsoft.EntityFrameworkCore;
using StudyApp.Server.Data;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", builder =>
    {
        builder.WithOrigins("https://localhost:5173") // Allow React app origin
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

//SpotifyAPI
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
builder.Services.AddDbContext<AppDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AppDBContext")));




builder.Services.AddHttpClient<PixelaService>(client =>
{
    // Optionally configure the HttpClient here
    client.BaseAddress = new Uri("https://pixe.la"); // Example base URL
    client.DefaultRequestHeaders.Add("User-Agent", "PixelaClientApp");
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Apply the CORS policy

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowLocalhost");
app.UseSession();
app.MapControllers();
app.MapFallbackToFile("/index.html");


app.Run();
