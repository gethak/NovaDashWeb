using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using EntityFramework.DynamicFilters;
using Nova.Admin.Interfaces;
//using TrackerEnabledDbContext;
//using TrackerEnabledDbContext.Common.Interfaces;

namespace Nova.Admin.Models
{
    public interface INovaAdminContext : IDisposable //IFilterableContext, ITrackerContext 
    {
        IDbSet<Customer> Customers { get; set; }
        IDbSet<Address> Addresses { get; set; }
        IDbSet<State> States { get; set; }
        IDbSet<CountryRegion> Countries { get; set; }
        int SaveChanges();
    }
}
