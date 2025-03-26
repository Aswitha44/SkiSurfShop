using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SkiSurf.Core.Entities;
using SkiSurf.Core.Entities.Identity;
using SkiSurf.Core.Entities.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SkiSurf.Infrastructure.Data
{
    public class StoreContextSeed
    {
        public static async Task SeedAsync(StoreContext context,UserManager<AppUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                var user = new AppUser
                {
                    DisplayName = "Bob",
                    Email = "bob@test.com",
                    UserName = "bob@test.com",
                    Address = new Core.Entities.Identity.Address
                    {
                        FirstName = "Bob",
                        LastName = "Bobbity",
                        Street = "10 The Street",
                        City = "New York",
                        State = "NY",
                        ZipCode = "90210"
                    }
                };

                await userManager.CreateAsync(user, "Pa$$w0rd");
            }

                var path=Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (!context.ProductBrands.Any())
            {
                //var brandsData = File.ReadAllText("C:\\Users\\abhin\\SkiSurfWeb\\SkiSurf.Infrastructure\\Data\\SeedData\\brands.json");

                 var brandsData = File.ReadAllText(path+@"\\Data\\SeedData\\brands.json");
                var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandsData);
                await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT ProductBrands ON");
                context.ProductBrands.AddRange(brands);
            }

            if (!context.ProductTypes.Any())
            {
                //var typesData = File.ReadAllText("C:\\Users\\abhin\\SkiSurfWeb\\SkiSurf.Infrastructure\\Data\\SeedData\\types.json");
                var typesData = File.ReadAllText(path+@"\\Data\\SeedData\\types.json");
                var types = JsonSerializer.Deserialize<List<ProductType>>(typesData);
                await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT ProductTypes ON");
                context.ProductTypes.AddRange(types);
            }

            if (!context.Products.Any())
            {
                //var productsData = File.ReadAllText("C:\\Users\\abhin\\SkiSurfWeb\\SkiSurf.Infrastructure\\Data\\SeedData\\products.json");
                var productsData = File.ReadAllText(path+@"\\Data\\SeedData\\products.json");
                var products = JsonSerializer.Deserialize<List<Product>>(productsData);
                await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Products ON");
                context.Products.AddRange(products);
            }

            if (!context.DeliveryMethods.Any())
            {
                //var deliveryData = File.ReadAllText("C:\\Users\\abhin\\SkiSurfWeb\\SkiSurf.Infrastructure\\Data\\SeedData\\delivery.json");
                var deliveryData = File.ReadAllText(path+@"\\Data\\SeedData\\delivery.json");
                var methods = JsonSerializer.Deserialize<List<DeliveryMethod>>(deliveryData);
                await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT DeliveryMethods ON");
                context.DeliveryMethods.AddRange(methods);

            }



            if (context.ChangeTracker.HasChanges())
            {

                await context.SaveChangesAsync();


            }
        }
    }
}
