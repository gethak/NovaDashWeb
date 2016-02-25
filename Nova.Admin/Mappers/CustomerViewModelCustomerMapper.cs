using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nova.Admin.Models;
using Nova.Admin.ViewModels;

namespace Nova.Admin.Mappers
{
    public static class CustomerViewModelCustomerMapper
    {
        public static Customer ToCustomer(this CustomerViewModel customerViewModel)
        {
            if (customerViewModel == null)
                throw new ArgumentNullException("customerViewModel");

            var customer = new Customer();
            customer.Id = customerViewModel.Id;
            customer.Gender = customerViewModel.Gender;
            customer.Name = customerViewModel.Name;
            customer.Category = customerViewModel.Category;
            customer.DateOfBirth = customerViewModel.DateOfBirth;
            if (customer.Address == null)
                customer.Address = new Address();
            customer.AddressId = customerViewModel.AddressId;
            customer.Address.Id = customerViewModel.AddressId;
            customer.Address.AddressLine1 = customerViewModel.AddressLine1;
            customer.Address.HouseNumber = customerViewModel.HouseNumber;
            customer.Address.StateId = customerViewModel.StateId;

            return customer;
        }

        public static CustomerViewModel ToCustomerViewModel(this Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException("customerViewModel");

            return new CustomerViewModel
            {
                Id = customer.Id,
                Gender = customer.Gender,
                Name = customer.Name,
                Category = customer.Category,
                DateOfBirth = customer.DateOfBirth,
                AddressLine1 = customer.Address.AddressLine1,
                HouseNumber = customer.Address.HouseNumber,
                //StateName = customer.Address.State.Name,
                //CountryRegionName = customer.Address.State.Country.Name,
                StateId = customer.Address.StateId,
                StateName = customer.Address.State.Name,
                CountryRegionId = customer.Address.State.CountryId,
                CountryRegionName = customer.Address.State.Country.Name,
                IsDeleted = customer.IsDeleted
            };
        }
    }
}