using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Repository.Pattern.Repositories
{
    public interface ISoftDeletable
    {
        bool IsDeleted { get; set; }
    }
}