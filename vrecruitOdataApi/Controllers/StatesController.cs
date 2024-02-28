using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
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
    builder.EntitySet<State>("States");
    builder.EntitySet<City>("Cities"); 
    builder.EntitySet<Country>("Countries"); 
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class StatesController : ODataController
    {
        private vRecruitEntities db = new vRecruitEntities();

        // GET: odata/States
        [EnableQuery]
        public IQueryable<State> GetStates()
        {
            return db.States;
        }

        // GET: odata/States(5)
        [EnableQuery]
        public IHttpActionResult GetState([FromODataUri] int key)
        {
            var StateData = db.States.Where(x=>x.CountryId==key).ToList();
            List<StateVM> StateList = new List<StateVM>();
            foreach (var item in StateData)
            {
                StateVM model = new StateVM();
                model.Id = item.Id;
                model.State = item.State1;
                StateList.Add(model);
            }
            if (StateList != null)
            {
                Success Succ = new Success() { Code = "1", Message = "State", Data = StateList };
                return new SuccessResult(Succ, Request);
            }
            else
            {
                Error Err = new Error() { Code = "0", Message = "Data not Found" };
                return new ErrorResult(Err, Request);
            }
        }

        // PUT: odata/States(5)
        public IHttpActionResult Put([FromODataUri] int key, Delta<State> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            State state = db.States.Find(key);
            if (state == null)
            {
                return NotFound();
            }

            patch.Put(state);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StateExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(state);
        }

        // POST: odata/States
        public IHttpActionResult Post(State state)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.States.Add(state);
            db.SaveChanges();

            return Created(state);
        }

        // PATCH: odata/States(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<State> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            State state = db.States.Find(key);
            if (state == null)
            {
                return NotFound();
            }

            patch.Patch(state);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StateExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(state);
        }

        // DELETE: odata/States(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            State state = db.States.Find(key);
            if (state == null)
            {
                return NotFound();
            }

            db.States.Remove(state);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/States(5)/Cities
        [EnableQuery]
        public IQueryable<City> GetCities([FromODataUri] int key)
        {
            return db.States.Where(m => m.Id == key).SelectMany(m => m.Cities);
        }

        // GET: odata/States(5)/Country
        [EnableQuery]
        public SingleResult<Country> GetCountry([FromODataUri] int key)
        {
            return SingleResult.Create(db.States.Where(m => m.Id == key).Select(m => m.Country));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool StateExists(int key)
        {
            return db.States.Count(e => e.Id == key) > 0;
        }
    }
}
