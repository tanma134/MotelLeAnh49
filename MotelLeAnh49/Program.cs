using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using MotelLeAnh49.Data;
using DataAccess.Repositories;
using DataAccess.Repositories.Interfaces;
using BusinessLogic.Services;
using BusinessLogic.Interfaces;
using BusinessLogic.Service;
using BusinessLogic.Config;

var builder = WebApplication.CreateBuilder(args);

// ================== SERVICES ==================

// DbContext
builder.Services.AddDbContext<MotelDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IEventRegistrationRepository, EventRegistrationRepository>();
builder.Services.AddScoped<IServiceItemRepository, ServiceItemRepository>();
builder.Services.AddScoped<IBookingServiceRepository, BookingServiceRepository>();

// Services
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IServiceItemService, ServiceItemService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<AuthService>();


builder.Services.Configure<OpenAIConfig>(
    builder.Configuration.GetSection("OpenAI"));
builder.Services.AddHttpClient<IOpenAIService, OpenAIService>(client =>
{
    client.DefaultRequestHeaders.Add("x-api-key",
        builder.Configuration["OpenAI:ApiKey"]);
    client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
});
builder.Services.AddScoped<IChatService, ChatService>();

builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IEventRegistrationService, EventRegistrationService>();

// Email config
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

// ================== SESSION ==================

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ================== AUTHENTICATION ==================

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
.AddCookie(options =>
{
    options.Cookie.Name = "MotelAuthCookie";
    options.ExpireTimeSpan = TimeSpan.FromHours(24);
    options.SlidingExpiration = true;

    // mặc định
    options.LoginPath = "/Auth/Login";
    options.AccessDeniedPath = "/Auth/AccessDenied";

    // redirect đúng login page
    options.Events.OnRedirectToLogin = context =>
    {
        if (context.Request.Path.StartsWithSegments("/Bookings") || context.Request.Path.StartsWithSegments("/Admin"))
        {
            context.Response.Redirect("/Admin/Login");
        }
        else
        {
            context.Response.Redirect("/Auth/Login");
        }

        return Task.CompletedTask;
    };
});

// ================== AUTHORIZATION ==================

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));
});

// ================== MVC ==================

builder.Services.AddControllersWithViews();

builder.Services.AddSignalR();

var app = builder.Build();

// ================== MIDDLEWARE ==================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Session trước Auth
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// ================== ROUTING ==================

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapHub<MotelLeAnh49.Hubs.RoomHub>("/roomHub");
app.Run();