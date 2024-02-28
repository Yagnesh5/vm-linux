using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using vrecruit.DataBase.EntityDataModel;

namespace vrecruitOdataApi.Controllers
{
    public class CandidateTestURLsController : ApiController
    {
        private vRecruitEntities db = new vRecruitEntities();

        // GET: api/CandidateTestURLs
        public IQueryable<CandidateTestURL> GetCandidateTestURLs()
        {
            return db.CandidateTestURLs;
        }

        // GET: api/CandidateTestURLs/5
        [ResponseType(typeof(CandidateTestURL))]
        public IHttpActionResult GetCandidateTestURL(int id)
        {
            CandidateTestURL candidateTestURL = db.CandidateTestURLs.Find(id);
            if (candidateTestURL == null)
            {
                return NotFound();
            }

            return Ok(candidateTestURL);
        }

        // PUT: api/CandidateTestURLs/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCandidateTestURL(int id, CandidateTestURL candidateTestURL)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != candidateTestURL.CandTestURLId)
            {
                return BadRequest();
            }

            db.Entry(candidateTestURL).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CandidateTestURLExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/CandidateTestURLs
        [ResponseType(typeof(CandidateTestURL))]
        public IHttpActionResult PostCandidateTestURL(CandidateTestURL candidateTestURL)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.CandidateTestURLs.Add(candidateTestURL);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = candidateTestURL.CandTestURLId }, candidateTestURL);
        }

        // DELETE: api/CandidateTestURLs/5
        [ResponseType(typeof(CandidateTestURL))]
        public IHttpActionResult DeleteCandidateTestURL(int id)
        {
            CandidateTestURL candidateTestURL = db.CandidateTestURLs.Find(id);
            if (candidateTestURL == null)
            {
                return NotFound();
            }

            db.CandidateTestURLs.Remove(candidateTestURL);
            db.SaveChanges();

            return Ok(candidateTestURL);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CandidateTestURLExists(int id)
        {
            return db.CandidateTestURLs.Count(e => e.CandTestURLId == id) > 0;
        }
    }
}