using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Nova.Admin.Entities.Models;
using Nova.Admin.Models;

namespace Nova.Admin.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<NovaAdminContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            //AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(Nova.Admin.Models.NovaAdminContext context)
        {
            //  This method will be called after migrating to the latest version.

            var categoryDictionary = new Dictionary<string, Category>()
            {
                 { "Category A", Category.CategoryA },
                 { "Category B", Category.CategoryB },
                 { "Category C", Category.CategoryC },
            };

            using (var dbContext = new NovaAdminSeedContext())
            {
                var countries = dbContext.DenormalizedCustomers.Select(x => x.Country).Distinct().AsEnumerable();
                foreach (var country in countries)
                {
                    dbContext.Countries.Add(new CountryRegion { Name =  country });
                }

                dbContext.SaveChanges("Seed Data Initializer");

                CountryRegion US = dbContext.Countries.First();

                var states = dbContext.DenormalizedCustomers.Select(x => x.State).Distinct().AsEnumerable();
                foreach (var state in states)
                {
                    dbContext.States.Add(new State {Name = state, Country = US});
                }

                dbContext.SaveChanges("Seed Data Initializer");
            }


            using (var dbContext = new NovaAdminSeedContext())
            {
                List<State> states = dbContext.States.ToList();
                List<CountryRegion> countries = dbContext.Countries.ToList();
                DbSet<Customer> customers = dbContext.Customers;

                foreach (var denormalizedCustomer in dbContext.DenormalizedCustomers.ToList())
                {
                    customers.Add(SampleData.GetCustomer(denormalizedCustomer, states, categoryDictionary));
                }

                dbContext.SaveChanges("Seed Data Initializer");
            }
        }
    }
}
