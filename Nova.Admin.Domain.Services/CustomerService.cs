#region

using System.Collections.Generic;
using Northwind.Entities.Models;
using Northwind.Repository.Models;
using Northwind.Repository.Repositories;
using Nova.Admin.Entities.Models;
using Repository.Pattern.Repositories;
using Service.Pattern;
using TrackerEnabledDbContext.Common.Models;

#endregion

namespace Northwind.Service
{
    public interface ICustomerService : IService<Customer>
    {
       



}

public class CustomerService : Service<Customer>, ICustomerService
    {
        private readonly IRepositoryAsync<Customer> _repository;

        public CustomerService(IRepositoryAsync<Customer> repository) : base(repository)
        {
            _repository = repository;
        }



        private NovaAdminContext db = new NovaAdminContext();

        public ActionResult AuditLogs_Read([DataSourceRequest] DataSourceRequest request, AuditLogViewModel customerAuditViewModel)
        {
            db.DisableAllFilters();

            var query =
                db.GetLogs<Customer>()
                    .Where(x => x.RecordId == customerAuditViewModel.RecordId)
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

        public ActionResult Customers_Read([DataSourceRequest] DataSourceRequest request)
        {
            db.DisableAllFilters();

            var customerDetailsViewModels = db.Customers.Include(customer => customer.Address.State.Country)
                .Select(customer => new CustomerViewModel
                {
                    Id = customer.Id,
                    Gender = customer.Gender,
                    Name = customer.Name,
                    Category = customer.Category,
                    DateOfBirth = customer.DateOfBirth.ToString(),
                    AddressLine1 = customer.Address.AddressLine1,
                    HouseNumber = customer.Address.HouseNumber,
                    AddressId = customer.AddressId,
                    //StateName = customer.Address.State.Name,
                    //CountryRegionName = customer.Address.State.Country.Name,
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
                using (var db = new NovaAdminContext())
                {
                    var entity = CustomerViewModelCustomerMapper.MapCustomerForCreate(customerViewModel);
                    db.Customers.Add(entity);
                    db.SaveChanges();
                    customerViewModel.Id = entity.Id;
                    customerViewModel.AddressId = entity.AddressId;
                }
            }
            return Json(new[] { customerViewModel }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Customers_Update([DataSourceRequest]DataSourceRequest request, CustomerViewModel customerViewModel)
        {
            db.DisableAllFilters();

            if (ModelState.IsValid)
            {
                using (var db = new NovaAdminContext())
                {
                    Customer entity = db.Customers.Include(customer => customer.Address).FirstOrDefault(c => c.Id == customerViewModel.Id);
                    if (entity == null)
                    {
                        string errorMessage = string.Format("Cannot update record with OrderID:{0} as it's not available.", customerViewModel.Id);
                        ModelState.AddModelError("", errorMessage);
                    }
                    else
                    {
                        CustomerViewModelCustomerMapper.MapCustomerForUpdate(customerViewModel, entity, entity.Address);
                        db.Customers.Attach(entity);
                        db.Entry(entity).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            return Json(new[] { customerViewModel }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Customers_Destroy([DataSourceRequest]DataSourceRequest request, CustomerViewModel customerViewModel)
        {
            if (ModelState.IsValid)
            {
                using (var db = new NovaAdminContext())
                {
                    var entity = db.Customers.Include(customer => customer.Address).FirstOrDefault(c => c.Id == customerViewModel.Id);
                    if (entity == null)
                    {
                        string errorMessage = string.Format("Cannot update record with OrderID:{0} as it's not available.", customerViewModel.Id);
                        ModelState.AddModelError("", errorMessage);
                    }
                    else
                    {
                        CustomerViewModelCustomerMapper.MapCustomerForUpdate(customerViewModel, entity, entity.Address);
                        db.Customers.Attach(entity);
                        db.Customers.Remove(entity);
                        db.SaveChanges();
                    }
                }
            }
            return Json(new[] { customerViewModel }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult StateCustomersTotal(string stateName, DateTime FromDate, DateTime ToDate)
        {
            // #todo IQueryable<Order> data = northwind.Orders.Where(o => o.OrderDate >= FromDate && o.OrderDate <= ToDate && o.ShipCountry == Country);

            var totalCustomersByState = (from address in db.Addresses
                                         join customer in db.Customers on address equals customer.Address
                                         join state in db.States on address.State equals state
                                         where state.Name == stateName
                                         select new
                                         {
                                             customer.Id
                                         }).Count();

            return Json(new CustomersTotal_Result { CustomersCount = totalCustomersByState }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult StateCustomersTotalForAllStates(string countryName, DateTime FromDate, DateTime ToDate)
        {
            // #todo IQueryable<Order> data = northwind.Orders.Where(o => o.OrderDate >= FromDate && o.OrderDate <= ToDate && o.ShipCountry == Country);
            // #todo Remove hardcoding of Country name
            countryName = "United States";

            var totalCustomersByAllStates = from address in db.Addresses
                                            join customer in db.Customers on address equals customer.Address
                                            join state in db.States on address.State equals state
                                            orderby state.Id
                                            group customer by state.Name
                into customersByStatesGroup
                                            select new MarketShare_Result
                                            {
                                                Area = customersByStatesGroup.Key,
                                                CustomersCount = customersByStatesGroup.Count()
                                            };

            return Json(totalCustomersByAllStates, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MarketShareByState(string stateName, DateTime FromDate, DateTime ToDate)
        {
            var totalCustomersByState = (from address in db.Addresses
                                         join customer in db.Customers on address equals customer.Address
                                         join state in db.States on address.State equals state
                                         where state.Name == stateName
                                         select customer.Id).Count();

            var totalCustomersAcrossAllStates = db.Customers.Count();

            return Json(new MarketShare_Result[]
            {
                new MarketShare_Result {Area = "All", CustomersCount = totalCustomersAcrossAllStates},
                new MarketShare_Result {Area = stateName, CustomersCount = totalCustomersByState}
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GenderShareByState(string stateName, DateTime FromDate, DateTime ToDate)
        {
            var totalGenderByState = from address in db.Addresses
                                     join customer in db.Customers on address equals customer.Address
                                     join state in db.States on address.State equals state
                                     where state.Name == stateName
                                     group customer by new { customer.Gender, state.Name } into customersByGenderGroup
                                     select new
                                     {
                                         Gender = customersByGenderGroup.Key.Gender.ToString(),
                                         CustomersCount = customersByGenderGroup.Count()
                                     };

            return Json(totalGenderByState.ToArray(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult CategoryShareByState(string stateName, DateTime FromDate, DateTime ToDate)
        {
            var totalcategoryByState = from address in db.Addresses
                                       join customer in db.Customers on address equals customer.Address
                                       join state in db.States on address.State equals state
                                       where state.Name == stateName
                                       group customer by new { customer.Category, state.Name } into customersByCategoryGroup
                                       select new CategoryShare_Result
                                       {
                                           Category = customersByCategoryGroup.Key.Category.ToString(),
                                           CustomersCount = customersByCategoryGroup.Count()
                                       };

            return Json(
                totalcategoryByState.ToArray()
            , JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCustomersForStatesAutoComplete(string text)
        {
            var db = new NovaAdminContext();

            var customers = db.States.Select(state => new LocationAutoCompleteViewModel
            {
                Id = state.Id,
                Name = state.Name
            });

            if (!string.IsNullOrEmpty(text))
            {
                customers = customers.Where(c => c.Name.StartsWith(text)).Take(5);
            }

            return Json(customers, JsonRequestBehavior.AllowGet);
        }




        // GET: Customers
        public ActionResult Index()
        {
            //var customerDetailsViewModels = new List<CustomerViewModel>();

            //var customers = await db.Customers.Include(c => c.Address.State.Country).ToListAsync();
            //foreach (var customer in customers)
            //{
            //    customerDetailsViewModels.Add(CustomerViewModelCustomerMapper.GetCustomerViewModel(customer));
            //}

            //return View(customerDetailsViewModels);

            ViewData["countries"] = GetAllCountries();
            ViewData["states"] = GetAllStates();

            return View();
        }

        public IEnumerable<CountryRegion> GetAllCountries()
        {
            return db.Countries.OrderBy(c => c.Name).ToList();
        }

        public IEnumerable<State> GetAllStates()
        {
            return db.States.OrderBy(s => s.Name).ToList();
        }

        private IQueryable<CustomerViewModel> GetAllCustomersViewModels()
        {
            return
                db.Customers.Include(c => c.Address.State.Country)
                    .Select(customer => CustomerViewModelCustomerMapper.GetCustomerViewModel(customer));
        }

        public JsonResult GetCustomersForAutoComplete(string text)
        {
            var db = new NovaAdminContext();

            var customers = db.Customers.Select(customer => new CustomerAutoCompleteViewModel
            {
                Id = customer.Id,
                Name = customer.Name
            });

            if (!string.IsNullOrEmpty(text))
            {
                customers = customers.Where(c => c.Name.StartsWith(text)).Take(5);
            }

            return Json(customers, JsonRequestBehavior.AllowGet);
        }

        // GET: Customers/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = await db.Customers.Include(c => c.Address.State.Country).FirstAsync(x => x.Id == id);
            CustomerViewModel customerDetailsViewModel =
                CustomerViewModelCustomerMapper.GetCustomerViewModel(customer);
            if (customerDetailsViewModel == null)
            {
                return HttpNotFound();
            }
            return View(customerDetailsViewModel);
        }

        // GET: Customers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,DateOfBirth,Gender,Category,HouseNumber,AddressLine1")] CustomerViewModel customerViewModel)
        {
            if (ModelState.IsValid)
            {
                db.Customers.Add(CustomerViewModelCustomerMapper.MapCustomerForCreate(customerViewModel));
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(customerViewModel);
        }

        // GET: Customers/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = await db.Customers.FindAsync(id);
            CustomerViewModel customerViewModel = CustomerViewModelCustomerMapper.GetCustomerViewModel(customer);
            if (customerViewModel == null)
            {
                return HttpNotFound();
            }
            return View(customerViewModel);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,DateOfBirth,Gender,Category,HouseNumber,AddressLine1")] CustomerViewModel customerViewModel)
        {
            if (ModelState.IsValid)
            {
                using (var db = new NovaAdminContext())
                {
                    Customer entity = await db.Customers.Include(customer => customer.Address).FirstOrDefaultAsync(c => c.Id == customerViewModel.Id);
                    if (entity == null)
                    {
                        string errorMessage = string.Format("Cannot update record with OrderID:{0} as it's not available.", customerViewModel.Id);
                        ModelState.AddModelError("", errorMessage);
                    }
                    else
                    {
                        CustomerViewModelCustomerMapper.MapCustomerForUpdate(customerViewModel, entity, entity.Address);
                        db.Customers.Attach(entity);
                        db.Entry(entity).State = EntityState.Modified;
                        db.SaveChangesAsync();
                        return RedirectToAction("Index");
                    }
                }
            }
            return View(customerViewModel);
        }

        // GET: Customers/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = await db.Customers.FindAsync(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(CustomerViewModelCustomerMapper.GetCustomerViewModel(customer));
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Customer customer = await db.Customers.FindAsync(id);
            db.Customers.Remove(customer);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
