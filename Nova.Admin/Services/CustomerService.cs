using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using EntityFramework.DynamicFilters;
using Nova.Admin.Models;
using Nova.Admin.ViewModels;
using TrackerEnabledDbContext.Common.Models;

namespace Nova.Admin.Services
{
    public class CustomerService : IDisposable
    {
        private NovaAdminContext _context = new NovaAdminContext();

        public CustomerService()
        {
            _context.DisableAllFilters();
        }

        public IQueryable<AuditLog> AuditLogs_Read(string recordId)
        {
            _context.DisableAllFilters();
            return _context.GetLogs<Customer>()
                    .Where(x => x.RecordId == recordId);
        }

        public IQueryable<Customer> GetCustomers(int skip, int take)
        {
            _context.DisableAllFilters();
            var customers = _context.Customers.Include(c => c.Address.State.Country)
                .OrderByDescending(x => x.Id);
            return customers;
        }

        public Customer GetCustomer(int id)
        {
            _context.DisableAllFilters();
            return _context.Customers.Include(c => c.Address.State.Country).FirstOrDefault(c => c.Id == id);
        }
        
        public Customer PostCustomer(Customer customer)
        {
            customer = _context.Customers.Add(customer);
            _context.SaveChanges();

            return customer;
        }

        public Customer PutCustomer(int id, Customer customer)
        {
            if (id != customer.Id)
                throw new ArgumentException("Id");

            Customer disconnectedCustomer;

            using (var context = new NovaAdminContext())
            {
                context.Configuration.ProxyCreationEnabled = false;
                disconnectedCustomer = context.Customers.Where(c => c.Id == id).Include(c => c.Address).FirstOrDefault<Customer>();
            }

            disconnectedCustomer.Gender = customer.Gender;
            disconnectedCustomer.Name = customer.Name;
            disconnectedCustomer.Category = customer.Category;
            disconnectedCustomer.DateOfBirth = customer.DateOfBirth;

            disconnectedCustomer.Address.Id = customer.AddressId;
            disconnectedCustomer.Address.AddressLine1 = customer.Address.AddressLine1;
            disconnectedCustomer.Address.HouseNumber = customer.Address.HouseNumber;
            disconnectedCustomer.Address.StateId = customer.Address.StateId;

            using (var newContext = new NovaAdminContext())
            {
                newContext.Entry(disconnectedCustomer).State = EntityState.Modified;
                newContext.Entry(disconnectedCustomer.Address).State = EntityState.Modified;
                newContext.SaveChanges();
            }

            return disconnectedCustomer;
        }

        public IQueryable<CustomerAutoCompleteViewModel> GetCustomersForAutoComplete(string text)
        {
            var customers = _context.Customers.Select(customer => new CustomerAutoCompleteViewModel
            {
                Id = customer.Id,
                Name = customer.Name
            });

            if (!string.IsNullOrEmpty(text))
            {
                customers = customers.Where(c => c.Name.StartsWith(text)).Take(5);
            }

            return customers;
        }

        public Customer DeleteCustomer(int id)
        {
            Customer disconnectedCustomer;

            using (var context = new NovaAdminContext())
            {
                context.Configuration.ProxyCreationEnabled = false;
                disconnectedCustomer = context.Customers.Where(c => c.Id == id).Include(c => c.Address.State.Country).FirstOrDefault<Customer>();
            }

            if (disconnectedCustomer == null)
                throw new ArgumentException("Id");

            using (var newContext = new NovaAdminContext())
            {
                newContext.Entry(disconnectedCustomer).State = System.Data.Entity.EntityState.Deleted;
                newContext.SaveChanges();
            }

            disconnectedCustomer.IsDeleted = true;

            return disconnectedCustomer;
        }
        
        public IEnumerable<CountryRegion> GetAllCountries()
        {
            return _context.Countries.OrderBy(c => c.Name).ToList();
        }

        public IEnumerable<State> GetAllStates()
        {
            return _context.States.OrderBy(s => s.Name).ToList();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}