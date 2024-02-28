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
using vrecruitOdataApi.Models;
using vrecruit.DataBase.EntityDataModel;
using vrecruitOdataApi.Models.ViewModel;
using System.Web.Configuration;
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
    builder.EntitySet<QuestionGroup>("QuestionGroups");
    builder.EntitySet<Company>("Companies"); 
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class QuestionGroupsController : ODataController
    {
        private vRecruitEntities db = new vRecruitEntities();
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        int secs = 3;
        // GET: odata/QuestionGroups
        [EnableQuery]
        public IHttpActionResult GetQuestionGroups()
        {
            List<QuestionGroupsVM> lstQue = new List<QuestionGroupsVM>();
            //  string superadminid = WebConfigurationManager.AppSettings["SuperAdminId"].ToString(); 
            var cmplist = db.QuestionGroups.ToList();
            if (cmplist.Count() > 0)
            {
                try
                {
                    foreach (var item in cmplist)
                    {
                        QuestionGroupsVM vm = new QuestionGroupsVM();
                        vm.ID = item.ID;
                        vm.GroupName = item.GroupName;
                        if (item.CompanyId == null)
                        {
                            vm.CompanyName = "All";
                        }
                        else
                        {
                            vm.CompanyName = item.Company.CompanyName;
                        }
                        vm.CompanyId = item.CompanyId;
                        vm.LastUpdateBy = db.AspNetUsers.Where(x => x.Id == item.LastUpdateBy).Select(x => x.UserName).FirstOrDefault();
                        vm.CrntuserId = item.LastUpdateBy;
                        lstQue.Add(vm);
                    }
                    Success Succ = new Success() { Code = "1", Message = "LoadData", Data = lstQue };
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

        // GET: odata/QuestionGroups(5)
        [EnableQuery]
        public IHttpActionResult GetQuestionGroup(int key)
        {
            try
            {
                var Datas = db.QuestionGroups.Where(questionGroup => questionGroup.ID == key).FirstOrDefault();
                QuestionGroupsVM vm = new QuestionGroupsVM();
                if (Datas != null)
                {
                    vm.ID = Datas.ID;
                    vm.CompanyId = Datas.CompanyId;
                    vm.GroupName = Datas.GroupName;
                    vm.LastUpdateBy = Datas.LastUpdateBy;
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

        // PUT: odata/QuestionGroups(5)
        public IHttpActionResult Put(int key, QuestionGroup patch)
        {

            QuestionGroup questionGroup = db.QuestionGroups.Find(key);
            if (questionGroup == null)
            {
                //return NotFound();
                Error Err = new Error() { Code = "0", Message = "Data not Found" };
                return new ErrorResult(Err, Request);
            }
            //var grupdata = db.QuestionGroups.ToList();
            questionGroup.GroupName = patch.GroupName.ToUpper();
            if (patch.CompanyId != 0)
            {
                questionGroup.CompanyId = patch.CompanyId;
            }
            questionGroup.LastUpdateBy = patch.LastUpdateBy;
            try
            {
                db.SaveChanges();
                Success Succ = new Success() { Code = "1", Message = "Update" };
                return new SuccessResult(Succ, Request);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!QuestionGroupExists(key))
                {
                    Log.Error("Start log ERROR...");
                    Log.Error(ex.Message);
                    Thread.Sleep(TimeSpan.FromSeconds(secs));
                    Error Err = new Error() { Code = "0", Message = "Group Already Exists" };
                    return new ErrorResult(Err, Request);
                }
                else
                {
                    Error Err = new Error() { Code = "0", Message = ex.Message };
                    return new ErrorResult(Err, Request);
                }
            }

            //return Updated(questionGroup);
        }

        // POST: odata/QuestionGroups
        public IHttpActionResult Post(QuestionGroup questionGroup)
        {
            var result = db.QuestionGroups.ToList().FirstOrDefault(x => x.GroupName == questionGroup.GroupName);
            if (result != null)
            {
                Error Err = new Error() { Code = "0", Message = "Group Already Exists" };
                return new ErrorResult(Err, Request);
            }
            try
            {
                questionGroup.GroupName.ToUpper();
                questionGroup.LastUpdateDate = DateTime.Now;
                db.QuestionGroups.Add(questionGroup);
                db.SaveChanges();
                Success Succ = new Success() { Code = "1", Message = "Inserted" };
                return new SuccessResult(Succ, Request);
            }
            catch (DbUpdateException ex)
            {
                if (QuestionGroupExists(questionGroup.ID))
                {
                    Log.Error("Start log ERROR...");
                    Log.Error(ex.Message);
                    Thread.Sleep(TimeSpan.FromSeconds(secs));
                    Error Err = new Error() { Code = "0", Message = "Group Already exist...!!!" };
                    return new ErrorResult(Err, Request);
                }
                else
                {
                    Error Err = new Error() { Code = "0", Message = ex.Message };
                    return new ErrorResult(Err, Request);
                }
            }

            //  return Created(questionGroup);
        }

        // PATCH: odata/QuestionGroups(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<QuestionGroup> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            QuestionGroup questionGroup = db.QuestionGroups.Find(key);
            if (questionGroup == null)
            {
                return NotFound();
            }

            patch.Patch(questionGroup);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuestionGroupExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(questionGroup);
        }

        // DELETE: odata/QuestionGroups(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            QuestionGroup questionGroup = db.QuestionGroups.Find(key);
            if (questionGroup == null)
            {
                Error Err = new Error() { Code = "0", Message = "Data not Found" };
                return new ErrorResult(Err, Request);
            }
            try
            {
                List<Test> testlist = db.Tests.Where(x => x.GroupId == key).ToList();
                db.Tests.RemoveRange(testlist);
                db.QuestionGroups.Remove(questionGroup);
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

        // GET: odata/QuestionGroups(5)/Company
        [EnableQuery]
        public SingleResult<Company> GetCompany([FromODataUri] int key)
        {
            return SingleResult.Create(db.QuestionGroups.Where(m => m.ID == key).Select(m => m.Company));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool QuestionGroupExists(int key)
        {
            return db.QuestionGroups.Count(e => e.ID == key) > 0;
        }
    }
}
