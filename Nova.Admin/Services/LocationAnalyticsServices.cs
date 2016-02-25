using System.Collections.Generic;
using System.Linq;
using Nova.Admin.Models;
using Nova.Admin.ViewModels;

namespace Nova.Admin.Services
{
    public class LocationAnalyticsServices
    {
        private NovaAdminContext _context = new NovaAdminContext();

        private const string _UNITED_STATES = "United States";

        public LocationAnalyticsServices()
        {}

        public int StateCustomersTotal(string stateName)
        {
            var totalCustomersByState = (from address in _context.Addresses
                                         join customer in _context.Customers on address equals customer.Address
                                         join state in _context.States on address.State equals state
                                         where state.Name == stateName
                                         select new
                                         {
                                             customer.Id
                                         }).Count();

            return totalCustomersByState;
        }

        public IQueryable<MarketShare_Result> CustomersTotalForAllStatesByCountry(string countryName = _UNITED_STATES)
        {
            var totalCustomersByAllStates = from address in _context.Addresses
                                            join customer in _context.Customers on address equals customer.Address
                                            join state in _context.States on address.State equals state
                                            where state.Country.Name == countryName
                                            orderby state.Id
                                            group customer by state.Name
                into customersByStatesGroup
                                            select new MarketShare_Result
                                            {
                                                Area = customersByStatesGroup.Key,
                                                CustomersCount = customersByStatesGroup.Count()
                                            };

            return totalCustomersByAllStates;
        }

        public MarketShare_Result[] MarketShareByState(string stateName)
        {
            var totalCustomersByState = (from address in _context.Addresses
                                         join customer in _context.Customers on address equals customer.Address
                                         join state in _context.States on address.State equals state
                                         where state.Name == stateName
                                         select customer.Id).Count();

            var totalCustomersAcrossAllStates = _context.Customers.Count();

            return new MarketShare_Result[]
            {
                new MarketShare_Result {Area = "All", CustomersCount = totalCustomersAcrossAllStates},
                new MarketShare_Result {Area = stateName, CustomersCount = totalCustomersByState}
            };
        }
         
        public GenderShare_Result[] GenderShareByState(string stateName)
        {
            var totalGenderByState = from address in _context.Addresses
                                     join customer in _context.Customers on address equals customer.Address
                                     join state in _context.States on address.State equals state
                                     where state.Name == stateName
                                     group customer by new { customer.Gender, state.Name } into customersByGenderGroup
                                     select new GenderShare_Result
                                     {
                                         Gender = customersByGenderGroup.Key.Gender.ToString(),
                                         CustomersCount = customersByGenderGroup.Count()
                                     };

            return totalGenderByState.ToArray();
        }

        public CategoryShare_Result[] CategoryShareByState(string stateName)
        {
            var totalcategoryByState = from address in _context.Addresses
                                       join customer in _context.Customers on address equals customer.Address
                                       join state in _context.States on address.State equals state
                                       where state.Name == stateName
                                       group customer by new { customer.Category, state.Name } into customersByCategoryGroup
                                       select new CategoryShare_Result
                                       {
                                           Category = customersByCategoryGroup.Key.Category.ToString(),
                                           CustomersCount = customersByCategoryGroup.Count()
                                       };

            return totalcategoryByState.ToArray();
        }

        public IQueryable<LocationAutoCompleteViewModel> GetStatesForAutoComplete(string text)
        {
            var customers = _context.States.Select(state => new LocationAutoCompleteViewModel
            {
                Id = state.Id,
                Name = state.Name
            });

            if (!string.IsNullOrEmpty(text))
            {
                customers = customers.Where(c => c.Name.StartsWith(text)).Take(5);
            }

            return customers;
        }
    }
}