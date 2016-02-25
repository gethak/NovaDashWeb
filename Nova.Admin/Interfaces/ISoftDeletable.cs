using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nova.Admin.Interfaces
{
    interface ISoftDeletable
    {
        bool IsDeleted { get; set; }
    }
}