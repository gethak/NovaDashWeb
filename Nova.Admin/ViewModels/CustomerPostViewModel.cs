using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nova.Admin.Models;

namespace Nova.Admin.ViewModels
{
    public class CustomerPostViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public Category Category { get; set; }
        public int HouseNumber { get; set; }
        public string AddressLine1 { get; set; }
        public int StateId { get; set; }
        public int CountryRegionId { get; set; }
    }
}