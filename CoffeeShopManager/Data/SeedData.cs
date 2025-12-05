using Microsoft.AspNetCore.Identity;

namespace CoffeeShopManager.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            // 1. Tạo 3 role nếu chưa có
            string[] roleNames = { "Admin", "Staff", "Cashier" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // 2. Tạo tài khoản Admin
            var adminEmail = "admin@coffee.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(adminUser, "Admin@123");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // 3. Tạo tài khoản Staff
            var staffEmail = "staff@coffee.com";
            var staffUser = await userManager.FindByEmailAsync(staffEmail);
            if (staffUser == null)
            {
                staffUser = new IdentityUser
                {
                    UserName = staffEmail,
                    Email = staffEmail,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(staffUser, "Staff@123");
                await userManager.AddToRoleAsync(staffUser, "Staff");
            }

            // 4. Tạo tài khoản Cashier (thu ngân)
            var cashierEmail = "cashier@coffee.com";
            var cashierUser = await userManager.FindByEmailAsync(cashierEmail);
            if (cashierUser == null)
            {
                cashierUser = new IdentityUser
                {
                    UserName = cashierEmail,
                    Email = cashierEmail,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(cashierUser, "Cashier@123");
                await userManager.AddToRoleAsync(cashierUser, "Cashier");
            }
        }
    }
}