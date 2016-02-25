using System;
using System.ComponentModel.DataAnnotations;
using Repository.Pattern.Repositories;

namespace Nova.Admin.Entities.Models
{
    [TrackChanges]
    public class Address : ISoftDeletable
    {
        public int Id { get; set; }
        public int HouseNumber { get; set; }
        public string AddressLine1 { get; set; }
        public int StateId { get; set; }
        public State State { get; set; }
        public bool IsDeleted { get; set; }
    }
}