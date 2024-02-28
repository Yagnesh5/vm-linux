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
    builder.EntitySet<Job>("Jobs");
    builder.EntitySet<CandidateExam>("CandidateExams"); 
    builder.EntitySet<Client>("Clients"); 
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class JobsController : ODataController
    {
        private vRecruitEntities db = new vRecruitEntities();
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        int secs = 3;
        // GET: odata/Jobs
        [EnableQuery]
        public IHttpActionResult GetJobs()
        {
            List<JobViewModel> joblst = new List<JobViewModel>();
            var jobs = db.Jobs.ToList();
            if (jobs != null)
            {
                try
                {
                    foreach (var item in jobs)
                    {
                        JobViewModel J = new JobViewModel();
                        J.JobId = item.JobId;
                        J.JobCount = item.JobCount;
                        J.JobDescription = item.JobDescription;
                        J.JobName = item.JobName;
                        if (item.JobStatus == true)
                        {
                            J.JobStatus = "Active";
                        }
                        else
                        {
                            J.JobStatus = "In-Active";
                        }
                        J.ClientName = item.Client.ClientName;
                        J.CompanyId = Convert.ToInt32(item.CompanyId);
                        J.CompanyName = db.Companies.Where(x=>x.ID==item.CompanyId).Select(x=>x.CompanyName).SingleOrDefault();
                        joblst.Add(J);
                    }
                    Success Succ = new Success() { Code = "1", Message = "LoadData", Data = joblst };
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

        // GET: odata/Jobs(5)
        [EnableQuery]
        public IHttpActionResult GetJob([FromODataUri] int key)
        {
            try
            {
                var JobData = db.Jobs.Where(job => job.JobId == key).FirstOrDefault();
                JobViewModel model = new JobViewModel();
                model.JM = new JobVM();
                //JobVM vm = new JobVM();
                if (JobData != null)
                {
                    model.JM.ClientId = JobData.ClientId;
                    model.JM.JobCount = JobData.JobCount;
                    model.JM.JobDescription = JobData.JobDescription;
                    model.JM.JobId = JobData.JobId;
                    model.JM.JobName = JobData.JobName;
                    model.JM.JobStatus =Convert.ToBoolean(JobData.JobStatus);
                    model.JM.LastUpdatedBy=JobData.LastUpdatedBy;
                    DateTime dt = Convert.ToDateTime(JobData.JobTargetDate);
                    model.JM.JobTargetDate = dt.ToString("dd-MMM-yyyy");
                    model.CompanyId =Convert.ToInt32(JobData.CompanyId);
                    model.JM.Years_of_Exp = Convert.ToInt32(JobData.Years_of_Exp);
                    model.JM.Location = JobData.Location;
                    model.JM.Contact_Person = JobData.Contact_Person;
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
        // PUT: odata/Jobs(5)
        public IHttpActionResult Put([FromODataUri] int key, JobViewModel model)
        {  
            Job job = db.Jobs.Find(key);
            if (job == null)
            {
                Log.Error("Start log ERROR...");
                Log.Error("Data not Found");
                Thread.Sleep(TimeSpan.FromSeconds(secs));
                Error Err = new Error() { Code = "0", Message = "Data not Found" };
                return new ErrorResult(Err, Request);
            }

            job.JobCount = model.JobCount;
            job.ClientId = model.JM.ClientId;
            job.JobDescription = model.JM.JobDescription;
            job.JobName = model.JM.JobName;
            job.JobStatus =Convert.ToBoolean(model.JobStatus);
            job.JobTargetDate =Convert.ToDateTime(model.JM.JobTargetDate);
            job.LastUpdatedBy = model.LastUpdatedBy;
            job.CompanyId = model.CompanyId;
            job.Years_of_Exp = model.JM.Years_of_Exp;
            job.Location = model.JM.Location;
            job.Contact_Person = model.JM.Contact_Person;
            job.LastUpdateDate = DateTime.Now.Date;
            try
            {
                db.SaveChanges();
                Success Succ = new Success() { Code = "1", Message = "Update" };
                return new SuccessResult(Succ, Request);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!JobExists(key))
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

        // POST: odata/Jobs
        public IHttpActionResult Post(JobViewModel model)
        {
            
            Job J = new Job();
            J.JobCount = model.JobCount;
            J.ClientId = model.JM.ClientId;
            J.JobDescription = model.JM.JobDescription;
            J.JobName = model.JM.JobName;
            J.JobStatus = true;
            J.JobTargetDate =Convert.ToDateTime(model.JM.JobTargetDate);
            J.LastUpdatedBy = model.LastUpdatedBy;
            J.CompanyId = model.CompanyId;
            J.Years_of_Exp = model.JM.Years_of_Exp;
            J.Location = model.JM.Location;
            J.Contact_Person = model.JM.Contact_Person;
            J.LastUpdateDate = DateTime.Now.Date;
            db.Jobs.Add(J);
            try
            {
                if (J.JobName != null && J.JobCount != null && J.JobDescription != null && J.CompanyId != null && J.Contact_Person != null && J.Years_of_Exp != null && 
                    J.Location != null && J.ClientId != null) {
                    db.SaveChanges();
                }
                Success Succ = new Success() { Code = "1", Message = "Inserted" };
                return new SuccessResult(Succ, Request);
            }
            catch (DbUpdateException ex)
            {
                if (JobExists(model.JM.JobId))
                {
                    Log.Error("Start log ERROR...");
                    Log.Error(ex.Message);
                    Thread.Sleep(TimeSpan.FromSeconds(secs));
                    Error Err = new Error() { Code = "0", Message = "Company Already exist...!!!" };
                    return new ErrorResult(Err, Request);
                }
                else
                {

                    Error Err = new Error() { Code = "0", Message = ex.Message };
                    return new ErrorResult(Err, Request);
                }
            }
            //return Created(job);
        }

        // PATCH: odata/Jobs(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<Job> patch)
        {
            //Validate(patch.GetEntity());

            Job job = db.Jobs.Find(key);
            if (job == null)
            {
                return NotFound();
            }

            var model = db.Jobs.Where(x => x.JobId == key).FirstOrDefault();
            Job J = new Job();
            J.JobCount = model.JobCount;
            J.ClientId = model.ClientId;
            J.JobDescription = model.JobDescription;
            J.JobName = model.JobName;
            J.JobStatus = model.JobStatus;
            J.JobTargetDate = model.JobTargetDate;
            J.LastUpdatedBy = model.LastUpdatedBy;
            J.CompanyId = model.CompanyId;
            J.LastUpdateDate = DateTime.Now.Date;
            db.Jobs.Add(J);
            try
            {
                db.SaveChanges();
                Success Succ = new Success() { Code = "1", Message = "Inserted" };
                return new SuccessResult(Succ, Request);
            }
            catch (DbUpdateException ex)
            {
                if (!JobExists(key))
                {
                    
                    Error Err = new Error() { Code = "0", Message = "Data dose not Exists...!!!" };
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
        }

        // DELETE: odata/Jobs(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            try
            {
                Job job = db.Jobs.Find(key);
                if (job == null)
                {
                    Error Err = new Error() { Code = "0", Message = "Data not Found" };
                    return new ErrorResult(Err, Request);
                }

                db.Jobs.Remove(job);
                db.SaveChanges();

                Success Succ = new Success() { Code = "1", Message = "Delete" };
                return new SuccessResult(Succ, Request);
            }
            catch(Exception ex)
            {
                Log.Error("Start log ERROR...");
                Log.Error(ex.Message);
                Thread.Sleep(TimeSpan.FromSeconds(secs));
                Error Err = new Error() { Code = "0", Message = ex.Message };
                return new ErrorResult(Err, Request);
            }
            
        }

        // GET: odata/Jobs(5)/CandidateExams
        [EnableQuery]
        public IQueryable<CandidateExam> GetCandidateExams([FromODataUri] int key)
        {
            return db.Jobs.Where(m => m.JobId == key).SelectMany(m => m.CandidateExams);
        }

        // GET: odata/Jobs(5)/Client
        [EnableQuery]
        public SingleResult<Client> GetClient([FromODataUri] int key)
        {
            return SingleResult.Create(db.Jobs.Where(m => m.JobId == key).Select(m => m.Client));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool JobExists(int key)
        {
            return db.Jobs.Count(e => e.JobId == key) > 0;
        }
    }
}
