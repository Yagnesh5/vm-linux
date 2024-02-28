using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using vrecruitOdataApi.CustomModels;
using vrecruit.DataBase.EntityDataModel;
using vrecruit.DataBase.ViewModel;

namespace vrecruitOdataApi.Controllers
{
    public class MasterController : ApiController
    {
        private vRecruitEntities db = new vRecruitEntities();

        [HttpGet]
        public IHttpActionResult GetCompanyName()
        {
            var CmpData = db.Companies.ToList().Select(x => new { x.ID, x.CompanyName });
            if (CmpData != null)
            {
                Success Succ = new Success() { Code = "1", Message = "BindDrp", Data = CmpData };
                return new SuccessResult(Succ, Request);
            }
            else
            {
                Error Err = new Error() { Code = "0", Message = "Data not Found" };
                return new ErrorResult(Err, Request);
            }
        }
    }
    
}
