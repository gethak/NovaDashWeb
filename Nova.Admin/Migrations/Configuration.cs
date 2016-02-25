using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using CsvHelper;
using Nova.Admin.Models;

namespace Nova.Admin.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Nova.Admin.Models.NovaAdminContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            //AutomaticMigrationDataLossAllowed B= true;
        }

        protected override void Seed(Nova.Admin.Models.NovaAdminContext context)
        {
            //  This method will be called after migrating to the latest version.
        }
    }
}
