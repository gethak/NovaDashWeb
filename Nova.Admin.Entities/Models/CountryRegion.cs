using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nova.Admin.Entities.Models
{
    public class CountryRegion
    {
        public int Id { get; set; }
        //[Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string CountryRegionCode { get; set; }
        public string Name { get; set; }
    }
}