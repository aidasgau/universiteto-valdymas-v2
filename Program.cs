using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using project_mvc.Data;
using Microsoft.EntityFrameworkCore;
using MySql.EntityFrameworkCore.Extensions;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using MySqlConnector;
using Rotativa.AspNetCore;
using System.IO;

var serverVersion = new MySqlServerVersion(new Version(10, 4, 27));

var builder = WebApplication.CreateBuilder(args);

// Get the root path of the web application
var rootPath = AppContext.BaseDirectory;

// Construct the Rotativa tools path relative to the root path
var rotativaPath = Path.Combine(rootPath, "RotativaTools");

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<MySqlDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("MvcConnectionString"), serverVersion));

// Configure Rotativa
RotativaConfiguration.Setup(rotativaPath);

// Configure AntiForgery options
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Name = "project_mvcAntiforgeryCookie";
    options.HeaderName = "X-CSRF-TOKEN";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
