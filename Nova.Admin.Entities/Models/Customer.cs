using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Repository.Pattern.Repositories;
using Repository.Pattern.Ef6;

namespace Nova.Admin.Entities.Models
{
    [TrackChanges]
    public class Customer : Entity, ISoftDeletable
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