using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MedSoft.DataAccess.Data;
using MedSoft.Models;
using MedSoft.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSoft.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public DbInitializer(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _db = db;
        }

        public void Initialize()
        {
            // Apply migrations if they are not applied
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch(Exception ex)
            {

            }

            // Create roles if they are not created
            if (!_roleManager.RoleExistsAsync(SD.Rol_Pacient).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Rol_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Rol_Medic)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Rol_Pacient)).GetAwaiter().GetResult();

                // If roles are not yet created => create the admin user
                _userManager.CreateAsync(new User
                {
                    UserName = "Admin",
                    Email = "admin@yahoo.com",
                    PhoneNumber = "0723712324", 
                }, "Admin123!").GetAwaiter().GetResult();

                User? user = _db.Users.FirstOrDefault(user => user.Email == "admin@yahoo.com");
                if (user != null)
                {
                    _userManager.AddToRoleAsync(user, SD.Rol_Admin).GetAwaiter().GetResult();
                    Administrator admin = new Administrator()
                    {
                        Nume = "Admin",
                        Prenume = "",
                        Adresa = "Bucuresti",
                        CNP = "5011112450016"
                    };
                    _db.Administrator.Add(admin);
                }            
            }

            return;
        }
    }
}
