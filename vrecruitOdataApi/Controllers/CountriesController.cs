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
    builder.EntitySet<Country>("Countries");
    builder.EntitySet<State>("States"); 
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class CountriesController : ODataController
    {
        private vRecruitEntities db = new vRecruitEntities();

        // GET: odata/Countries
        [EnableQuery]
        public IHttpActionResult GetCountries()
        {
            var CountyData= db.Countries.ToList();
            List<CountryVM> CountyList = new List<CountryVM>();
            foreach (var item in CountyData)
            {
                CountryVM model = new CountryVM();
                model.Id = item.Id;
                model.County = item.Country1;
                CountyList.Add(model);
            }
            if (CountyData != null)
            {
                Success Succ = new Success() { Code = "1", Message = "County", Data = CountyList };
                return new SuccessResult(Succ, Request);
            }
            else
            {
                Error Err = new Error() { Code = "0", Message = "Data not Found" };
                return new ErrorResult(Err, Request);
            }
        }

        // GET: odata/Countries(5)
        [EnableQuery]
        public SingleResult<Country> GetCountry([FromODataUri] int key)
        {
            return SingleResult.Create(db.Countries.Where(country => country.Id == key));
        }

        // PUT: odata/Countries(5)
        public IHttpActionResult Put([FromODataUri] int key, Delta<Country> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Country country = db.Countries.Find(key);
            if (country == null)
            {
                return NotFound();
            }

            patch.Put(country);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CountryExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(country);
        }

        // POST: odata/Countries
        public IHttpActionResult Post(Country country)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Countries.Add(country);
            db.SaveChanges();

            return Created(country);
        }

        // PATCH: odata/Countries(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<Country> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Country country = db.Countries.Find(key);
            if (country == null)
            {
                return NotFound();
            }

            patch.Patch(country);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CountryExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(country);
        }

        // DELETE: odata/Countries(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            Country country = db.Countries.Find(key);
            if (country == null)
            {
                return NotFound();
            }

            db.Countries.Remove(country);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        // GET: odata/Countries(5)/States
        [EnableQuery]
        public IQueryable<State> GetStates([FromODataUri] int key)
        {
            return db.Countries.Where(m => m.Id == key).SelectMany(m => m.States);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CountryExists(int key)
        {
            return db.Countries.Count(e => e.Id == key) > 0;
        }
    }
}
