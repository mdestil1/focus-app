using StudyApp.Server;

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

builder.Services.AddHttpClient<PixelaSignUpService>(client =>
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
app.MapControllers();
app.MapFallbackToFile("/index.html");


app.Run();
