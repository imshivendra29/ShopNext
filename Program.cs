using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShopNext.Data;
using ShopNext.Extensions;
using ShopNext.Helpers;
using ShopNext.Infrastructure.Cloudinary.Interfaces;
using ShopNext.Infrastructure.Payment.Implementations;
using ShopNext.Infrastructure.Payment.Interfaces;
using ShopNext.Middleware;
using ShopNext.Repositories.Implementations;
using ShopNext.Repositories.Interfaces;
using ShopNext.Services;
using ShopNext.Services.Implementations;
using ShopNext.Services.Interfaces;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Render ke liye
var port = Environment.GetEnvironmentVariable("PORT");

if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.UseUrls($"http://*:{port}");
}

// for rander proxy rander ke liye
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:3000",
                "http://localhost:5173",
                "https://shopnext-client.vercel.app"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

//SERVICES

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IReviewService, ReviewService>();

builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICartService, CartService>();

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddScoped<IAddressRepository, AddressRepository>();
builder.Services.AddScoped<IAddressService, AddressService>();

builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();

builder.Services.AddScoped<IRazorpayService, RazorpayService>();

builder.Services.AddScoped<IOtpRepository, OtpRepository>();
builder.Services.AddScoped<IOtpService, OtpService>();

builder.Services.AddHttpClient<OtpService>();

builder.Services.AddScoped<IBannerRepository, BannerRepository>();
builder.Services.AddScoped<IBannerService, BannerService>();

builder.Services.AddScoped<JwtHelper>();

// DATABASE

builder.Services.AddDbContext<ShopNextDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

//JWT

builder.Services.AddAuthentication("Bearer")
.AddJwtBearer("Bearer", options =>
{
    options.TokenValidationParameters =
        new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    builder.Configuration["Jwt:Key"]
                    ?? throw new InvalidOperationException("Jwt:Key missing")
                )
            )
        };
});
builder.Services.AddControllers();
builder.Services.AddShopNextRateLimiter();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                       ForwardedHeaders.XForwardedProto
});

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.MapGet("/", () => "ShopNest API Running...");

app.UseCors("AllowFrontend");

app.UseRateLimiter();

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();