using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using NLog.Web;
using System.Text;
using Repositories;
using Services;
using Entities;
using Entities.Models;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using MyShop.middleWares; 

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MyShopContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Home")));

// ================== Register Services ==================
builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();

// Repositories & Services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IRatingService, RatingService>();
builder.Services.AddScoped<IRatingRepository, RatingRepository>();


// Redis (לא חובה – אפשר להסיר אם לא בשימוש)
builder.Services.AddStackExchangeRedisCache(options =>
{
    var config = builder.Configuration.GetSection("Redis");
    var host = config["Host"];
    var port = config["Port"];
    var password = config["Password"];
    options.Configuration = $"{host}:{port},password={password}";
    options.InstanceName = "MyShopRedis:";
});

// ================== JWT ==================
var jwtKey = builder.Configuration["Jwt:Key"] ?? "your_secret_key_here";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "your_issuer_here";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };

    // קבלת הטוקן מה-cookie בצורה מסודרת
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var token = context.Request.Cookies["jwtToken"]; // או "access_token" אם את משתמשת בשם הזה
            if (!string.IsNullOrEmpty(token))
            {
                context.Token = token;
            }
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            // מונע מהמערכת להחזיר WWW-Authenticate (ולמנוע חלון קופץ בדפדפן)
            context.HandleResponse();
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync("{\"error\": \"Unauthorized: Token is invalid or expired.\"}");
        }
    };
});

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
});

builder.Host.UseNLog();

// ================== Build App ==================
var app = builder.Build();
// ================== Configure App ==============
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Content-Security-Policy",
        "default-src 'self'; script-src 'self'; style-src 'self';");
    await next();
});

// Middleware לשגיאות כלליות
app.UseHandleErrorMiddleware();

// Swagger (רק בפיתוח)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Static Files + HTTPS
app.UseHttpsRedirection();
app.UseStaticFiles();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Controllers
app.MapControllers();

// Middleware פנימי שלך (אם יש לך למשל דירוגים)
app.UseRatingMiddleware();

app.Run();
