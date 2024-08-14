using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using online_library_management_system.Models;
using online_library_management_system.Services;
using System.Linq;
using System.Threading.Tasks;

public class DatabaseInitializer
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public DatabaseInitializer(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitializeAsync()
    {
        await _context.Database.MigrateAsync();
        await SeedRolesAsync();
        await SeedAdminUserAsync();
    }

    private async Task SeedRolesAsync()
    {
        var adminRoleId = "c9d81569-b65c-498e-bce5-8df93ddc8a8f";
        var userRoleId = "f1b83e1a-bb3e-4a92-951c-5aefc68b3a20";

        if (!await _roleManager.RoleExistsAsync("Admin"))
        {
            var adminRole = new IdentityRole
            {
                Id = adminRoleId,
                Name = "Admin",
                NormalizedName = "ADMIN"
            };
            await _roleManager.CreateAsync(adminRole);
        }

        if (!await _roleManager.RoleExistsAsync("User"))
        {
            var userRole = new IdentityRole
            {
                Id = userRoleId,
                Name = "User",
                NormalizedName = "USER"
            };
            await _roleManager.CreateAsync(userRole);
        }
    }

    private async Task SeedAdminUserAsync()
    {
        var adminUserId = "2105a4e7-9e72-4141-96f6-a546c3a4f8fa";
        var adminUser = await _userManager.FindByIdAsync(adminUserId);

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                Id = adminUserId,
                Email = "sydenhamlibrary@gmail.com",
                FirstName = "Sydenham",
                LastName = "Admin",
                NormalizedEmail = "SYDENHAMLIBRARY@GMAIL.COM",
                UserName = "sydenhamlibrary@gmail.com",
                NormalizedUserName = "SYDENHAMLIBRARY@GMAIL.COM",
                ConcurrencyStamp = adminUserId,
            };

            var passwordHasher = new PasswordHasher<ApplicationUser>();
            adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "Sydenham@2024");

            await _userManager.CreateAsync(adminUser);
        }

        if (!await _userManager.IsInRoleAsync(adminUser, "Admin"))
        {
            await _userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}
