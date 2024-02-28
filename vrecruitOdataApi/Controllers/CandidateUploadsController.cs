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

namespace vrecruitOdataApi.Controllers
{
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.Http.OData.Builder;
    using System.Web.Http.OData.Extensions;
    using vrecruit.DataBase.EntityDataModel;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<CandidateUpload>("CandidateUploads");
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class CandidateUploadsController : ODataController
    {
        private vRecruitEntities db = new vRecruitEntities();

        // GET: odata/CandidateUploads
        [EnableQuery]
        public IQueryable<CandidateUpload> GetCandidateUploads()
        {
            return db.CandidateUploads;
        }

        // GET: odata/CandidateUploads(5)
        [EnableQuery]
        public SingleResult<CandidateUpload> GetCandidateUpload([FromODataUri] int key)
        {
            return SingleResult.Create(db.CandidateUploads.Where(candidateUpload => candidateUpload.Id == key));
        }

        // PUT: odata/CandidateUploads(5)
        public IHttpActionResult Put([FromODataUri] int key, Delta<CandidateUpload> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CandidateUpload candidateUpload = db.CandidateUploads.Find(key);
            if (candidateUpload == null)
            {
                return NotFound();
            }

            patch.Put(candidateUpload);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CandidateUploadExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(candidateUpload);
        }

        // POST: odata/CandidateUploads
        public IHttpActionResult Post(CandidateUpload candidateUpload)
        {
            //CandidateUpload candidate1 = db.CandidateUploads.Where(c => c.Email == candidateUpload.Email).FirstOrDefault();
            //if (candidate1 != null)
            //{
            //    Error Err = new Error() { Code = "0", Message = "User is available with the same email id." };
            //    return new vrecruitOdataApi.CustomModels.ErrorResult(Err, Request);
            //}

            //CandidateUpload candidate = new CandidateUpload();
            //candidate.Name = candidateUpload.Name;
            //candidate.Email = candidateUpload.Email;
            //candidate.PhoneNumber = candidateUpload.PhoneNumber;
            //candidate.Age = candidateUpload.Age;
            //candidate.Qualification = candidateUpload.Qualification;
            //candidate.Location = candidateUpload.Location;
            //candidate.Company = candidateUpload.Company;
            //candidate.Project = candidateUpload.Project;
            //candidate.Technology = candidateUpload.Technology;
            //candidate.Achievement = candidateUpload.Achievement;
            //db.CandidateUploads.Add(candidate);
            //try
            //{
            //    db.SaveChanges();

            //    CandidateUpload data = db.CandidateUploads.Find(candidateUpload.Id);

            //    // data.CandidateResume = "Candidate" + "_" + candidate.CandidateId + "_" + model.CandidateResume;
            //    data.CandidateResume = AddCandidateResume(model, candidate);
            //    db.SaveChanges();
            //    Success Succ = new Success() { Code = "1", Message = "Inserted", Data = candidate.CandidateId };
            //    return new SuccessResult(Succ, Request);
            //}
            //catch (DbUpdateException ex)
            //{
            //    if (CandidateExists(candidate.CandidateId))
            //    {
            //        Log.Error("Start log ERROR...");
            //        Log.Error(ex.Message);
            //        Thread.Sleep(TimeSpan.FromSeconds(secs));
            //        Error Err = new Error() { Code = "0", Message = "Candidate Already exist...!!!" };
            //        return new ErrorResult(Err, Request);
            //    }
            //    else
            //    {
            //        Log.Error("Start log ERROR...");
            //        Log.Error(ex.Message);
            //        Thread.Sleep(TimeSpan.FromSeconds(secs));
            //        Error Err = new Error() { Code = "0", Message = ex.Message };
            //        return new ErrorResult(Err, Request);
            //    }

            //}
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.CandidateUploads.Add(candidateUpload);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (CandidateUploadExists(candidateUpload.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Created(candidateUpload);
        }

        // PATCH: odata/CandidateUploads(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<CandidateUpload> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CandidateUpload candidateUpload = db.CandidateUploads.Find(key);
            if (candidateUpload == null)
            {
                return NotFound();
            }

            patch.Patch(candidateUpload);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CandidateUploadExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(candidateUpload);
        }

        // DELETE: odata/CandidateUploads(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            CandidateUpload candidateUpload = db.CandidateUploads.Find(key);
            if (candidateUpload == null)
            {
                return NotFound();
            }

            db.CandidateUploads.Remove(candidateUpload);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CandidateUploadExists(int key)
        {
            return db.CandidateUploads.Count(e => e.Id == key) > 0;
        }
    }
}
