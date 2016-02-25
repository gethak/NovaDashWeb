namespace Nova.Admin.Entities.Models
{
    public class State
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int CountryId { get; set; }
        public CountryRegion Country { get; set; }
    }
}