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

namespace vrecruitOdataApi.Controllers
{
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.Http.OData.Builder;
    using System.Web.Http.OData.Extensions;
    using vrecruit.DataBase.EntityDataModel;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<ClientCandidateMapping>("ClientCandidateMappings");
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class ClientCandidateMappingsController : ODataController
    {
        private vRecruitEntities db = new vRecruitEntities();

        // GET: odata/ClientCandidateMappings
        [EnableQuery]
        public IQueryable<ClientCandidateMapping> GetClientCandidateMappings()
        {
            return db.ClientCandidateMappings;
        }

        // GET: odata/ClientCandidateMappings(5)
        [EnableQuery]
        public SingleResult<ClientCandidateMapping> GetClientCandidateMapping([FromODataUri] int key)
        {
            return SingleResult.Create(db.ClientCandidateMappings.Where(clientCandidateMapping => clientCandidateMapping.Id == key));
        }

        // PUT: odata/ClientCandidateMappings(5)
        public IHttpActionResult Put([FromODataUri] int key, Delta<ClientCandidateMapping> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ClientCandidateMapping clientCandidateMapping = db.ClientCandidateMappings.Find(key);
            if (clientCandidateMapping == null)
            {
                return NotFound();
            }

            patch.Put(clientCandidateMapping);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientCandidateMappingExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(clientCandidateMapping);
        }

        // POST: odata/ClientCandidateMappings
        public IHttpActionResult Post(ClientCandidateMapping clientCandidateMapping)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ClientCandidateMappings.Add(clientCandidateMapping);
            db.SaveChanges();

            return Created(clientCandidateMapping);
        }

        // PATCH: odata/ClientCandidateMappings(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] int key, Delta<ClientCandidateMapping> patch)
        {
            Validate(patch.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ClientCandidateMapping clientCandidateMapping = db.ClientCandidateMappings.Find(key);
            if (clientCandidateMapping == null)
            {
                return NotFound();
            }

            patch.Patch(clientCandidateMapping);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientCandidateMappingExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(clientCandidateMapping);
        }

        // DELETE: odata/ClientCandidateMappings(5)
        public IHttpActionResult Delete([FromODataUri] int key)
        {
            ClientCandidateMapping clientCandidateMapping = db.ClientCandidateMappings.Find(key);
            if (clientCandidateMapping == null)
            {
                return NotFound();
            }

            db.ClientCandidateMappings.Remove(clientCandidateMapping);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ClientCandidateMappingExists(int key)
        {
            return db.ClientCandidateMappings.Count(e => e.Id == key) > 0;
        }
    }
}
