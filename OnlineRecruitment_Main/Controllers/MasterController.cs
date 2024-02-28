using log4net;
using OnlineRecruitment_Main.CustomModels;
using OnlineRecruitment_Main.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using vrecruit.DataBase.Comman;
using vrecruit.DataBase.EntityDataModel;
using vrecruit.DataBase.ViewModel;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using System.ComponentModel;

namespace OnlineRecruitment_Main.Controllers
{
    public class MasterController : Controller
    {
        vRecruitEntities db = new vRecruitEntities();
        Comman Comman = new Comman();
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        int secs = 3;
        // GET: Master
        public ActionResult Index()
        {
            return View();
        }
        #region
        public ActionResult Dashboard()
        {
            if (Session["CurrentUserId"] == null)
            {
                Session.Clear();
                Session.Abandon();
                return RedirectToAction("Login", "Account");
            }
            IndexVM VM = new IndexVM();
            int cmpId = Convert.ToInt32(Session["currentCompanyId"]);
            var userid = Session["CurrentUserId"].ToString();
            string Currentrole = string.Empty;
            if (Session["CurrentRole"] != null)
            {
                Currentrole = Session["CurrentRole"].ToString();
            }

            if (Currentrole == "SUPER ADMIN")
            {
                VM.TotalComapnies = db.Companies.Count();
                VM.TotalCandidates = db.Candidates.Count();
                VM.TotalClients = db.Clients.Count();
                VM.TotalJobs = db.Jobs.Count();
            }
            else if (Currentrole == "CLIENT")
            {
                var ClientId = db.AspNetUsers.Where(x => x.Id == userid).Select(u => u.ClientId).FirstOrDefault();
                VM.TotalJobs = db.Jobs.Where(x => x.ClientId == ClientId).Count();
                //VM.TotalCandidates = (db.Activities.Where(x => x.ClientId == ClientId)).GroupBy(x => x.CandidateId).Count();
                VM.TotalCandidates = db.ClientCandidateMappings.Where(c => c.ClientId == ClientId).Count();
            }
            else if (Currentrole == "ADMIN")
            {
                VM.TotalCandidates = db.Candidates.Where(x => x.CompanyId == cmpId).Count();
                VM.TotalClients = db.Clients.Where(x => x.CompanyId == cmpId).Count();
                VM.TotalJobs = db.Jobs.Where(x => x.CompanyId == cmpId).Count();
                var v = from user in db.AspNetUsers
                        from role in user.AspNetRoles
                        where role.Name != "ADMIN" && role.Name != "CLIENT" && role.Name != "SUPER ADMIN" && user.CompanyId == cmpId
                        select user;

                VM.TotalUser = v.Count();
                // VM.TotalUser = db.AspNetUsers.Where(x => x.CompanyId == cmpId).Count();

            }
            else
            {
                VM.TotalCandidates = db.Candidates.Where(x => x.CompanyId == cmpId).Count();
                VM.TotalClients = db.Clients.Where(x => x.CompanyId == cmpId).Count();
                VM.TotalJobs = db.Jobs.Where(x => x.CompanyId == cmpId).Count();
                VM.TotalUser = db.AspNetUsers.Where(x => x.CompanyId == cmpId).Count();
            }

            return View(VM);
        }
        public ActionResult Candidate()
        {
            if (Session["CurrentUserId"] == null)
            {
                Session.Clear();
                Session.Abandon();
                return RedirectToAction("Login", "Account");
            }
            CandidateVM model = new CandidateVM();
            int cmpid = Convert.ToInt32(Session["currentCompanyId"]);
            model.ClientList = new List<Client>();
            model.ActivityTypeList = db.ActivityTypes.ToList();
            model.TestList = db.Tests.Where(x => x.CompanyId == cmpid).ToList();
            model.TestList = model.TestList.GroupBy(test => test.GroupId)
                   .Select(grp => grp.First())
                   .ToList();
            //model.ClientList = db.Clients.ToList();
            int currentClientId = Convert.ToInt32(Session["CurrentClientId"]);
            var currentRole = this.Session["CurrentRole"].ToString();
            model.ClientCurrentRole = currentRole;
            model.ClientList = currentRole == "CLIENT" ? db.Clients.Where(c => c.ClientId == currentClientId).ToList() : db.Clients.ToList();
            AccessPermission AccessData = GetCurrentAccesspermission("Candidate");
            model.IsAdd = Convert.ToBoolean(AccessData.IsAdd);
            model.IsEdit = Convert.ToBoolean(AccessData.IsEdit);
            model.IsDelete = Convert.ToBoolean(AccessData.IsDelete);
            model.IsExport = Convert.ToBoolean(AccessData.IsExport);
            model.IsPrint = Convert.ToBoolean(AccessData.IsPrint);

            return View(model);
        }
        public ActionResult Client()
        {
            if (Session["CurrentUserId"] == null)
            {
                Session.Clear();
                Session.Abandon();
                return RedirectToAction("Login", "Account");
            }
            ClientVM model = new ClientVM();
            //model.CompanyList = db.Companies.ToList();
            AccessPermission AccessData = GetCurrentAccesspermission("Client");
            model.IsAdd = Convert.ToBoolean(AccessData.IsAdd);
            model.IsEdit = Convert.ToBoolean(AccessData.IsEdit);
            model.IsDelete = Convert.ToBoolean(AccessData.IsDelete);
            model.IsExport = Convert.ToBoolean(AccessData.IsExport);
            model.IsPrint = Convert.ToBoolean(AccessData.IsPrint);

            int companyToLook = Convert.ToInt32(this.Session["currentCompanyId"]);
            string currentRole = string.Empty;

            if (this.Session["CurrentRole"] != null)
                currentRole = this.Session["CurrentRole"].ToString();
            List<Company> result = new List<Company>();

            if (currentRole == "SUPER ADMIN")
            {
                result = db.Companies.ToList();
            }
            else
            {
                result = db.Companies.Where(x => x.ID == companyToLook).ToList();
            }

            model.CompanyList = result;
            model.CountryList = db.Countries.ToList();
            return View(model);
        }
        public ActionResult Job()
        {
            if (Session["CurrentUserId"] == null)
            {
                Session.Clear();
                Session.Abandon();
                return RedirectToAction("Login", "Account");
            }
            JobViewModel model = new JobViewModel();

            // model.ClientList = db.Clients.ToList();

            int companyToLook = Convert.ToInt32(this.Session["currentCompanyId"]);
            string currentRole = string.Empty;
            if (this.Session["CurrentRole"] != null)
                currentRole = this.Session["CurrentRole"].ToString();
            List<Client> result = new List<Client>();

            if (currentRole == "SUPER ADMIN")
            {
                result = db.Clients.ToList();
            }
            else
            {
                result = db.Clients.Where(x => x.CompanyId == companyToLook).ToList();
            }

            model.ClientList = result;

            AccessPermission AccessData = GetCurrentAccesspermission("Job");
            model.IsAdd = Convert.ToBoolean(AccessData.IsAdd);
            model.IsEdit = Convert.ToBoolean(AccessData.IsEdit);
            model.IsDelete = Convert.ToBoolean(AccessData.IsDelete);
            model.IsExport = Convert.ToBoolean(AccessData.IsExport);
            model.IsPrint = Convert.ToBoolean(AccessData.IsPrint);
            model.IsView = Convert.ToBoolean(AccessData.IsView);
            return View(model);
        }
        public ActionResult Question()
        {
            if (Session["CurrentUserId"] == null)
            {
                Session.Clear();
                Session.Abandon();
                return RedirectToAction("Login", "Account");
            }
            QuestionAnswerVM VM = new QuestionAnswerVM();
            VM.QuegroupList = db.QuestionGroups.ToList();
            VM.CompanyList = db.Companies.ToList();
            VM.QuestionSkillList = new SelectList(db.SkillMasters.Where(s => s.Active == true).ToList(), "SkillId", "SkillName");
            AccessPermission AccessData = GetCurrentAccesspermission("Question");
            VM.IsAdd = Convert.ToBoolean(AccessData.IsAdd);
            VM.IsEdit = Convert.ToBoolean(AccessData.IsEdit);
            VM.IsDelete = Convert.ToBoolean(AccessData.IsDelete);
            VM.IsExport = Convert.ToBoolean(AccessData.IsExport);
            VM.IsPrint = Convert.ToBoolean(AccessData.IsPrint);
            return View(VM);
        }
        public ActionResult QuestionGroup()
        {
            if (Session["CurrentUserId"] == null)
            {
                Session.Clear();
                Session.Abandon();
                return RedirectToAction("Login", "Account");
            }
            QuestionGroupVM model = new QuestionGroupVM();
            model.CompanyList = db.Companies.ToList();
            AccessPermission AccessData = GetCurrentAccesspermission("QuestionGroup");
            model.IsAdd = Convert.ToBoolean(AccessData.IsAdd);
            model.IsEdit = Convert.ToBoolean(AccessData.IsEdit);
            model.IsDelete = Convert.ToBoolean(AccessData.IsDelete);
            model.IsExport = Convert.ToBoolean(AccessData.IsExport);
            model.IsPrint = Convert.ToBoolean(AccessData.IsPrint);
            return View(model);
        }
        public ActionResult Roles()
        {
            if (Session["CurrentUserId"] == null)
            {
                Session.Clear();
                Session.Abandon();
                return RedirectToAction("Login", "Account");
            }
            RolesViewModel model = new RolesViewModel();
            AccessPermission AccessData = GetCurrentAccesspermission("Roles");
            model.IsAdd = Convert.ToBoolean(AccessData.IsAdd);
            model.IsEdit = Convert.ToBoolean(AccessData.IsEdit);
            model.IsDelete = Convert.ToBoolean(AccessData.IsDelete);
            model.IsExport = Convert.ToBoolean(AccessData.IsExport);
            model.IsPrint = Convert.ToBoolean(AccessData.IsPrint);
            return View(model);
        }
        public ActionResult Test()
        {
            if (Session["CurrentUserId"] == null)
            {
                Session.Clear();
                Session.Abandon();
                return RedirectToAction("Login", "Account");
            }
            QuestionAnswerVM VM = new QuestionAnswerVM();
            string crntRoleName = Session["CurrentRole"].ToString();
            if (crntRoleName == "SUPER ADMIN")
            {
                VM.QuegroupList = db.QuestionGroups.ToList();
            }
            else
            {
                int cmpid = Convert.ToInt32(Session["currentCompanyId"]);
                var suAdminId = WebConfigurationManager.AppSettings["SuperAdminId"].ToString();
                VM.QuegroupList = db.QuestionGroups.Where(x => x.CompanyId == cmpid || x.LastUpdateBy == suAdminId).ToList();
            }
            VM.QuestionSkillList = new SelectList(db.SkillMasters.Where(s => s.Active == true).ToList(), "SkillId", "SkillName");
            AccessPermission AccessData = GetCurrentAccesspermission("Test");
            VM.IsAdd = Convert.ToBoolean(AccessData.IsAdd);
            VM.IsEdit = Convert.ToBoolean(AccessData.IsEdit);
            VM.IsDelete = Convert.ToBoolean(AccessData.IsDelete);
            VM.IsExport = Convert.ToBoolean(AccessData.IsExport);
            VM.IsPrint = Convert.ToBoolean(AccessData.IsPrint);
            return View(VM);
        }
        public ActionResult Users()
        {
            if (Session["CurrentUserId"] == null)
            {
                Session.Clear();
                Session.Abandon();
                return RedirectToAction("Login", "Account");
            }
            UserViewModel model = new UserViewModel();
            model.Roleslist = (from R in db.AspNetRoles select R).ToList();
            model.CompanyList = db.Companies.ToList();
            AccessPermission AccessData = GetCurrentAccesspermission("Users");
            model.IsAdd = Convert.ToBoolean(AccessData.IsAdd);
            model.IsEdit = Convert.ToBoolean(AccessData.IsEdit);
            model.IsDelete = Convert.ToBoolean(AccessData.IsDelete);
            model.IsExport = Convert.ToBoolean(AccessData.IsExport);
            model.IsPrint = Convert.ToBoolean(AccessData.IsPrint);
            return View(model);
        }
        public ActionResult Companies()
        {
            if (Session["CurrentUserId"] == null)
            {
                Session.Clear();
                Session.Abandon();
                return RedirectToAction("Login", "Account");
            }
            CompaniesVM model = new CompaniesVM();
            AccessPermission AccessData = GetCurrentAccesspermission("Companies");
            model.IsAdd = Convert.ToBoolean(AccessData.IsAdd);
            model.IsEdit = Convert.ToBoolean(AccessData.IsEdit);
            model.IsDelete = Convert.ToBoolean(AccessData.IsDelete);
            model.IsExport = Convert.ToBoolean(AccessData.IsExport);
            model.IsPrint = Convert.ToBoolean(AccessData.IsPrint);
            return View(model);
        }
        public ActionResult CreateAdmin()
        {
            if (Session["CurrentUserId"] == null)
            {
                Session.Clear();
                Session.Abandon();
                return RedirectToAction("Login", "Account");
            }
            UserViewModel model = new UserViewModel();
            model.Roleslist = (from R in db.AspNetRoles select R).ToList();
            model.CompanyList = (from C in db.Companies select C).Where(x => x.IsActive == true).ToList();
            AccessPermission AccessData = GetCurrentAccesspermission("CreateAdmin");
            model.IsAdd = Convert.ToBoolean(AccessData.IsAdd);
            model.IsEdit = Convert.ToBoolean(AccessData.IsEdit);
            model.IsDelete = Convert.ToBoolean(AccessData.IsDelete);
            model.IsExport = Convert.ToBoolean(AccessData.IsExport);
            model.IsPrint = Convert.ToBoolean(AccessData.IsPrint);
            return View(model);

        }
        public ActionResult Skills()
        {
            if (Session["CurrentUserId"] == null)
            {
                Session.Clear();
                Session.Abandon();
                return RedirectToAction("Login", "Account");
            }

            SkillMasterModel model = new SkillMasterModel();
            AccessPermission AccessData = GetCurrentAccesspermission("Skills");
            model.IsAdd = Convert.ToBoolean(AccessData.IsAdd);
            model.IsEdit = Convert.ToBoolean(AccessData.IsEdit);
            model.IsDelete = Convert.ToBoolean(AccessData.IsDelete);
            model.IsExport = Convert.ToBoolean(AccessData.IsExport);
            model.IsPrint = Convert.ToBoolean(AccessData.IsPrint);
            //return View(model);
            //JobViewModel model = new JobViewModel();
            //model.ClientList = db.Clients.ToList();
            //AccessPermission AccessData = GetCurrentAccesspermission("Job");
            //model.IsAdd = Convert.ToBoolean(AccessData.IsAdd);
            //model.IsEdit = Convert.ToBoolean(AccessData.IsEdit);
            //model.IsDelete = Convert.ToBoolean(AccessData.IsDelete);
            //model.IsExport = Convert.ToBoolean(AccessData.IsExport);
            //model.IsPrint = Convert.ToBoolean(AccessData.IsPrint);
            //model.IsView = Convert.ToBoolean(AccessData.IsView);
            return View(model);
        }

        public ActionResult NextAction()
        {
            if (Session["CurrentUserId"] == null)
            {
                Session.Clear();
                Session.Abandon();
                return RedirectToAction("Login", "Account");
            }

            NextActionVM model = new NextActionVM();
            AccessPermission AccessData = GetCurrentAccesspermission("NextAction");
            model.IsAdd = Convert.ToBoolean(AccessData.IsAdd);
            model.IsEdit = Convert.ToBoolean(AccessData.IsEdit);
            model.IsDelete = Convert.ToBoolean(AccessData.IsDelete);
            model.IsExport = Convert.ToBoolean(AccessData.IsExport);
            model.IsPrint = Convert.ToBoolean(AccessData.IsPrint);

            return View(model);
        }

        public ActionResult SendSMS()
        {
            if (Session["CurrentUserId"] == null)
            {
                Session.Clear();
                Session.Abandon();
                return RedirectToAction("Login", "Account");
            }

            SendMesssageVM model = new SendMesssageVM();
            AccessPermission AccessData = GetCurrentAccesspermission("SendSMS");
            model.IsAdd = Convert.ToBoolean(AccessData.IsAdd);
            model.IsEdit = Convert.ToBoolean(AccessData.IsEdit);
            model.IsDelete = Convert.ToBoolean(AccessData.IsDelete);
            model.IsExport = Convert.ToBoolean(AccessData.IsExport);
            model.IsPrint = Convert.ToBoolean(AccessData.IsPrint);

            return View(model);
        }

        public ActionResult Report()
        {
            return (ActionResult)this.View();
        }
        #endregion
        #region Others
        [HttpGet]
        public ActionResult GetClientJob(string key, string searchtext)
        {
            if (key != "")
            {
                int Id = Convert.ToInt32(key);
                if (searchtext != null)
                {
                    var result = (from a in db.Jobs.OrderBy(a => a.JobName)
                                  where a.ClientId == Id && a.JobName.Contains(searchtext)
                                  select new { a.JobId, a.JobName }).ToList();
                    return Json(new { Message = "GetClient", Data = result }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var result = (from a in db.Jobs.OrderBy(a => a.JobName)
                                  where a.ClientId == Id
                                  select new { a.JobId, a.JobName }).ToList().Take(10);
                    return Json(new { Message = "GetClient", Data = result }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json("Data Not Found...!!!");
            }
        }
        [HttpPost]
        public ActionResult UploadResume()
        {
            string sasUrl = "https://eqhire.blob.core.windows.net/eqhires?sp=racwdli&st=2024-02-19T07:07:53Z&se=2024-12-31T15:07:53Z&sv=2022-11-02&sr=c&sig=o4GoLC%2BHYF99%2FbMmPvUBD0MJyt6w5qy3%2FmzMCA4kv8A%3D";
            CloudBlobContainer container = new CloudBlobContainer(sasUrl);
            string uploadedFileUrl = "";
            if (Request.Files.Count > 0)
            {
                try
                {
                    HttpFileCollectionBase files = Request.Files;
                    int candidateId = 0;
                    string[] keys = Request.Form.AllKeys;

                    for (int i = 0; i < keys.Length; i++)
                    {
                        candidateId = Convert.ToInt32(Request.Form[keys[i]]);
                    }
                    #region OldLogic
                    //for (int i = 0; i < files.Count; i++)
                    //{
                    //    HttpPostedFileBase file = files[i];
                    //    string fname;
                    //    if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                    //    {
                    //        string[] testfiles = file.FileName.Split(new char[] { '\\' });
                    //        fname = testfiles[testfiles.Length - 1];
                    //    }
                    //    else
                    //    {
                    //        fname = file.FileName;
                    //    }
                    //    if (fname != "")
                    //    {
                    //        fname = "Candidate" + "_" + candidateId + "_" + fname;
                    //    }
                    //    string path = Server.MapPath("~/UploadedResume");
                    //    if (!Directory.Exists(path))
                    //    {
                    //        DirectoryInfo di = Directory.CreateDirectory(path);
                    //    }
                    //    var addedfiles = Directory.GetFiles(path);
                    //    List<string> rsFiles = new List<string>();
                    //    foreach (var item in addedfiles)
                    //    {
                    //        if (item.Contains(candidateId))
                    //        {
                    //            rsFiles.Add(item);
                    //        }
                    //    }
                    //    var counts = rsFiles.Count();
                    //    fname = counts == 0 ? "1_" + fname : (counts += 1) + "_" + fname;
                    //    fname = Path.Combine(path, fname);
                    //    file.SaveAs(fname);
                    //}
                    #endregion

                    var resumes = db.CandidatesResumes.Where(d => d.CandidateId == candidateId).ToList();
                    
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];

                        //string blobName = $"UploadedResume/{i + 1}_Candidate_{candidateId}{Path.GetExtension(file.FileName)}";
                        string blobName = "UploadedResume/" + db.CandidatesResumes
                            .Where(d => d.CandidateId == candidateId)
                            .OrderByDescending(d => d.Candidate_resume_Id)
                            .Select(d => d.CandidateResume)
                            .FirstOrDefault();

                        if (file != null && file.ContentLength > 0)
                        {
                            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blobName);

                            using (Stream fileStream = file.InputStream)
                            {
                                blockBlob.UploadFromStream(fileStream);
                            }
                            uploadedFileUrl = blockBlob.Uri.ToString();
                        }
                    }
                    //return Json(new { Message = "Resume Uploaded Successfully!", Data = uploadedFileUrl }, JsonRequestBehavior.AllowGet);
                    return Json("Resume Uploaded Successfully!");
                }
                catch (Exception ex)
                {
                    Log.Error("Start log ERROR...");
                    Log.Error(ex.Message);
                    Thread.Sleep(TimeSpan.FromSeconds(secs));
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }
        [HttpGet]
        public ActionResult GetCandidateActivity(int key)
        {
            List<ActivityVM> ActList = new List<ActivityVM>();
            var q = (from J in db.Jobs
                     join A in db.Activities on J.JobId equals A.JobId into tblact
                     from act in tblact
                     where act.CandidateId == key
                     orderby act.ActivityId
                     select new ActivityVM()
                     {
                         JobId = J.JobId,
                         JobName = J.JobName,
                         CandId = key,
                         ClientIds = J.ClientId.ToString(),
                         ActivityTypeList = db.ActivityTypes.ToList(),
                         UserId = act.UserId.ToString(),
                         CompanyId = act.Company.ID,
                         CompanyName = act.Company.CompanyName,
                         Activities = tblact.Where(x => x.CandidateId == key).ToList(),
                     }).ToList();
            var dat = q.GroupBy(x => x.JobId).ToList();
            foreach (var item in dat)
            {
                ActList.Add(item.First());
            }
            return PartialView("_CandidateActivity", ActList);
        }
        public ActionResult UpdateCandidateActivity(ActivityVM model)
        {
            Activity activity = new Activity();
            var CandidateData = (from can in db.Candidates where can.CandidateId == model.CandId select new { can.CandidateEmail, can.CandidateName }).SingleOrDefault();
            var UserEmail = (from user in db.AspNetUsers where user.Id == model.UserId select new { user.Email }).SingleOrDefault();
            Job jobData = db.Jobs.Where(x => x.JobId == model.JobId).SingleOrDefault();
            var body = "";
            ActivityVM ActVM = new ActivityVM();

            ActVM.ActivityDate = model.ActivityDate;
            ActVM.UserEmail = UserEmail.Email;
            ActVM.Activity1 = model.Activity1;
            ActVM.ActivityType = model.ActivityType;
            ActVM.CandidateName = CandidateData.CandidateName;
            if (model.ActivityType == "TEST")
            {
                string url = WebConfigurationManager.AppSettings["TestLink"].ToString() + model.TestLink + "&email=" + CandidateData.CandidateEmail + "&useremail=" + UserEmail.Email;
                ActVM.TestLink = url;
            }
            using (var client = new HttpClient())
            {
                string apiUrl = WebConfigurationManager.AppSettings["apiUrl"].ToString();
                var input = new
                {
                    viewName = "~/Views/EmailView/_EmailTemplate.cshtml",
                    model = ActVM,
                };
                string inputJson = (new JavaScriptSerializer()).Serialize(input);
                WebClient clients = new WebClient();
                clients.Headers["Content-type"] = "application/json";
                clients.Encoding = Encoding.UTF8;
                string json = clients.UploadString(apiUrl + "/EmailTemplate", inputJson);
                body = json;
            }
            EmailViewModel EVM = new EmailViewModel();
            EmailViewModels Emodels = new EmailViewModels();
            Emodels.Subject = jobData.JobName;
            Emodels.body = body;
            Emodels.Smtpemail = EVM.Smtpemail;
            Emodels.Smtppassword = EVM.Smtppassword;
            Emodels.Smtp = EVM.Stmp;
            Emodels.Port = EVM.Port;
            if (CandidateData.CandidateEmail != null)
            {
                try
                {
                    Emodels.to = CandidateData.CandidateEmail;
                    bool sendmail = Comman.SendEmail(Emodels);
                    if (sendmail == false)
                    {
                        return Json(new { Error = true, Message = "Email delivery failed" }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Start log ERROR...");
                    Log.Error(ex.Message);
                    Thread.Sleep(TimeSpan.FromSeconds(secs));
                    return Json(new { Error = true, Message = ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            if (UserEmail != null)
            {
                try
                {
                    Emodels.to = UserEmail.Email;
                    bool sendmail = Comman.SendEmail(Emodels);
                    if (sendmail == false)
                    {
                        return Json(new { Error = true, Message = "Email delivery failed" }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Start log ERROR...");
                    Log.Error(ex.Message);
                    Thread.Sleep(TimeSpan.FromSeconds(secs));
                    return Json(new { Error = true, Message = ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            activity.ActivityType = model.ActivityType;
            activity.ActivityDate = Convert.ToDateTime(model.ActivityDate);
            activity.ActivityComplete = Convert.ToBoolean(model.ActivityComplete);
            activity.CandidateId = model.CandId;
            activity.JobId = model.JobId;
            activity.UserId = new Guid(model.UserId);
            activity.ClientId = model.ClientId;
            activity.Companyid = model.CompanyId;
            db.Activities.Add(activity);
            try
            {
                db.SaveChanges();
                return Json(new { Error = false, Message = "Email Send successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Log.Error("Start log ERROR...");
                Log.Error(ex.Message);
                Thread.Sleep(TimeSpan.FromSeconds(secs));
                return Json(new { Error = true, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult CreateTest(List<Testlist> list, int hr, int min)
        {
            if (list != null)
            {
                try
                {
                    List<Test> tstLst = new List<Test>();
                    int Key = 0;
                    foreach (var item in list)
                    {
                        Test test = new Test();

                        int h = (hr * 3600);
                        int t = (min * 60);
                        test.Testtime = h + t;
                        Key = Convert.ToInt32(item.GroupId);
                        test.GroupId = Convert.ToInt32(item.GroupId);
                        test.QueId = Convert.ToInt32(item.QueId);
                        test.TestName = item.TestName;
                        test.CompanyId = item.CompanyId;
                        test.IsActive = true;
                        tstLst.Add(test);
                    }
                    List<Test> removdta = db.Tests.Where(x => x.GroupId == Key).ToList();
                    db.Tests.RemoveRange(removdta);
                    db.Tests.AddRange(tstLst);
                    db.SaveChanges();

                    var Datas = (from T in db.Tests
                                 join Q in db.Questions on T.QueId equals Q.QuestionId
                                 join G in db.QuestionGroups on T.GroupId equals G.ID
                                 where G.ID == Key
                                 orderby Q.QuestionId
                                 select new
                                 {
                                     Q.Question1,
                                     Q.QuestionId
                                 }).ToList();
                    return Json(new { Error = false, Message = "Inserted", Data = Datas }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Log.Error("Start log ERROR...");
                    Log.Error(ex.Message);
                    Thread.Sleep(TimeSpan.FromSeconds(secs));
                    return Json(new { Error = true, Message = ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Error = true, Message = "Please Insert Data In List" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult getTest(string Key)
        {
            if (Key != "")
            {
                int Gid = Convert.ToInt32(Key);
                var Datas = (from T in db.Tests
                             join Q in db.Questions on T.QueId equals Q.QuestionId
                             join G in db.QuestionGroups on T.GroupId equals G.ID
                             where G.ID == Gid
                             orderby Q.QuestionId
                             select new
                             {
                                 Q.Question1,
                                 Q.QuestionId,
                                 Q.QuestionSkill,
                                 T.TestName
                             }).ToList();
                if (Datas.Count > 0)
                {
                    return Json(new { Error = false, Message = "Success", Data = Datas }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { Error = true, Message = "Data Not Found For This Question Group...!!!" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Error = true, Message = "Invalid Question Group....!!!" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult ImageUpload(List<QuestionImages> objimg, int quid)
        {
            try
            {
                if (quid != 0)
                {
                    List<QuestionAnswerImage> modalimglist = db.QuestionAnswerImages.Where(x => x.QuestionId == quid).ToList();
                    db.QuestionAnswerImages.RemoveRange(modalimglist);
                    db.SaveChanges();
                }


                foreach (var item in objimg)
                {

                    QuestionAnswerImage Imagemodel = new QuestionAnswerImage();
                    Imagemodel.Images = item.Images;
                    Imagemodel.QuestionId = item.QuestionId;
                    db.QuestionAnswerImages.Add(Imagemodel);
                    db.SaveChanges();
                }

                return Json("Image Uploaded Successfully!");
            }
            catch (Exception ex)
            {
                Log.Error("Start log ERROR...");
                Log.Error(ex.Message);
                Thread.Sleep(TimeSpan.FromSeconds(secs));
                return Json(new { Error = true, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpGet]
        public ActionResult GetCandidateByJob(int? Jobid)
        {
            if (Jobid != 0)
            {
                var Data = db.Activities.Where(x => x.JobId == Jobid).GroupBy(x => x.CandidateId).ToList();
                List<CandidateVM> canList = new List<CandidateVM>();
                foreach (var item in Data)
                {
                    CandidateVM model = new CandidateVM();
                    var candata = db.Candidates.Where(x => x.CandidateId == item.Key).SingleOrDefault();
                    model.CandidateId = candata.CandidateId;
                    model.CandidateName = candata.CandidateName;
                    model.ClientDesignation = candata.ClientDesignation;
                    model.Education = candata.Education;
                    DateTime dt = Convert.ToDateTime(candata.CandidateDOB);
                    model.CandidateDOB = dt.ToString("dd-MMM-yyyy");

                    if (string.IsNullOrEmpty(candata.CandidateExperience))
                    {
                        model.CandidateExperienceYears = "";
                        model.CandidateExperienceMonths = "";
                    }
                    else
                    {
                        model.CandidateExperienceYears = candata.CandidateExperience.Contains('.') ? candata.CandidateExperience.Split('.')[0] : candata.CandidateExperience;
                        model.CandidateExperienceMonths = candata.CandidateExperience.Contains('.') ? candata.CandidateExperience.Split('.')[1] : "0";
                    }

                    model.CandidateLocation = candata.CandidateLocation;
                    canList.Add(model);
                }
                return Json(new { Error = false, Message = "CandidateByJob", Data = canList }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Error = true, Message = "Data not Found" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult JobView(string key)
        {
            JobVM J = new JobVM();
            if (key != null)
            {
                int id = Convert.ToInt32(key);
                var jobs = db.Jobs.Find(id);
                J.JobCount = jobs.JobCount;
                J.JobDescription = jobs.JobDescription;
                J.JobName = jobs.JobName;
                J.JobStatus = Convert.ToBoolean(jobs.JobStatus);
                DateTime dt = Convert.ToDateTime(jobs.JobTargetDate);
                J.JobTargetDate = (dt.ToString("dd-MMM-yyyy"));
                J.Location = jobs.Location;
                J.Contact_Person = jobs.Contact_Person;
                if (jobs.Client != null)
                {
                    J.ClientName = jobs.Client.ClientName;
                    J.uniqueJobid = jobs.Client.ClientName + "_" + J.JobName + "_" + J.JobTargetDate;
                }
            }
            return Json(new { Message = "JobView", Data = J }, JsonRequestBehavior.AllowGet);
        }
        public AccessPermission GetCurrentAccesspermission(string Entity)
        {
            string currentroleId = string.Empty;
            if (Session["CurrentRoleId"] != null)
            {
                currentroleId = Session["CurrentRoleId"].ToString();
            }
            var AccessData = (from Acc in db.AccessPermissions
                              join Ent in db.Entities on Acc.EntityId equals Ent.Id
                              where Ent.EntityName == Entity && Acc.RoleId == currentroleId
                              select Acc).SingleOrDefault();
            return AccessData;
        }
        #endregion
        #region Datatables

        [HttpPost]
        public ActionResult QuestionLoadData()
        {
            //jQuery DataTables Param
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            //Find paging info
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            //Find order columns info
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault()
                                    + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            //find search columns info
            var Searchquestion = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
            var SearchQuestiontype = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            using (vRecruitEntities dc = new vRecruitEntities())
            {
                dc.Configuration.LazyLoadingEnabled = false; // if your table is relational, contain foreign key
                int currentcompanyId = Convert.ToInt32(Session["currentCompanyId"]);
                IQueryable<Question> v;
                string currentrole = string.Empty;
                if (Session["CurrentRole"] != null)
                {
                    currentrole = Session["CurrentRole"].ToString();
                }
                if (currentrole == "SUPER ADMIN")
                {
                    v = (from a in dc.Questions select a);
                }
                else
                {
                    v = (from a in dc.Questions where a.IsActive == true && a.CompanyId == currentcompanyId || a.CompanyId == null select a);
                }
                //SEARCHING...
                if (!string.IsNullOrEmpty(SearchQuestiontype))
                {
                    v = v.Where(a => a.QuestionType.Contains(SearchQuestiontype));
                }
                if (!string.IsNullOrEmpty(Searchquestion))
                {
                    v = v.Where(a => a.Question1.Contains(Searchquestion));
                }
                //SORTING...  (For sorting we need to add a reference System.Linq.Dynamic)
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    v = v.OrderBy(sortColumn + " " + sortColumnDir);
                }
                recordsTotal = v.Count();
                var data = v.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                    JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult CandidateLoadData()
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault()
                                  + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var SearchText = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            using (vRecruitEntities dc = new vRecruitEntities())
            {
                dc.Configuration.LazyLoadingEnabled = false;
                int currentcompanyId = Convert.ToInt32(Session["currentCompanyId"]);
                IQueryable<Candidate> v;
                string currentrole = string.Empty;
                if (Session["CurrentRole"] != null)
                {
                    currentrole = Session["CurrentRole"].ToString();
                }
                if (currentrole == "SUPER ADMIN")
                {
                    v = (from a in dc.Candidates
                         .Include("Interviews")
                         select a);
                }
                else if (currentrole == "CLIENT")
                {
                    var currentClientID = Convert.ToInt32(Session["CurrentClientId"]);
                    v = (from a in dc.Candidates.Include("Interviews")
                         join cc in dc.ClientCandidateMappings on a.CandidateId equals cc.CandidateId
                         join cl in dc.Clients on cc.ClientId equals cl.ClientId
                         where cl.ClientId == currentClientID && a.IsActive == true
                         select a);
                    //where a.IsActive == true && a.CompanyId == currentcompanyId select a);
                }
                else
                {
                    v = (from a in dc.Candidates.Include("Interviews") where a.IsActive == true && a.CompanyId == currentcompanyId select a);
                }
                //SEARCHING...
                if (!string.IsNullOrEmpty(SearchText))
                {
                    v = v.Where(a => a.CandidateName.Contains(SearchText) || a.ClientDesignation.Contains(SearchText) || a.CandidateEmail.Contains(SearchText) || a.CandidatePhone.Contains(SearchText));
                }
                //if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir) && !sortColumn.Equals("uniqueJobid") && !sortColumn.Equals("ClientName") && !sortColumn.Equals("CompanyName"))
                if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir) && !sortColumn.Equals("CandidateId") && !sortColumn.Equals("CandidateName") && !sortColumn.Equals("ClientDesignation") && !sortColumn.Equals("CandidateEmail") && !sortColumn.Equals("CandidatePhone"))
                {
                    v = v.OrderBy(sortColumn + " " + sortColumnDir);
                    v = v.OrderBy("LastUpdateDate" + " " + "desc");
                }
                List<CandidateVM> canList = new List<CandidateVM>();
                foreach (var candata in v)
                {
                    CandidateVM model = new CandidateVM();
                    //var candata = db.Candidates.Where(x => x.CandidateId == item.Key).SingleOrDefault();
                    model.CandidateId = candata.CandidateId;
                    model.CandidateName = candata.CandidateName;
                    model.ClientDesignation = candata.ClientDesignation;
                    model.Education = candata.Education;
                    DateTime dt = Convert.ToDateTime(candata.CandidateDOB);
                    model.CandidateDOB = dt.ToString("dd-MMM-yyyy");
                    model.CandidateExperience = candata.CandidateExperience;
                    if (string.IsNullOrEmpty(candata.CandidateExperience))
                    {
                        model.CandidateExperienceYears = "";
                        model.CandidateExperienceMonths = "";
                    }
                    else
                    {
                        model.CandidateExperienceYears = candata.CandidateExperience.Contains('.') ? candata.CandidateExperience.Split('.')[0] : candata.CandidateExperience;
                        model.CandidateExperienceMonths = candata.CandidateExperience.Contains('.') ? candata.CandidateExperience.Split('.')[1] : "0";
                    }
                    model.CandidateLocation = candata.CandidateLocation;
                    model.CandidateEmail = candata.CandidateEmail;
                    model.CandidatePhone = candata.CandidatePhone;
                    model.CandidateResume = candata.CandidateResume;
                    model.CandidateSkill = candata.CandidateSkill;
                    model.CandidatePhone2 = candata.CandidatePhone2;
                    model.CandidateLastJobChange = candata.CandidateLastJobChange;
                    model.PreviousDesignation = candata.PreviousDesignation;
                    model.CurrentOrganisation = candata.CurrentOrganisation;
                    model.CurrentCTC = Convert.ToInt32(candata.CurrentCTC);
                    model.ExpectedCTC = Convert.ToInt32(candata.ExpectedCTC);
                    model.OffersInHand = Convert.ToInt32(candata.OffersInHand);
                    model.NoticePeriod = Convert.ToInt32(candata.NoticePeriod);
                    model.SkypeId = candata.SkypeId;
                    model.Remarks = candata.Remarks;
                    model.CandidateVideo = candata.CandidateVideo;
                    model.LastUpdateDateString = candata.LastUpdateDate != null ? candata.LastUpdateDate.Value.Date.ToString("dd/MM/yy") : "";
                    model.InterviewId = candata.Interviews.OrderByDescending(x => x.InterviewDate).Select(z => z.InterviewId).FirstOrDefault();
                    canList.Add(model);
                }
                recordsTotal = canList.Count();
                if (sortColumn.Equals("CandidateId"))
                {
                    if (sortColumnDir.Equals("asc"))
                    {
                        canList = canList.OrderBy(a => a.CandidateId).ToList();
                    }
                    if (sortColumnDir.Equals("desc"))
                    {
                        canList = canList.OrderByDescending(a => a.CandidateId).ToList();
                    }
                }
                if (sortColumn.Equals("CandidateName"))
                {
                    if (sortColumnDir.Equals("asc"))
                    {
                        canList = canList.OrderBy(a => a.CandidateName).ToList();
                    }
                    if (sortColumnDir.Equals("desc"))
                    {
                        canList = canList.OrderByDescending(a => a.CandidateName).ToList();
                    }
                }
                if (sortColumn.Equals("ClientDesignation"))
                {
                    if (sortColumnDir.Equals("asc"))
                    {
                        canList = canList.OrderBy(a => a.ClientDesignation).ToList();
                    }
                    if (sortColumnDir.Equals("desc"))
                    {
                        canList = canList.OrderByDescending(a => a.ClientDesignation).ToList();
                    }
                }
                if (sortColumn.Equals("CandidateEmail"))
                {
                    if (sortColumnDir.Equals("asc"))
                    {
                        canList = canList.OrderBy(a => a.CandidateEmail).ToList();
                    }
                    if (sortColumnDir.Equals("desc"))
                    {
                        canList = canList.OrderByDescending(a => a.CandidateEmail).ToList();
                    }
                }
                if (sortColumn.Equals("CandidatePhone"))
                {
                    if (sortColumnDir.Equals("asc"))
                    {
                        canList = canList.OrderBy(a => a.CandidatePhone).ToList();
                    }
                    if (sortColumnDir.Equals("desc"))
                    {
                        canList = canList.OrderByDescending(a => a.CandidatePhone).ToList();
                    }
                }
                var data = canList.Skip(skip).Take(pageSize);
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                    JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GetCountries()
        {
            var data = (from a in db.Countries select new { a.Id, a.Country1 }).ToList();
            return Json(new { data = data },
    JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult GetStats(int id = 0)
        {
            var data = (from a in db.States where a.CountryId == id select new { a.Id, a.State1 }).ToList();
            return Json(new { data = data },
    JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult GetCities(int id = 0)
        {
            var data = (from a in db.Cities where a.StateId == id select new { a.Id, a.City1 }).ToList();
            return Json(new { data = data },
    JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult ClientLoadData()
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault()
                                    + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var SearchText = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
            var SearchStatus = Request.Form.GetValues("columns[4][search][value]").FirstOrDefault();
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            using (vRecruitEntities dc = new vRecruitEntities())
            {
                dc.Configuration.LazyLoadingEnabled = false;
                int currentcompanyId = Convert.ToInt32(Session["currentCompanyId"]);
                IQueryable<Client> v;
                string currentrole = string.Empty;
                if (Session["CurrentRole"] != null)
                {
                    currentrole = Session["CurrentRole"].ToString();
                }
                if (currentrole == "SUPER ADMIN")
                {
                    v = (from a in dc.Clients select a);
                }
                else
                {
                    v = (from a in dc.Clients where a.CompanyId == currentcompanyId select a);
                }

                //SEARCHING...
                if (!string.IsNullOrEmpty(SearchText))
                {
                    v = v.Where(a => a.ClientName.Contains(SearchText) || a.ClientEmail.Contains(SearchText) || a.ClientPhone.Contains(SearchText) || a.ClientMobile.Contains(SearchText) || a.ClientGST.Contains(SearchText) || a.ClientAddress.Contains(SearchText));
                }
                if (!string.IsNullOrEmpty(SearchStatus))
                {
                    v = v.Where(a => a.IsActive.ToString().Contains(SearchStatus));
                }
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    v = v.OrderBy(sortColumn + " " + sortColumnDir);
                }
                //recordsTotal = v.Count();
                //var data = v.Skip(skip).Take(pageSize).ToList();
                var countries = db.Countries.ToList();
                List<ClientVM> lstclient = new List<ClientVM>();
                foreach (var item in v)
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
                    vm.UserId = db.AspNetUsers.Where(x => x.ClientId == vm.ClientId).Select(x => x.Id).FirstOrDefault();
                    vm.LastUpdateDate = item.LastUpdateDate;
                    vm.LastUpdateDateString = item.LastUpdateDate != null ? item.LastUpdateDate.Value.Date.ToString("dd/MM/yy") : "";
                    vm.ClientIsactive = item.IsActive == true ? "Active" : "Deactive";
                    vm.ClientCity = item.ClientCity;
                    var country = countries.Where(d => d.Id == item.ClientCountryId).FirstOrDefault();
                    if (country != null)
                    {
                        vm.CountryId = country.Id;
                        vm.CountryName = country.Country1;
                    }
                    lstclient.Add(vm);
                }

                recordsTotal = lstclient.Count();
                var data = lstclient.Skip(skip).Take(pageSize).ToList();

                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                    JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult CompanyLoadData()
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault()
                                    + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var SearchText = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            using (vRecruitEntities dc = new vRecruitEntities())
            {
                dc.Configuration.LazyLoadingEnabled = false;
                int currentcompanyId = Convert.ToInt32(Session["currentCompanyId"]);
                IQueryable<Company> v;
                string currentrole = string.Empty;
                if (Session["CurrentRole"] != null)
                {
                    currentrole = Session["CurrentRole"].ToString();
                }
                if (currentrole == "SUPER ADMIN")
                {
                    v = (from a in dc.Companies select a);
                    var vv = dc.Companies.ToList();
                }
                else
                {
                    v = (from a in dc.Companies where a.IsActive == true && a.ID == currentcompanyId select a);
                }
                //SEARCHING...
                // bool result = Convert.ToBoolean(SearchText);
                if (!string.IsNullOrEmpty(SearchText))
                {
                    v = v.Where(a => a.CompanyName.Contains(SearchText) || a.CompanyEmail.Contains(SearchText) || a.PhoneNumber.Contains(SearchText) || a.CompanyAddress.Contains(SearchText));
                }
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    v = v.OrderBy(sortColumn + " " + sortColumnDir);
                }
                //List<Company> lstcmp = new List<Company>();
                //foreach (var item in v)
                //{
                //    Company cmp = new Company();
                //    cmp.CompanyName = item.CompanyName;
                //    cmp.UserID = db.AspNetUsers.Where(x => x.Id == item.UserID).Select(x => x.UserName).SingleOrDefault();
                //    cmp.CompanyEmail = item.CompanyEmail;
                //    cmp.PhoneNumber = item.PhoneNumber;
                //    cmp.CompanyAddress = item.CompanyAddress;
                //    cmp.IsActive = item.IsActive;
                //    cmp.ID = item.ID;
                //    lstcmp.Add(cmp);
                //}
                List<CompaniesVM> lstcmp = new List<CompaniesVM>();
                var city = (from c in dc.Cities select c).ToList();
                var v2 = (from a in v.AsEnumerable()
                          from c in city
                          where c.Id == Convert.ToInt32(a.City)
                          select new
                          {
                              a.CompanyName,
                              a.CompanyEmail,
                              a.UserID,
                              a.PhoneNumber,
                              a.IsActive,
                              a.LastUpdateDate,
                              a.ID,
                              City = c.City1

                          });
                foreach (var item in v2)
                {
                    CompaniesVM cmp = new CompaniesVM();
                    cmp.CompanyName = item.CompanyName;
                    cmp.UserID = db.AspNetUsers.Where(x => x.Id == item.UserID).Select(x => x.UserName).SingleOrDefault();
                    cmp.CompanyEmail = item.CompanyEmail;
                    cmp.PhoneNumber = item.PhoneNumber;
                    cmp.IsActive = item.IsActive;
                    cmp.LastUpdateDateString = item.LastUpdateDate != null ? item.LastUpdateDate.Value.Date.ToString("dd/MM/yy") : "";
                    cmp.ID = item.ID;
                    cmp.City = item.City;
                    lstcmp.Add(cmp);
                }
                //v = v.Where(a => a.UserID == a.AspNetUser.UserName);
                recordsTotal = lstcmp.Count();
                var data = lstcmp.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                    JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult CompanyAdminLoadData()
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault()
                                    + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var SearchText = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            using (vRecruitEntities dc = new vRecruitEntities())
            {
                //dc.Configuration.LazyLoadingEnabled = false;
                int currentcompanyId = Convert.ToInt32(Session["currentCompanyId"]);
                var createdby = Session["CurrentUserId"].ToString();
                IQueryable<AspNetUser> v;
                //List<AspNetUser> userlist;
                string currentrole = string.Empty;
                if (Session["CurrentRole"] != null)
                {
                    currentrole = Session["CurrentRole"].ToString();
                }
                List<AspNetRole> roles = db.AspNetRoles.ToList();

                if (currentrole == "SUPER ADMIN")
                {
                    v = from user in dc.AspNetUsers
                        from role in user.AspNetRoles
                        where role.Name == "ADMIN" && user.ClientId == null
                        select user;
                }
                else
                {
                    v = from user in dc.AspNetUsers
                        from role in user.AspNetRoles
                        where role.Name == "ADMIN" && user.CreatedBy == createdby && user.ClientId == null
                        select user;
                }
                //SEARCHING...
                if (!string.IsNullOrEmpty(SearchText))
                {
                    v = v.Where(a => a.UserName.Contains(SearchText) || a.Email.Contains(SearchText) || a.PhoneNumber.Contains(SearchText));
                    //|| a.Companies.Where(x=>x.CompanyName.Contains(SearchText))
                }
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    v = v.OrderBy(sortColumn + " " + sortColumnDir);
                }
                List<UserViewModel> lstuser = new List<UserViewModel>();
                foreach (var item in v)
                {
                    UserViewModel user = new UserViewModel();
                    user.Id = item.Id;
                    user.UserName = item.UserName;
                    user.Email = item.Email;
                    user.CompanyName = item.Companies.Where(y => y.ID == item.CompanyId).Select(x => x.CompanyName).FirstOrDefault();
                    user.PhoneNumber = item.PhoneNumber;
                    user.RoleName = "ADMIN";
                    user.LastUpdateDate = item.LastUpdateDate != null ? item.LastUpdateDate.Value.Date.ToString("dd/MM/yy") : "";
                    lstuser.Add(user);
                }
                recordsTotal = v.Count();
                var data = lstuser.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                    JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult CompanyUserLoadData()
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault()
                                    + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var SearchText = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            using (vRecruitEntities dc = new vRecruitEntities())
            {
                // dc.Configuration.LazyLoadingEnabled = false;
                int currentcompanyId = Convert.ToInt32(Session["currentCompanyId"]);
                var createdby = Session["CurrentUserId"].ToString();
                IQueryable<AspNetUser> v;
                string currentrole = string.Empty;
                if (Session["CurrentRole"] != null)
                {
                    currentrole = Session["CurrentRole"].ToString();
                }
                List<AspNetRole> roles = db.AspNetRoles.ToList();
                if (currentrole == "SUPER ADMIN")
                {
                    v = from user in dc.AspNetUsers
                        from role in user.AspNetRoles
                        where role.Name != "ADMIN" && role.Name != "CLIENT" && role.Name != "SUPER ADMIN"
                        select user;
                }
                else
                {
                    v = from user in dc.AspNetUsers
                        from role in user.AspNetRoles
                        where role.Name != "ADMIN" && role.Name != "CLIENT" && role.Name != "SUPER ADMIN" && user.CompanyId == currentcompanyId
                        select user;
                }
                //SEARCHING...
                if (!string.IsNullOrEmpty(SearchText))
                {
                    v = v.Where(a => a.UserName.Contains(SearchText) || a.Email.Contains(SearchText) || a.PhoneNumber.Contains(SearchText));
                }
                //if (!string.IsNullOrEmpty(SearchText))
                //{
                //    v = v.Where(a => a.AspNetRoles.Where(x => x.Name.Contains(SearchText));
                //}
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    v = v.OrderBy(sortColumn + " " + sortColumnDir);
                }
                List<UserViewModel> lstuser = new List<UserViewModel>();
                foreach (var item in v)
                {
                    UserViewModel user = new UserViewModel();
                    user.Id = item.Id;
                    user.UserName = item.UserName;
                    user.Email = item.Email;
                    user.CompanyName = item.Companies.Select(x => x.CompanyName).FirstOrDefault();
                    user.PhoneNumber = item.PhoneNumber;
                    user.RoleName = item.AspNetRoles.Select(x => x.Name).FirstOrDefault();
                    user.LastUpdateDate = item.LastUpdateDate != null ? item.LastUpdateDate.Value.ToString("dd/MM/yy") : "";
                    lstuser.Add(user);
                }
                recordsTotal = v.Count();
                var data = lstuser.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                    JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult TestLoadData()
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault()
                                    + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var Searchquestion = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
            var searchSkill = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
            //var SearchQuestiontype = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            using (vRecruitEntities dc = new vRecruitEntities())
            {
                dc.Configuration.LazyLoadingEnabled = false; // if your table is relational, contain foreign key
                int currentcompanyId = Convert.ToInt32(Session["currentCompanyId"]);
                IQueryable<Question> v;
                string currentrole = string.Empty;
                if (Session["CurrentRole"] != null)
                {
                    currentrole = Session["CurrentRole"].ToString();
                }
                if (currentrole == "SUPER ADMIN")
                {
                    v = (from a in dc.Questions select a);
                }
                else
                {
                    v = (from a in dc.Questions where a.IsActive == true && a.CompanyId == currentcompanyId || a.CompanyId == null select a);
                }
                //SEARCHING...
                if (!string.IsNullOrEmpty(Searchquestion))
                {
                    v = v.Where(a => a.Question1.Contains(Searchquestion));
                }
                // SEARCHING FOR SKILL
                if (!string.IsNullOrEmpty(searchSkill))
                {
                    var skillId = Convert.ToInt32(searchSkill);
                    var skill = db.SkillMasters.Where(s => s.SkillId == skillId).FirstOrDefault();

                    v = v.Where(s => s.QuestionSkill == skill.SkillName || s.QuestionSkill == searchSkill);
                }


                //SORTING...  (For sorting we need to add a reference System.Linq.Dynamic)
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    v = v.OrderBy(sortColumn + " " + sortColumnDir);
                }
                recordsTotal = v.Count();
                var data = v.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                    JsonRequestBehavior.AllowGet);

            }
        }
        [HttpPost]
        public ActionResult JobLoadData()
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault()
                                    + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var SearchText = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
            var SearchStatus = Request.Form.GetValues("columns[4][search][value]").FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            using (vRecruitEntities dc = new vRecruitEntities())
            {
                //dc.Configuration.LazyLoadingEnabled = false;
                int currentcompanyId = Convert.ToInt32(Session["currentCompanyId"]);
                IQueryable<Job> v;
                string currentrole = string.Empty;
                if (Session["CurrentRole"] != null)
                {
                    currentrole = Session["CurrentRole"].ToString();
                }
                if (currentrole == "SUPER ADMIN")
                {
                    v = (from a in dc.Jobs select a);
                }
                else if (currentrole == "CLIENT")
                {
                    var currentClientID = Convert.ToInt32(Session["CurrentClientId"]);
                    v = (from a in dc.Jobs.Where(c => c.ClientId == currentClientID) select a);
                }
                else
                {
                    v = (from a in dc.Jobs where a.CompanyId == currentcompanyId select a);
                }
                //SEARCHING...
                if (!string.IsNullOrEmpty(SearchText))
                {
                    v = v.Where(a => a.JobName.Contains(SearchText) || a.JobCount.ToString().Contains(SearchText) || a.Client.ClientName.Contains(SearchText) || a.Client.Company.CompanyName.Contains(SearchText));
                }
                if (!string.IsNullOrEmpty(SearchStatus))
                {
                    v = v.Where(a => a.JobStatus.ToString().Contains(SearchStatus));
                }
                if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir) && !sortColumn.Equals("uniqueJobid") && !sortColumn.Equals("ClientName") && !sortColumn.Equals("CompanyName"))
                {
                    v = v.OrderBy(sortColumn + " " + sortColumnDir);
                }
                List<JobVM> joblst = new List<JobVM>();

                foreach (var item in v)
                {
                    JobVM J = new JobVM();
                    J.JobId = item.JobId;
                    J.JobCount = item.JobCount;
                    J.JobDescription = item.JobDescription;
                    J.JobName = item.JobName;
                    J.JobStatus = Convert.ToBoolean(item.JobStatus);
                    J.ClientName = item.Client.ClientName;
                    J.CompanyId = Convert.ToInt32(item.CompanyId);
                    var com = db.Companies.Where(c => c.ID == item.CompanyId).FirstOrDefault();
                    J.CompanyName = com != null ? com.CompanyName : ""; // item.Company.CompanyName;
                    DateTime dt = Convert.ToDateTime(item.JobTargetDate);
                    J.JobTargetDate = (dt.ToString("dd-MMM-yyyy"));
                    J.uniqueJobid = item.Client.ClientName + "_" + J.JobName + "_" + J.JobTargetDate;
                    J.LastUpdateDateString = item.LastUpdateDate != null ? item.LastUpdateDate.Value.Date.ToString("dd/MM/yy") : "";
                    joblst.Add(J);
                }
                recordsTotal = v.Count();

                if (sortColumn.Equals("CompanyName"))
                {
                    if (sortColumnDir.Equals("asc"))
                    {
                        joblst = joblst.OrderBy(a => a.CompanyName).ToList();
                    }
                    if (sortColumnDir.Equals("desc"))
                    {
                        joblst = joblst.OrderByDescending(a => a.CompanyName).ToList();
                    }
                }

                if (sortColumn.Equals("ClientName"))
                {
                    if (sortColumnDir.Equals("asc"))
                    {
                        joblst = joblst.OrderBy(a => a.ClientName).ToList();
                    }
                    if (sortColumnDir.Equals("desc"))
                    {
                        joblst = joblst.OrderByDescending(a => a.ClientName).ToList();
                    }
                }

                var data = joblst.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                    JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        [HttpGet]
        public ActionResult GetCandidateTestList(string key)
        {
            string currentrole = string.Empty;
            List<CandidateTestViewModel> lstTest = new List<CandidateTestViewModel>();
            List<CandidateTestResult> lstTest1 = new List<CandidateTestResult>();
            //if (Session["CurrentRole"] != null)
            //{
            //    currentrole = Session["CurrentRole"].ToString();
            //}
            //if (currentrole == "SUPER ADMIN")
            //{
            //    lstTest1 = db.CandidateTestResults.ToList();
            //}
            //else
            //{
            //    int canId = Convert.ToInt32(key);
            //    var lstTest11 = db.CandidateTestResults.Where(x => x.CandidateId == canId).GroupBy(x => x.SubmitDate).ToList();
            //}
            int canId = Convert.ToInt32(key);
            var Data = from c in db.CandidateTestResults.Where(x => x.CandidateId == canId)
                       join r in db.CandidateTestRecords.Where(cc => cc.CandidateId == canId) on c.CandidateId equals r.CandidateId
                       into p
                       from pr in p.DefaultIfEmpty()
                       where pr.TestName == c.TestName
                       group c by new
                       {
                           c.SubmitDate,
                           c.CandidateId,
                           c.TestGroupId,
                           c.TestName,
                           c.AssignBy,
                           pr.TestScore,
                           pr.TestStatus
                       } into gcs
                       select new CandidateTestViewModel()
                       {
                           SubmitDate = gcs.Key.SubmitDate.ToString(),
                           CandidateId = gcs.Key.CandidateId,
                           TestGroupId = gcs.Key.TestGroupId,
                           AssignBy = gcs.Key.AssignBy,
                           TestName = gcs.Key.TestName,
                           TestScore = gcs.Key.TestScore,
                           TestStatus = gcs.Key.TestStatus
                       };

            var d = Data.ToList();

            return Json(Data, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult getTestResult(string submitdate, string candidateid, string testgroupid)
        {
            List<QuestionAnswerVM> Data = new List<QuestionAnswerVM>();
            //  string Testname = "Quest";
            List<QuestionAnswerVM> queansList = new List<QuestionAnswerVM>();
            DateTime Sdate = Convert.ToDateTime(submitdate);
            int canid = Convert.ToInt32(candidateid);
            int grpid = Convert.ToInt32(testgroupid);
            var Datas = db.CandidateTestResults.Where(x => x.SubmitDate == Sdate && x.CandidateId == canid && x.TestGroupId == grpid).ToList();

            foreach (var item in Datas)
            {
                QuestionAnswerVM VM = new QuestionAnswerVM();
                VM.QuestionId = item.QuestionId;
                QuestionAnswerVM questionAnswerVM = Data.FirstOrDefault(x => x.QuestionId == item.QuestionId);
                VM.SelectedAnswerList = new List<string>();
                VM.CorrectAnswerList = new List<string>();
                if (questionAnswerVM != null)
                {
                    VM.Question = db.Questions.Where(x => x.QuestionId == item.QuestionId).Select(x => x.Question1).FirstOrDefault();
                    VM.SelectedAnswer = db.Answers.Where(x => x.AnswerId == item.CandidateAnswerId).Select(x => x.Answer1).FirstOrDefault();
                    questionAnswerVM.SelectedAnswerList.Add(VM.SelectedAnswer);
                    VM.CorrectAnswerList = db.Answers.Where(x => x.QuestiondId == item.QuestionId && x.AnswerCorrect == true).Select(x => x.Answer1).ToList();
                    //VM.CorrectAnswerList = db.Answers.Where(x => x.an == item.QuestionId).ToList();
                }
                else
                {
                    VM.Question = db.Questions.Where(x => x.QuestionId == item.QuestionId).Select(x => x.Question1).FirstOrDefault();
                    VM.SelectedAnswer = db.Answers.Where(x => x.AnswerId == item.CandidateAnswerId).Select(x => x.Answer1).FirstOrDefault();
                    VM.SelectedAnswerList.Add(VM.SelectedAnswer);
                    VM.CorrectAnswerList = db.Answers.Where(x => x.QuestiondId == item.QuestionId && x.AnswerCorrect == true).Select(x => x.Answer1).ToList();

                    Data.Add(VM);
                    //VM.SelectedAnswerList = db.Answers.Where(x => x.AnswerId == item.CandidateAnswerId).Select(x => x.Answer1).FirstOrDefault();
                    //VM.CorrectAnswerList = db.Answers.Where(x => x.an == item.QuestionId).ToList();
                }
            }
            return PartialView("_CandidateTestResutlt", Data);
        }

        [HttpPost]
        public ActionResult GetSkillData()
        {

            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault()
                                    + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var SearchText = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
            var SearchStatus = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            using (vRecruitEntities dc = new vRecruitEntities())
            {
                IQueryable<Job> v;
                IQueryable<SkillMaster> vv;
                IQueryable<SkillMasterModel> Skilmodel;
                string currentrole = string.Empty;
                //if (Session["CurrentRole"] != null)
                //{
                //    currentrole = Session["CurrentRole"].ToString();
                //}
                //if (currentrole == "SUPER ADMIN")
                //{
                //    v = (from a in dc.Jobs select a);
                //}
                //else
                //{
                //    v = (from a in dc.Jobs where a.CompanyId == currentcompanyId select a);
                //}

                vv = db.SkillMasters;

                //SEARCHING...
                if (!string.IsNullOrEmpty(SearchText))
                {
                    vv = vv.Where(a => a.SkillName.Contains(SearchText));
                }
                if (!string.IsNullOrEmpty(SearchStatus))
                {
                    bool status = SearchStatus == "Active" ? true : false;
                    vv = vv.Where(a => a.Active == status);
                }
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    vv = vv.OrderBy(sortColumn + " " + sortColumnDir);
                }


                List<SkillMasterModel> model = new List<SkillMasterModel>();
                foreach (var item in vv)
                {
                    SkillMasterModel sm = new SkillMasterModel();
                    sm.SkillId = item.SkillId;
                    sm.SkillName = item.SkillName;
                    sm.Active = item.Active;
                    sm.LastUpdateDate = item.LastUpdateDate;
                    sm.LastUpdateDateString = item.LastUpdateDate != null ? item.LastUpdateDate.Value.ToString("dd/MM/yy") : "";
                    model.Add(sm);
                }

                Skilmodel = model.AsQueryable();

                recordsTotal = vv.Count();
                var data = Skilmodel.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                    JsonRequestBehavior.AllowGet);

            }
        }

        [HttpPost]
        public ActionResult GetNextActionData()
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault()
                                    + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var SearchText = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            using (vRecruitEntities dc = new vRecruitEntities())
            {
                IQueryable<NextActionVM> NextActionData;
                IQueryable<NextAction> NextActionDbData;
                NextActionDbData = db.NextActions;
                NextActionData = NextActionDbData.Select(x => new NextActionVM()
                {
                    Id = x.Id,
                    Name = x.Name
                });

                if (!string.IsNullOrEmpty(SearchText))
                {
                    NextActionData = NextActionData.Where(a => a.Name.Contains(SearchText));
                }

                recordsTotal = NextActionData.Count();
                var data = NextActionData.ToList().Skip(skip).Take(pageSize);

                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                    JsonRequestBehavior.AllowGet);
            }
            //var data = NextActionData.Skip(skip).Take(pageSize).ToList();
        }

        [HttpPost]
        public ActionResult GetSendMessages()
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault()
                                    + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var SearchText = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            using (vRecruitEntities dc = new vRecruitEntities())
            {
                IQueryable<SendMesssageVM> SendMessageData;
                IQueryable<SendMesssage> SendMesageDbData;
                SendMesageDbData = db.SendMesssages;
                SendMessageData = SendMesageDbData.Select(x => new SendMesssageVM()
                {
                    Id = x.Id,
                    Name = x.Name
                });

                if (!string.IsNullOrEmpty(SearchText))
                {
                    SendMessageData = SendMessageData.Where(a => a.Name.Contains(SearchText));
                }

                recordsTotal = SendMessageData.Count();
                var data = SendMessageData.ToList().Skip(skip).Take(pageSize);

                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                    JsonRequestBehavior.AllowGet);
            }
            //var data = NextActionData.Skip(skip).Take(pageSize).ToList();
        }

        //[HttpGet]
        //public ActionResult TestStatus()
        //{
        //    CandidateTestStatusVM model = new CandidateTestStatusVM();

        //    return View(model);

        //    //var result = from r in db.CandidateTestRecords
        //    //             join c in db.Candidates on r.CandidateId equals c.CandidateId
        //    //             select new CandidateTestStatusVM
        //    //             {
        //    //                 CandidateId = (int)r.CandidateId,
        //    //                 CandidateName = c.CandidateName,
        //    //                 TestLink = r.TestLink,
        //    //                 TestName = r.TestName,
        //    //                 TestStatus = r.TestStatus,
        //    //                 TestScore = r.TestScore
        //    //             };
        //    //var R = result.ToList();
        //    //return View(result);
        //}
        //[HttpPost]
        //public ActionResult TestStatusLoadData()
        //{
        //    var draw = Request.Form.GetValues("draw").FirstOrDefault();
        //    var start = Request.Form.GetValues("start").FirstOrDefault();
        //    var length = Request.Form.GetValues("length").FirstOrDefault();

        //    int pageSize = length != null ? Convert.ToInt32(length) : 0;
        //    int skip = start != null ? Convert.ToInt32(start) : 0;
        //    int recordsTotal = 0;

        //    var result = (from r in db.CandidateTestRecords
        //                  join c in db.Candidates on r.CandidateId equals c.CandidateId
        //                  select new CandidateTestStatusVM
        //                  {
        //                      CandidateId = (int)r.CandidateId,
        //                      CandidateName = c.CandidateName,
        //                      TestLink = r.TestLink,
        //                      TestName = r.TestName,
        //                      TestStatus = r.TestStatus,
        //                      TestScore = r.TestScore
        //                  }).ToList();

        //    recordsTotal = result.Count();
        //    var data = result.Skip(skip).Take(pageSize).ToList();
        //    return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
        //        JsonRequestBehavior.AllowGet);
        //}

        [HttpGet]
        public ActionResult CompanyUserData(string companyId)
        {
            int companyToLook = Convert.ToInt32(this.Session["currentCompanyId"]);
            if (!string.IsNullOrEmpty(companyId))
                companyToLook = Convert.ToInt32(companyId);

            using (vRecruitEntities dc = new vRecruitEntities())
            {
                int currentcompanyId = Convert.ToInt32(Session["currentCompanyId"]);
                string currentUserId = Session["CurrentUserId"].ToString();
                string currentrole = string.Empty;
                if (Session["CurrentRole"] != null)
                    currentrole = this.Session["CurrentRole"].ToString();
                this.db.AspNetRoles.ToList<AspNetRole>();
                IQueryable<AspNetUser> v;
                if (currentrole == "SUPER ADMIN" || currentrole == "ADMIN" || currentrole == "MANAGER")
                {
                    v = from user in dc.AspNetUsers
                        from role in user.AspNetRoles
                        where role.Name != "SUPER ADMIN" && role.Name != "ADMIN" && role.Name != "CLIENT" && user.CompanyId == companyToLook
                        select user;
                }
                else if (currentrole == "USER" || currentrole == "TALENT ACQUISITION")
                {
                    v = from user in dc.AspNetUsers
                        where user.Id == currentUserId
                        select user;
                }
                else
                {
                    v = from user in dc.AspNetUsers
                        from role in user.AspNetRoles
                        where role.Name == "CLIENT" && user.CompanyId == companyToLook
                        select user;
                }
                List<UserViewModel> source = new List<UserViewModel>();
                foreach (AspNetUser aspNetUser in (IEnumerable<AspNetUser>)v)
                {
                    UserViewModel userViewModel1 = new UserViewModel();
                    userViewModel1.Id = aspNetUser.Id;
                    userViewModel1.UserName = aspNetUser.UserName;
                    userViewModel1.Email = aspNetUser.Email;
                    userViewModel1.CompanyName = aspNetUser.Companies.Select<Company, string>((Func<Company, string>)(x => x.CompanyName)).FirstOrDefault<string>();
                    userViewModel1.PhoneNumber = aspNetUser.PhoneNumber;
                    userViewModel1.RoleName = aspNetUser.AspNetRoles.Select<AspNetRole, string>((Func<AspNetRole, string>)(x => x.Name)).FirstOrDefault<string>();
                    UserViewModel userViewModel2 = userViewModel1;
                    DateTime? lastUpdateDate = aspNetUser.LastUpdateDate;
                    string str;
                    if (!lastUpdateDate.HasValue)
                    {
                        str = "";
                    }
                    else
                    {
                        lastUpdateDate = aspNetUser.LastUpdateDate;
                        str = lastUpdateDate.Value.ToString("dd/MM/yy");
                    }
                    userViewModel2.LastUpdateDate = str;
                    source.Add(userViewModel1);
                }
                return (ActionResult)this.Json((object)source.ToList<UserViewModel>(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult ClientsData(string companyId)
        {
            int companyToLook = Convert.ToInt32(this.Session["currentCompanyId"]);
            if (!string.IsNullOrEmpty(companyId))
                companyToLook = Convert.ToInt32(companyId);

            int currentClient = Convert.ToInt32(Session["CurrentClientId"]);

            using (vRecruitEntities vRecruitEntities = new vRecruitEntities())
            {
                vRecruitEntities.Configuration.LazyLoadingEnabled = false;
                int currentcompanyId = Convert.ToInt32(this.Session["currentCompanyId"]);
                string currentrole = string.Empty;
                if (this.Session["CurrentRole"] != null)
                    currentrole = this.Session["CurrentRole"].ToString();
                IQueryable<Client> queryable;
                if (currentrole != "CLIENT")
                {
                    queryable = (from a in vRecruitEntities.Clients.OrderBy(a => a.ClientName) where a.CompanyId == companyToLook select a);
                }
                else
                {
                    queryable = (from a in vRecruitEntities.Clients.OrderBy(a => a.ClientName) where a.ClientId == currentClient select a);
                }
                List<ClientVM> source = new List<ClientVM>();
                foreach (Client client in queryable)
                {
                    ClientVM vm = new ClientVM();
                    vm.ClientId = client.ClientId;
                    vm.ClientName = client.ClientName;
                    vm.ClientAddress = client.ClientAddress;
                    vm.ClientEmail = client.ClientEmail;
                    vm.ClientGST = client.ClientGST;
                    vm.ClientMobile = client.ClientMobile;
                    vm.ClientPhone = client.ClientPhone;
                    ClientVM clientVm1 = vm;
                    bool? isActive = client.IsActive;
                    string str1 = isActive.ToString();
                    clientVm1.ClientIsactive = str1;
                    vm.CompanyId = client.CompanyId;
                    vm.LastUpdateBy = vm.LastUpdateBy;
                    vm.UserId = vRecruitEntities.AspNetUsers.Where(x => x.ClientId == client.ClientId).Select(x => x.Id).FirstOrDefault();
                    vm.LastUpdateDate = client.LastUpdateDate;
                    ClientVM clientVm2 = vm;
                    DateTime? lastUpdateDate = client.LastUpdateDate;
                    string str2;
                    if (!lastUpdateDate.HasValue)
                    {
                        str2 = "";
                    }
                    else
                    {
                        lastUpdateDate = client.LastUpdateDate;
                        DateTime date = lastUpdateDate.Value;
                        date = date.Date;
                        str2 = date.ToString("dd/MM/yy");
                    }
                    clientVm2.LastUpdateDateString = str2;
                    ClientVM clientVm3 = vm;
                    isActive = client.IsActive;
                    bool flag = true;
                    string str3 = isActive.GetValueOrDefault() == flag & isActive.HasValue ? "Active" : "Deactive";
                    clientVm3.ClientIsactive = str3;
                    source.Add(vm);
                }
                return (ActionResult)this.Json((object)source.ToList<ClientVM>(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult ClientGridData(
          string submitdate,
          string companyId,
          string clientId,
          string activeId,
          string updatedBy)
        {
            using (vRecruitEntities vRecruitEntities = new vRecruitEntities())
            {
                int companyToLook = Convert.ToInt32(this.Session["currentCompanyId"]);
                int currentClient = Convert.ToInt32(Session["CurrentClientId"]);
                if (!string.IsNullOrEmpty(companyId))
                    companyToLook = Convert.ToInt32(companyId);
                List<Job> outer = new List<Job>();
                List<Job> jobList1 = new List<Job>();
                List<Job> jobList2 = new List<Job>();
                int num1 = 0;


                int num2 = vRecruitEntities.Activities.Where(a => a.Companyid == companyToLook && a.ActivityType == "Shortlisted").Count();
                int num3 = vRecruitEntities.Activities.Where(a => a.Companyid == companyToLook && a.ActivityType == "INTERVIEW ROUND 1").Count();
                int num4 = vRecruitEntities.Activities.Where(a => a.Companyid == companyToLook && a.ActivityType == "JOINED").Count();
                int num5 = vRecruitEntities.Activities.Where(a => a.Companyid == companyToLook && a.ActivityType == "REJECTED").Count();
                int totalClients = vRecruitEntities.Clients.Where(a => a.CompanyId == companyToLook).Count();
                string empty = string.Empty;
                if (this.Session["CurrentRole"] != null)
                    empty = this.Session["CurrentRole"].ToString();
                if (empty == "SUPER ADMIN")
                {
                    num1 = vRecruitEntities.Candidates.Where(a => a.CompanyId == companyToLook).Count();
                    IQueryable<Job> queryable = vRecruitEntities.Jobs.Where(a => a.CompanyId == companyToLook);
                    jobList2.AddRange((IEnumerable<Job>)queryable);
                }
                else if (empty == "CLIENT")
                {

                    num1 = (from c in db.Candidates
                            join cm in db.ClientCandidateMappings on c.CandidateId equals cm.CandidateId
                            where cm.ClientId == currentClient
                            select c).Count();
                    num2 = vRecruitEntities.Activities.Where(a => a.ClientId == currentClient && a.ActivityType == "Shortlisted").Count();
                    num3 = vRecruitEntities.Activities.Where(a => a.ClientId == currentClient && a.ActivityType == "INTERVIEW ROUND 1").Count();
                    num4 = vRecruitEntities.Activities.Where(a => a.ClientId == currentClient && a.ActivityType == "JOINED").Count();
                    num5 = vRecruitEntities.Activities.Where(a => a.ClientId == currentClient && a.ActivityType == "REJECTED").Count();
                    totalClients = vRecruitEntities.Activities.Where(a => a.ClientId == currentClient).Count();
                }
                if (empty == "SUPER ADMIN" && clientId != "")
                {
                    num1 = num2 = num3 = num4 = num5 = 0;
                    outer.Clear();
                    string str1 = clientId;
                    char[] chArray = new char[1] { ',' };
                    foreach (string str2 in str1.Split(chArray))
                    {
                        int receivedClientId = Convert.ToInt32(str2);
                        IQueryable<Job> queryable = vRecruitEntities.Jobs.Where(a => a.ClientId == receivedClientId && a.CompanyId == companyToLook);
                        string userId = str2;
                        num1 += vRecruitEntities.ClientCandidateMappings.Where(a => a.ClientId == receivedClientId).Count();
                        //num2 += vRecruitEntities.Activities.Where(a => a.Companyid == companyToLook && a.UserId == newGuid && a.ActivityType == "Shortlisted").Count();
                        //num3 += vRecruitEntities.Activities.Where(a => a.Companyid == companyToLook && a.UserId == newGuid && a.ActivityType == "INTERVIEW ROUND 1").Count();
                        //num4 += vRecruitEntities.Activities.Where(a => a.Companyid == companyToLook && a.UserId == newGuid && a.ActivityType == "JOINED").Count();
                        //num5 += vRecruitEntities.Activities.Where(a => a.Companyid == companyToLook && a.UserId == newGuid && a.ActivityType == "REJECTED").Count();
                        outer.AddRange((IEnumerable<Job>)queryable);
                    }
                    vRecruitEntities.Jobs.ToList<Job>();
                }
                if (empty == "SUPER ADMIN" && updatedBy != "")
                {
                    int num6;
                    num5 = num6 = 0;
                    num4 = num6;
                    num3 = num6;
                    num2 = num6;
                    num1 = num6;
                    totalClients = num6;

                    jobList1.Clear();
                    string str1 = updatedBy;
                    char[] chArray = new char[1] { ',' };
                    foreach (string str2 in str1.Split(chArray))
                    {
                        string userId = str2;
                        IQueryable<Job> queryable = vRecruitEntities.Jobs.Where(a => a.LastUpdatedBy == userId && a.CompanyId == companyToLook);
                        Guid newGuid = Guid.Parse(userId);
                        num1 += vRecruitEntities.Candidates.Where(a => a.LastUpdateBy == newGuid && a.CompanyId == companyToLook).Count();
                        num2 += vRecruitEntities.Activities.Where(a => a.Companyid == companyToLook && a.UserId == newGuid && a.ActivityType == "Shortlisted").Count();
                        num3 += vRecruitEntities.Activities.Where(a => a.Companyid == companyToLook && a.UserId == newGuid && a.ActivityType == "INTERVIEW ROUND 1").Count();
                        num4 += vRecruitEntities.Activities.Where(a => a.Companyid == companyToLook && a.UserId == newGuid && a.ActivityType == "JOINED").Count();
                        num5 += vRecruitEntities.Activities.Where(a => a.Companyid == companyToLook && a.UserId == newGuid && a.ActivityType == "REJECTED").Count();
                        totalClients += vRecruitEntities.Clients.Where(a => a.LastUpdateBy == newGuid && a.CompanyId == companyToLook).Count();

                        jobList1.AddRange((IEnumerable<Job>)queryable);
                    }
                }
                if (string.IsNullOrEmpty(clientId) && string.IsNullOrEmpty(updatedBy))
                {
                    jobList2.Clear();
                    IQueryable<Job> queryable = vRecruitEntities.Jobs.Where(a => a.CompanyId == companyToLook);
                    jobList2.AddRange((IEnumerable<Job>)queryable);
                }
                List<JobVM> source = new List<JobVM>();
                if (!string.IsNullOrEmpty(clientId) || !string.IsNullOrEmpty(updatedBy))
                {
                    jobList2.Clear();
                    jobList2.AddRange((IEnumerable<Job>)outer);
                    jobList2.AddRange((IEnumerable<Job>)jobList1);
                }
                if (empty == "CLIENT")
                {
                    jobList2.Clear();
                    IQueryable<Job> queryable = vRecruitEntities.Jobs.Where(a => a.ClientId == currentClient);
                    jobList2.AddRange((IEnumerable<Job>)queryable);
                }
                if (!string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(updatedBy))
                    jobList2 = outer.Join<Job, Job, int, Job>((IEnumerable<Job>)jobList1, (Func<Job, int>)(p => p.JobId), (Func<Job, int>)(o => o.JobId), (Func<Job, Job, Job>)((p, o) => p)).ToList<Job>();

                if (activeId != "" && activeId != "undefined")
                {
                    if (activeId == "1")
                        jobList2 = jobList2.Where(x => x.JobStatus == true).ToList();
                    else
                        jobList2 = jobList2.Where(x => x.JobStatus == false).ToList();
                }

                foreach (Job job in jobList2)
                {
                    Job item = job;
                    JobVM jobVm1 = new JobVM();
                    jobVm1.JobId = item.JobId;
                    jobVm1.JobCount = item.JobCount;
                    jobVm1.JobDescription = item.JobDescription;
                    jobVm1.JobName = item.JobName;
                    bool boolean = Convert.ToBoolean((object)item.JobStatus);
                    jobVm1.JobStatusString = boolean ? "Active" : "In Active";
                    jobVm1.ClientName = item.Client.ClientName;
                    jobVm1.CompanyId = Convert.ToInt32((object)item.CompanyId);
                    Company company = this.db.Companies.Where(c => c.ID == item.CompanyId).FirstOrDefault();
                    jobVm1.CompanyName = company != null ? company.CompanyName : "";
                    DateTime dateTime = Convert.ToDateTime((object)item.JobTargetDate);
                    jobVm1.JobTargetDate = dateTime.ToString("dd-MMM-yyyy");
                    jobVm1.uniqueJobid = item.Client.ClientName + "_" + jobVm1.JobName + "_" + jobVm1.JobTargetDate;
                    JobVM jobVm2 = jobVm1;
                    string str;
                    if (!item.LastUpdateDate.HasValue)
                    {
                        str = "";
                    }
                    else
                    {
                        DateTime date = item.LastUpdateDate.Value;
                        date = date.Date;
                        str = date.ToString("dd/MM/yy");
                    }
                    jobVm2.LastUpdateDateString = str;
                    jobVm1.LastUpdatedByName = this.db.AspNetUsers.Where(c => c.Id == item.LastUpdatedBy).Select(x => x.UserName).SingleOrDefault();
                    source.Add(jobVm1);
                }

                int? totalPositions = 0;
                if (source.Count > 0)
                {
                    totalPositions = source.Select(x => x.JobCount).Sum();
                }
                var data = new
                {
                    Job = source.ToList<JobVM>(),
                    ID = num1,
                    ShortListedCount = num2,
                    interviewedCount = num3,
                    joinedCount = num4,
                    rejectedCount = num5,
                    totalClients = totalClients,
                    totalPositions = totalPositions,
                };
                return (ActionResult)this.Json((object)data, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult CompanyData()
        {
            using (vRecruitEntities vRecruitEntities = new vRecruitEntities())
            {
                vRecruitEntities.Configuration.LazyLoadingEnabled = false;
                int currentcompanyId = Convert.ToInt32(this.Session["currentCompanyId"]);
                string empty = string.Empty;
                if (this.Session["CurrentRole"] != null)
                    empty = this.Session["CurrentRole"].ToString();
                IQueryable<Company> queryable;
                if (empty == "SUPER ADMIN")
                {
                    queryable = vRecruitEntities.Companies.Select(a => a);
                    vRecruitEntities.Companies.ToList<Company>();
                }
                else
                    queryable = vRecruitEntities.Companies.Where(a => a.IsActive == (bool?)true && a.ID == currentcompanyId);
                List<CompaniesVM> source = new List<CompaniesVM>();
                foreach (Company company in (IEnumerable<Company>)queryable)
                {
                    Company item = company;
                    CompaniesVM companiesVm1 = new CompaniesVM();
                    companiesVm1.CompanyName = item.CompanyName;
                    companiesVm1.UserID = this.db.AspNetUsers.Where(x => x.Id == item.UserID).Select(x => x.UserName).SingleOrDefault();
                    companiesVm1.CompanyEmail = item.CompanyEmail;
                    companiesVm1.PhoneNumber = item.PhoneNumber;
                    companiesVm1.CompanyAddress = item.CompanyAddress;
                    companiesVm1.IsActive = item.IsActive;
                    CompaniesVM companiesVm2 = companiesVm1;
                    string str;
                    if (!item.LastUpdateDate.HasValue)
                    {
                        str = "";
                    }
                    else
                    {
                        DateTime date = item.LastUpdateDate.Value;
                        date = date.Date;
                        str = date.ToString("dd/MM/yy");
                    }
                    companiesVm2.LastUpdateDateString = str;
                    companiesVm1.ID = item.ID;
                    source.Add(companiesVm1);
                }
                return (ActionResult)this.Json((object)source.ToList<CompaniesVM>(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult ReportJobData(
          string submitdate,
          string clientId,
          string updatedBy)
        {
            using (vRecruitEntities vRecruitEntities = new vRecruitEntities())
            {
                vRecruitEntities.Configuration.LazyLoadingEnabled = false;
                int currentcompanyId = Convert.ToInt32(this.Session["currentCompanyId"]);
                string empty = string.Empty;
                if (this.Session["CurrentRole"] != null)
                    empty = this.Session["CurrentRole"].ToString();
                List<Job> jobList = new List<Job>();
                if (empty == "SUPER ADMIN")
                {
                    IQueryable<Job> queryable = vRecruitEntities.Jobs.Select(a => a);
                    jobList.AddRange((IEnumerable<Job>)queryable);
                    vRecruitEntities.Jobs.ToList<Job>();
                }
                if (empty == "SUPER ADMIN" && clientId != "")
                {
                    jobList.Clear();
                    string str1 = clientId;
                    char[] chArray = new char[1] { ',' };
                    foreach (string str2 in str1.Split(chArray))
                    {
                        int receivedClientId = Convert.ToInt32(str2);
                        IQueryable<Job> queryable = vRecruitEntities.Jobs.Where(a => a.ClientId == receivedClientId);
                        jobList.AddRange((IEnumerable<Job>)queryable);
                    }
                }
                else
                {
                    jobList.Clear();
                    IQueryable<Job> queryable = vRecruitEntities.Jobs.Where(a => a.JobStatus == true && a.CompanyId == currentcompanyId);
                    jobList.AddRange((IEnumerable<Job>)queryable);
                }
                List<JobViewModel> source = new List<JobViewModel>();
                foreach (Job job in jobList)
                {
                    Job item = job;
                    JobViewModel jobViewModel = new JobViewModel();
                    jobViewModel.JobId = item.JobId;
                    jobViewModel.JobCount = item.JobCount;
                    jobViewModel.JobDescription = item.JobDescription;
                    jobViewModel.JobName = item.JobName;
                    bool? jobStatus = item.JobStatus;
                    bool flag = true;
                    jobViewModel.JobStatus = !(jobStatus.GetValueOrDefault() == flag & jobStatus.HasValue) ? "In-Active" : "Active";
                    jobViewModel.ClientName = this.db.Clients.Where(x => x.ClientId == item.ClientId).Select(x => x.ClientName).SingleOrDefault();
                    jobViewModel.CompanyId = Convert.ToInt32((object)item.CompanyId);
                    jobViewModel.CompanyName = this.db.Companies.Where(x => x.ID == item.CompanyId).Select(x => x.CompanyName).SingleOrDefault();
                    source.Add(jobViewModel);
                }
                return (ActionResult)this.Json((object)source.ToList<JobViewModel>(), JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public ActionResult CandidateClientMapping()
        {
            using (vRecruitEntities vRecruitEntities = new vRecruitEntities())
            {
                vRecruitEntities.Configuration.LazyLoadingEnabled = false;
                int currentcompanyId = Convert.ToInt32(this.Session["currentCompanyId"]);
                string empty = string.Empty;
                if (this.Session["CurrentRole"] != null)
                    empty = this.Session["CurrentRole"].ToString();
                IQueryable<ClientCandidateMapping> queryable;
                if (empty == "SUPER ADMIN")
                {
                    queryable = vRecruitEntities.ClientCandidateMappings.Select(a => a);
                    vRecruitEntities.ClientCandidateMappings.ToList<ClientCandidateMapping>();
                }
                else
                    queryable = vRecruitEntities.ClientCandidateMappings.Where(a => a.ClientId == currentcompanyId);
                List<ClientCandidateMappingVM> source = new List<ClientCandidateMappingVM>();
                foreach (ClientCandidateMapping item in queryable)
                {
                    ClientCandidateMappingVM ClientCandidateMappingVm1 = new ClientCandidateMappingVM();
                    ClientCandidateMappingVm1.Id = item.Id;
                    ClientCandidateMappingVm1.ClientId = item.ClientId;
                    ClientCandidateMappingVm1.CandidateId = item.CandidateId;
                    source.Add(ClientCandidateMappingVm1);
                }
                return (ActionResult)this.Json((object)source.ToList<ClientCandidateMappingVM>(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult CandidateClient(string clientId, string candidateId)
        {
            if (!string.IsNullOrEmpty(clientId))
            {
                using (vRecruitEntities vRecruitEntities = new vRecruitEntities())
                {
                    vRecruitEntities.Configuration.LazyLoadingEnabled = false;
                    int currentcompanyId = Convert.ToInt32(this.Session["currentCompanyId"]);
                    string empty = string.Empty;
                    if (this.Session["CurrentRole"] != null)
                        empty = this.Session["CurrentRole"].ToString();
                    List<ClientCandidateMapping> clientCandidateMappingList = new List<ClientCandidateMapping>();

                    if (clientId != "" && !string.IsNullOrEmpty(candidateId))
                    {
                        clientCandidateMappingList.Clear();
                        string str1 = clientId;
                        char[] chArray = new char[1] { ',' };
                        foreach (string str2 in str1.Split(chArray))
                        {
                            int receivedClientId = Convert.ToInt32(str2);
                            ClientCandidateMapping data = new ClientCandidateMapping();
                            data.ClientId = receivedClientId;
                            data.CandidateId = Convert.ToInt32(candidateId);
                            clientCandidateMappingList.Add(data);
                        }
                        vRecruitEntities.Jobs.ToList<Job>();
                    }

                    int? canId = Convert.ToInt32(candidateId);
                    db.ClientCandidateMappings.RemoveRange(db.ClientCandidateMappings.Where(x => x.CandidateId == canId));
                    db.ClientCandidateMappings.AddRange(clientCandidateMappingList);
                    db.SaveChanges();

                    List<ClientCandidateMappingVM> source = new List<ClientCandidateMappingVM>();
                    //return Created(clientCandidateMapping);
                    return (ActionResult)this.Json((object)source.ToList<ClientCandidateMappingVM>(), JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json("Data Not Found...!!!");
            }
        }


        [HttpGet]
        public ActionResult CandidateGridData(string clientId, string companyId, string userId, string activeId)
        {
            using (vRecruitEntities dc = new vRecruitEntities())
            {
                int companyToLook = Convert.ToInt32(this.Session["currentCompanyId"]);
                if (!string.IsNullOrEmpty(companyId))
                    companyToLook = Convert.ToInt32(companyId);

                dc.Configuration.LazyLoadingEnabled = false;
                int currentcompanyId = Convert.ToInt32(Session["currentCompanyId"]);
                IQueryable<Candidate> v;
                IQueryable<ClientCandidateMapping> clientCandidateId;
                List<ClientCandidateMapping> ClientCandidateMapping1 = new List<ClientCandidateMapping>();
                List<Candidate> CandidateList = new List<Candidate>();
                List<Candidate> clientsCandidate = new List<Candidate>();
                List<Candidate> CandidateUserList = new List<Candidate>();


                List<CandidateVM> canList = new List<CandidateVM>();

                string currentrole = string.Empty;
                if (Session["CurrentRole"] != null)
                {
                    currentrole = Session["CurrentRole"].ToString();
                }
                if (currentrole == "SUPER ADMIN")
                {
                    v = (from a in dc.Candidates where a.CandidateName != null select a);
                    CandidateList.AddRange(v);
                }
                if (currentrole == "SUPER ADMIN" && !string.IsNullOrEmpty(clientId))
                {
                    CandidateList.Clear();
                    clientsCandidate.Clear();
                    string str1 = clientId;
                    char[] chArray = new char[1] { ',' };
                    foreach (string str2 in str1.Split(chArray))
                    {
                        int receivedClientId = Convert.ToInt32(str2);
                        clientCandidateId = (from a in dc.ClientCandidateMappings where a.ClientId == receivedClientId select a);
                        //ClientCandidateMapping1.AddRange(clientCandidateId);
                        foreach (var candat in clientCandidateId)
                        {
                            v = (from a in dc.Candidates where a.CandidateId == candat.CandidateId select a);
                            clientsCandidate.AddRange(v);
                        }
                    }
                }
                if (currentrole == "SUPER ADMIN" && !string.IsNullOrEmpty(userId))
                {
                    CandidateList.Clear();
                    CandidateUserList.Clear();
                    string str1 = userId;
                    char[] chArray = new char[1] { ',' };
                    foreach (string str2 in str1.Split(chArray))
                    {
                        Guid receivedUserId = Guid.Parse(str2);
                        IQueryable<Candidate> queryable = dc.Candidates.Where(a => a.LastUpdateBy == receivedUserId && a.CompanyId == companyToLook);
                        CandidateUserList.AddRange(queryable);
                    }
                }
                if (string.IsNullOrEmpty(clientId) && string.IsNullOrEmpty(userId))
                {
                    CandidateList.Clear();
                    IQueryable<Candidate> queryable = dc.Candidates.Where(a => a.CompanyId == companyToLook && a.CandidateName != null);
                    CandidateList.AddRange((IEnumerable<Candidate>)queryable);
                }
                if (!string.IsNullOrEmpty(clientId) || !string.IsNullOrEmpty(userId))
                {
                    CandidateList.Clear();
                    CandidateList.AddRange((IEnumerable<Candidate>)clientsCandidate);
                    CandidateList.AddRange((IEnumerable<Candidate>)CandidateUserList);
                }
                if (!string.IsNullOrEmpty(clientId) && !string.IsNullOrEmpty(userId))
                    CandidateList = clientsCandidate.Join<Candidate, Candidate, int, Candidate>((IEnumerable<Candidate>)CandidateUserList, (Func<Candidate, int>)(p => p.CandidateId), (Func<Candidate, int>)(o => o.CandidateId), (Func<Candidate, Candidate, Candidate>)((p, o) => p)).ToList<Candidate>();
                //else
                //{
                //    CandidateList.Clear();
                //       v = (from a in dc.Candidates where a.IsActive == true && a.CompanyId == currentcompanyId select a);
                //    CandidateList.AddRange(v);
                //}
                if (currentrole == "CLIENT")
                {
                    CandidateList.Clear();
                    CandidateUserList.Clear();
                    try
                    {
                        string str1 = userId;
                        char[] chArray = new char[1] { ',' };
                        int? currentClientId = Convert.ToInt32(Session["CurrentClientId"]);

                        foreach (string str2 in str1.Split(chArray))
                        {
                            //Guid receivedUserId = Guid.Parse(str2);
                            var candies = (from c in db.Candidates
                                           join dm in db.ClientCandidateMappings on c.CandidateId equals dm.CandidateId
                                           where dm.ClientId == currentClientId
                                           select c).ToList();

                            CandidateList = candies;
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }

                if (activeId != "" && activeId != "undefined")
                {
                    if (activeId == "1")
                        CandidateList = CandidateList.Where(x => x.IsActive == true).ToList();
                    else
                        CandidateList = CandidateList.Where(x => x.IsActive == false).ToList();
                }

                foreach (var candata in CandidateList)
                {
                    CandidateVM model = new CandidateVM();
                    //var candata = db.Candidates.Where(x => x.CandidateId == item.Key).SingleOrDefault();
                    model.CandidateId = candata.CandidateId;
                    model.CandidateName = candata.CandidateName;
                    model.ClientDesignation = candata.ClientDesignation;
                    model.Education = candata.Education;
                    DateTime dt = Convert.ToDateTime(candata.CandidateDOB);
                    model.CandidateDOB = dt.ToString("dd-MMM-yyyy");
                    model.CandidateExperience = candata.CandidateExperience;

                    if (string.IsNullOrEmpty(candata.CandidateExperience))
                    {
                        model.CandidateExperienceYears = "";
                        model.CandidateExperienceMonths = "";
                    }
                    else
                    {
                        model.CandidateExperienceYears = candata.CandidateExperience.Contains('.') ? candata.CandidateExperience.Split('.')[0] : candata.CandidateExperience;
                        model.CandidateExperienceMonths = candata.CandidateExperience.Contains('.') ? candata.CandidateExperience.Split('.')[1] : "0";
                    }

                    model.CandidateLocation = candata.CandidateLocation;
                    model.CandidateEmail = candata.CandidateEmail;
                    model.CandidatePhone = candata.CandidatePhone;
                    model.CandidateResume = candata.CandidateResume;
                    model.CandidateSkill = candata.CandidateSkill;
                    model.CandidatePhone2 = candata.CandidatePhone2;
                    model.CandidateLastJobChange = candata.CandidateLastJobChange;
                    model.PreviousDesignation = candata.PreviousDesignation;
                    model.CurrentOrganisation = candata.CurrentOrganisation;
                    model.CurrentCTC = Convert.ToInt32(candata.CurrentCTC);
                    model.ExpectedCTC = Convert.ToInt32(candata.ExpectedCTC);
                    model.OffersInHand = Convert.ToInt32(candata.OffersInHand);
                    model.NoticePeriod = Convert.ToInt32(candata.NoticePeriod);
                    model.SkypeId = candata.SkypeId;
                    model.Remarks = candata.Remarks;

                    model.LastUpdateDateString = candata.LastUpdateDate != null ? candata.LastUpdateDate.Value.Date.ToString("dd/MM/yy") : "";
                    model.LastUpdateDate = candata.LastUpdateDate;
                    canList.Add(model);
                }
                var data = canList.AsEnumerable().OrderByDescending(c => c.LastUpdateDate);
                //var data = canList.ToList();
                return Json(data,
                JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult GetActivityClients()
        {
            int companyToLook = Convert.ToInt32(this.Session["currentCompanyId"]);
            string currentRole = string.Empty;
            if (this.Session["CurrentRole"] != null)
                currentRole = this.Session["CurrentRole"].ToString();
            var result = (System.Collections.IList)null;

            if (currentRole == "SUPER ADMIN")
            {
                result = (from a in db.Clients.OrderBy(a => a.ClientName)
                          select new { a.ClientId, a.ClientName }).ToList();
            }
            else if (currentRole == "CLIENT")
            {
                int currentClientId = Convert.ToInt32(Session["CurrentClientId"]);
                result = (from a in db.Clients.Where(c => c.ClientId == currentClientId).OrderBy(a => a.ClientName)
                          select new { a.ClientId, a.ClientName }).ToList();
            }
            else
            {
                result = (from a in db.Clients.OrderBy(a => a.ClientName)
                          where a.CompanyId == companyToLook
                          select new { a.ClientId, a.ClientName }).ToList();
            }
            if (result != null)
            {
                return Json(new { Message = "GetClients", Data = result }, JsonRequestBehavior.AllowGet);
            }

            else
            {
                return Json("Data Not Found...!!!");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult UploadCandVideo()
        {
            if (Request.Files.Count > 0)
            {
                try
                {
                    HttpFileCollectionBase files = Request.Files;
                    string candidateId = "";
                    string[] keys = Request.Form.AllKeys;
                    for (int i = 0; i < keys.Length; i++)
                    {
                        candidateId = Request.Form[keys[i]];
                    }
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        string fname;
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                        }
                        if (fname != "")
                        {
                            fname = "CandidateVideo" + "_" + candidateId + "_" + fname;
                        }
                        string path = Server.MapPath("~/UploadedVideo");
                        if (!Directory.Exists(path))
                        {
                            DirectoryInfo di = Directory.CreateDirectory(path);
                        }
                        var addedfiles = Directory.GetFiles(path);
                        List<string> rsFiles = new List<string>();
                        foreach (var item in addedfiles)
                        {
                            if (item.Contains(candidateId))
                            {
                                rsFiles.Add(item);
                            }
                        }
                        var counts = rsFiles.Count();
                        fname = counts == 0 ? "1_" + fname : (counts += 1) + "_" + fname;
                        string fileName = fname;
                        fname = Path.Combine(path, fname);
                        file.SaveAs(fname);

                        int intCandidateId = Convert.ToInt32(candidateId);
                        Candidate data = db.Candidates.Find(intCandidateId);

                        data.CandidateVideo = fileName;
                        db.SaveChanges();
                    }
                    //return Json("Video Uploaded Successfully!");
                    return Json(new { Error = false, redirectUrl = Url.Action("InterviewCompleted", "CommonViews"), JsonRequestBehavior.AllowGet });
                }
                catch (Exception ex)
                {
                    Log.Error("Start log ERROR...");
                    Log.Error(ex.Message);
                    Thread.Sleep(TimeSpan.FromSeconds(secs));
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }

        [HttpGet]
        public ActionResult MCallerLoadData(string clientId, string companyId, string userId, string daysId)
        {
            using (vRecruitEntities vRecruitEntities = new vRecruitEntities())
            {
                int companyToLook = Convert.ToInt32(this.Session["currentCompanyId"]);
                int currentClient = Convert.ToInt32(Session["CurrentClientId"]);
                int currentCompany = Convert.ToInt32(Session["currentCompanyId"]);
                var currentUser = Convert.ToString(Session["CurrentUserId"]);

                if (!string.IsNullOrEmpty(companyId))
                    companyToLook = Convert.ToInt32(companyId);

                string currentrole = string.Empty;
                if (Session["CurrentRole"] != null)
                {
                    currentrole = Session["CurrentRole"].ToString();
                }

                List<MCallDataVM> returnModel = new List<MCallDataVM>();
                List<MCallerData> lstMCallderData = new List<MCallerData>();
                List<MCallerData> ClientMCallerData = new List<MCallerData>();
                List<MCallerData> RecruiterMCallerData = new List<MCallerData>();

                if (currentrole == "CLIENT")
                {
                    List<int?> clientCandidate = new List<int?>();
                    clientCandidate = vRecruitEntities.ClientCandidateMappings.Where(x => x.ClientId == currentClient).Select(y => y.CandidateId).ToList();
                    lstMCallderData = vRecruitEntities.MCallerDatas.Where(x => x.ClientId == currentClient && clientCandidate.Contains(x.CandidateId)).ToList();
                }
                else if (currentrole == "USER" || currentrole == "TALENT ACQUISITION")
                {
                    lstMCallderData = vRecruitEntities.MCallerDatas.Where(x => x.UserId == currentUser).ToList();
                }
                else
                {
                    lstMCallderData = vRecruitEntities.MCallerDatas.Where(y => y.AspNetUser.CompanyId == companyToLook).ToList();
                }

                if (clientId != "")
                {
                    ClientMCallerData.Clear();
                    string str1 = clientId;
                    char[] chArray = new char[1] { ',' };
                    foreach (string str2 in str1.Split(chArray))
                    {
                        int receivedClientId = Convert.ToInt32(str2);
                        List<MCallerData> queryable = lstMCallderData.Where(a => a.ClientId == receivedClientId && a.AspNetUser.CompanyId == companyToLook).ToList();
                        ClientMCallerData.AddRange(queryable);
                    }
                }

                if (!string.IsNullOrEmpty(userId))
                {
                    RecruiterMCallerData.Clear();

                    string str1 = userId;
                    char[] chArray = new char[1] { ',' };
                    foreach (string str2 in str1.Split(chArray))
                    {
                        // Guid receivedUserId = Guid.Parse(str2);
                        string receivedUserId = str2;
                        List<MCallerData> queryable = lstMCallderData.Where(a => a.UserId == receivedUserId && a.AspNetUser.CompanyId == companyToLook).ToList();
                        RecruiterMCallerData.AddRange(queryable);
                    }
                }

                if (RecruiterMCallerData.Count > 0 || ClientMCallerData.Count > 0)
                {
                    lstMCallderData.Clear();
                    lstMCallderData.AddRange(RecruiterMCallerData);
                    lstMCallderData.AddRange(ClientMCallerData);
                }

                if (!string.IsNullOrWhiteSpace(daysId))
                {
                    List<MCallerData> queryable = new List<MCallerData>();
                    if (daysId.Equals("7"))
                    {
                        var daysAgo = DateTime.Today.AddDays(-7);
                        lstMCallderData = lstMCallderData.Where(x => x.DateTime >= daysAgo && x.AspNetUser.CompanyId == companyToLook).ToList();
                    }
                    else if (daysId.Equals("15"))
                    {
                        var daysAgo = DateTime.Today.AddDays(-15);
                        lstMCallderData = lstMCallderData.Where(x => x.DateTime >= daysAgo && x.AspNetUser.CompanyId == companyToLook).ToList();
                    }
                    else if (daysId.Equals("30"))
                    {
                        var daysAgo = DateTime.Today.AddDays(-30);
                        lstMCallderData = lstMCallderData.Where(x => x.DateTime >= daysAgo && x.AspNetUser.CompanyId == companyToLook).ToList();
                    }
                }

                returnModel = lstMCallderData.Select(x => new MCallDataVM()
                {
                    Id = x.Id,
                    Name = x.Client != null ? x.Client.ClientName : x.Candidate.CandidateName,
                    Mobile = x.Client != null ? x.Client.ClientMobile : x.Candidate.CandidatePhone,
                    ContactType = x.Client != null ? "Client" : "Candidate",
                    Stauts = x.Status,
                    CallType = x.CallType,
                    CallDuration = x.CallDuration,
                    DateTime = x.DateTime.ToString(),
                    Remarks = x.Remarks,
                    NextAction = x.NextAction,
                    CalledBy = x.AspNetUser.UserName
                }).ToList();


                var data = new
                {
                    MCallerData = returnModel,
                    totalCalls = returnModel.Count
                };

                return (ActionResult)this.Json((object)data, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult CandidateInterviewReport()
        {
            if (Session["CurrentUserId"] == null)
            {
                Session.Clear();
                Session.Abandon();
                return RedirectToAction("Login", "Account");
            }

            var userid = Session["CurrentUserId"].ToString();
            string Currentrole = string.Empty;

            if (Session["CurrentRole"] != null)
            {
                Currentrole = Session["CurrentRole"].ToString();
            }
            if (Currentrole == "SUPER ADMIN")
            {
                ViewBag.Companies = new SelectList(db.Companies, "ID", "CompanyName");
            }
            else
            {
                ViewBag.Companies = new SelectList(db.Companies.Where(x => x.UserID == userid), "ID", "CompanyName");
            }
            return View();
        }

        //[HttpGet]
        //public JsonResult CandInterviewVedio(int candidateId)
        //{
        //    using (vRecruitEntities dc = new vRecruitEntities())
        //    {
        //        dc.Configuration.LazyLoadingEnabled = false;
        //        dc.Configuration.ProxyCreationEnabled = false;

        //        var candidatesrecord = db.Candidates.
        //            Join(db.Interviews, c => c.CandidateId, i => i.CandidateId,
        //             (c, i) => new { c.CandidateId, c.CandidateName, i.InterviewId })
        //              //Join(db.CandidateTestRecords, r => r.CandidateId, ctr => ctr.CandidateId, (r, ctr) => new { r.CandidateId, r.CandidateName, r.InterviewId, ctr.InterviewVideo, ctr.TestStatus, ctr.TestName })
        //              //.Join(db.CandidateTestResults, p => p.CandidateId, ctres => ctres.CandidateId, (p, ctres) => new { p.CandidateId, p.CandidateName, p.InterviewId, p.InterviewVideo, p.TestStatus, ctres })
        //              .Join(db.Tests, s => s.TestName, tes => tes.TestName, (s, tes) => new { s.CandidateId, s.CandidateName, s.InterviewId, s.InterviewVideo, s.TestStatus, tes.QueId, s.TestName })
        //              .Join(db.Questions, t => t.QueId, q => q.QuestionId, (t, q) => new { t, q.Question1 })

        //              .Where(x => x.t.CandidateId == candidateId && x.t.TestStatus.ToUpper() == "COMPLETED" && x.t.QueId != 0)
        //              .Distinct()
        //             .Select(m => new
        //             {
        //                 m.t.InterviewId,
        //                 m.t.InterviewVideo,
        //                 m.t.QueId,
        //                 m.Question1
        //             }).ToList();
        //        var interviewId = candidatesrecord.FirstOrDefault().InterviewId;
        //        var interviewEmotions = db.InterviewEmotions.Where(x => x.InterviewId == interviewId).Select(x => new
        //        {
        //            x.EmotionName,
        //            x.EmotionTime
        //        }).ToList();

        //        var interviewFeedback = db.InterviewFeedbacks.Where(x => x.InterviewId == interviewId).Select(x => new
        //        {
        //            x.Comment,
        //            x.CommentedBy,
        //            x.CommentDate
        //        }).ToList();

        //        var interviewQuestions = db.InterviewQuestionsTimes.Where(x => x.InterviewId == interviewId).Select(x => new
        //        {
        //            x.QuestionId,
        //            x.QuestionTime
        //        });

        //        var answers = db.InterviewFeedbacks.
        //          Join(db.Interviews, ife => ife.InterviewId, i => i.InterviewId,
        //           (ife, i) => new { ife.Id, i.InterviewId, }).
        //            Join(db.InterviewQuestionsFeedbacks, iqf => iqf.Id, ife => ife.InterviewFeedbackId, (iqf, ife) => new { ife.IsAnswerCorrect, ife.QuestionId, iqf.InterviewId }).
        //            Where(x => x.InterviewId == interviewId).Select(y => y.IsAnswerCorrect).ToList();

        //        var result = new { candidatesrecord, interviewEmotions, interviewFeedback, interviewQuestions,answers };
        //        return Json(result, JsonRequestBehavior.AllowGet);
        //    }
        //}

        [HttpGet]
        public JsonResult ClientListByCompanyId(int companyId)
        {
            using (vRecruitEntities dc = new vRecruitEntities())
            {
                dc.Configuration.LazyLoadingEnabled = false;
                dc.Configuration.ProxyCreationEnabled = false;
                var clientList = dc.Clients.Where(x => x.CompanyId == companyId).Select(x => new { x.ClientId, x.ClientName }).ToList();
                return Json(new { clientList }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult JobListByClientId(int clientId)
        {
            using (vRecruitEntities dc = new vRecruitEntities())
            {
                dc.Configuration.LazyLoadingEnabled = false;
                dc.Configuration.ProxyCreationEnabled = false;
                var jobList = dc.Jobs.Where(x => x.ClientId == clientId).Select(x => new { x.JobId, x.JobName }).ToList();
                return Json(new { jobList }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult CandidateListByFilter(int companyid, int clientId, int jobId)
        {
            using (vRecruitEntities dc = new vRecruitEntities())
            {
                dc.Configuration.LazyLoadingEnabled = false;
                dc.Configuration.ProxyCreationEnabled = false;
                if (companyid != 0 && clientId == 0 && jobId == 0)
                {
                    var candidateList1 = db.Candidates.
                     Join(db.Companies, c => c.CompanyId, co => co.ID, (c, co) => new { c.CandidateId, c.CandidateName, co.ID }).
                     Join(db.Interviews, r => r.CandidateId, i => i.CandidateId, (r, i) => new { r.CandidateId, r.CandidateName, r.ID, i.InterviewId }).
                     Join(db.CandidateTestRecords, y => y.CandidateId, ctr => ctr.CandidateId, (y, ctr) => new { y.CandidateId, y.CandidateName, y.ID, y.InterviewId, ctr.TestStatus }).
                     Where(z => z.TestStatus.ToUpper() == "COMPLETED" && z.ID == companyid).
                     OrderByDescending(x => x.InterviewId).
                     Select(m => new
                     {
                         m.CandidateId,
                         m.CandidateName
                     }).ToList();
                    return Json(candidateList1, JsonRequestBehavior.AllowGet);
                }
                else if (companyid != 0 && clientId != 0 && jobId == 0)
                {
                    var candidateList2 = dc.Clients.
                        Join(dc.ClientCandidateMappings, cl => cl.ClientId, ccm => ccm.ClientId, (cl, ccm) => new { cl.ClientId, cl.ClientName, ccm.CandidateId }).
                        Join(dc.Candidates, ccm => ccm.CandidateId, c => c.CandidateId, (ccm, c) => new { c.CandidateId, c.CandidateName, ccm.ClientId }).
                        Join(dc.Interviews, c => c.CandidateId, i => i.CandidateId, (c, i) => new { c.CandidateId, c.CandidateName, i.InterviewId, c.ClientId }).
                        Join(dc.CandidateTestRecords, i => i.CandidateId, ctr => ctr.CandidateId, (i, ctr) => new { i.CandidateId, i.CandidateName, i.InterviewId, ctr.TestStatus, i.ClientId }).
                        Where(z => z.TestStatus.ToUpper() == "COMPLETED" && z.ClientId == clientId).
                        OrderByDescending(x => x.InterviewId).
                        Select(m => new
                        {
                            m.CandidateId,
                            m.CandidateName
                        }).ToList();
                    return Json(candidateList2, JsonRequestBehavior.AllowGet);
                }

                else if (companyid != 0 && clientId != 0 && jobId != 0)
                {
                    var candidateList3 = dc.Jobs.
                    Join(dc.Activities, j => j.JobId, a => a.JobId, (j, a) => new { j.JobId, j.JobName, a.ActivityType, a.ActivityComplete, a.CandidateId }).
                    Join(dc.Candidates, a => a.CandidateId, c => c.CandidateId, (a, c) => new { c.CandidateId, c.CandidateName, a.ActivityType, a.ActivityComplete, a.JobId, a.JobName }).
                    Join(dc.Interviews, c => c.CandidateId, i => i.CandidateId, (c, i) => new { c.CandidateId, c.CandidateName, i.InterviewId, c.JobId, c.JobName, c.ActivityType, c.ActivityComplete }).
                    Join(dc.CandidateTestRecords, i => i.CandidateId, ctr => ctr.CandidateId, (i, ctr) => new { i.CandidateId, i.CandidateName, ctr.TestStatus, i.JobId, i.JobName, i.ActivityType, i.ActivityComplete, i.InterviewId }).
                    Where(z => z.TestStatus.ToUpper() == "COMPLETED" && z.JobId == jobId && z.ActivityType.ToUpper() == "ONLINE INTERVIEW"
                            && z.ActivityComplete == true).
                    Distinct().
                    OrderByDescending(x => x.InterviewId).
                    Select(m => new
                    {
                        m.CandidateId,
                        m.CandidateName
                    }).ToList();
                    return Json(candidateList3, JsonRequestBehavior.AllowGet);
                }

                return null;
            }
        }
        public ActionResult InterviewReport()
        {
            return (ActionResult)this.View();
        }

        [HttpGet]
        public ActionResult InterviewCandidateData(string clientId, string jobId)
        {
            List<InterviewCandidatesVM> interviewCandidates = new List<InterviewCandidatesVM>();
            using (vRecruitEntities dc = new vRecruitEntities())
            {
                IQueryable<Candidate> v;

                string currentrole = string.Empty;
                if (Session["CurrentRole"] != null)
                {
                    currentrole = Session["CurrentRole"].ToString();
                }
                int cId = Convert.ToInt32(clientId);
                int jId = Convert.ToInt32(jobId);
                if (currentrole == "SUPER ADMIN")
                {
                    List<Candidate> lstCandidates = new List<Candidate>();
                    List<int> InterviewIds = new List<int>();
                    if (cId != 0)
                    {
                        if (jId != 0)
                        {
                            lstCandidates = db.Candidates.Where(x => x.Activities.Any(y => y.ClientId == cId && y.JobId == jId && y.ActivityType == "ONLINE INTERVIEW") && x.Interviews.Count > 0).ToList();
                        }
                        else
                        {
                            lstCandidates = db.Candidates.Where(x => x.Activities.Any(y => y.ClientId == cId && y.ActivityType == "ONLINE INTERVIEW") && x.Interviews.Count > 0).ToList();
                        }
                        if (lstCandidates.Count > 0)
                        {
                            InterviewIds = lstCandidates.Select(y => y.Interviews.Select(z => z.InterviewId)).FirstOrDefault().ToList();
                        }
                        interviewCandidates = db.Interviews.Where(x => InterviewIds.Contains(x.InterviewId)).Select(x => new InterviewCandidatesVM()
                        {
                            Id = x.Candidate.CandidateId,
                            CandidateName = x.Candidate.CandidateName,
                            TestName = x.TestName,
                            InterviewId = x.InterviewId
                        }).ToList();
                    }
                }

                else if (currentrole == "CLIENT")
                {
                    var currentClientID = Convert.ToInt32(Session["CurrentClientId"]);
                    v = (from a in dc.Candidates
                         join cc in dc.ClientCandidateMappings on a.CandidateId equals cc.CandidateId
                         join cl in dc.Clients on cc.ClientId equals cl.ClientId
                         where cl.ClientId == currentClientID && a.IsActive == true
                         select a);
                    //where a.IsActive == true && a.CompanyId == currentcompanyId select a);
                }
            }

            return (ActionResult)this.Json((object)interviewCandidates, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetInterviewData(string InterviewId)
        {
            if (Session["CurrentUserId"] == null)
            {
                Session.Clear();
                Session.Abandon();
                return RedirectToAction("Login", "Account");
            }
            InterviewVM interviewData = new InterviewVM();
            using (vRecruitEntities dc = new vRecruitEntities())
            {
                int iId = Convert.ToInt32(InterviewId);

                interviewData = db.Interviews.Where(x => x.InterviewId == iId).Select(x => new InterviewVM()
                {
                    InterviewId = x.InterviewId,
                    TestName = x.TestName,
                    InterviewVideo = x.InterviewVideo,
                    lstEmotions = x.InterviewEmotions.Where(y => y.InterviewId == iId).Select(e => new InterviewEmotionVM()
                    {
                        EmotionTime = e.EmotionTime,
                        EmotionName = e.EmotionName,
                        EmotionScore = e.EmotionScore
                    }).ToList(),
                    lstQuestions = x.InterviewQuestionsTimes.Where(y => y.InterviewId == iId).Select(e => new InterviewQuestionsTimeVM()
                    {
                        Id = e.Id,
                        QuestionId = e.QuestionId,
                        QuestionName = e.Question.Question1,
                        QuestionTime = e.QuestionTime,
                        IsAnswerCorrect = e.IsAnswerCorrect,
                        UpdatedBy = e.UpdatedBy,
                        UpdatedDate = e.UpdatedDate.ToString()
                    }).ToList(),
                    lstComments = x.InterviewFeedbacks.Where(y => y.InterviewId == iId).OrderByDescending(z => z.CommentDate).Select(e => new InterviewFeedbackVM()
                    {
                        Comment = e.Comment,
                        UpdatedBy = e.UpdatedBy,
                        UpdatedByName = e.AspNetUser.UserName,
                        CommentDate = e.CommentDate.ToString(),
                    }).ToList()
                }).FirstOrDefault();

            }

            var data = new
            {
                InterviewData = interviewData,
            };

            return (ActionResult)this.Json((object)data, JsonRequestBehavior.AllowGet);
        }

    }
}