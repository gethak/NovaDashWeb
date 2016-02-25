using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Nova.Admin.Models;

namespace Nova.Admin.ViewModels
{
    public class CustomerViewModel
    {
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [UIHint("GenderEditor")]
        public Gender Gender { get; set; }

        [Required]
        [UIHint("CategoryEditor")]
        public Category Category { get; set; }

   
        [ScaffoldColumn(false)]
        public int AddressId { get; set; }

        [Required]
        public int HouseNumber { get; set; }

        [Required]
        public string AddressLine1 { get; set; }


        [ScaffoldColumn(false)]
        public string StateName { get; set; }
        [ScaffoldColumn(false)]
        public string CountryRegionName { get; set; }


        [UIHint("CustomGridForeignKey")]
        [DisplayName("State")]
        public int StateId { get; set; }


        [UIHint("CustomGridForeignKey")]
        [DisplayName("Country")]
        public int CountryRegionId { get; set; }

        [ReadOnly(true)]
        public bool IsDeleted { get; set; }
    }
}