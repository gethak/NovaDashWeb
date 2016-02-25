using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
//using TrackerEnabledDbContext.Common.Models;

namespace Nova.Admin.ViewModels
{
    [DataContract]
    public class AuditLogViewModel
    {
        public DateTime EventDateUtc { get; set; }
        public string EventType { get; set; }
        
        //public virtual ICollection<AuditLogDetail> LogDetails { get; set; }     
        public string RecordId { get; set; }

        public string UserName { get; set; }
    }
}