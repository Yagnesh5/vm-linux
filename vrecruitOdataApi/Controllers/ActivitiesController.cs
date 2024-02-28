using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.OData;
using System.Web.Http.OData.Routing;
using System.Web.Mvc;
using System.Web.Routing;
using vrecruit.DataBase.Comman;
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
    builder.EntitySet<Activity>("Activities");
    builder.EntitySet<Candidate>("Candidates"); 
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class ActivitiesController : ODataController
    {
        private vRecruitEntities db = new vRecruitEntities();
        Comman comman = new Comman();
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        int secs = 3;

        // GET: odata/Activities
        [EnableQuery]
        public IHttpActionResult GetActivities()
        {
            List<ActivityVM> ActList = new List<ActivityVM>();
            var list = db.Activities.ToList();
            if (list.Count() > 0)
            {
                try
                {
                    foreach (var itm in list)
                    {
                        ActivityVM act = new ActivityVM();
                        act.Activity1 = itm.Activity1;
                        act.ActivityComplete = (itm.ActivityComplete).ToString();
                        DateTime dt = Convert.ToDateTime(itm.ActivityDate);
                        act.ActivityDate = dt.ToString("dd-MMM-yyyy");
                        act.ActivityId = itm.ActivityId;
                        act.ActivityRemark = itm.ActivityRemark;
                        act.ActivityType = itm.ActivityType;
                        act.CandId = Convert.ToInt32(itm.CandidateId);
                        act.ClientId = Convert.ToInt32(itm.ClientId);
                        act.JobId = Convert.ToInt32(itm.JobId);
                        act.UserId = itm.UserId.ToString();
                        act.JobName = db.Jobs.Where(j => j.JobId == itm.JobId).Select(x => x.JobName).SingleOrDefault();
                        ActList.Add(act);
                    }
                    Success Succ = new Success() { Code = "1", Message = "Activitylist", Data = ActList };
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
        // GET: odata/Activities(5)
        [EnableQuery]
        public IHttpActionResult GetActivity([FromODataUri] int key)
        {
            var ActivityData = db.Activities.Where(activity => activity.ActivityId == key).SingleOrDefault();
            if (ActivityData != null)
            {
                try
                {
                    ActivityVM act = new ActivityVM();
                    act.Activity1 = ActivityData.Activity1;
                    act.ActivityComplete = (ActivityData.ActivityComplete).ToString();
                    DateTime dt = Convert.ToDateTime(ActivityData.ActivityDate);
                    act.ActivityDate = dt.ToString("dd-MMM-yyyy");
                    act.ActivityId = ActivityData.ActivityId;
                    act.ActivityRemark = ActivityData.ActivityRemark;
                    act.ActivityType = ActivityData.ActivityType.Trim();
                    act.CandId = Convert.ToInt32(ActivityData.CandidateId);
                    act.ClientId = Convert.ToInt32(ActivityData.ClientId);
                    act.JobId = Convert.ToInt32(ActivityData.JobId);
                    act.UserId = ActivityData.UserId.ToString();
                    act.JobName = db.Jobs.Where(j => j.JobId == ActivityData.JobId).Select(x => x.JobName).SingleOrDefault();
                    act.CompanyId = Convert.ToInt32(ActivityData.Companyid);
                    act.CompanyName = ActivityData.Company.CompanyName;
                    act.ActivityTypeList = db.ActivityTypes.ToList();
                    Success Succ = new Success() { Code = "1", Message = "Activity", Data = act };
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

        // PUT: odata/Activities(5)
        public IHttpActionResult Put([FromODataUri] int key, Activity patch)
        {

            Activity activity = db.Activities.Find(key);
            if (activity == null)
            {
                Error Err = new Error() { Code = "0", Message = "Activity not Found" };
                return new ErrorResult(Err, Request);
            }
            activity.ActivityType = patch.ActivityType;
            activity.Activity1 = patch.Activity1;
            activity.ActivityDate = patch.ActivityDate;
            activity.ActivityRemark = patch.ActivityRemark;
            activity.ActivityComplete = patch.ActivityComplete;
            try
            {
                db.SaveChanges();
                Success Succ = new Success() { Code = "1", Message = "ActivityUpdate" };
                return new SuccessResult(Succ, Request);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ActivityExists(key))
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

        // POST: odata/Activities
        public IHttpActionResult Post(ActivityVM model)
        {
            if (model.ActivityType != "Reassign" && model.ActivityType != "INTRO VIDEO")
            {
                var Exist = db.Activities.Where(x => x.CandidateId == model.CandId && x.ClientId == model.ClientId && x.ActivityType == "ASSIGN").FirstOrDefault();
                if (Exist != null)
                {
                    Error Err = new Error() { Code = "0", Message = "Activity already assigned...!!!" };
                    return new ErrorResult(Err, Request);
                }
            }

            Activity activity = new Activity();
            var CandidateData = (from can in db.Candidates where can.CandidateId == model.CandId select can).SingleOrDefault();
            var UserEmail = (from user in db.AspNetUsers where user.Id == model.UserId select user.Email).SingleOrDefault();
            Job jobData = db.Jobs.Where(x => x.JobId == model.JobId).SingleOrDefault();
            var body = "";
            model.ActivityDate = DateTime.Now.ToString("dd-MMM-yyyy");
            model.UserEmail = UserEmail;
            model.CandidateName = CandidateData.CandidateName;
            if (model.ActivityType == "TEST" || model.ActivityType == "Reassign")
            {
                var guild = this.CandidateURLStatus(CandidateData.CandidateId);
                string Uemail = UserEmail;
                string Cemail = CandidateData.CandidateEmail;
                string url = WebConfigurationManager.AppSettings["TestLink"].ToString() + model.TestLink + "&email=" + Uemail + "&useremail=" + Cemail + "&CanId=" + model.CandId + "&e=" + guild;
                model.TestLink = url;
            }

            if (model.ActivityType == "INTRO VIDEO")
            {
                var guild = this.CandidateURLStatus(CandidateData.CandidateId);
                string Uemail = UserEmail;
                string Cemail = CandidateData.CandidateEmail;
                string url = WebConfigurationManager.AppSettings["VideoLink"].ToString() + model.TestLink + "useremail=" + Cemail + "&CanId=" + model.CandId + "&e=" + guild;
                model.TestLink = url;
            }

            if (model.ActivityType == "ONLINE INTERVIEW")
            {
                var guild = this.CandidateURLStatus(CandidateData.CandidateId);
                string Uemail = UserEmail;
                string Cemail = CandidateData.CandidateEmail;
                string url = WebConfigurationManager.AppSettings["InterviewLink"].ToString() + model.TestLink + "&email=" + Uemail + "&useremail=" + Cemail + "&CanId=" + model.CandId + "&e=" + guild;
                model.TestLink = url;
            }

            model.JobDescription = jobData.JobDescription;
            model.JobCount = Convert.ToInt32(jobData.JobCount);
            model.JobLocation = jobData.Location;
            model.ContactPerson = jobData.Contact_Person;
            EmailViewController EmailViewController = new EmailViewController();
            if (model.ActivityType == "INTRO VIDEO")
            {
                body = EmailViewController.EmailTemplate("~/Views/EmailView/VideoEmailTemplate.cshtml", model);
            }
            else
            {
                body = EmailViewController.EmailTemplate("~/Views/EmailView/_EmailTemplate.cshtml", model);
            }
            EmailViewModels Emodels = new EmailViewModels();
            Emodels.Smtpemail = WebConfigurationManager.AppSettings["EmailId"].ToString();
            Emodels.Smtppassword = WebConfigurationManager.AppSettings["Password"].ToString();
            Emodels.Smtp = WebConfigurationManager.AppSettings["Smtp"].ToString();
            Emodels.Port = Convert.ToInt32(WebConfigurationManager.AppSettings["Port"]);
            Emodels.Subject = jobData.JobName;
            Emodels.body = body;
            if (CandidateData != null)
            {
                try
                {
                    if (model.ActivityType == "TEST" || model.ActivityType == "Reassign" || model.ActivityType == "INTRO VIDEO" || model.ActivityType == "ONLINE INTERVIEW")
                    {
                        Emodels.to = CandidateData.CandidateEmail;
                        bool sendmail = comman.SendEmail(Emodels);
                        if (sendmail == false)
                        {
                            Error Err = new Error() { Code = "0", Message = "Email delivery failed" };
                            return new ErrorResult(Err, Request);
                        }
                    }
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
            if (UserEmail != null)
            {
                try
                {
                    Emodels.to = UserEmail;
                    bool sendmail = comman.SendEmail(Emodels);
                    if (sendmail == false)
                    {
                        Error Err = new Error() { Code = "0", Message = "Email delivery failed" };
                        return new ErrorResult(Err, Request);
                    }
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
            activity.Activity1 = model.Activity1;
            if (model.ActivityType == "Reassign")
            {
                activity.ActivityType = "REASSIGN";
            }
            else
            {
                activity.ActivityType = model.ActivityType;
            }
            activity.ActivityDate = DateTime.Now;
            activity.ActivityComplete = false;
            activity.CandidateId = model.CandId;
            activity.JobId = model.JobId;
            activity.UserId = new Guid(model.UserId);
            activity.ClientId = model.ClientId;
            activity.Companyid = model.CompanyId;
            db.Activities.Add(activity);
            if (model.ActivityType == "TEST" || model.ActivityType == "Reassign")
            {
                Activity testactivity = new Activity();
                testactivity.Activity1 = model.Activity1;
                testactivity.ActivityType = model.ActivityType;
                testactivity.ActivityDate = DateTime.Now;
                testactivity.ActivityComplete = false;
                testactivity.CandidateId = model.CandId;
                testactivity.JobId = model.JobId;
                testactivity.UserId = new Guid(model.UserId);
                testactivity.ClientId = model.ClientId;
                testactivity.Companyid = model.CompanyId;
                db.Activities.Add(testactivity);
            }
            try
            {
                db.SaveChanges();
                Success Succ = new Success() { Code = "1", Message = "Send" };
                return new SuccessResult(Succ, Request);
            }
            catch (DbUpdateException ex)
            {
                if (ActivityExists(activity.ActivityId))
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

            // return Created(activity);
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.Route("[action]")]
        // POST: odata/Reassign
        public IHttpActionResult ReAssign(ActivityVM model)
        {

            var Exist = db.Activities.Where(x => x.CandidateId == model.CandId && x.ClientId == model.ClientId && x.ActivityType == "ASSIGN").FirstOrDefault();
            if (Exist != null)
            {
                Error Err = new Error() { Code = "0", Message = "job already assigned...!!!" };
                return new ErrorResult(Err, Request);
            }
            Activity activity = new Activity();
            var CandidateData = (from can in db.Candidates where can.CandidateId == model.CandId select can).SingleOrDefault();
            var UserEmail = (from user in db.AspNetUsers where user.Id == model.UserId select user.Email).SingleOrDefault();
            Job jobData = db.Jobs.Where(x => x.JobId == model.JobId).SingleOrDefault();
            var body = "";
            model.ActivityDate = DateTime.Now.ToString("dd-MMM-yyyy");
            model.UserEmail = UserEmail;
            model.CandidateName = CandidateData.CandidateName;
            if (model.ActivityType == "TEST")
            {
                var guild = this.CandidateURLStatus(CandidateData.CandidateId);
                string Uemail = UserEmail;
                string Cemail = CandidateData.CandidateEmail;
                string url = WebConfigurationManager.AppSettings["TestLink"].ToString() + model.TestLink + "&email=" + Uemail + "&useremail=" + Cemail + "&CanId=" + model.CandId + "&e=" + guild;
                model.TestLink = url;
            }
            model.JobDescription = jobData.JobDescription;
            model.JobCount = Convert.ToInt32(jobData.JobCount);
            model.JobLocation = jobData.Location;
            model.ContactPerson = jobData.Contact_Person;
            EmailViewController EmailViewController = new EmailViewController();
            body = EmailViewController.EmailTemplate("~/Views/EmailView/_EmailTemplate.cshtml", model);
            EmailViewModels Emodels = new EmailViewModels();
            Emodels.Smtpemail = WebConfigurationManager.AppSettings["EmailId"].ToString();
            Emodels.Smtppassword = WebConfigurationManager.AppSettings["Password"].ToString();
            Emodels.Smtp = WebConfigurationManager.AppSettings["Smtp"].ToString();
            Emodels.Port = Convert.ToInt32(WebConfigurationManager.AppSettings["Port"]);
            Emodels.Subject = jobData.JobName;
            Emodels.body = body;
            if (CandidateData != null)
            {
                try
                {
                    Emodels.to = CandidateData.CandidateEmail;
                    bool sendmail = comman.SendEmail(Emodels);
                    if (sendmail == false)
                    {
                        Error Err = new Error() { Code = "0", Message = "Email delivery failed" };
                        return new ErrorResult(Err, Request);
                    }
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
            if (UserEmail != null)
            {
                try
                {
                    Emodels.to = UserEmail;
                    bool sendmail = comman.SendEmail(Emodels);
                    if (sendmail == false)
                    {
                        Error Err = new Error() { Code = "0", Message = "Email delivery failed" };
                        return new ErrorResult(Err, Request);
                    }
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
            activity.Activity1 = model.Activity1;
            activity.ActivityType = "ASSIGN";
            activity.ActivityDate = DateTime.Now;
            activity.ActivityComplete = false;
            activity.CandidateId = model.CandId;
            activity.JobId = model.JobId;
            activity.UserId = new Guid(model.UserId);
            activity.ClientId = model.ClientId;
            activity.Companyid = model.CompanyId;
            db.Activities.Add(activity);
            if (model.ActivityType == "TEST")
            {
                Activity testactivity = new Activity();
                testactivity.Activity1 = model.Activity1;
                testactivity.ActivityType = model.ActivityType;
                testactivity.ActivityDate = DateTime.Now;
                testactivity.ActivityComplete = false;
                testactivity.CandidateId = model.CandId;
                testactivity.JobId = model.JobId;
                testactivity.UserId = new Guid(model.UserId);
                testactivity.ClientId = model.ClientId;
                testactivity.Companyid = model.CompanyId;
                db.Activities.Add(testactivity);
            }
            try
            {
                db.SaveChanges();
                Success Succ = new Success() { Code = "1", Message = "Send" };
                return new SuccessResult(Succ, Request);
            }
            catch (DbUpdateException ex)
            {
                if (ActivityExists(activity.ActivityId))
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

            // return Created(activity);
        }

        // PATCH: odata/Activities(5)
        //[AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<Activity> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Activity activity = db.Activities.Find(key);
            if (activity == null)
            {
                return NotFound();
            }

            patch.Patch(activity);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActivityExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(activity);
        }

        // DELETE: odata/Activities(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            Activity activity = db.Activities.Find(key);
            if (activity == null)
            {
                return NotFound();
            }

            db.Activities.Remove(activity);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/Activities(5)/Candidate
        [EnableQuery]
        public SingleResult<Candidate> GetCandidate([FromODataUri] int key)
        {
            return SingleResult.Create(db.Activities.Where(m => m.ActivityId == key).Select(m => m.Candidate));
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ActivityExists(int key)
        {
            return db.Activities.Count(e => e.ActivityId == key) > 0;
        }

        private string CandidateURLStatus(int candidateID)
        {
            string CAND_GUID = string.Empty;
            try
            {
                CandidateTestURL url = new CandidateTestURL();
                url.CandId = candidateID;
                url.TestURLID = Guid.NewGuid();
                CAND_GUID = url.TestURLID.ToString();
                url.Status = false;
                db.CandidateTestURLs.Add(url);
                db.SaveChanges();
                return CAND_GUID;
            }
            catch (Exception)
            {

                return CAND_GUID;
            }
        }
    }

    public class FakeController : ControllerBase { protected override void ExecuteCore() { } }
}
