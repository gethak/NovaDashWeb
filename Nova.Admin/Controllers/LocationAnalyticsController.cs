using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using Nova.Admin.Models;
using Nova.Admin.Services;

namespace Nova.Admin.Api
{
    public class LocationAnalyticsController : Controller
    {
        private const string _UNITED_STATES = "United States";

        // #todo: Extract ILocationAnalyticsServices
        private readonly LocationAnalyticsServices _locationService;

        public LocationAnalyticsController()
        {
            
            _locationService = new LocationAnalyticsServices(); 
        }
        public LocationAnalyticsController(LocationAnalyticsServices locationService)
        {
            _locationService = locationService;
        }

        public ActionResult StateCustomersTotal(string stateName, DateTime FromDate, DateTime ToDate)
        {
            var totalCustomersByState = _locationService.StateCustomersTotal(stateName);

            return Json(new CustomersTotal_Result { CustomersCount = totalCustomersByState });
        }

        public ActionResult CustomersTotalForAllStatesByCountry(string countryName, DateTime FromDate, DateTime ToDate)
        {
            countryName = _UNITED_STATES;
            var totalCustomersByAllStates = _locationService.CustomersTotalForAllStatesByCountry(countryName);

            return Json(totalCustomersByAllStates, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MarketShareByState(string stateName, DateTime FromDate, DateTime ToDate)
        {
            return Json(_locationService.MarketShareByState(stateName), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GenderShareByState(string stateName, DateTime FromDate, DateTime ToDate)
        {
            return Json(_locationService.GenderShareByState(stateName), JsonRequestBehavior.AllowGet);
        }

        public ActionResult CategoryShareByState(string stateName, DateTime FromDate, DateTime ToDate)
        {
            return Json(_locationService.CategoryShareByState(stateName), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStatesForAutoComplete(string text)
        {
            return Json(_locationService.GetStatesForAutoComplete(text), JsonRequestBehavior.AllowGet);
        }
    }
}
