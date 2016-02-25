using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using EntityFramework.DynamicFilters;
using Nova.Admin.Interfaces;
using TrackerEnabledDbContext;

namespace Nova.Admin.Models
{
    public class NovaAdminContext : TrackerContext
    {
        public NovaAdminContext() : base("name=NovaAdminContext")
        {}

        public IDbSet<Customer> Customers { get; set; }
        public IDbSet<Address> Addresses { get; set; }
        public IDbSet<State> States { get; set; }
        public IDbSet<CountryRegion> Countries { get; set; }

        public override int SaveChanges()
        {
            var Changed = ChangeTracker.Entries();
            if (Changed != null)
            {
                foreach (var entry in Changed.Where(e => e.State == EntityState.Deleted))
                {
                    entry.State = EntityState.Unchanged;
                    if (entry.Entity is ISoftDeletable)
                    {
                        (entry.Entity as ISoftDeletable).IsDeleted = true;
                    }
                }
            }

            return base.SaveChanges();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Filter("IsDeleted", (ISoftDeletable d) => d.IsDeleted, false);
            base.OnModelCreating(modelBuilder);
        }
    }
}
