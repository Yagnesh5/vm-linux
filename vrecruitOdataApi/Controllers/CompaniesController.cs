using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.OData;
using System.Web.Http.OData.Routing;
using vrecruitOdataApi.CustomModels;
using vrecruitOdataApi.Models.ViewModel;
using vrecruit.DataBase.EntityDataModel;
using System.Threading;
using log4net;

namespace vrecruitOdataApi.Controllers
{
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.Http.OData.Builder;
    using System.Web.Http.OData.Extensions;
    using vrecruitOdataApi.Models;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<Company>("Companies");
    builder.EntitySet<AspNetUser>("AspNetUsers"); 
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class CompaniesController : ODataController
    {
        private vRecruitEntities db = new vRecruitEntities();
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        int secs = 3;
        // GET: odata/Companies
        [EnableQuery]
        public IHttpActionResult GetCompanies()
        {
            List<Companies> lstcmp = new List<Companies>();
            var cmplist = db.Companies.ToList();
            if (cmplist != null)
            {
                try
                {
                    foreach (var item in cmplist)
                    {
                        Companies vm = new Companies();
                        vm.ID = item.ID;
                        vm.CompanyName = item.CompanyName;
                        vm.CompanyEmail = item.CompanyEmail;
                        vm.CompanyAddress = item.CompanyAddress;
                        vm.City = item.City;
                        vm.Country = item.Country;
                        vm.State = item.State;
                        vm.PhoneNumber = item.PhoneNumber;
                        vm.Pincode = item.Pincode;
                        vm.UserID = db.AspNetUsers.Where(x => x.Id == item.UserID).Select(x => x.UserName).SingleOrDefault();
                        vm.IsActive =item.IsActive.ToString();
                        lstcmp.Add(vm);
                    }
                    Success Succ = new Success() { Code = "1", Message = "LoadData", Data = lstcmp };
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

        // GET: odata/Companies(5)
        [EnableQuery]
        public IHttpActionResult GetCompany(int key)
        {
            try
            {
                var cmpData = db.Companies.Where(company => company.ID == key).FirstOrDefault();
                Companies vm = new Companies();
                if (cmpData != null)
                {
                    vm.ID = cmpData.ID;
                    vm.CompanyName = cmpData.CompanyName;
                    vm.CompanyEmail = cmpData.CompanyEmail;
                    vm.CompanyAddress = cmpData.CompanyAddress;
                    vm.City = cmpData.City;
                    vm.Country = cmpData.Country;
                    vm.State = cmpData.State;
                    vm.PhoneNumber = cmpData.PhoneNumber;
                    vm.Pincode = cmpData.Pincode;
                    vm.UserID = cmpData.UserID;
                    vm.IsActive =cmpData.IsActive.ToString();
                }
                Success Succ = new Success() { Code = "1", Message = "Edit", Data = vm };
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

        // PUT: odata/Companies(5)
        public IHttpActionResult Put([FromODataUri] int key, Company model)
        {
            Company company = db.Companies.Find(key);
            if (company == null)
            {
                Error Err = new Error() { Code = "0", Message = "Data not Found" };
                return new ErrorResult(Err, Request);
            }

            company.CompanyName = model.CompanyName;
            company.CompanyEmail = model.CompanyEmail;
            company.CompanyAddress = model.CompanyAddress;
            company.City = model.City;
            company.Country = model.Country;
            company.State = model.State;
            company.PhoneNumber = model.PhoneNumber;
            company.UserID = model.UserID;
            company.Pincode = model.Pincode;
            company.LastUpdateDate = DateTime.Now;
            company.LastUpdateBy = model.LastUpdateBy;
            company.IsActive = model.IsActive;
            List<AspNetUser> user = db.AspNetUsers.Where(x => x.CompanyId == key).ToList();
            user.ForEach(a => a.IsActive = model.IsActive);
            db.SaveChanges();

            List<Client> clients = db.Clients.Where(x => x.CompanyId == key).ToList();
            clients.ForEach(a => a.IsActive = model.IsActive);
            db.SaveChanges();

            List<Question> questions = db.Questions.Where(x => x.CompanyId == key).ToList();
            questions.ForEach(a => a.IsActive = model.IsActive);
            db.SaveChanges();

            List<Candidate> candidates = db.Candidates.Where(x => x.CompanyId == key).ToList();
            candidates.ForEach(a => a.IsActive = model.IsActive);
            db.SaveChanges();

            List<Test> tests = db.Tests.Where(x => x.CompanyId == key).ToList();
            tests.ForEach(a => a.IsActive = model.IsActive);
            db.SaveChanges();

            try
            {
                db.SaveChanges();
                Success Succ = new Success() { Code = "1", Message = "Update" };
                return new SuccessResult(Succ, Request);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!CompanyExists(key))
                {
                    Error Err = new Error() { Code = "0", Message = "Data not Found" };
                    return new ErrorResult(Err, Request);
                }
                else
                {
                    Log.Error("Start log ERROR...");
                    Log.Error(ex.Message);
                    Thread.Sleep(TimeSpan.FromSeconds(secs));
                    Error Err = new Error() { Code = "0", Message = ex.Message };
                    return new ErrorResult(Err, Request);
                }
            }

            //return Updated(company);
        }

        // POST: odata/Companies
        public IHttpActionResult Post(Company company)
        {

            company.LastUpdateDate = DateTime.Now;
            company.IsActive = true;
            db.Companies.Add(company);
            try
            {
                db.SaveChanges();
                Success Succ = new Success() { Code = "1", Message = "Inserted" };
                return new SuccessResult(Succ, Request);
            }
            catch (DbUpdateException ex)
            {
                if (CompanyExists(company.ID))
                {
                    Log.Error("Start log ERROR...");
                    Log.Error(ex.Message);
                    Thread.Sleep(TimeSpan.FromSeconds(secs));
                    Error Err = new Error() { Code = "0", Message = "Company Already exist...!!!" };
                    return new ErrorResult(Err, Request);
                }
                else
                {
                    Log.Error("Start log ERROR...");
                    Log.Error(ex.Message);
                    Thread.Sleep(TimeSpan.FromSeconds(secs));
                    Error Err = new Error() { Code = "0", Message = ex.Message };
                    return new ErrorResult(Err, Request);
                }
            }

            //return Created(company);
        }

        // PATCH: odata/Companies(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<Company> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Company company = db.Companies.Find(key);
            if (company == null)
            {
                return NotFound();
            }

            patch.Patch(company);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(company);
        }

        // DELETE: odata/Companies(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            try
            {
                Company company = db.Companies.Find(key);
                if (company == null)
                {
                    Error Err = new Error() { Code = "0", Message = "Data not Found" };
                    return new ErrorResult(Err, Request);
                }
                company.IsActive = false;
                List<AspNetUser> user = db.AspNetUsers.Where(x => x.CompanyId == key).ToList();
                foreach (var item in user)
                {
                    item.IsActive = false;
                    db.SaveChanges();
                }
                List<Client> clients = db.Clients.Where(x => x.CompanyId == key).ToList();
                foreach (var item in clients)
                {
                    item.IsActive = false;
                    db.SaveChanges();
                }
                List<Question> questions = db.Questions.Where(x => x.CompanyId == key).ToList();
                foreach (var item in questions)
                {
                    item.IsActive = false;
                    db.SaveChanges();
                }
                List<Candidate> candidates = db.Candidates.Where(x => x.CompanyId == key).ToList();
                foreach (var item in candidates)
                {
                    item.IsActive = false;
                    db.SaveChanges();
                }
                List<Test> tests = db.Tests.Where(x => x.CompanyId == key).ToList();
                foreach (var item in tests)
                {
                    item.IsActive = false;
                    db.SaveChanges();
                }
                db.SaveChanges();
                Success Succ = new Success() { Code = "1", Message = "Delete" };
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

        // GET: odata/Companies(5)/AspNetUser
        [EnableQuery]
        public SingleResult<AspNetUser> GetAspNetUser([FromODataUri] int key)
        {
            return SingleResult.Create(db.Companies.Where(m => m.ID == key).Select(m => m.AspNetUser));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CompanyExists(int key)
        {
            return db.Companies.Count(e => e.ID == key) > 0;
        }
    }
}
