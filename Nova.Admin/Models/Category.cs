using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Nova.Admin.Models
{
    public enum Category
    {
        [Display(Name = "Category A")]
        CategoryA,
        [Display(Name = "Category B")]
        CategoryB,
        [Display(Name = "Category C")]
        CategoryC
    }
}