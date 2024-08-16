using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using online_library_management_system.Models;
using online_library_management_system.Services;
using Microsoft.AspNetCore.Identity;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        builder.Services.AddDbContext<ApplicationDbContext>(
            options => options.UseSqlServer(connectionString));

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>(
            options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        // app.MapControllerRoute(
        //     name: "default",
        //     pattern: "{controller=Home}/{action=Index}/{id?}");
        
        #pragma warning disable ASP0014
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapAreaControllerRoute(
               name: "Admin",
                areaName: "Admin",
                pattern: "Admin/{controller=Dashboard}/{action=Index}/{id?}");

            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        });
        #pragma warning restore ASP0014


        // Middleware to handle 404 errors
        app.UseStatusCodePages(async context =>
        {
            var request = context.HttpContext.Request;
            if (context.HttpContext.Response.StatusCode == 404)
            {
                var path = request.Path.Value?.ToLower() ?? string.Empty;

                if (path.StartsWith("/admin"))
                {
                    context.HttpContext.Response.Redirect("/Admin/404");
                }
                else
                {
                    context.HttpContext.Response.Redirect("/404");
                }
            }

            await Task.CompletedTask;
        });


        // Initialize and seed the database
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var dbContext = services.GetRequiredService<ApplicationDbContext>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            var initializer = new DatabaseInitializer(dbContext, userManager, roleManager);
            await initializer.InitializeAsync();
        }

        app.Run();
    }
}
