using Microsoft.Data.SqlClient;
using Dapper;
using Stripe;
using Stripe.Checkout;

var builder = WebApplication.CreateBuilder(args);

// Stripe configuration
StripeConfiguration.ApiKey = "sk_test_51QEm3b04sDxI77uZmcC1fy3N1XLbn3yMNpswsEribk6hueeGRtMujK0R4pXZLTY5cgViEFcQVDW3L3G5sfnlapf600ZjVUAKyy";

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddSingleton(new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();
app.UseCors("AllowFrontend");

// User sign-up
app.MapPost("/api/signup", async (HttpContext context, SqlConnection db) =>
{
    var data = await context.Request.ReadFromJsonAsync<dynamic>();
    string username = data.username;
    string password = BCrypt.Net.BCrypt.HashPassword((string)data.password); // Hash and salt the password

    var query = @"
        INSERT INTO Users (username, password, subscription_status)
        OUTPUT Inserted.id
        VALUES (@username, @password, 'inactive')";

    var userId = await db.ExecuteScalarAsync<int>(query, new { username, password });
    await context.Response.WriteAsJsonAsync(new { userId });
});

// User login
app.MapPost("/api/login", async (HttpContext context, SqlConnection db) =>
{
    var data = await context.Request.ReadFromJsonAsync<dynamic>();
    string username = data.username;
    string password = data.password;

    var query = "SELECT * FROM Users WHERE username = @username";
    var user = await db.QuerySingleOrDefaultAsync(query, new { username });

    if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.password))
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsJsonAsync(new { message = "Invalid credentials" });
        return;
    }

    await context.Response.WriteAsJsonAsync(new { userId = user.id, subscription_status = user.subscription_status });
});

// Save payment data
app.MapPost("/api/payments", async (HttpContext context, SqlConnection db) =>
{
    var data = await context.Request.ReadFromJsonAsync<dynamic>();
    int userId = data.userId;
    decimal amount = data.amount;

    var query = @"
        INSERT INTO Payments (user_id, amount)
        VALUES (@userId, @amount);
        
        UPDATE Users SET subscription_status = 'active' WHERE id = @userId";

    await db.ExecuteAsync(query, new { userId, amount });
    await context.Response.WriteAsJsonAsync(new { message = "Payment recorded" });
});

// Record productivity rating
app.MapPost("/api/productivity", async (HttpContext context, SqlConnection db) =>
{
    var data = await context.Request.ReadFromJsonAsync<dynamic>();
    int userId = data.userId;
    int rating = data.rating;

    var query = "INSERT INTO Productivity (user_id, rating) VALUES (@userId, @rating)";
    await db.ExecuteAsync(query, new { userId, rating });
    await context.Response.WriteAsJsonAsync(new { message = "Productivity rating recorded" });
});

app.Run();
