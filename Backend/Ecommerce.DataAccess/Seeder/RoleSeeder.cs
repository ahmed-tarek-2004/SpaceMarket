using Ecommerce.Entities.Models.Auth.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.DataAccess.Seeder
{
    public static class RoleSeeder
    {
        public static async Task SeedAsync(RoleManager<Role> _roleManager)
        {
            var rolesCount = await _roleManager.Roles.CountAsync();
            if (rolesCount <= 0)
            {
                await _roleManager.CreateAsync(new Role()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                });

                await _roleManager.CreateAsync(new Role()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "User",
                    NormalizedName = "USER"
                });

                await _roleManager.CreateAsync(new Role()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Client",
                    NormalizedName = "CLIENT"
                });

                await _roleManager.CreateAsync(new Role()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "ServiceProvider",
                    NormalizedName = "SERVICEPROVIDER"
                });
            }
        }
    }
}