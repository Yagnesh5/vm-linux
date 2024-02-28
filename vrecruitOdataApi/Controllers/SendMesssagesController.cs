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
    builder.EntitySet<SkillMaster>("SkillMasters");
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class SendMesssagesController : ODataController
    {
        private vRecruitEntities db = new vRecruitEntities();
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        int secs = 3;

        // GET: odata/SendMesssages
        [EnableQuery]
        public IQueryable<SendMesssage> GetSendMesssages()
        {
            return db.SendMesssages;
        }

        // GET: odata/SendMesssages(5)
        [EnableQuery]
        public IHttpActionResult GetSendMesssage([FromODataUri] int key)
        {
            try
            {
                SendMesssageVM model = new SendMesssageVM();
                var sm = db.SendMesssages.Find(key);
                if (sm != null)
                {
                    model = getSendMessageFromMaster(sm);
                }
                Success Succ = new Success() { Code = "1", Message = "Edit", Data = model };
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

        // PUT: odata/SendMesssages(5)
        public IHttpActionResult Put([FromODataUri] int key, SendMesssageVM patch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var model = db.SendMesssages.Where(s => s.Id == patch.Id).FirstOrDefault();
            model.Name = patch.Name;

            try
            {
                db.SaveChanges();
                Success Succ = new Success() { Code = "1", Message = "Update" };
                return new SuccessResult(Succ, Request);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!SendMessageExists(key))
                {
                    Log.Error("Start log ERROR...");
                    Log.Error(ex.Message);
                    Thread.Sleep(TimeSpan.FromSeconds(secs));
                    Error Err = new Error() { Code = "0", Message = "Data not Found" };
                    return new ErrorResult(Err, Request);
                }
                else
                {
                    Error Err = new Error() { Code = "0", Message = ex.Message };
                    return new ErrorResult(Err, Request);
                }
            }
        }

        // POST: odata/SendMesssages
        public IHttpActionResult Post(SendMesssageVM sendMesssageVM)
        { 
            if (string.IsNullOrEmpty(sendMesssageVM.Name))
            {
                return BadRequest(ModelState);
            }
            var sendMessageName = sendMesssageVM.Name.ToLower();
            var matchMessage = db.SendMesssages.Where(s => s.Name.ToLower() == sendMessageName).FirstOrDefault();
            if (matchMessage != null)
            {
                Log.Error("Start log ERROR...");
                Thread.Sleep(TimeSpan.FromSeconds(secs));
                Error Err = new Error() { Code = "0", Message = "Message Already exist...!!!" };
                return new ErrorResult(Err, Request);
            }

            SendMesssage model = getSendMessageFromModel(sendMesssageVM);
            db.SendMesssages.Add(model);
            try
            {
                db.SaveChanges();
                Success Succ = new Success() { Code = "1", Message = "Inserted" };
                return new SuccessResult(Succ, Request);
            }
            catch (DbUpdateException ex)
            {
                if (SendMessageExists(model.Id))
                {
                    Log.Error("Start log ERROR...");
                    Log.Error(ex.Message);
                    Thread.Sleep(TimeSpan.FromSeconds(secs));
                    Error Err = new Error() { Code = "0", Message = "Message Already exist...!!!" };
                    return new ErrorResult(Err, Request);
                }
                else
                {

                    Error Err = new Error() { Code = "0", Message = ex.Message };
                    return new ErrorResult(Err, Request);
                }
            }


        }

        // PATCH: odata/SendMesssages(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<SendMesssage> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            SendMesssage sendMessage = db.SendMesssages.Find(key);
            if (sendMessage == null)
            {
                return NotFound();
            }

            patch.Patch(sendMessage);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SendMessageExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(sendMessage);
        }

        // DELETE: odata/SendMesssages(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            try
            {
                SendMesssage sendMessage = db.SendMesssages.Find(key);
                if (sendMessage == null)
                {
                    Error Err = new Error() { Code = "0", Message = "Data not Found" };
                    return new ErrorResult(Err, Request);
                }

                db.SendMesssages.Remove(sendMessage);
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SendMessageExists(int key)
        {
            return db.SendMesssages.Count(e => e.Id == key) > 0;
        }

        private SendMesssage getSendMessageFromModel(SendMesssageVM model)
        {
            SendMesssage mst = new SendMesssage();
            mst.Id = model.Id;
            mst.Name = model.Name;
            return mst;
        }

        private SendMesssageVM getSendMessageFromMaster(SendMesssage model)
        {
            SendMesssageVM mst = new SendMesssageVM();
            mst.Id = model.Id;
            mst.Name = model.Name;
            return mst;
        }
    }
}
