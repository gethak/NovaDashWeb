using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nova.Admin.Models;
using Nova.Admin.Services;
using Nova.Admin.Tests.Contexts;
using Nova.Admin.ViewModels;
//using Nova.Admin.Tests.Contexts;
using NUnit.Framework;

namespace Nova.Admin.Tests.Services
{
    public class CustomerServicesTest
    {
        public NovaAdminContext TestContext { get; set; }

        public CustomerService CustomerService { get; set; }


        [SetUp]
        public void Setup()
        {
            DbConnection connection = Effort.DbConnectionFactory.CreateTransient();
            TestContext = new TestNovaAdminContext(connection);
            CustomerService = new CustomerService();
        }

        [TestCase]
        public void CanGetCustomers()
        {
            //Instrument
            IQueryable<Customer> query = CustomerService.GetCustomers(0, 6);

            //Verify
            var customers = query.ToList();
            Assert.AreEqual(customers.Count, 6);
        }

        [TestCase]
        public void CanGetCustomersWhenparametersOutOfBounds()
        {
            //Instrument
            IQueryable<Customer> query = CustomerService.GetCustomers(4, 6);

            //Verify
            var customers = query.ToList();
            Assert.AreEqual(customers.Count, 2);
        }

        [TestCase]
        public void AreGetCustomersOrderedByDescending()
        {
            //Instrument
            IQueryable<Customer> query = CustomerService.GetCustomers(1, 6);

            //Verify
            var customers = query.ToList();
            int maxId = customers.Max(x => x.Id);
            Assert.AreEqual(customers.Count, 5);
            Assert.AreEqual(customers[0].Id, maxId);
        }

        [TestCase]
        public void CanGetCustomersById()
        {
            //Verify
            Customer customer = CustomerService.GetCustomer(4);

            //Instrument
            var testCustomer = new Customer
            {
                Id = 4,
                Name = "Customer 4",
                DateOfBirth = DateTime.Today.AddDays(-4),
                Category = Category.CategoryB,
                Gender = Gender.Unknown,
                AddressId = 4
            };

            Assert.IsTrue(customer.EqualTo(testCustomer));
        }

        [Test]
        [ExpectedException("ArgumentException")]
        public void CantPutInvalidCustomer()
        {
            //Setup
            var testCustomer = new Customer
            {
                Id = 20,
                Name = "Customer 4",
                DateOfBirth = DateTime.Today.AddDays(-4),
                Category = Category.CategoryB,
                Gender = Gender.Unknown,
                AddressId = 4
            };

            //Instrument
            Customer customer = CustomerService.PutCustomer(testCustomer.Id, testCustomer);
        }

        public void CanDeleteCustomer(int id)
        {
            Assert.IsNotNull(CustomerService.GetCustomer(4));

            //Instrument
            Customer customer = CustomerService.DeleteCustomer(4);

            //Verify
            Assert.IsNull(CustomerService.GetCustomer(4));
        }

        [TestCase]
        public void CanGetOnlyTop5CustomersForAutoComplete()
        {
            // Instrument
            IQueryable<CustomerAutoCompleteViewModel> result =
                CustomerService.GetCustomersForAutoComplete("Cus");

            // Verify
            var customers = result.ToList();
            Assert.AreEqual(customers.Count, 5);
            Assert.IsTrue(customers.Any(x => x.Name == "Customer 5"));
            Assert.IsFalse(customers.Any(x => x.Name == "Customer 6"));
        }
    }

    public static class EntityExtensions
    {
        public static bool EqualTo(this Customer current, Customer value)
        {
            return current.Id == value.Id
                   && current.Name == value.Name
                   && current.DateOfBirth == value.DateOfBirth
                   && current.AddressId == value.AddressId
                   && current.Category == value.Category
                   && current.Gender == value.Gender
                   && current.IsDeleted == value.IsDeleted;
        }
    }
}
