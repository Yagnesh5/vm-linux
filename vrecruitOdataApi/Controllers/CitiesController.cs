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
    builder.EntitySet<City>("Cities");
    builder.EntitySet<State>("States"); 
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class CitiesController : ODataController
    {
        private vRecruitEntities db = new vRecruitEntities();

        // GET: odata/Cities
        [EnableQuery]
        public IQueryable<City> GetCities()
        {
            return db.Cities;
        }

        // GET: odata/Cities(5)
        [EnableQuery]
        public IHttpActionResult GetCity([FromODataUri] int key)
        {
            var CityData = db.Cities.Where(x => x.StateId == key).ToList();
            List<CityVM> CityList = new List<CityVM>();
            foreach (var item in CityData)
            {
                CityVM model = new CityVM();
                model.Id = item.Id;
                model.City = item.City1;
                CityList.Add(model);
            }
            if (CityList != null)
            {
                Success Succ = new Success() { Code = "1", Message = "City", Data = CityList };
                return new SuccessResult(Succ, Request);
            }
            else
            {
                Error Err = new Error() { Code = "0", Message = "Data not Found" };
                return new ErrorResult(Err, Request);
            }
        }

        // PUT: odata/Cities(5)
        public IHttpActionResult Put([FromODataUri] int key, Delta<City> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            City city = db.Cities.Find(key);
            if (city == null)
            {
                return NotFound();
            }

            patch.Put(city);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CityExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(city);
        }

        // POST: odata/Cities
        public IHttpActionResult Post(City city)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Cities.Add(city);
            db.SaveChanges();

            return Created(city);
        }

        // PATCH: odata/Cities(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<City> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            City city = db.Cities.Find(key);
            if (city == null)
            {
                return NotFound();
            }

            patch.Patch(city);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CityExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(city);
        }

        // DELETE: odata/Cities(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            City city = db.Cities.Find(key);
            if (city == null)
            {
                return NotFound();
            }

            db.Cities.Remove(city);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/Cities(5)/State
        [EnableQuery]
        public SingleResult<State> GetState([FromODataUri] int key)
        {
            return SingleResult.Create(db.Cities.Where(m => m.Id == key).Select(m => m.State));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CityExists(int key)
        {
            return db.Cities.Count(e => e.Id == key) > 0;
        }
    }
}
