
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nova.Admin.Models
{
    internal class SampleData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Category { get; set; }

        public double HouseNumber { get; set; }
        public string AddressLine1 { get; set; }

        public string State { get; set; }
        
        public string Country { get; set; }

        public static Customer GetCustomer(SampleData denormalizedCustomer, List<State> states, Dictionary<string, Category> categoryDictionary)
        {
            return new Customer
            {
                Name = denormalizedCustomer.Name,
                DateOfBirth = DateTime.Parse(denormalizedCustomer.DateOfBirth),
                Gender = (Gender)Enum.Parse(typeof(Gender),
                    denormalizedCustomer.Gender),
                Category = categoryDictionary[denormalizedCustomer.Category],
                IsDeleted = false,

                Address = new Address
                {
                    State = states.First(x => x.Name == denormalizedCustomer.State),
                    AddressLine1 = denormalizedCustomer.AddressLine1,
                    HouseNumber = (int)denormalizedCustomer.HouseNumber,
                    IsDeleted = false
                }
            };
        }
    }
}
