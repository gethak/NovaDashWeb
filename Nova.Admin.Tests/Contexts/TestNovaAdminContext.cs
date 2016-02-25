using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nova.Admin.Models;

namespace Nova.Admin.Tests.Contexts
{
    public class TestNovaAdminContext : NovaAdminContext
    {
        public TestNovaAdminContext()
            //: base("Name=TestContext")
        {}

        public TestNovaAdminContext(bool enableLazyLoading, bool enableProxyCreation)
            //: base("Name=TestContext")
        {
            Configuration.ProxyCreationEnabled = enableProxyCreation;
            Configuration.LazyLoadingEnabled = enableLazyLoading;
        }

        public TestNovaAdminContext(DbConnection connection)
            //: base(connection, true)
        {
            Configuration.LazyLoadingEnabled = false;
        }

        public IDbSet<Customer> Customers { get; set; }
        public IDbSet<Address> Addresses { get; set; }
        public IDbSet<State> States { get; set; }
        public IDbSet<CountryRegion> Countries { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Suppress code first model migration check          
            Database.SetInitializer<TestNovaAdminContext>(new AlwaysCreateInitializer());

            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            base.OnModelCreating(modelBuilder);
        }

        public void Seed(NovaAdminContext context)
        {
            #region Countries

            context.Countries.Add(new CountryRegion
            {
                Id = 1,
                Name = "United States"
            });

            context.Countries.Add(new CountryRegion
            {
                Id = 2,
                Name = "South Africa"
            });

            #endregion

            #region States

            context.States.Add(new State
            {
                Id = 1,
                Name = "New York",
                CountryId = 1
            });


            context.States.Add(new State
            {
                Id = 2,
                Name = "California",
                CountryId = 1
            });

            context.States.Add(new State
            {
                Id = 3,
                Name = "Western Cape",
                CountryId = 2
            });

            #endregion

            #region Customers

            context.Customers.Add(new Customer
            {
                Id = 1,
                Name = "Customer 1",
                DateOfBirth = DateTime.Today.AddDays(-1),
                Category = Category.CategoryA,
                Gender = Gender.Female,
                AddressId = 1
            });

            context.Customers.Add(new Customer
            {
                Id = 2,
                Name = "Customer 2",
                DateOfBirth = DateTime.Today.AddDays(-2),
                Category = Category.CategoryA,
                Gender = Gender.Female,
                AddressId = 2
            });

            context.Customers.Add(new Customer
            {
                Id = 3,
                Name = "Customer 3",
                DateOfBirth = DateTime.Today.AddDays(-3),
                Category = Category.CategoryB,
                Gender = Gender.Male,
                AddressId = 3
            });

            context.Customers.Add(new Customer
            {
                Id = 4,
                Name = "Customer 4",
                DateOfBirth = DateTime.Today.AddDays(-4),
                Category = Category.CategoryB,
                Gender = Gender.Unknown,
                AddressId = 4
            });

            context.Customers.Add(new Customer
            {
                Id = 5,
                Name = "Customer 5",
                DateOfBirth = DateTime.Today.AddDays(-5),
                Category = Category.CategoryC,
                Gender = Gender.Female,
                AddressId = 5
            });

            context.Customers.Add(new Customer
            {
                Id = 6,
                Name = "Customer 6",
                DateOfBirth = DateTime.Today.AddDays(-5),
                Category = Category.CategoryC,
                Gender = Gender.Female,
                AddressId = 6
            });

            #endregion

            #region Addresses

            context.Addresses.Add(new Address
            {
                AddressLine1 = "Test Address 1 line 1",
                Id = 1,
                IsDeleted = false,
                HouseNumber = 1,
                StateId = 1
            });

            context.Addresses.Add(new Address
            {
                AddressLine1 = "Test Address 2 line 2",
                Id = 2,
                IsDeleted = false,
                HouseNumber = 2,
                StateId = 2
            });

            context.Addresses.Add(new Address
            {
                AddressLine1 = "Test Address 3 line 3",
                Id = 3,
                IsDeleted = false,
                HouseNumber = 3,
                StateId = 2
            });

            context.Addresses.Add(new Address
            {
                AddressLine1 = "Test Address 4 line 4",
                Id = 4,
                IsDeleted = false,
                HouseNumber = 4,
                StateId = 2
            });

            context.Addresses.Add(new Address
            {
                AddressLine1 = "Test Address 5 line 5",
                Id = 5,
                IsDeleted = false,
                HouseNumber = 5,
                StateId = 2
            });

            context.Addresses.Add(new Address
            {
                AddressLine1 = "Test Address 6 line 6",
                Id = 6,
                IsDeleted = false,
                HouseNumber = 6,
                StateId = 3
            });

            #endregion

            context.SaveChanges();
        }

        public class DropCreateIfChangeInitializer : DropCreateDatabaseIfModelChanges<TestNovaAdminContext>
        {
            protected override void Seed(TestNovaAdminContext context)
            {
                context.Seed(context);
                base.Seed(context);
            }
        }

        public class CreateInitializer : CreateDatabaseIfNotExists<TestNovaAdminContext>
        {
            protected override void Seed(TestNovaAdminContext context)
            {
                context.Seed(context);
                base.Seed(context);
            }
        }

        public class AlwaysCreateInitializer : DropCreateDatabaseAlways<TestNovaAdminContext>
        {
            protected override void Seed(TestNovaAdminContext context)
            {
                context.Seed(context);
                base.Seed(context);
            }
        }
    }
}