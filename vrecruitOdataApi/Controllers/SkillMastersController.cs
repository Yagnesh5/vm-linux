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
    public class SkillMastersController : ODataController
    {
        private vRecruitEntities db = new vRecruitEntities();
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        int secs = 3;

        // GET: odata/SkillMasters
        [EnableQuery]
        public IQueryable<SkillMaster> GetSkillMasters()
        {
            return db.SkillMasters;
        }

        // GET: odata/SkillMasters(5)
        [EnableQuery]
        //public SingleResult<SkillMaster> GetSkillMaster([FromODataUri] int key)
        public IHttpActionResult GetSkillMaster([FromODataUri] int key)
        {
            try
            {
                SkillMasterModel model = new SkillMasterModel();
                var sm = db.SkillMasters.Find(key);
                if(sm != null)
                {
                    model = getSkillModelFromMaster(sm);
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
            //return SingleResult.Create(db.SkillMasters.Where(skillMaster => skillMaster.SkillId == key));
        }

        // PUT: odata/SkillMasters(5)
        public IHttpActionResult Put([FromODataUri] int key, SkillMasterModel patch)
        {
           // Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var model = db.SkillMasters.Where(s => s.SkillId == patch.SkillId).FirstOrDefault();
            model.SkillName = patch.SkillName;
            model.Active = patch.Active;
            model.LastUpdateDate = DateTime.Now.Date;

            try
            {
                db.SaveChanges();
                Success Succ = new Success() { Code = "1", Message = "Update" };
                return new SuccessResult(Succ, Request);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!SkillMasterExists(key))
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

            // SkillMaster skillMaster = db.SkillMasters.Find(key);
            // if (skillMaster == null)
            // {
            //     return NotFound();
            // }

            //// patch.Put(skillMaster);

            // try
            // {
            //     db.SaveChanges();
            // }
            // catch (DbUpdateConcurrencyException)
            // {
            //     if (!SkillMasterExists(key))
            //     {
            //         return NotFound();
            //     }
            //     else
            //     {
            //         throw;
            //     }
            // }

            // return Updated(skillMaster);
        }

        // POST: odata/SkillMasters
        public IHttpActionResult Post(SkillMasterModel skillMaster)
        {
            //skillMaster.SkillId = 1;
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            if (string.IsNullOrEmpty(skillMaster.SkillName))
            {
                return BadRequest(ModelState);
            }
            var skillname = skillMaster.SkillName.ToLower();
            var matchSkill = db.SkillMasters.Where(s => s.SkillName.ToLower() == skillname).FirstOrDefault();
            if(matchSkill != null)
            {
                Log.Error("Start log ERROR...");
                Thread.Sleep(TimeSpan.FromSeconds(secs));
                Error Err = new Error() { Code = "0", Message = "Skill Already exist...!!!" };
                return new ErrorResult(Err, Request);
            }

            SkillMaster model = getSkillMasterFromModel(skillMaster);
            db.SkillMasters.Add(model);
            try
            {
                db.SaveChanges();
                Success Succ = new Success() { Code = "1", Message = "Inserted" };
                return new SuccessResult(Succ, Request);
            }
            catch (DbUpdateException ex)
            {
                if (SkillMasterExists(model.SkillId))
                {
                    Log.Error("Start log ERROR...");
                    Log.Error(ex.Message);
                    Thread.Sleep(TimeSpan.FromSeconds(secs));
                    Error Err = new Error() { Code = "0", Message = "Skill Already exist...!!!" };
                    return new ErrorResult(Err, Request);
                }
                else
                {

                    Error Err = new Error() { Code = "0", Message = ex.Message };
                    return new ErrorResult(Err, Request);
                }
            }

           
        }

        // PATCH: odata/SkillMasters(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<SkillMaster> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            SkillMaster skillMaster = db.SkillMasters.Find(key);
            if (skillMaster == null)
            {
                return NotFound();
            }

            patch.Patch(skillMaster);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SkillMasterExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(skillMaster);
        }

        // DELETE: odata/SkillMasters(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            try
            {
                SkillMaster skillMaster = db.SkillMasters.Find(key);
                if (skillMaster == null)
                {
                    Error Err = new Error() { Code = "0", Message = "Data not Found" };
                    return new ErrorResult(Err, Request);
                }

                db.SkillMasters.Remove(skillMaster);
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

        private bool SkillMasterExists(int key)
        {
            return db.SkillMasters.Count(e => e.SkillId == key) > 0;
        }

        private SkillMaster getSkillMasterFromModel(SkillMasterModel model)
        {
            SkillMaster mst = new SkillMaster();
            mst.SkillId = model.SkillId;
            mst.SkillName = model.SkillName;
            mst.Active = model.Active;
            mst.LastUpdateDate = DateTime.Now.Date;

            return mst;
        }

        private SkillMasterModel getSkillModelFromMaster(SkillMaster model)
        {
            SkillMasterModel mst = new SkillMasterModel();
            mst.SkillId = model.SkillId;
            mst.SkillName = model.SkillName;
            mst.Active = model.Active;
            mst.LastUpdateDate = DateTime.Now.Date;
            mst.LastUpdateDateString = mst.LastUpdateDate != null ? mst.LastUpdateDate.Value.ToString("dd/MM/yy") : "";

            return mst;
        }
    }
}
