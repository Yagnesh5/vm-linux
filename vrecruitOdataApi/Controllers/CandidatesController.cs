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
    builder.EntitySet<Candidate>("Candidates");
    builder.EntitySet<Activity>("Activities"); 
    builder.EntitySet<Customer>("Customers"); 
    builder.EntitySet<CandidateExam>("CandidateExams"); 
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class CandidatesController : ODataController
    {
        private vRecruitEntities db = new vRecruitEntities();
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        int secs = 3;
        // GET: odata/Candidates
        [EnableQuery]
        public IHttpActionResult GetCandidates()
        {
            //return db.Candidates;
            List<CandidateVM> canList = new List<CandidateVM>();
            var list = db.Candidates.Where(x => x.IsActive == true).ToList();
            if (list.Count() > 0)
            {
                try
                {
                    foreach (var item in list)
                    {
                        CandidateVM candidate = new CandidateVM();
                        candidate.ActivityList = new List<ActivityVM>();
                        DateTime dobdate = Convert.ToDateTime(item.CandidateDOB);
                        candidate.CandidateDOB = dobdate.ToString("dd-MMM-yyyy");
                        candidate.CandidateEmail = item.CandidateEmail;
                        candidate.CandidateLastJobChange = item.CandidateLastJobChange;
                        candidate.PreviousDesignation = item.PreviousDesignation;
                        candidate.CandidateLocation = item.CandidateLocation;
                        candidate.CandidateName = item.CandidateName;
                        candidate.CurrentOrganisation = item.CurrentOrganisation;
                        candidate.CandidatePhone = item.CandidatePhone;
                        candidate.CandidatePhone2 = item.CandidatePhone2;
                        candidate.CandidateResume = item.CandidateResume;
                        candidate.CandidateSkill = item.CandidateSkill;
                        candidate.LastUpdateBy = item.LastUpdateBy;
                        candidate.CandidateExperience = item.CandidateExperience;
                        candidate.CandidateExperienceYears = item.CandidateExperience.Contains('.') ? item.CandidateExperience.Split('.')[0] : item.CandidateExperience;
                        candidate.CandidateExperienceMonths = item.CandidateExperience.Contains('.') ? item.CandidateExperience.Split('.')[1] : "0";
                        candidate.CandidateId = item.CandidateId;
                        candidate.CurrentCTC = Convert.ToDecimal(item.CurrentCTC);
                        candidate.ExpectedCTC = Convert.ToDecimal(item.ExpectedCTC);
                        candidate.OffersInHand = Convert.ToDecimal(item.OffersInHand);
                        candidate.ClientDesignation = item.ClientDesignation;
                        candidate.NoticePeriod = Convert.ToInt32(item.NoticePeriod);
                        candidate.SkypeId = item.SkypeId;
                        candidate.Remarks = item.Remarks;
                        candidate.BuyoutOption = Convert.ToBoolean(item.BuyoutOption);
                        candidate.Education = item.Education;
                        candidate.CompanyId = Convert.ToInt32(item.CompanyId);
                        foreach (var itm in item.Activities)
                        {
                            ActivityVM act = new ActivityVM();
                            act.Activity1 = itm.Activity1;
                            act.ActivityComplete = (itm.ActivityComplete).ToString();
                            act.ActivityDate = (itm.ActivityDate).ToString();
                            act.ActivityId = itm.ActivityId;
                            act.ActivityRemark = itm.ActivityRemark;
                            act.ActivityType = itm.ActivityType;
                            act.CandId = Convert.ToInt32(itm.CandidateId);
                            act.ClientId = Convert.ToInt32(itm.ClientId);
                            act.JobId = Convert.ToInt32(itm.JobId);
                            act.UserId = itm.UserId.ToString();
                            act.JobName = db.Jobs.Where(j => j.JobId == itm.JobId).Select(x => x.JobName).SingleOrDefault();
                            candidate.ActivityList.Add(act);
                        }

                        canList.Add(candidate);
                    }
                    Success Succ = new Success() { Code = "1", Message = "LoadData", Data = canList };
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

        // GET: odata/Candidates(5)
        [EnableQuery]
        public IHttpActionResult GetCandidate([FromODataUri] int key)
        {
            try
            {
                var Datas = db.Candidates.Where(x => x.CandidateId == key).FirstOrDefault();
                CandidateVM candidate = new CandidateVM();
                if (Datas != null)
                {

                    DateTime dobdate = Convert.ToDateTime(Datas.CandidateDOB);
                    candidate.CandidateDOB = dobdate.ToString("dd-MMM-yyyy");
                    candidate.CandidateEmail = Datas.CandidateEmail;
                    candidate.CandidateLastJobChange = Datas.CandidateLastJobChange;
                    candidate.PreviousDesignation = Datas.PreviousDesignation;
                    candidate.CandidateLocation = Datas.CandidateLocation;
                    candidate.CandidateName = Datas.CandidateName;
                    candidate.CurrentOrganisation = Datas.CurrentOrganisation;
                    candidate.CandidatePhone = Datas.CandidatePhone;
                    candidate.CandidatePhone2 = Datas.CandidatePhone2;
                    candidate.CandidateResume = Datas.CandidateResume;
                    candidate.CandidateSkill = Datas.CandidateSkill;
                    candidate.LastUpdateBy = Datas.LastUpdateBy;
                    candidate.CandidateExperience = Datas.CandidateExperience;
                    candidate.CandidateExperienceYears = Datas.CandidateExperience.Contains('.') ? Datas.CandidateExperience.Split('.')[0] : Datas.CandidateExperience;
                    candidate.CandidateExperienceMonths = Datas.CandidateExperience.Contains('.') ? Datas.CandidateExperience.Split('.')[1] : "0";
                    candidate.CandidateId = Datas.CandidateId;
                    candidate.CurrentCTC = Convert.ToDecimal(Datas.CurrentCTC);
                    candidate.ExpectedCTC = Convert.ToDecimal(Datas.ExpectedCTC);
                    candidate.OffersInHand = Convert.ToDecimal(Datas.OffersInHand);
                    candidate.ClientDesignation = Datas.ClientDesignation;
                    candidate.NoticePeriod = Convert.ToInt32(Datas.NoticePeriod);
                    candidate.SkypeId = Datas.SkypeId;
                    candidate.Remarks = Datas.Remarks;
                    candidate.BuyoutOption = Convert.ToBoolean(Datas.BuyoutOption);
                    candidate.Education = Datas.Education;
                    candidate.CompanyId = Convert.ToInt32(Datas.CompanyId);
                    candidate.Relevant_Exp = Datas.Relevant_Exp;
                    candidate.Preferred_Location = Datas.Preferred_Location;
                    candidate.Last_Job_change_reason = Datas.Last_Job_change_reason;
                    //candidate.CandidateResumesList = Datas.CandidatesResumes.ToList();
                    List<Client> clients = new List<Client>();
                    foreach (var item in Datas.ClientCandidateMappings)
                    {
                        Client client = new Client();
                        client.ClientId = item.Client.ClientId;
                        client.ClientName = item.Client.ClientName;
                        clients.Add(client);
                    }
                    candidate.ClientList = clients;
                    
                    List<CandidatesResume> resumes = new List<CandidatesResume>();
                    foreach (var item in Datas.CandidatesResumes.OrderByDescending(d => d.Candidate_resume_Id))
                    {
                        CandidatesResume res = new CandidatesResume();
                        res.CandidateId = item.CandidateId;
                        res.CandidateResume = item.CandidateResume;
                        res.CreatedDateString = item.CreatedDate != null ? item.CreatedDate.Value.Date.ToString("dd/MM/yy") : "";
                        resumes.Add(res);
                    }
                    candidate.CandidateResumesList = resumes;
                }
                Success Succ = new Success() { Code = "1", Message = "Edit", Data = candidate };
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

        // PUT: odata/Candidates(5)
        public IHttpActionResult Put([FromODataUri] int key, CandidateVM Datas)
        {
            Candidate candidate = db.Candidates.Find(key);
            if (candidate == null)
            {
                Error Err = new Error() { Code = "0", Message = "Data not Found" };
                return new ErrorResult(Err, Request);
            }
            candidate.CandidateDOB = Convert.ToDateTime(Datas.CandidateDOB);
            candidate.CandidateEmail = Datas.CandidateEmail;
            candidate.CandidateLastJobChange = Datas.CandidateLastJobChange;
            candidate.PreviousDesignation = Datas.PreviousDesignation;
            candidate.CandidateLocation = Datas.CandidateLocation;
            candidate.CandidateName = Datas.CandidateName;
            candidate.CurrentOrganisation = Datas.CurrentOrganisation;
            candidate.CandidatePhone = Datas.CandidatePhone;
            candidate.CandidatePhone2 = Datas.CandidatePhone2;
            if (Datas.CandidateResume != null)
            {
                //    candidate.CandidateResume = "Candidate" + "_" + key + "_" + Datas.CandidateResume;
                candidate.CandidateResume = AddCandidateResume(Datas, candidate);
            }
            candidate.CandidateSkill = Datas.CandidateSkill;
            candidate.LastUpdateBy = Datas.LastUpdateBy;
            candidate.CandidateExperience = String.Concat(Datas.CandidateExperienceYears, '.', Datas.CandidateExperienceMonths);
            candidate.CurrentCTC = Datas.CurrentCTC;
            candidate.ExpectedCTC = Datas.ExpectedCTC;
            candidate.OffersInHand = Datas.OffersInHand;
            candidate.ClientDesignation = Datas.ClientDesignation;
            candidate.NoticePeriod = Datas.NoticePeriod;
            candidate.SkypeId = Datas.SkypeId;
            candidate.Relevant_Exp = Datas.Relevant_Exp;
            candidate.Preferred_Location = Datas.Preferred_Location;
            candidate.Last_Job_change_reason = Datas.Last_Job_change_reason;
            candidate.Education = Datas.Education;
            if (Datas.Remarks != null)
            {
                candidate.Remarks = Datas.Remarks;
            }
            candidate.BuyoutOption = Datas.BuyoutOption;
            candidate.CompanyId = Convert.ToInt32(Datas.CompanyId);
            candidate.LastUpdateDate = DateTime.Now.Date;
            try
            {
                db.SaveChanges();

                Success Succ = new Success() { Code = "1", Message = "Update" };
                return new SuccessResult(Succ, Request);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!CandidateExists(key))
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

            // return Updated(candidate);
        }

        private string AddCandidateResume(CandidateVM Datas, Candidate candidate)
        {
            string CandResume = string.Empty;
            if (!string.IsNullOrEmpty(Datas.CandidateResume))
            {
                CandidatesResume resume = new CandidatesResume();
                var resumes = db.CandidatesResumes.Where(d => d.CandidateId == candidate.CandidateId).ToList();
                if (resumes.Count > 0)
                {
                    var total = resumes.Count + 1;
                    resume.CandidateResume = total + "_Candidate_" + candidate.CandidateId + Path.GetExtension(Datas.CandidateResume);
                }
                else
                {
                    resume.CandidateResume = "1_Candidate_" + candidate.CandidateId + Path.GetExtension(Datas.CandidateResume);
                }

                resume.CandidateId = candidate.CandidateId;

                resume.CreatedDate = DateTime.Now.Date;
                db.CandidatesResumes.Add(resume);
                db.SaveChanges();
                candidate.CandidateResume = resume.CandidateResume;
            }
            CandResume = !string.IsNullOrEmpty(candidate.CandidateResume) ? candidate.CandidateResume : !string.IsNullOrEmpty(Datas.CandidateResume) ? Datas.CandidateResume : "";
            return CandResume;
        }

        // POST: odata/Candidates
        public IHttpActionResult Post(CandidateVM model)
        {
            if (model.CandidateId > 0)
            {

                Candidate candidate = db.Candidates.Find(model.CandidateId);
                if (candidate == null)
                {
                    Error Err = new Error() { Code = "0", Message = "Data not Found" };
                    return new ErrorResult(Err, Request);
                }
                candidate.IsActive = true;
                candidate.CandidateDOB = Convert.ToDateTime(model.CandidateDOB);
                candidate.CandidateEmail = model.CandidateEmail;
                candidate.CandidateLastJobChange = model.CandidateLastJobChange;
                candidate.PreviousDesignation = model.PreviousDesignation;
                candidate.CandidateLocation = model.CandidateLocation;
                candidate.CandidateName = model.CandidateName;
                candidate.CurrentOrganisation = model.CurrentOrganisation;
                candidate.CandidatePhone = model.CandidatePhone;
                candidate.CandidatePhone2 = model.CandidatePhone2;
                candidate.LastUpdateDate = DateTime.Now.Date;
                if (model.CandidateResume != null)
                {
                    //candidate.CandidateResume = "Candidate" + "_" + model.CandidateId + "_" + model.CandidateResume;
                    candidate.CandidateResume = AddCandidateResume(model, candidate);
                }
                candidate.CandidateSkill = model.CandidateSkill;
                candidate.LastUpdateBy = model.LastUpdateBy;
                candidate.CandidateExperience = string.Concat(model.CandidateExperienceYears , '.' , model.CandidateExperienceMonths);
                candidate.CurrentCTC = model.CurrentCTC;
                candidate.ExpectedCTC = model.ExpectedCTC;
                candidate.OffersInHand = model.OffersInHand;
                candidate.ClientDesignation = model.ClientDesignation;
                candidate.NoticePeriod = model.NoticePeriod;
                candidate.SkypeId = model.SkypeId;
                candidate.Remarks = model.Remarks;
                candidate.BuyoutOption = model.BuyoutOption;
                candidate.Education = model.Education;
                candidate.CompanyId = Convert.ToInt32(model.CompanyId);
                candidate.Relevant_Exp = model.Relevant_Exp;
                candidate.Preferred_Location = model.Preferred_Location;
                candidate.Last_Job_change_reason = model.Last_Job_change_reason;
                try
                {
                    db.SaveChanges();

                    Success Succ = new Success() { Code = "1", Message = "Update" };
                    return new SuccessResult(Succ, Request);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!CandidateExists(model.CandidateId))
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
            else
            {
                Candidate candidate1 = db.Candidates.Where(c=>c.CandidateEmail == model.CandidateEmail & c.CompanyId == model.CompanyId).FirstOrDefault();
                if (candidate1 != null)
                {
                    Error Err = new Error() { Code = "0", Message = "Candidate is available with the same email id." };
                    return new ErrorResult(Err, Request);
                }

                Candidate candidate = new Candidate();
                candidate.CandidateDOB = Convert.ToDateTime(model.CandidateDOB);
                candidate.CandidateEmail = model.CandidateEmail;
                candidate.CandidateLastJobChange = model.CandidateLastJobChange;
                candidate.PreviousDesignation = model.PreviousDesignation;
                candidate.CandidateLocation = model.CandidateLocation;
                candidate.CandidateName = model.CandidateName;
                candidate.CurrentOrganisation = model.CurrentOrganisation;
                candidate.CandidatePhone = model.CandidatePhone;
                candidate.CandidatePhone2 = model.CandidatePhone2;
                candidate.CandidateResume = model.CandidateResume;
                candidate.CandidateSkill = model.CandidateSkill;
                candidate.LastUpdateBy = model.LastUpdateBy;
                candidate.CandidateExperience = string.Concat(model.CandidateExperienceYears , '.' , model.CandidateExperienceMonths);
                candidate.CurrentCTC = model.CurrentCTC;
                candidate.ExpectedCTC = model.ExpectedCTC;
                candidate.OffersInHand = model.OffersInHand;
                candidate.ClientDesignation = model.ClientDesignation;
                candidate.NoticePeriod = model.NoticePeriod;
                candidate.SkypeId = model.SkypeId;
                candidate.Remarks = model.Remarks;
                candidate.BuyoutOption = model.BuyoutOption;
                candidate.Education = model.Education;
                candidate.CompanyId = Convert.ToInt32(model.CompanyId);
                candidate.Relevant_Exp = model.Relevant_Exp;
                candidate.Preferred_Location = model.Preferred_Location;
                candidate.Last_Job_change_reason = model.Last_Job_change_reason;
                candidate.IsActive = true;
                candidate.LastUpdateDate = DateTime.Now.Date;
                candidate.Created = DateTime.Now.Date;
                candidate.CreatedBy = model.LastUpdateBy;
                db.Candidates.Add(candidate);
                try
                {
                    db.SaveChanges();

                    Candidate data = db.Candidates.Find(candidate.CandidateId);

                    // data.CandidateResume = "Candidate" + "_" + candidate.CandidateId + "_" + model.CandidateResume;
                    data.CandidateResume = AddCandidateResume(model, candidate);
                    db.SaveChanges();
                    Success Succ = new Success() { Code = "1", Message = "Inserted", Data = candidate.CandidateId };
                    return new SuccessResult(Succ, Request);
                }
                catch (DbUpdateException ex)
                {
                    if (CandidateExists(candidate.CandidateId))
                    {
                        Log.Error("Start log ERROR...");
                        Log.Error(ex.Message);
                        Thread.Sleep(TimeSpan.FromSeconds(secs));
                        Error Err = new Error() { Code = "0", Message = "Candidate Already exist...!!!" };
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

        }
        // PATCH: odata/Candidates(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<Candidate> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Candidate candidate = db.Candidates.Find(key);
            if (candidate == null)
            {
                return NotFound();
            }

            patch.Patch(candidate);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CandidateExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(candidate);
        }

        // DELETE: odata/Candidates(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            Candidate candidate = db.Candidates.Find(key);
            if (candidate == null)
            {
                Error Err = new Error() { Code = "0", Message = "Data not Found" };
                return new ErrorResult(Err, Request);
            }
            try
            {
                List<Activity> act = db.Activities.Where(x => x.CandidateId == key).ToList();
                db.Activities.RemoveRange(act);
                db.Candidates.Remove(candidate);
                db.SaveChanges();
                Success Succ = new Success() { Code = "1", Message = "Delete" };
                return new SuccessResult(Succ, Request);
            }
            catch (Exception ex)
            {
                Log.Error("Start log ERROR...");
                Log.Error(ex.Message);
                Thread.Sleep(TimeSpan.FromSeconds(secs));
            }
            return null;
        }

        // GET: odata/Candidates(5)/Activities
        [EnableQuery]
        public IQueryable<Activity> GetActivities([FromODataUri] int key)
        {
            return db.Candidates.Where(m => m.CandidateId == key).SelectMany(m => m.Activities);
        }

        // GET: odata/Candidates(5)/Customer
        [EnableQuery]
        public SingleResult<Customer> GetCustomer([FromODataUri] int key)
        {
            return SingleResult.Create(db.Candidates.Where(m => m.CandidateId == key).Select(m => m.Customer));
        }

        // GET: odata/Candidates(5)/CandidateExams
        [EnableQuery]
        public IQueryable<CandidateExam> GetCandidateExams([FromODataUri] int key)
        {
            return db.Candidates.Where(m => m.CandidateId == key).SelectMany(m => m.CandidateExams);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CandidateExists(int key)
        {
            return db.Candidates.Count(e => e.CandidateId == key) > 0;
        }
    }
}
