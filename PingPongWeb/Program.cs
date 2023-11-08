using PingPongWeb.Services;
using PingPongWeb.Services.IServices;
using PingPongWeb;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(typeof(MappingConfig));

// Add services to the container.
builder.Services.AddControllersWithViews();
//add cache and session token for login token
builder.Services.AddDistributedMemoryCache();

//add dependency injection
builder.Services.AddHttpClient<IPlayerService, PlayerService>();
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddHttpClient<IMatchService, MatchService>();
builder.Services.AddScoped<IMatchService, MatchService>();
builder.Services.AddScoped<IDashboard, Dashboard>();
builder.Services.AddHttpClient<IAuthService, AuthService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserDataService, UserDataService>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

//so that log out work in web project
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.SlidingExpiration = true;
    });
builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromMinutes(10);
    option.Cookie.HttpOnly = true;
    option.Cookie.IsEssential = true;
});

var app = builder.Build();

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
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Player}/{action=Index}");

app.Run();

