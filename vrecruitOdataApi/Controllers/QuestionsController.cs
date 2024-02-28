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
    builder.EntitySet<Question>("Questions");
    builder.EntitySet<Answer>("Answers"); 
    builder.EntitySet<Customer>("Customers"); 
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class QuestionsController : ODataController
    {
        private vRecruitEntities db = new vRecruitEntities();
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        int secs = 3;
        // GET: odata/Questions
        [EnableQuery]
        public IHttpActionResult GetQuestions()
        {
            List<QuestionAnswerVM> queansList = new List<QuestionAnswerVM>();
            var quelst = db.Questions.Where(x=>x.IsActive==true).OrderByDescending(x=>x.QuestionId).ToList().Take(1000);
            if (quelst.Count() > 0)
            {
                foreach (var item in quelst)
                {
                    QuestionAnswerVM VM = new QuestionAnswerVM();
                    VM.QuestionId = item.QuestionId;
                    VM.Question = item.Question1;
                    //VM.Answer = item.Answers.Where(x => x.AnswerCorrect == true).Select(x => x.Answer1).FirstOrDefault();
                    //VM.QuestionGroupName = db.QuestionGroups.Where(x => x.ID == item.QuestionGroupId).Select(x => x.GroupName).FirstOrDefault();
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
                    //VM.QuestionStatus = item.QuestionStatus;
                    VM.CrntuserId =new Guid(item.LastUpdateBy);
                    if (item.AspNetUser != null)
                    {
                        VM.Createdby = item.AspNetUser.UserName;
                    }
                    //VM.Createdby = db.AspNetUsers.Where(x => x.Id == item.LastUpdateBy.ToString()).Select(x => x.UserName).FirstOrDefault();
                    queansList.Add(VM);
                }
                Success Succ = new Success() { Code = "1", Message = "LoadData", Data = queansList };
                return new SuccessResult(Succ, Request);
            }
            else
            {
                Error Err = new Error() { Code = "0", Message = "Data not Found" };
                return new ErrorResult(Err, Request);
            }
        }

        // GET: odata/Questions(5)
        [EnableQuery]
        public IHttpActionResult GetQuestion(int key)
        {
            try
            {
                var Datas = db.Questions.Where(question => question.QuestionId == key).FirstOrDefault();
                QuestionAnswerVM vm = new QuestionAnswerVM();
                //string[] Option = new string[4];
                if (Datas != null)
                {
                    vm.QuestionId = Datas.QuestionId;
                    vm.Question = Datas.Question1;
                    vm.QuestionGroupId = Datas.QuestionGroupId;
                    vm.CompanyId = Datas.CompanyId;
                    vm.QuestionSkill = Datas.QuestionSkill;
                    vm.QuestionType = Datas.QuestionType;
                    vm.AnswerId = Datas.Answers.Select(x => x.AnswerId).FirstOrDefault();
                    vm.optionAnswersliist = new List<Answer>();
                    foreach (var item in Datas.Answers)
                    {
                        Answer ans = new Answer();
                        ans.Answer1 = item.Answer1;
                        ans.AnswerCorrect = item.AnswerCorrect;
                        vm.optionAnswersliist.Add(ans);
                    }
                    vm.QuestionImagelist = new List<QuestionImages>();
                    foreach (var item in Datas.QuestionAnswerImages)
                    {
                        QuestionImages objqmg = new QuestionImages();
                        objqmg.Images = item.Images;
                        objqmg.Id = item.Id;
                        vm.QuestionImagelist.Add(objqmg);
                    }
                    vm.QuestionImagelist = new List<QuestionImages>();
                    foreach (var item in Datas.QuestionAnswerImages)
                    {
                        QuestionImages objqimg = new QuestionImages();
                        objqimg.Images = item.Images;
                        objqimg.Id = item.Id;
                        vm.QuestionImagelist.Add(objqimg);
                    }
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

        // PUT: odata/Questions(5)
        public IHttpActionResult Put([FromODataUri] int key, QuestionAnswerVM model)
        {
          
            Question question = db.Questions.Find(key);
            if (question == null)
            {
                Error Err = new Error() { Code = "0", Message = "Data not Found" };
                return new ErrorResult(Err, Request);
            }

            Question que = db.Questions.Find(model.QuestionId);
            que.Question1 = model.Question;
            que.QuestionStatus = true;
            que.QuestionType = model.QuestionType;
            que.QuestionSkill = model.QuestionSkill;
            que.CompanyId = model.CompanyId;
            que.QuestionGroupId = model.QuestionGroupId;
            que.LastUpdateBy = model.LastUpdateBy.ToString();

            try
            {
                db.SaveChanges();
                List<Answer> delans = db.Answers.Where(x => x.QuestiondId == model.QuestionId).ToList();
                db.Answers.RemoveRange(delans);
                db.SaveChanges();

                List<Answer> ansList = new List<Answer>();
                ansList.Add(new Answer { QuestiondId = que.QuestionId, Answer1 = model.Option1, AnswerCorrect = model.AnsChk1 });
                ansList.Add(new Answer { QuestiondId = que.QuestionId, Answer1 = model.Option2, AnswerCorrect = model.AnsChk2 });
                ansList.Add(new Answer { QuestiondId = que.QuestionId, Answer1 = model.Option3, AnswerCorrect = model.AnsChk3 });
                ansList.Add(new Answer { QuestiondId = que.QuestionId, Answer1 = model.Option4, AnswerCorrect = model.AnsChk4 });
                ansList.Add(new Answer { QuestiondId = que.QuestionId, Answer1 = model.Option5, AnswerCorrect = model.AnsChk5 });
                ansList.Add(new Answer { QuestiondId = que.QuestionId, Answer1 = model.Option6, AnswerCorrect = model.AnsChk6 });
                ansList.Add(new Answer { QuestiondId = que.QuestionId, Answer1 = model.Option7, AnswerCorrect = model.AnsChk7 });
                ansList.Add(new Answer { QuestiondId = que.QuestionId, Answer1 = model.Option8, AnswerCorrect = model.AnsChk8 });
                ansList.Add(new Answer { QuestiondId = que.QuestionId, Answer1 = model.Option5, AnswerCorrect = model.AnsChk9 });
                ansList.Add(new Answer { QuestiondId = que.QuestionId, Answer1 = model.Option10, AnswerCorrect = model.AnsChk10 });
                foreach (var item in ansList)
                {
                    if (item.Answer1 != null)
                    {
                        Answer ans = new Answer();
                        ans.QuestiondId = item.QuestiondId;
                        ans.Answer1 = item.Answer1;
                        ans.AnswerCorrect = item.AnswerCorrect;
                        db.Answers.Add(ans);
                        db.SaveChanges();
                    }
                }
                Success Succ = new Success() { Code = "1", Message = "Update", Data = que.QuestionId };
                return new SuccessResult(Succ, Request);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!QuestionExists(key))
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

        // POST: odata/Questions
        public IHttpActionResult Post(QuestionAnswerVM model)
        {
           
            Question que = new Question();
            que.Question1 = model.Question;
            que.QuestionStatus = true;
            que.QuestionType = model.QuestionType;
            que.QuestionSkill = model.QuestionSkill;
            que.CompanyId = model.CompanyId;
            que.QuestionGroupId = model.QuestionGroupId;
            que.LastUpdateBy = model.LastUpdateBy.ToString();
            que.IsActive = true;
            db.Questions.Add(que);
            try
            {
                db.SaveChanges();

                List<Answer> ansList = new List<Answer>();
                ansList.Add(new Answer { QuestiondId = que.QuestionId, Answer1 = model.Option1, AnswerCorrect = model.AnsChk1 });
                ansList.Add(new Answer { QuestiondId = que.QuestionId, Answer1 = model.Option2, AnswerCorrect = model.AnsChk2 });
                ansList.Add(new Answer { QuestiondId = que.QuestionId, Answer1 = model.Option3, AnswerCorrect = model.AnsChk3 });
                ansList.Add(new Answer { QuestiondId = que.QuestionId, Answer1 = model.Option4, AnswerCorrect = model.AnsChk4 });
                ansList.Add(new Answer { QuestiondId = que.QuestionId, Answer1 = model.Option5, AnswerCorrect = model.AnsChk5 });
                ansList.Add(new Answer { QuestiondId = que.QuestionId, Answer1 = model.Option6, AnswerCorrect = model.AnsChk6 });
                ansList.Add(new Answer { QuestiondId = que.QuestionId, Answer1 = model.Option7, AnswerCorrect = model.AnsChk7 });
                ansList.Add(new Answer { QuestiondId = que.QuestionId, Answer1 = model.Option8, AnswerCorrect = model.AnsChk8 });
                ansList.Add(new Answer { QuestiondId = que.QuestionId, Answer1 = model.Option5, AnswerCorrect = model.AnsChk9 });
                ansList.Add(new Answer { QuestiondId = que.QuestionId, Answer1 = model.Option10, AnswerCorrect = model.AnsChk10 });
                foreach (var item in ansList)
                {
                    if (item.Answer1 != null)
                    {
                        Answer ans = new Answer();
                        ans.QuestiondId = item.QuestiondId;
                        ans.Answer1 = item.Answer1;
                        ans.AnswerCorrect = item.AnswerCorrect;
                        db.Answers.Add(ans);
                        db.SaveChanges();
                    }
                }
                Success Succ = new Success() { Code = "1", Message = "Inserted", Data= que.QuestionId };
                return new SuccessResult(Succ, Request);
            }
            catch (DbUpdateException ex)
            {
                if (QuestionExists(que.QuestionId))
                {
                    Log.Error("Start log ERROR...");
                    Log.Error(ex.Message);
                    Thread.Sleep(TimeSpan.FromSeconds(secs));
                    Error Err = new Error() { Code = "0", Message = "Question Already exist...!!!" };
                    return new ErrorResult(Err, Request);
                }
                else
                {
                    Error Err = new Error() { Code = "0", Message = ex.Message };
                    return new ErrorResult(Err, Request);
                }
            }
        }

        // PATCH: odata/Questions(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<Question> patch)
        {
            List<QuestionAnswerVM> queansList = new List<QuestionAnswerVM>();
            var quelst = db.Questions.Where(x => x.IsActive == true).OrderByDescending(x => x.QuestionId).ToList().Skip(1 * 1000).Take(1000);
            if (quelst.Count() > 0)
            {
                foreach (var item in quelst)
                {
                    QuestionAnswerVM VM = new QuestionAnswerVM();
                    VM.QuestionId = item.QuestionId;
                    VM.Question = item.Question1;
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
                    VM.CrntuserId = new Guid(item.LastUpdateBy);
                    if (item.AspNetUser != null)
                    {
                        VM.Createdby = item.AspNetUser.UserName;
                    }
                    queansList.Add(VM);
                }
                Success Succ = new Success() { Code = "1", Message = "LoadData", Data = queansList };
                return new SuccessResult(Succ, Request);
            }
            else
            {
                Error Err = new Error() { Code = "0", Message = "Data not Found" };
                return new ErrorResult(Err, Request);
            }
        }

        // DELETE: odata/Questions(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            try
            {
                Question question = db.Questions.Find(key);
                if (question == null)
                {
                    Error Err = new Error() { Code = "0", Message = "Data not Found" };
                    return new ErrorResult(Err, Request);
                }
                List<Answer> delans = db.Answers.Where(x => x.QuestiondId == key).ToList();
                if (delans.Count != 0)
                {
                    db.Answers.RemoveRange(delans);
                    db.SaveChanges();
                }
                List<Test> delTest = db.Tests.Where(x => x.QueId == key).ToList();
                if (delTest.Count != 0)
                {
                    db.Tests.RemoveRange(delTest);
                    db.SaveChanges();
                }
                db.Questions.Remove(question);
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

        // GET: odata/Questions(5)/Answers
        [EnableQuery]
        public IQueryable<Answer> GetAnswers([FromODataUri] int key)
        {
            return db.Questions.Where(m => m.QuestionId == key).SelectMany(m => m.Answers);
        }

        // GET: odata/Questions(5)/Customer
        [EnableQuery]
        public SingleResult<Customer> GetCustomer([FromODataUri] int key)
        {
            return SingleResult.Create(db.Questions.Where(m => m.QuestionId == key).Select(m => m.Customer));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool QuestionExists(int key)
        {
            return db.Questions.Count(e => e.QuestionId == key) > 0;
        }
    }
}
