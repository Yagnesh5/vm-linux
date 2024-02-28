using log4net;
using OnlineRecruitment_Main.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Runtime;
using vrecruit.DataBase.Comman;
using vrecruit.DataBase.EntityDataModel;
using vrecruit.DataBase.ViewModel;
using System.IO;
using OnlineRecruitment_Main.CustomModels;

namespace OnlineRecruitment_Main.Controllers
{
    public class CommonViewsController : Controller
    {
        vRecruitEntities db = new vRecruitEntities();
        Comman Comman = new Comman();
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        int secs = 3;
        // GET: CommonViews
        //public ActionResult Login()
        //{
        //    if (Session["CurrentUserId"] != null)
        //    {
        //        var curntUserId = Session["CurrentUserId"].ToString();
        //        var Roles = Session["CurrentRole"].ToString();
        //        var loginuser = db.AspNetUsers.Where(x => x.Id == curntUserId).Select(x => x.UserName).SingleOrDefault();
        //        if(Roles== "ADMIN" ||Roles== "COMPANY USER")
        //        {
        //            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        //        }
        //        if(Roles== "SUPER ADMIN")
        //        {
        //            return RedirectToAction("Index", "Dashboard", new { area = "SuperAdmin" });
        //        }
        //        if(Roles== "CLIENT")
        //        {
        //            return RedirectToAction("Index", "Dashboard", new { area = "Client" });
        //        }
        //    }
        //    return View();
        //}

        public ActionResult Register()
        {
            return View();
        }

        public ActionResult Error_One()
        {
            return View();
        }

        public ActionResult Error_Two()
        {
            return View();
        }

        public ActionResult LockScreen()
        {
            return View();
        }

        public ActionResult PasswordRecovery()
        {
            return View();
        }

        public bool ExamGiven(int CanId, string examId)
        {
            bool status = true;
            var gid = Guid.Parse(examId);
            var candTest = db.CandidateTestURLs.Where(c => c.CandId == CanId & c.TestURLID == gid).FirstOrDefault();
            if (candTest != null)
            {
                if (candTest.Status)
                {
                    status = false;
                }
                else
                {
                    candTest.Status = true;
                    db.SaveChanges();
                }

            }
            return status;
        }

        public ActionResult CandidateTest(string Testname, string email, string useremail, int CanId, string e)
        {
            var url = HttpContext.Request.Url.PathAndQuery;

            var ExamStatus = ExamGiven(CanId, e);
            if (!ExamStatus)
            {
                return View("ExamLinkExpired");

            }
            //Testname = "Test2";
            List<QuestionAnswerVM> queansList = new List<QuestionAnswerVM>();
            var Datas = (from T in db.Tests
                         join Q in db.Questions on T.QueId equals Q.QuestionId
                         join G in db.QuestionGroups on T.GroupId equals G.ID
                         where T.TestName == Testname
                         orderby Q.QuestionId
                         select Q).ToList();
            TempData["Testname"] = Testname;
            Session["TestName"] = Testname;
            foreach (var item in Datas)
            {
                QuestionAnswerVM VM = new QuestionAnswerVM();
                VM.QuestionId = item.QuestionId;
                VM.Question = item.Question1;
                VM.Answerlst = item.Answers;
                VM.QuestionGroupName = db.QuestionGroups.Where(x => x.ID == item.QuestionGroupId).Select(x => x.GroupName).FirstOrDefault();
                int? time = db.Tests.Where(x => x.TestName == Testname).Select(x => x.Testtime).FirstOrDefault();
                VM.testSecond = Convert.ToInt32(time);
                if (item.QuestionType == "1")
                {
                    VM.QuestionType = "Single Answer";
                }
                else
                {
                    VM.QuestionType = "Multi Answer";
                }
                if (item.Company == null)
                {
                    VM.CompanyName = "All";
                }
                else
                {
                    VM.CompanyName = item.Company.CompanyName;
                }
                VM.QuestionStatus = item.QuestionStatus;
                VM.CrntuserId = new Guid(item.LastUpdateBy);
                VM.Createdby = db.AspNetUsers.Where(x => x.Id == item.LastUpdateBy.ToString()).Select(x => x.UserName).FirstOrDefault();
                VM.Candidateemail = email;
                VM.useremail = useremail;
                VM.imagelist = new List<QuestionAnswerImage>();
                foreach (var img in item.QuestionAnswerImages)
                {
                    QuestionAnswerImage model = new QuestionAnswerImage();
                    model.Images = img.Images;
                    VM.imagelist.Add(model);
                }
                queansList.Add(VM);
                //  return View(VM);
            }
            MaintainCandidateScoreRecords(CanId, useremail, Testname, url, "Opened", "");
            return View(queansList);
        }

        /// <summary>
        /// Used to mainatain Candidate test result
        /// </summary>
        /// <param name="candidateId">Candidate Id</param>
        /// <param name="userEmail">User Email</param>
        /// <param name="testname">Test name</param>
        /// <param name="testLink">Test Link</param>
        /// <param name="status">Status</param>
        /// <param name="score">Score</param>
        protected void MaintainCandidateScoreRecords(int candidateId, string userEmail, string testname, string testLink, string status, string score)
        {
            Candidate candidate = new Candidate();
            if (candidateId == 0)
            {
                candidate = db.Candidates.Where(d => d.CandidateEmail == userEmail).FirstOrDefault();
            }
            else
            {
                candidate = db.Candidates.Where(d => d.CandidateId == candidateId).FirstOrDefault();
            }
            CandidateTestRecord canRec = new CandidateTestRecord();
            if (!string.IsNullOrEmpty(testname))
            {
                canRec = db.CandidateTestRecords.Where(i => i.CandidateId == candidate.CandidateId && i.TestName == testname).FirstOrDefault();
            }
            else
            {
                canRec = db.CandidateTestRecords.Where(i => i.CandidateId == candidate.CandidateId && i.TestStatus == "Opened").FirstOrDefault();
            }


            if (canRec == null)
            {
                CandidateTestRecord record = new CandidateTestRecord();
                record.CandidateId = candidate.CandidateId;
                record.TestName = testname;
                record.TestLink = testLink;
                record.TestStatus = status;
                record.TestScore = "0";
                db.CandidateTestRecords.Add(record);
                db.SaveChanges();
            }
            else
            {
                canRec.CandidateId = candidate.CandidateId;
                //canRec.TestName = testname;
                //canRec.TestLink = testLink;
                canRec.TestStatus = status;
                canRec.TestScore = score;
                db.SaveChanges();
            }
        }


        [HttpPost]
        public ActionResult SubmitTest(List<Testlist> list, string canEmail, string useremail, int TotalCount, string TestName)
        {
            if (list != null)
            {
                int marks = 0;
                foreach (var item in list)
                {
                    int quid = Convert.ToInt32(item.QueId);
                    var ansdata = db.Answers.Where(x => x.QuestiondId == quid && x.AnswerCorrect == true).ToList();
                    if (ansdata.Count() > 1)
                    {
                        if (item.AnsId.Count() == ansdata.Count())
                        {
                            int isAnswerRight = 1;
                            foreach (var answer in ansdata)
                            {
                                isAnswerRight = isAnswerRight * (item.AnsId.Contains(answer.AnswerId) ? 1 : 0);
                            }
                            if (isAnswerRight == 1)
                            {
                                marks++;
                            }
                        }
                    }
                    else
                    {
                        if (item.AnsId.Count == 1)
                        {
                            int Aid = item.AnsId[0];
                            var singleAnswer = db.Answers.FirstOrDefault(x => x.AnswerId == Aid);
                            if (singleAnswer != null)
                            {
                                if (singleAnswer.AnswerCorrect == true)
                                {
                                    marks++;
                                }
                            }
                        }
                    }
                }
                int percentComplete = (int)Math.Round((double)(100 * marks) / TotalCount);
                var body = "";
                ActivityVM ActVM = new ActivityVM();
                ActVM.ActivityType = "Test Result";
                ActVM.ActivityDate = DateTime.Now.ToString("dd-MMM-yyyy");
                ActVM.UserEmail = useremail;
                //if (percentComplete >= 35)
                //{
                //    //ActVM.Activity1 = "You pass this exam so now you eligible to next round we are contact you as soon as possible. your total marks is:" + percentComplete + "%";
                //    ActVM.Activity1 = "Hello Candidate<br/><br/>You have successfully completed the test and your score would be submitted to the HR.<br/><br/>For more information feel free to get in touch with Synoris<br/><br/>";
                //}
                //else
                //{
                //    ActVM.Activity1 = "sorry you failed in your test. your total marks is:" + percentComplete;
                //}
                StringBuilder text = new StringBuilder();
                text.AppendLine("");
                text.AppendLine(Environment.NewLine);
                text.AppendLine(Environment.NewLine);
                text.AppendLine("You have successfully completed the test and your score would be submitted to the HR.");
                text.AppendLine(Environment.NewLine);
                text.AppendLine(Environment.NewLine);
                text.AppendLine("For more information feel free to get in touch with Synoris");
                text.AppendLine(Environment.NewLine);
                text.AppendLine(Environment.NewLine);
                //ActVM.Activity1 = "Hello Candidate. You have successfully completed the test and your score would be submitted to the HR.For more information feel free to get in touch with Synoris";
                ActVM.Activity1 = text.ToString();
                //var TestName = TempData["TestName"].ToString();



                // MaintainCandidateScoreRecords(0, useremail, "", "Completed", percentComplete.ToString());
                MaintainCandidateScoreRecords(0, useremail, TestName, "", "Completed", percentComplete.ToString());
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
                string emailSubject = "Test Result";
                EmailViewModel EVM = new EmailViewModel();
                EmailViewModels Emodels = new EmailViewModels();
                Emodels.Subject = emailSubject;
                Emodels.body = body;
                Emodels.Smtpemail = EVM.Smtpemail;
                Emodels.Smtppassword = EVM.Smtppassword;
                Emodels.Smtp = EVM.Stmp;
                Emodels.Port = EVM.Port;
                if (canEmail != null)
                {
                    try
                    {
                        //Emodels.to = canEmail;
                        //bool sendmail = Comman.SendEmail(Emodels);
                        //if (sendmail == false)
                        //{
                        //    return Json(new { Error = true, Message = "Email delivery failed" }, JsonRequestBehavior.AllowGet);
                        //}
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Start log ERROR...");
                        Log.Error(ex.Message);
                        Thread.Sleep(TimeSpan.FromSeconds(secs));
                        return Json(new { Error = true, Message = ex.Message }, JsonRequestBehavior.AllowGet);
                    }
                }
                if (useremail != null)
                {
                    try
                    {
                        Emodels.to = useremail;
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
            }
            return Json(new { Error = false, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UploadVideo()
        {
            if (Request.Files.Count > 0)
            {
                try
                {
                    string fileName = string.Empty;
                    HttpFileCollectionBase files = Request.Files;

                    for (int i = 0; i < files.Count; i++)
                    {
                        string[] keys = Request.Form.AllKeys;


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
                            fname = "CandidateVideo" + "_" + 100 + "_" + fname;
                        }
                        string path = Server.MapPath("~/InterviewVideo");
                        if (!Directory.Exists(path))
                        {
                            DirectoryInfo di = Directory.CreateDirectory(path);
                        }
                        var addedfiles = Directory.GetFiles(path);
                        List<string> rsFiles = new List<string>();
                        foreach (var item in addedfiles)
                        {
                            if (item.Contains("100"))
                            {
                                rsFiles.Add(item);
                            }
                        }
                        var counts = rsFiles.Count();
                        fname = counts == 0 ? "1_" + fname : (counts += 1) + "_" + fname;
                        fileName = fname;
                        fname = Path.Combine(path, fname);
                        file.SaveAs(fname);

                    }
                    return Json("Video Uploaded Successfully!");
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

        public ActionResult EmotionDetection()
        {

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult InterviewSubmit(string canEmail, string useremail)
        {
            string fileName = string.Empty;
            string candidateId = "";
            if (Request.Files.Count > 0)
            {
                try
                {
                    HttpFileCollectionBase files = Request.Files;

                    string[] keys = Request.Form.AllKeys;
                    for (int i = 0; i < keys.Length; i++)
                    {
                        candidateId = Request.Form[keys[0]];
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
                        string path = Server.MapPath("~/InterviewVideo");
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
                        fileName = fname;
                        fname = Path.Combine(path, fname);
                        file.SaveAs(fname);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Start log ERROR...");
                    Log.Error(ex.Message);
                    Thread.Sleep(TimeSpan.FromSeconds(secs));
                    return Json(new { Error = true, Message = "Error occurred. Error details:" }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { Error = true, Message = "No files selected" }, JsonRequestBehavior.AllowGet);
            }


            int percentComplete = 0;
            var body = "";
            ActivityVM ActVM = new ActivityVM();
            ActVM.ActivityType = "Test Result";
            ActVM.ActivityDate = DateTime.Now.ToString("dd-MMM-yyyy");
            ActVM.UserEmail = useremail;

            StringBuilder text = new StringBuilder();
            text.AppendLine("");
            text.AppendLine(Environment.NewLine);
            text.AppendLine(Environment.NewLine);
            text.AppendLine("You have successfully completed the test and your score would be submitted to the HR.");
            text.AppendLine(Environment.NewLine);
            text.AppendLine(Environment.NewLine);
            text.AppendLine("For more information feel free to get in touch with Synoris");
            text.AppendLine(Environment.NewLine);
            text.AppendLine(Environment.NewLine);
            //ActVM.Activity1 = "Hello Candidate. You have successfully completed the test and your score would be submitted to the HR.For more information feel free to get in touch with Synoris";
            ActVM.Activity1 = text.ToString();
            //var TestName = TempData["TestName"].ToString();
            string TestName = string.Empty;

            TestName = Request.Form[1].ToString();

            // MaintainCandidateScoreRecords(0, useremail, "", "Completed", percentComplete.ToString());
            MaintainCandidateScoreRecords(0, useremail, TestName, "", "Completed", percentComplete.ToString());

            var jsSer = new JavaScriptSerializer();
            //List<InterviewEmotionVM> lstEmotions = jsSer.Deserialize<List<InterviewEmotionVM>>(Request.Form[1].ToString());
            Interview interview = new Interview();

            interview.CandidateId = Convert.ToInt16(candidateId);
            interview.TestName = TestName;
            interview.InterviewVideo = fileName;
            interview.InterviewDate = System.DateTime.Now;

            //if (lstEmotions != null)
            //{               
            //    List<InterviewEmotion> lstInterviewEmotions = new List<InterviewEmotion>();

            //    foreach (var item in lstEmotions)
            //    {
            //        InterviewEmotion interviewEmotion = new InterviewEmotion();
            //        interviewEmotion.EmotionTime = item.EmotionTime;
            //        interviewEmotion.EmotionScore = item.EmotionScore;
            //        interviewEmotion.EmotionName = item.EmotionName;
            //        lstInterviewEmotions.Add(interviewEmotion);
            //    }

            //    interivew.InterviewEmotions = lstInterviewEmotions;

            //    db.Interviews.Add(interivew);
            //}
            db.Interviews.Add(interview);
            db.SaveChanges();
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
            string emailSubject = "Test Result";
            EmailViewModel EVM = new EmailViewModel();
            EmailViewModels Emodels = new EmailViewModels();
            Emodels.Subject = emailSubject;
            Emodels.body = body;
            Emodels.Smtpemail = EVM.Smtpemail;
            Emodels.Smtppassword = EVM.Smtppassword;
            Emodels.Smtp = EVM.Stmp;
            Emodels.Port = EVM.Port;
            if (canEmail != null)
            {
                try
                {
                    //Emodels.to = canEmail;
                    //bool sendmail = Comman.SendEmail(Emodels);
                    //if (sendmail == false)
                    //{
                    //    return Json(new { Error = true, Message = "Email delivery failed" }, JsonRequestBehavior.AllowGet);
                    //}
                }
                catch (Exception ex)
                {
                    Log.Error("Start log ERROR...");
                    Log.Error(ex.Message);
                    Thread.Sleep(TimeSpan.FromSeconds(secs));
                    return Json(new { Error = true, Message = ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            if (useremail != null)
            {
                try
                {
                    Emodels.to = useremail;
                    //bool sendmail = Comman.SendEmail(Emodels);
                    //if (sendmail == false)
                    //{
                    //    return Json(new { Error = true, Message = "Email delivery failed" }, JsonRequestBehavior.AllowGet);
                    //}
                }
                catch (Exception ex)
                {
                    Log.Error("Start log ERROR...");
                    Log.Error(ex.Message);
                    Thread.Sleep(TimeSpan.FromSeconds(secs));
                    return Json(new { Error = true, Message = ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }

            //return Json(new { Error = false, Message = "Success" }, JsonRequestBehavior.AllowGet);
            return Json(new { Error = false, redirectUrl = Url.Action("InterviewCompleted", "CommonViews"), JsonRequestBehavior.AllowGet });
            //return RedirectToAction("InterviewCompleted");
        }

        public ActionResult ResetPsdByForget(string UserId)
        {
            ForgotViewModel model = new ForgotViewModel();
            model.UserId = UserId;
            return View(model);
        }

        [HttpPost]
        public ActionResult SaveCandidateResult(List<Testlist> list, string Testname, string CanId, string AssignBy)
        {
            if (list != null)
            {
                var groupId = db.Tests.Where(x => x.TestName == Testname).Select(x => x.GroupId).FirstOrDefault();
                foreach (var item in list)
                {
                    foreach (var ans in item.AnsId)
                    {
                        CandidateTestResult result = new CandidateTestResult();
                        result.QuestionId = Convert.ToInt32(item.QueId);
                        result.IsMulti = item.isMulti;
                        result.CandidateAnswerId = ans;
                        result.TestGroupId = groupId;
                        result.CandidateId = Convert.ToInt32(CanId);
                        result.SubmitDate = DateTime.Now;
                        result.AssignBy = AssignBy;
                        result.TestName = Testname;
                        db.CandidateTestResults.Add(result);
                        db.SaveChanges();
                    }
                }
            }
            return Json(new { Error = false, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }

        
        public ActionResult CandidateUpload(string Testname, string email, string useremail, int CanId, string e)
        {
            var url = HttpContext.Request.Url.PathAndQuery;

            int VideoUploadedCount = VideoUploaded(CanId, e);
            if (VideoUploadedCount == -1)
            {
                return View("InvalidLink");
            }
            if (VideoUploadedCount > 1)
            {
                return View("VideoLinkExpired");
            }
            return View();
        }

        public ActionResult InterviewUpload(string Testname, string email, string useremail, int CanId, string e)
        {
            var url = HttpContext.Request.Url.PathAndQuery;

            var ExamStatus = ExamGiven(CanId, e);
            if (!ExamStatus)
            {
                return View("ExamLinkExpired");

            }
            List<QuestionAnswerVM> queansList = new List<QuestionAnswerVM>();
            var Datas = (from T in db.Tests
                         join Q in db.Questions on T.QueId equals Q.QuestionId
                         join G in db.QuestionGroups on T.GroupId equals G.ID
                         where T.TestName == Testname
                         orderby Q.QuestionId
                         select Q).ToList();
            TempData["Testname"] = Testname;
            Session["TestName"] = Testname;
            foreach (var item in Datas)
            {
                QuestionAnswerVM VM = new QuestionAnswerVM();
                VM.QuestionId = item.QuestionId;
                VM.Question = item.Question1;
                VM.QuestionGroupName = db.QuestionGroups.Where(x => x.ID == item.QuestionGroupId).Select(x => x.GroupName).FirstOrDefault();
                int? time = db.Tests.Where(x => x.TestName == Testname).Select(x => x.Testtime).FirstOrDefault();
                VM.testSecond = Convert.ToInt32(time);

                if (item.Company == null)
                {
                    VM.CompanyName = "All";
                }
                else
                {
                    VM.CompanyName = item.Company.CompanyName;
                }
                VM.QuestionStatus = item.QuestionStatus;
                VM.CrntuserId = new Guid(item.LastUpdateBy);
                VM.Createdby = db.AspNetUsers.Where(x => x.Id == item.LastUpdateBy.ToString()).Select(x => x.UserName).FirstOrDefault();
                VM.Candidateemail = email;
                VM.useremail = useremail;
                VM.imagelist = new List<QuestionAnswerImage>();
                foreach (var img in item.QuestionAnswerImages)
                {
                    QuestionAnswerImage model = new QuestionAnswerImage();
                    model.Images = img.Images;
                    VM.imagelist.Add(model);
                }
                queansList.Add(VM);
                //  return View(VM);
            }
            MaintainCandidateScoreRecords(CanId, useremail, Testname, url, "Opened", "");
            return View(queansList);
        }

        public int VideoUploaded(int CanId, string examId)
        {
            int status = 0;
            var gid = Guid.Parse(examId);

            var candTest = db.CandidateTestURLs.Where(c => c.CandId == CanId & c.TestURLID == gid).Count();
            var validLink = db.CandidateTestURLs.Where(c => c.CandId == CanId & c.TestURLID == gid & c.Status == false).Count();

            if (validLink < 1 && candTest >= 0)
            {
                status = -1;
            }
            if (validLink == 1 && candTest >= 0)
            {
                status = candTest;
                CandidateTestURL newEntry = new CandidateTestURL();
                newEntry.CandId = CanId;
                newEntry.TestURLID = gid;
                newEntry.Status = true;
                db.CandidateTestURLs.Add(newEntry);
                db.SaveChanges();
            }
            return status;
        }

        public ActionResult InterviewCompleted()
        {
            return View("InterviewCompleted");
        }

        [HttpPost]
        public ActionResult SaveInterviewFeedback(InterviewFeedback interviewFeedback)
        {
            string currentUserId;
            if (Session["CurrentUserId"] == null)
            {
                Session.Clear();
                Session.Abandon();
                return RedirectToAction("Login", "Account");
            }
            else
            {
                currentUserId = Session["CurrentUserId"].ToString();
            }            
            if (interviewFeedback != null)
            {
                InterviewFeedback result = new InterviewFeedback();
                result.InterviewId = Convert.ToInt32(interviewFeedback.InterviewId);
                result.Comment = interviewFeedback.Comment;
                result.UpdatedBy = currentUserId;
                result.CommentDate = DateTime.Now;
                db.InterviewFeedbacks.Add(result);
                db.SaveChanges();
                var id = db.InterviewFeedbacks.Max(x => x.Id);                
            }
            return Json(new { Error = false, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveInterviewResult(string Id, string IsAnswerCorrect)
        {
            string currentUserId;
            if (Session["CurrentUserId"] == null)
            {
                Session.Clear();
                Session.Abandon();
                return RedirectToAction("Login", "Account");
            }
            else
            {
                currentUserId = Session["CurrentUserId"].ToString();
            }
            if (!string.IsNullOrEmpty(Id))
            {
                int intQueId = Convert.ToInt32(Id);
                InterviewQuestionsTime intQueData = db.InterviewQuestionsTimes.Where(x => x.Id == intQueId).FirstOrDefault();
                if(intQueData != null)
                {
                    intQueData.IsAnswerCorrect = Convert.ToBoolean(Convert.ToInt16(IsAnswerCorrect));
                    intQueData.UpdatedBy = currentUserId;
                    intQueData.UpdatedDate = DateTime.Now;
                    db.SaveChanges();
                }                
            }
            return Json(new { Error = false, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //public ActionResult SaveInterviewFeedback(InterviewFeedback interviewFeedback, List<InterviewQuestionsFeedback> interviewQuestionsFeedback)
        //{
        //    if (interviewFeedback != null)
        //    {
        //        var user = db.AspNetUsers.Where(x => x.Id == interviewFeedback.CommentedBy).FirstOrDefault().Email;

        //        InterviewFeedback result = new InterviewFeedback();
        //        result.InterviewId = Convert.ToInt32(interviewFeedback.InterviewId);
        //        result.Comment = interviewFeedback.Comment;
        //        result.CommentedBy = user;
        //        result.CommentDate = DateTime.Now;
        //        db.InterviewFeedbacks.Add(result);
        //        db.SaveChanges();
        //        var id = db.InterviewFeedbacks.Max(x => x.Id);

        //        foreach (InterviewQuestionsFeedback item in interviewQuestionsFeedback)
        //        {
        //            InterviewQuestionsFeedback interviewQuestionsFeedback1 = new InterviewQuestionsFeedback();
        //            interviewQuestionsFeedback1.InterviewFeedbackId = id;
        //            interviewQuestionsFeedback1.QuestionId = item.QuestionId;
        //            interviewQuestionsFeedback1.IsAnswerCorrect = item.IsAnswerCorrect;
        //            db.InterviewQuestionsFeedbacks.Add(interviewQuestionsFeedback1);
        //            db.SaveChanges();
        //        }
        //    }
        //    return Json(new { Error = false, Message = "Success" }, JsonRequestBehavior.AllowGet);
        //}

        [HttpPost]
        public ActionResult SaveInterviewQuestionsTime(List<InterviewQuestionsTime> interviewQuestionsTime, List<InterviewEmotionVM> lstEmotions)
        {
            var id = db.Interviews.Max(x => x.InterviewId);
            if (interviewQuestionsTime != null)
            {
                foreach (InterviewQuestionsTime item in interviewQuestionsTime)
                {
                    InterviewQuestionsTime interviewQuestionsTime1 = new InterviewQuestionsTime();
                    interviewQuestionsTime1.InterviewId = id;
                    interviewQuestionsTime1.QuestionId = item.QuestionId;
                    interviewQuestionsTime1.QuestionTime = item.QuestionTime;
                    db.InterviewQuestionsTimes.Add(interviewQuestionsTime1);                    
                }
            }
            if (lstEmotions != null)
            {
                List<InterviewEmotion> lstInterviewEmotions = new List<InterviewEmotion>();
                foreach (var item in lstEmotions)
                {
                    InterviewEmotion interviewEmotion = new InterviewEmotion();
                    interviewEmotion.InterviewId = id;
                    interviewEmotion.EmotionTime = item.EmotionTime;
                    interviewEmotion.EmotionScore = item.EmotionScore;
                    interviewEmotion.EmotionName = item.EmotionName;
                    lstInterviewEmotions.Add(interviewEmotion);
                }                
                db.InterviewEmotions.AddRange(lstInterviewEmotions);
            }
            db.SaveChanges();
            return Json(new { Error = false, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetComments(int interviewId)
        {
            List<InterviewFeedbackVM> lstComments = new List<InterviewFeedbackVM>();
            lstComments = db.InterviewFeedbacks.Where(y => y.InterviewId == interviewId).OrderByDescending(z => z.CommentDate).Select(e => new InterviewFeedbackVM()
            {
                Comment = e.Comment,
                UpdatedBy = e.UpdatedBy,
                UpdatedByName = e.AspNetUser.UserName,
                CommentDate = e.CommentDate.ToString(),
            }).ToList();

            return Json(lstComments, JsonRequestBehavior.AllowGet);
        }
    }
}