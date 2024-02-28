using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Http.OData;
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
    builder.EntitySet<Test>("Tests");
    builder.EntitySet<Question>("Questions"); 
    builder.EntitySet<QuestionGroup>("QuestionGroups"); 
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class TestsController : ODataController
    {
        private vRecruitEntities db = new vRecruitEntities();
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        int secs = 3;

        // GET: odata/Tests
        [EnableQuery]
        public IHttpActionResult GetTests()
        {
            try
            {
                List<QuestionAnswerVM> quest = new List<QuestionAnswerVM>();
                var list = db.Questions.Where(x=>x.IsActive==true).ToList();

                if (list.Count() > 0)
                {
                    foreach (var item in list)
                    {
                        QuestionAnswerVM VM = new QuestionAnswerVM();
                        VM.Question = item.Question1;
                        VM.QuestionId = item.QuestionId;
                        VM.QuestionSkill = item.QuestionSkill;
                        quest.Add(VM);
                    }
                }
                else
                {
                    Error Err = new Error() { Code = "0", Message = "Data not Found" };
                    return new ErrorResult(Err, Request);
                }
                Success Succ = new Success() { Code = "1", Message = "LoadData", Data = quest };
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

        // GET: odata/Tests(5)
        [EnableQuery]
        public SingleResult<Test> GetTest([FromODataUri] int key)
        {
            return SingleResult.Create(db.Tests.Where(test => test.Id == key));
        }

        // PUT: odata/Tests(5)
        public IHttpActionResult Put([FromODataUri] int key, Delta<Test> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Test test = db.Tests.Find(key);
            if (test == null)
            {
                return NotFound();
            }

            patch.Put(test);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TestExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(test);
        }

        // POST: odata/Tests
        public IHttpActionResult Post(Test test)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Tests.Add(test);
            db.SaveChanges();

            return Created(test);
        }

        // PATCH: odata/Tests(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<Test> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Test test = db.Tests.Find(key);
            if (test == null)
            {
                return NotFound();
            }

            patch.Patch(test);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TestExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(test);
        }

        // DELETE: odata/Tests(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            Test test = db.Tests.Find(key);
            if (test == null)
            {
                return NotFound();
            }

            db.Tests.Remove(test);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/Tests(5)/Question
        [EnableQuery]
        public SingleResult<Question> GetQuestion([FromODataUri] int key)
        {
            return SingleResult.Create(db.Tests.Where(m => m.Id == key).Select(m => m.Question));
        }

        // GET: odata/Tests(5)/QuestionGroup
        [EnableQuery]
        public SingleResult<QuestionGroup> GetQuestionGroup([FromODataUri] int key)
        {
            return SingleResult.Create(db.Tests.Where(m => m.Id == key).Select(m => m.QuestionGroup));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TestExists(int key)
        {
            return db.Tests.Count(e => e.Id == key) > 0;
        }
    }
}
