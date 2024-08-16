using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using online_library_management_system.Models;
using Microsoft.AspNetCore.Identity;

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

            // Configuring IdentityUser table
            builder.Entity<IdentityUser>(e => {
                e.ToTable(name: "Users").Ignore(c => c.AccessFailedCount)
                                        .Ignore(c => c.EmailConfirmed)
                                        .Ignore(c => c.LockoutEnabled)
                                        .Ignore(c => c.LockoutEnd)
                                        .Ignore(c => c.PhoneNumberConfirmed)
                                        .Ignore(c => c.SecurityStamp)
                                        .Ignore(c => c.TwoFactorEnabled);
            });

            // Adding custom properties to ApplicationUser
            builder.Entity<ApplicationUser>().Property<string>("FirstName");
            builder.Entity<ApplicationUser>().Property<string>("LastName");
            builder.Entity<ApplicationUser>().Property<string>("Address");

            // Configuring Identity Role tables
            builder.Entity<IdentityRole>(e => { e.ToTable(name: "Roles"); });
            builder.Entity<IdentityUserRole<string>>(e => { e.ToTable(name: "UserRoles").HasKey(x => new { x.UserId, x.RoleId }); });
            builder.Entity<IdentityUserToken<string>>(e => { e.ToTable(name: "UserTokens"); });
            builder.Entity<IdentityUserLogin<string>>(e => { e.ToTable(name: "UserLogins"); });
            builder.Entity<IdentityUserClaim<string>>(e => { e.ToTable(name: "UserClaims"); });
            builder.Entity<IdentityRoleClaim<string>>(e => { e.ToTable(name: "RoleClaims"); });
        }
        public DbSet<Category> Categories { get; set; }

        public DbSet<AuthorAndArtist> AuthorsAndArtists { get; set; }

    }
}
