using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Nova.Admin.Models;
using Nova.Admin.Interfaces;

namespace Nova.Admin.Models
{
    [TrackChanges]
    public class Customer : ISoftDeletable
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime DateOfBirth { get; set; }

        public Gender Gender { get; set; }

        public Category Category { get; set; }

        public int AddressId { set; get; }

        public Address Address { get; set; }

        public bool IsDeleted { get; set; }
    }
}