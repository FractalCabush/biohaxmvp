using BioHax.Models;
using BioHax.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BioHax.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, string testUserPw)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                var adminID = await EnsureUser(serviceProvider, testUserPw, "admin@biohax.tech");
                await EnsureRole(serviceProvider, adminID, Constants.ServiceAdministratorsRole);

                // Allowed user can create and edit services that they create

                var uid = await EnsureUser(serviceProvider, testUserPw, "manager@biohax.tech");
                await EnsureRole(serviceProvider, uid, Constants.ServiceManagersRole);

                SeedDB(context, adminID);
            }
        }

        private static async Task<string> EnsureUser(IServiceProvider serviceProvider, string testUserPw, string UserName)
        {
            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();

            var user = await userManager.FindByNameAsync(UserName);
            if (user == null)
            {
                user = new ApplicationUser { UserName = UserName };
                await userManager.CreateAsync(user, testUserPw);
            }


            return user.Id;
        }

        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider,
                                                              string uid, string role)
        {
            IdentityResult IR = null;
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (!await roleManager.RoleExistsAsync(role))
            {
                IR = await roleManager.CreateAsync(new IdentityRole(role));
            }

            var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();

            var user = await userManager.FindByIdAsync(uid);

            IR = await userManager.AddToRoleAsync(user, role);

            return IR;
        }


        public static void SeedDB(ApplicationDbContext context, string adminID)
        {
            if (context.Service.Any())
            {
                return;   // DB has been seeded
            }

            context.Service.AddRange(
                new Service
                {
                    Provider = "SJ",
                    Type = "Third Party",
                    Status = ServiceStatus.Approved,
                    OwnerID = adminID
                },
                new Service
                {
                    Provider = "Gymbolaget",
                    Type = "Third Party",
                    Status = ServiceStatus.Approved,
                    OwnerID = adminID
                },
                 new Service
                 {
                     Provider = "VCard",
                     Type = "NDEF",
                     Status = ServiceStatus.Approved,
                     OwnerID = adminID
                 },
                new Service
                {
                    Provider = "URI",
                    Type = "NDEF",
                    Status = ServiceStatus.Approved,
                    OwnerID = adminID
                },
                new Service
                {
                    Provider = "Application Trigger",
                    Type = "NDEF",
                    Status = ServiceStatus.Approved,
                    OwnerID = adminID
                }
             );
            context.SaveChanges();
        }
    }
}