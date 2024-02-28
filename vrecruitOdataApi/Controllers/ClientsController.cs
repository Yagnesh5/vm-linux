using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.OData;
using System.Web.Http.OData.Routing;
using vrecruit.DataBase.EntityDataModel;
using vrecruit.DataBase.ViewModel;
using vrecruitOdataApi.CustomModels;

namespace vrecruitOdataApi.Controllers
{
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.Http.OData.Builder;
    using System.Web.Http.OData.Extensions;
    using vrecruit.DataBase.EntityDataModel;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<Client>("Clients");
    builder.EntitySet<Customer>("Customers"); 
    builder.EntitySet<UserMapping>("UserMappings"); 
    builder.EntitySet<Company>("Companies"); 
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class ClientsController : ODataController
    {
        private vRecruitEntities db = new vRecruitEntities();
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        int secs = 3;
        // GET: odata/Clients
        [EnableQuery]
        public IHttpActionResult GetClients()
        {
            List<ClientVM> lstclient = new List<ClientVM>();
            var lst = db.Clients.ToList();
            if (lst.Count() > 0)
            {
                try
                {
                    var Countries = db.Countries.ToList();
                    foreach (var item in lst)
                    {
                        ClientVM vm = new ClientVM();
                        vm.ClientId = item.ClientId;
                        vm.ClientName = item.ClientName;
                        vm.ClientAddress = item.ClientAddress;
                        vm.ClientEmail = item.ClientEmail;
                        vm.ClientGST = item.ClientGST;
                        vm.ClientMobile = item.ClientMobile;
                        vm.ClientPhone = item.ClientPhone;
                        vm.ClientIsactive = item.IsActive.ToString();
                        vm.CompanyId = item.CompanyId;
                        vm.LastUpdateBy = vm.LastUpdateBy;
                        vm.ClientCity = item.ClientCity;
                        var country = Countries.Where(d => d.Id == item.ClientCountryId).FirstOrDefault();
                        if(country != null)
                        {
                            vm.CountryId = country.Id;
                            vm.CountryName = country.Country1;
                        }
                        vm.UserId = db.AspNetUsers.Where(x => x.ClientId == vm.ClientId).Select(x => x.Id).FirstOrDefault();
                        lstclient.Add(vm);
                    }
                    Success Succ = new Success() { Code = "1", Message = "LoadData", Data = lstclient };
                    return new SuccessResult(Succ, Request);
                }
                catch (Exception ex)
                {
                    Log.Error("Start log ERROR...");
                    Log.Error(ex.Message);
                    Thread.Sleep(TimeSpan.FromSeconds(secs));
                    Error Err = new Error() { Code = "0", Message = ex.Message };
                    return new ErrorResult(Err, Request);
                }

            }
            else
            {
                Error Err = new Error() { Code = "0", Message = "Data not Found" };
                return new ErrorResult(Err, Request);
            }
        }

        // GET: odata/Clients(5)
        [EnableQuery]
        public IHttpActionResult GetClient([FromODataUri] int key)
        {
            try
            {
                var Datas = db.Clients.Where(C => C.ClientId == key).FirstOrDefault();

                ClientVM client = new ClientVM();
                client.ClientAddress = Datas.ClientAddress;
                client.ClientEmail = Datas.ClientEmail;
                client.ClientGST = Datas.ClientGST;
                client.ClientMobile = Datas.ClientMobile;
                client.ClientName = Datas.ClientName;
                client.ClientPhone = Datas.ClientPhone;
                client.LastUpdateDate = Datas.LastUpdateDate;
                client.ClientIsactive = Datas.IsActive.ToString();
                client.ClientWWW = Datas.ClientWWW;
                client.CompanyId = Datas.CompanyId;
                client.ClientId = Datas.ClientId;
                client.Contact_Person = Datas.Contact_Person;
                client.Client_PAN_No = Datas.Client_PAN_No;
                client.UserId = db.AspNetUsers.Where(x => x.ClientId == Datas.ClientId).Select(x => x.Id).FirstOrDefault();
                client.ClientCity = Datas.ClientCity;
                var country = db.Countries.Where(d => d.Id == Datas.ClientCountryId).FirstOrDefault();
                if (country != null)
                {
                    client.CountryId = country.Id;
                    client.CountryName = country.Country1;
                }
                Success Succ = new Success() { Code = "1", Message = "Edit", Data = client };
                return new SuccessResult(Succ, Request);
            }
            catch (Exception ex)
            {
                Log.Error("Start log ERROR...");
                Log.Error(ex.Message);
                Thread.Sleep(TimeSpan.FromSeconds(secs));
                Error Err = new Error() { Code = "0", Message = ex.Message };
                return new ErrorResult(Err, Request);
            }
        }

        // PUT: odata/Clients(5)
        public IHttpActionResult Put([FromODataUri] int key, ClientVM model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Client client = db.Clients.Find(key);
            
            if (client == null)
            {
                Error Err = new Error() { Code = "0", Message = "Data not Found" };
                return new ErrorResult(Err, Request);
            }
            client.ClientName = model.ClientName;
            client.ClientAddress = model.ClientAddress;
            client.ClientEmail = model.ClientEmail;
            client.ClientGST = model.ClientGST;
            client.ClientMobile = model.ClientMobile;
            client.ClientPhone = model.ClientPhone;
            client.ClientWWW = model.ClientWWW;
            client.Client_PAN_No = model.Client_PAN_No;
            client.Contact_Person = model.Contact_Person;
            client.LastUpdateBy = model.LastUpdateBy;
            client.LastUpdateDate = DateTime.Now;
            client.CompanyId = model.CompanyId;
            client.ClientCity = model.ClientCity;
            client.ClientCountryId = model.CountryId;
            client.IsActive =Convert.ToBoolean(model.ClientIsactive);
            try
            {
                db.SaveChanges();
                AspNetUser user = db.AspNetUsers.Where(x => x.Id == model.UserId).SingleOrDefault();
                user.ClientId = client.ClientId;
                user.CompanyId = model.CompanyId;
                user.CreatedBy = model.LastUpdateBy.ToString();
                db.SaveChanges();
                Success Succ = new Success() { Code = "1", Message = "Update" };
                return new SuccessResult(Succ, Request);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ClientExists(key))
                {
                    Log.Error("Start log ERROR...");
                    Log.Error(ex.Message);
                    Thread.Sleep(TimeSpan.FromSeconds(secs));
                    Error Err = new Error() { Code = "0", Message = "Data Alredy Exists...!!!" };
                    return new ErrorResult(Err, Request);
                }
                else
                {
                    Error Err = new Error() { Code = "0", Message = ex.Message };
                    return new ErrorResult(Err, Request);
                }
            }
            //  return Updated(client);
        }

        // POST: odata/Clients
        public IHttpActionResult Post(ClientVM model)
        {
            try
            {
                Client client = new Client();
                client.ClientAddress = model.ClientAddress;
                client.ClientEmail = model.ClientEmail;
                client.ClientGST = model.ClientGST;
                client.ClientMobile = model.ClientMobile;
                client.ClientName = model.ClientName;
                client.ClientPhone = model.ClientPhone;
                client.LastUpdateDate = DateTime.Now;
                //client.ClientStatus = true;
                client.ClientWWW = model.ClientWWW;
                client.CompanyId = model.CompanyId;
                client.Contact_Person = model.Contact_Person;
                client.Client_PAN_No = model.Client_PAN_No;
                client.IsActive = true;
                client.ClientCountryId = model.CountryId;
                client.ClientCity = model.ClientCity;
                db.Clients.Add(client);
                db.SaveChanges();

                AspNetUser user = db.AspNetUsers.Where(x => x.Id == model.UserId).SingleOrDefault();
                //var v = Created(client);
                user.ClientId = client.ClientId;
                user.CompanyId = model.CompanyId;
                user.CreatedBy = model.LastUpdateBy.ToString();
                db.SaveChanges();
                Success Succ = new Success() { Code = "1", Message = "Inserted" };
                return new SuccessResult(Succ, Request);
            }
            catch (DbUpdateException ex)
            {
                if (ClientExists(model.ClientId))
                {
                    Log.Error("Start log ERROR...");
                    Log.Error(ex.Message);
                    Thread.Sleep(TimeSpan.FromSeconds(secs));
                    Error Err = new Error() { Code = "0", Message = "Client Already exist...!!!" };
                    return new ErrorResult(Err, Request);
                }
                else
                {
                    Error Err = new Error() { Code = "0", Message = ex.Message };
                    return new ErrorResult(Err, Request);
                }
            }

            // return Created(client);
        }

        // PATCH: odata/Clients(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<Client> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Client client = db.Clients.Find(key);
            if (client == null)
            {
                return NotFound();
            }

            patch.Patch(client);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(client);
        }

        // DELETE: odata/Clients(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            Client client = db.Clients.Find(key);
            if (client == null)
            {
                Error Err = new Error() { Code = "0", Message = "Data not Found" };
                return new ErrorResult(Err, Request);
            }
            AspNetUser user = db.AspNetUsers.Where(x => x.ClientId == key).SingleOrDefault();
            if (user == null)
            {
                Error Err = new Error() { Code = "0", Message = "Data not Found" };
                return new ErrorResult(Err, Request);
            }
            db.AspNetUsers.Remove(user);
            db.Clients.Remove(client);
            db.SaveChanges();

            Success Succ = new Success() { Code = "1", Message = "Delete" };
            return new SuccessResult(Succ, Request);
        }

        // GET: odata/Clients(5)/Customer
        [EnableQuery]
        public SingleResult<Customer> GetCustomer([FromODataUri] int key)
        {
            return SingleResult.Create(db.Clients.Where(m => m.ClientId == key).Select(m => m.Customer));
        }

        // GET: odata/Clients(5)/UserMappings
        [EnableQuery]
        public IQueryable<UserMapping> GetUserMappings([FromODataUri] int key)
        {
            return db.Clients.Where(m => m.ClientId == key).SelectMany(m => m.UserMappings);
        }

        // GET: odata/Clients(5)/Company
        [EnableQuery]
        public SingleResult<Company> GetCompany([FromODataUri] int key)
        {
            return SingleResult.Create(db.Clients.Where(m => m.ClientId == key).Select(m => m.Company));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ClientExists(int key)
        {
            return db.Clients.Count(e => e.ClientId == key) > 0;
        }
    }
}
