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
    builder.EntitySet<NextAction>("NextActions");
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class NextActionsController : ODataController
    {
        private vRecruitEntities db = new vRecruitEntities();
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        int secs = 3;

        // GET: odata/NextActions
        [EnableQuery]
        public IQueryable<NextAction> GetNextActions()
        {
            return db.NextActions;
        }

        // GET: odata/NextActions(5)
        [EnableQuery]
        public IHttpActionResult GetNextAction([FromODataUri] int key)
        {
            try
            {
                NextActionVM model = new NextActionVM();
                var sm = db.NextActions.Find(key);
                if(sm != null)
                {
                    model = getNextActionModelFromMaster(sm);
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

        // PUT: odata/NextActions(5)
        public IHttpActionResult Put([FromODataUri] int key, NextActionVM patch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var model = db.NextActions.Where(s => s.Id == patch.Id).FirstOrDefault();
            model.Name = patch.Name;

            try
            {
                db.SaveChanges();
                Success Succ = new Success() { Code = "1", Message = "Update" };
                return new SuccessResult(Succ, Request);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!NextActionExists(key))
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

        // POST: odata/NextActions
        public IHttpActionResult Post(NextActionVM nextActionVM)
        {
            if (string.IsNullOrEmpty(nextActionVM.Name))
            {
                return BadRequest(ModelState);
            }
            var actionName = nextActionVM.Name.ToLower();
            var matchNextAction = db.NextActions.Where(s => s.Name.ToLower() == actionName).FirstOrDefault();
            if(matchNextAction != null)
            {
                Log.Error("Start log ERROR...");
                Thread.Sleep(TimeSpan.FromSeconds(secs));
                Error Err = new Error() { Code = "0", Message = "Next Action Already exist...!!!" };
                return new ErrorResult(Err, Request);
            }

            NextAction model = getNextActionFromModel(nextActionVM);
            db.NextActions.Add(model);
            try
            {
                db.SaveChanges();
                Success Succ = new Success() { Code = "1", Message = "Inserted" };
                return new SuccessResult(Succ, Request);
            }
            catch (DbUpdateException ex)
            {
                if (NextActionExists(model.Id))
                {
                    Log.Error("Start log ERROR...");
                    Log.Error(ex.Message);
                    Thread.Sleep(TimeSpan.FromSeconds(secs));
                    Error Err = new Error() { Code = "0", Message = "Template Already exist...!!!" };
                    return new ErrorResult(Err, Request);
                }
                else
                {

                    Error Err = new Error() { Code = "0", Message = ex.Message };
                    return new ErrorResult(Err, Request);
                }
            }

           
        }

        // PATCH: odata/NextActions(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<NextAction> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            NextAction nextActionMaster = db.NextActions.Find(key);
            if (nextActionMaster == null)
            {
                return NotFound();
            }

            patch.Patch(nextActionMaster);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NextActionExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(nextActionMaster);
        }

        // DELETE: odata/NextActions(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            try
            {
                NextAction nextAction = db.NextActions.Find(key);
                if (nextAction == null)
                {
                    Error Err = new Error() { Code = "0", Message = "Data not Found" };
                    return new ErrorResult(Err, Request);
                }

                db.NextActions.Remove(nextAction);
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

        private bool NextActionExists(int key)
        {
            return db.NextActions.Count(e => e.Id == key) > 0;
        }

        private NextAction getNextActionFromModel(NextActionVM model)
        {
            NextAction mst = new NextAction();
            mst.Id = model.Id;
            mst.Name = model.Name;

            return mst;
        }

        private NextActionVM getNextActionModelFromMaster(NextAction model)
        {
            NextActionVM mst = new NextActionVM();
            mst.Id = model.Id;
            mst.Name = model.Name;

            return mst;
        }
    }
}
