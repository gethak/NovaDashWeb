using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Nova.Admin.Interfaces;
using Owin;
using TrackerEnabledDbContext.Common.Configuration;

[assembly: OwinStartup(typeof(Nova.Admin.Startup))]

namespace Nova.Admin
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);


            GlobalTrackingConfig.DisconnectedContext = true;
            GlobalTrackingConfig.SetSoftDeletableCriteria<ISoftDeletable>(entity => entity.IsDeleted);
        }
    }
}
