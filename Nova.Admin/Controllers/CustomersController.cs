using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Nova.Admin.Models;
using Nova.Admin.ViewModels;
using AutoMapper;
using EntityFramework.DynamicFilters;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Nova.Admin.Mappers;
using Nova.Admin.Services;

namespace Nova.Admin.Controllers
{
    public class CustomersController : Controller
    {
        // #todo: Extract ICustomerService
        private readonly CustomerService _customerService;

        public CustomersController()
        {
            _customerService = new CustomerService();
        }

        public CustomersController(CustomerService customerService)
        {
            _customerService = customerService;
        }

        // GET: Customers
        public ActionResult Index()
        {
            ViewData["countries"] = _customerService.GetAllCountries();
            ViewData["states"] = _customerService.GetAllStates();

            return View();
        }

        public ActionResult AuditLogs_Read([DataSourceRequest] DataSourceRequest request, AuditLogViewModel customerAuditViewModel)
        {
            var query = _customerService.AuditLogs_Read(customerAuditViewModel.RecordId)
                    .Select(auditLog => new AuditLogViewModel
                    {
                        EventDateUtc = auditLog.EventDateUTC,
                        EventType = auditLog.EventType.ToString(),
                        RecordId = auditLog.RecordId,
                        //LogDetails = auditLog.LogDetails,
                        //TypeFullName = auditLog.TypeFullName,
                        UserName = auditLog.UserName
                    });

            return Json(query.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public  ActionResult Customers_Read([DataSourceRequest] DataSourceRequest request)
        {
            var customerDetailsViewModels = _customerService.GetCustomers(request.Page, request.PageSize)
                .Select(customer => new CustomerViewModel
                {
                    Id = customer.Id,
                    Gender = customer.Gender,
                    Name = customer.Name,
                    Category = customer.Category,
                    DateOfBirth = customer.DateOfBirth,
                    AddressLine1 = customer.Address.AddressLine1,
                    HouseNumber = customer.Address.HouseNumber,
                    AddressId = customer.AddressId,
                    StateName = customer.Address.State.Name,
                    CountryRegionName = customer.Address.State.Country.Name,
                    StateId = customer.Address.StateId,
                    CountryRegionId = customer.Address.State.CountryId,
                    IsDeleted = customer.IsDeleted
                });

            return Json(customerDetailsViewModels.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Customers_Create([DataSourceRequest]DataSourceRequest request, CustomerViewModel customerViewModel)
        {
            if (ModelState.IsValid)
            {
                    Customer customer = customerViewModel.ToCustomer();
                    customer = _customerService.PostCustomer(customer);
                    customerViewModel.Id = customer.Id;
                    customerViewModel.AddressId = customer.AddressId;
            }
            return Json(new[] { customerViewModel }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Customers_Update([DataSourceRequest]DataSourceRequest request, CustomerViewModel customerViewModel)
        {
            if (ModelState.IsValid)
            {
                Customer customer = customerViewModel.ToCustomer();
                customer = _customerService.PutCustomer(customer.Id, customer);
            }
            return Json(new[] { customerViewModel }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Customers_Destroy([DataSourceRequest] DataSourceRequest request,
            CustomerViewModel customerViewModel)
        {
            if (ModelState.IsValid)
            {
                Customer customer = _customerService.DeleteCustomer(customerViewModel.Id);
                customerViewModel = customer.ToCustomerViewModel();
            }

            return Json(new[] {customerViewModel}.ToDataSourceResult(request, ModelState));
        }

        public JsonResult GetCustomersForAutoComplete(string text)
        {
            return Json(_customerService.GetCustomersForAutoComplete(text), JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _customerService.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
