using System;
using System.Text;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using Moq;
using Nova.Admin.Models;
using Nova.Admin.Services;
using Nova.Admin.ViewModels;
using NUnit.Framework;

namespace Nova.Admin.Tests.Services
{
    [TestFixture]
    public class LocationAnalyticsServicesTest
    {
        public const string _UNITED_STATES = "United States";
        public NovaAdminContext TestContext { get; set; }

        public LocationAnalyticsServices LocationAnalyticsService { get; set; }


        [SetUp]
        public void Setup()
        {
            DbConnection connection = Effort.DbConnectionFactory.CreateTransient();
            TestContext = new NovaAdminContext();
            LocationAnalyticsService = new LocationAnalyticsServices();
        }

        //public void Teardown()
        //{}

        [TestCase]
        public void CanGetCustomersTotalForAllStatesByCountry()
        {
            // Instrument
            IQueryable<MarketShare_Result> result =
                LocationAnalyticsService.CustomersTotalForAllStatesByCountry(_UNITED_STATES);

            //Verify
            var lookup = result.ToDictionary(pair => pair.Area, pair => pair.CustomersCount);
            Assert.AreEqual(lookup.Count, 2);
            Assert.AreEqual(lookup["New York"], 1);
            Assert.AreEqual(lookup["California"], 4);
        }

        [TestCase]
        public void CanGetMarketShareByStateForValidState()
        {
            // Instrument
            MarketShare_Result[] result =
                LocationAnalyticsService.MarketShareByState("California");

            //Verify
            var lookup = result.ToDictionary(pair => pair.Area, pair => pair.CustomersCount);
            Assert.AreEqual(lookup.Count, 2);
            Assert.AreEqual(lookup["All"], 6);
            Assert.AreEqual(lookup["California"], 4);
        }

        [TestCase]
        public void CanGetMarketShareByStateForState()
        {
            // Instrument
            MarketShare_Result[] result =
                LocationAnalyticsService.MarketShareByState("Eastern Cape");

            //Verify
            var lookup = result.ToDictionary(pair => pair.Area, pair => pair.CustomersCount);
            Assert.AreEqual(lookup.Count, 2);
            Assert.AreEqual(lookup["All"], 6);
            Assert.AreEqual(lookup["Eastern Cape"], 0);
        }

        [TestCase]
        public void CanGetGenderShareByState()
        {
            // Instrument
            GenderShare_Result[] result = LocationAnalyticsService.GenderShareByState("California");

            // Verify
            var lookup = result.ToDictionary(pair => pair.Gender.ToString(), pair => pair.CustomersCount);
            Assert.AreEqual(lookup.Count, 3);
            Assert.AreEqual(lookup["Unknown"], 1);
            Assert.AreEqual(lookup["Female"], 2);
            Assert.AreEqual(lookup["Male"], 1);
        }

        [TestCase]
        public void CanGetCategoryShareByState()
        {
            // Instrument
            CategoryShare_Result[] result = LocationAnalyticsService.CategoryShareByState("California");

            // Verify
            var lookup = result.ToDictionary(pair => pair.Category.ToString(), pair => pair.CustomersCount);
            Assert.AreEqual(lookup.Count, 3);
            Assert.AreEqual(lookup["CategoryA"], 1);
            Assert.AreEqual(lookup["CategoryB"], 2);
            Assert.AreEqual(lookup["CategoryC"], 1);
        }

    }
}
