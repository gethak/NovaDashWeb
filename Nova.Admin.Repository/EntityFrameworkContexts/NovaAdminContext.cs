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
using Repository.Pattern.Repositories;
using TrackerEnabledDbContext;

namespace Nova.Admin.Entities.Models
{
    public class NovaAdminContext : TrackerContext
    {
        public NovaAdminContext() : base("name=NovaAdminContext")
        {}

        public System.Data.Entity.DbSet<Nova.Admin.Entities.Models.Customer> Customers { get; set; }
        public System.Data.Entity.DbSet<Nova.Admin.Entities.Models.Address> Addresses { get; set; }
        public System.Data.Entity.DbSet<Nova.Admin.Entities.Models.State> States { get; set; }
        public System.Data.Entity.DbSet<Nova.Admin.Entities.Models.CountryRegion> Countries { get; set; }

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

            //foreach (var entry in ChangeTracker.Entries()
            //    .Where(p => p.State == EntityState.Deleted && p is ISoftDeletable))
            //{
            //    entry.State = EntityState.Unchanged;
            //    entry.Entity.GetType().GetMethod("Delete")
            //        .Invoke(entry.Entity, null);
            //}

            return base.SaveChanges();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Filter("IsDeleted", (ISoftDeletable d) => d.IsDeleted, false);
            base.OnModelCreating(modelBuilder);
        }

        //public override int SaveChanges()
        //{
        //    foreach (var entry in ChangeTracker.Entries()
        //        .Where(p => p.State == EntityState.Deleted && p.Entity is ISoftDeletable))
        //    {
        //        SoftDelete(entry);
        //    }

        //    return base.SaveChanges();
        //}

        //private void SoftDelete(DbEntityEntry entry)
        //{
        //    var entity = entry.Entity as ISoftDeletable;
        //    string tableName = GetTableName(entity.GetType());
        //    Database.ExecuteSqlCommand(
        //             String.Format("UPDATE {0} SET IsDeleted = 1 WHERE ID = @id", tableName)
        //             , new SqlParameter("id", entity));
        //    entry.State = EntityState.Detached;
        //}

        //private readonly static Dictionary<Type, EntitySetBase> _mappingCache = new Dictionary<Type, EntitySetBase>();

        //private ObjectContext _ObjectContext
        //{
        //    get { return (this as IObjectContextAdapter).ObjectContext; }
        //}

        //private EntitySetBase GetEntitySet(Type type)
        //{
        //    if (_mappingCache.ContainsKey(type))
        //        return _mappingCache[type];

        //    //type = GetObjectType(type);
        //    string baseTypeName = type.BaseType.Name;
        //    string typeName = type.Name;

        //    ObjectContext octx = _ObjectContext;
        //    var es = octx.MetadataWorkspace
        //                    .GetItemCollection(DataSpace.SSpace)
        //                    .GetItems<EntityContainer>()
        //                    .SelectMany(c => c.BaseEntitySets
        //                                    .Where(e => e.Name == typeName
        //                                    || e.Name == baseTypeName))
        //                    .FirstOrDefault();

        //    if (es == null)
        //        throw new ArgumentException("Entity type not found in GetEntitySet", typeName);

        //    return es;
        //}

        //internal String GetTableName(Type type)
        //{
        //    EntitySetBase es = GetEntitySet(type);

        //    //if you are using EF6
        //    return String.Format("[{0}].[{1}]", es.Schema, es.Table);

        //    //if you have a version prior to EF6
        //    //return string.Format( "[{0}].[{1}]", 
        //    //        es.MetadataProperties["Schema"].Value, 
        //    //        es.MetadataProperties["Table"].Value );
        //}



    }
}
