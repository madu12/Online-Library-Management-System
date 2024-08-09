using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using online_library_management_system.Models;
using Microsoft.AspNetCore.Identity;
using System.Reflection.Emit;

namespace online_library_management_system.Services
{
	public class ApplicationDbContext : IdentityDbContext
    {
		public ApplicationDbContext(DbContextOptions options) : base(options)
		{
		}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityUser>(e => {
                e.ToTable(name: "Users").Ignore(c => c.AccessFailedCount)
                                           .Ignore(c => c.EmailConfirmed)
                                           .Ignore(c => c.LockoutEnabled)
                                           .Ignore(c => c.LockoutEnd)
                                           .Ignore(c => c.PhoneNumberConfirmed)
                                           .Ignore(c => c.SecurityStamp)
                                           .Ignore(c => c.TwoFactorEnabled);
                                           
            });
            builder.Entity<ApplicationUser>().Property<string>("FirstName");
            builder.Entity<ApplicationUser>().Property<string>("LastName");
            builder.Entity<ApplicationUser>().Property<string>("Address");

            builder.Entity<IdentityRole>(e => { e.ToTable(name: "Roles"); });
            builder.Entity<IdentityUserRole<string>>(e => { e.ToTable(name: "UserRoles").HasKey(x => new { x.UserId, x.RoleId }); });
            builder.Entity<IdentityUserToken<string>>(e => { e.ToTable(name: "UserTokens"); });
            builder.Entity<IdentityUserLogin<string>>(e => { e.ToTable(name: "UserLogins"); });
            builder.Entity<IdentityUserClaim<string>>(e => { e.ToTable(name: "UserClaims"); });
            builder.Entity<IdentityRoleClaim<string>>(e => { e.ToTable(name: "RoleClaims"); });



            string ADMIN_ID = "2105a4e7-9e72-4141-96f6-a546c3a4f8fa";
            string ROLE_ID = "c9d81569-b65c-498e-bce5-8df93ddc8a8f";

            //seed admin role
            builder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Name = "Admin",
                NormalizedName = "admin",
                Id = ROLE_ID,
                ConcurrencyStamp = ROLE_ID
            });

            //seed user role
            builder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Name = "User",
                NormalizedName = "user"
            });

            //create user
            var appUser = new ApplicationUser
            {
                Id = ADMIN_ID,
                Email = "sydenhamlibrary@gmail.com",
                FirstName = "Sydenham",
                LastName = "Admin",
                NormalizedEmail="sydenhamlibrary@gmail.com",
                UserName = "Sydenham Admin",
                NormalizedUserName = "sydenhamlibrary@gmail.com",
                ConcurrencyStamp = ADMIN_ID,
            };

            //set user password
            PasswordHasher<ApplicationUser> ph = new PasswordHasher<ApplicationUser>();
            appUser.PasswordHash = ph.HashPassword(appUser, "Sydenham@2024");

            //seed user
            builder.Entity<ApplicationUser>().HasData(appUser);

            //set user role to admin
            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = ROLE_ID,
                UserId = ADMIN_ID
            });

        }
    }
}

