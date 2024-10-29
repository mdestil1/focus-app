using Stripe;
using Stripe.Checkout;

var builder = WebApplication.CreateBuilder(args);

// Stripe w/ secret key
StripeConfiguration.ApiKey = "sk_test_51QEm3b04sDxI77uZmcC1fy3N1XLbn3yMNpswsEribk6hueeGRtMujK0R4pXZLTY5cgViEFcQVDW3L3G5sfnlapf600ZjVUAKyy"; 

// Add CORS policy or others as needed
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();
app.UseCors("AllowFrontend");

// Stripe session creation endpoint
app.MapPost("/create-checkout-session", async (HttpContext context) =>
{
    var options = new SessionCreateOptions
    {
        PaymentMethodTypes = new List<string> { "card" },
        LineItems = new List<SessionLineItemOptions>
        {
            new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = 1000,
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "Monthly Subscription",
                    },
                },
                Quantity = 1,
            },
        },
        Mode = "payment",
        SuccessUrl = "http://localhost:3000/success",
        CancelUrl = "http://localhost:3000/cancel",
    };

    var service = new SessionService();
    Session session = await service.CreateAsync(options);

    Console.WriteLine("Stripe Session ID: " + session.Id); // Log session id for debugging
    Console.WriteLine("Stripe Session URL: " + session.Url); // Log session url for debugging

    context.Response.ContentType = "application/json";
    await context.Response.WriteAsJsonAsync(new { url = session.Url });
});

app.Run();
