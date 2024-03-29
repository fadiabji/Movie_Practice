using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Movie_Exercise.Data;
using Movie_Exercise.MapperHelper;
using Movie_Exercise.Models;
using Movie_Exercise.Services;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseLazyLoadingProxies().UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// using lazyloding here 
//builder.Services.AddDbContext<MovieProjectDbContext>(options =>
//options.UseLazyLoadingProxies().UseSqlServer(
//    builder.Configuration.GetConnectionString("MovieDatatbase")));


// Identity things here
builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()                              // in order to make the rols in the website you have to add this
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();




// Adding sessions 
builder.Services.AddDistributedMemoryCache();



builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


// to make mapper
var config = new MapperConfiguration(cfg =>
{
    cfg.AddProfile(new MappingHelper());
});
var mapper = config.CreateMapper();

builder.Services.AddSingleton(mapper);


// Adding Services Here
builder.Services.AddTransient<IMovieService, MovieService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Use session must be between app.UseRouting(); and app.MapRazorPages();
app.UseSession();

app.MapRazorPages();

app.Run();
