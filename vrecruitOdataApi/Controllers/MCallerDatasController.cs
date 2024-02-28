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
using vrecruitOdataApi.Models.ViewModel;

namespace vrecruitOdataApi.Controllers
{
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.Http.OData.Builder;
    using System.Web.Http.OData.Extensions;
    using vrecruit.DataBase.EntityDataModel;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<Candidate>("Candidates");
    builder.EntitySet<Activity>("Activities"); 
    builder.EntitySet<Customer>("Customers"); 
    builder.EntitySet<CandidateExam>("CandidateExams"); 
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    public class MCallerDatasController : ODataController
    {
        private vRecruitEntities db = new vRecruitEntities();
        AspNetUser UserDetails = new AspNetUser();

        [Authorize]
        // POST: odata/MCallerUpload
        [HttpPost]
        public IHttpActionResult Post(MCallerDataVM model)
        {
            if (model != null)
            {
                try
                {
                    MCallerData MCallerCandidate = new MCallerData();
                    UserDetails = db.AspNetUsers.Where(x => x.UserName == model.UserId).FirstOrDefault();
                    if (string.IsNullOrEmpty(model.Id))
                    {
                        if (string.IsNullOrWhiteSpace(model.CandidateId) && string.IsNullOrWhiteSpace(model.ClientId))
                        {
                            if (!string.IsNullOrWhiteSpace(model.Mobile) && !string.IsNullOrWhiteSpace(model.Name) && !string.IsNullOrWhiteSpace(model.ContactType))
                            {
                                var mobile = "(" + model.Mobile.Substring(0, 3) + ") " + model.Mobile.Substring(3, 3) + "-" + model.Mobile.Substring(6, 4);
                                if (model.ContactType.ToLower().Equals("client"))
                                {
                                    Client clientData = new Client();
                                    clientData = db.Clients.Where(x => x.ClientPhone == mobile || x.ClientMobile == mobile).FirstOrDefault();
                                    if (clientData != null)
                                    {
                                        MCallerCandidate.ClientId = clientData.ClientId;
                                    }
                                    else
                                    {
                                        clientData = new Client();
                                        clientData.ClientName = model.Name;
                                        clientData.ClientMobile = mobile;
                                        clientData.CompanyId = UserDetails.CompanyId;
                                        clientData.IsActive = true;
                                        db.Clients.Add(clientData);
                                        db.SaveChanges();

                                        //clientData = db.Clients.Where(x => x.ClientMobile == mobile && x.ClientName == model.Name).FirstOrDefault();
                                        MCallerCandidate.ClientId = clientData.ClientId;
                                    }
                                }
                                else if (model.ContactType.ToLower().Equals("candidate"))
                                {
                                    Candidate candidateData = new Candidate();
                                    candidateData = db.Candidates.Where(x => x.CandidatePhone == mobile || x.CandidatePhone2 == mobile).FirstOrDefault();
                                    if (candidateData != null)
                                    {
                                        MCallerCandidate.CandidateId = Convert.ToInt16(candidateData.CandidateId);
                                    }
                                    else
                                    {
                                        candidateData = new Candidate();
                                        candidateData.CandidateName = model.Name;
                                        candidateData.CandidatePhone = mobile;
                                        candidateData.CompanyId = UserDetails.CompanyId;
                                        candidateData.CreatedBy = new Guid(UserDetails.Id);
                                        candidateData.Created = System.DateTime.Today.Date;
                                        candidateData.IsActive = true;
                                        db.Candidates.Add(candidateData);
                                        db.SaveChanges();

                                        MCallerCandidate.CandidateId = candidateData.CandidateId;
                                    }
                                }
                                else
                                {
                                    Error Err = new Error() { Code = "400", Message = "Contact type is not proper." };
                                    return new ErrorResult(Err, Request);
                                }
                            }
                            else
                            {
                                Error Err = new Error() { Code = "400", Message = "Bad Request, If Client and Candidate Id both are null, then Mobile, Name and Contact Type fields are required" };
                                return new ErrorResult(Err, Request);
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(model.CandidateId) && !string.IsNullOrWhiteSpace(model.ClientId))
                        {
                            Error Err = new Error() { Code = "400", Message = "Bad Request, Either Candidate Id or Client Id Required" };
                            return new ErrorResult(Err, Request);
                        }

                        if (string.IsNullOrWhiteSpace(model.NextAction) || string.IsNullOrWhiteSpace(model.SendSMSMessage))
                        {
                            Error Err = new Error() { Code = "400", Message = "Bad Request, Next Action and Send Message Id should not be null" };
                            return new ErrorResult(Err, Request);
                        }


                        MCallerCandidate.CallDuration = model.CallDuration;
                        MCallerCandidate.DateTime = Convert.ToDateTime(model.DateTime);
                        MCallerCandidate.CallType = model.CallType;
                        MCallerCandidate.Status = model.Status;
                        MCallerCandidate.Remarks = model.Remarks;
                        if (!string.IsNullOrWhiteSpace(model.CandidateId))
                        {
                            MCallerCandidate.CandidateId = Convert.ToInt16(model.CandidateId);
                        }
                        if (!string.IsNullOrWhiteSpace(model.ClientId))
                        {
                            MCallerCandidate.ClientId = Convert.ToInt16(model.ClientId);
                        }
                        MCallerCandidate.NextAction = model.NextAction;
                        MCallerCandidate.SendSMSMessage = model.SendSMSMessage;


                        if (UserDetails == null)
                        {
                            MCallerCandidate.UserId = "";
                        }
                        else
                        {
                            MCallerCandidate.UserId = UserDetails.Id;
                        }

                        db.MCallerDatas.Add(MCallerCandidate);
                        db.SaveChanges();

                        model.Id = MCallerCandidate.Id.ToString();

                        Success Succ = new Success() { Code = "200", Message = "Data Saved Successfully", Data = model };
                        return new SuccessResult(Succ, Request);

                    }
                    else
                    {
                        int mCallerId = Convert.ToInt16(model.Id);
                        MCallerCandidate = db.MCallerDatas.Where(x => x.Id == mCallerId).FirstOrDefault();
                        MCallerCandidate.NextAction = model.NextAction;
                        MCallerCandidate.SendSMSMessage = model.SendSMSMessage;
                        MCallerCandidate.Remarks = model.Remarks;

                        //db.MCallerDatas.Add(MCallerCandidate);
                        db.SaveChanges();

                        Success Succ = new Success() { Code = "200", Message = "Data Saved Successfully", Data = model };
                        return new SuccessResult(Succ, Request);

                    }
                }
                catch (Exception e)
                {
                    Error Err = new Error() { Code = "500", Message = "" };
                    return new ErrorResult(Err, Request);
                }
            }
            else
            {
                Error Err = new Error() { Code = "400", Message = "Bad Request, Null Model" };
                return new ErrorResult(Err, Request);
            }

        }

        [Authorize]
        // POST: odata/GetData
        [HttpPost]
        public IHttpActionResult GetData(ODataActionParameters parameters)
        {
            try
            {
                string id = parameters.Where(x => x.Key == "Id").Select(y => y.Value).FirstOrDefault().ToString();
                if (!string.IsNullOrEmpty(id))
                {
                    MCallerData MCallerCandidate = new MCallerData();
                    int Id = Convert.ToInt16(id);
                    MCallerCandidate = db.MCallerDatas.Where(x => x.Id == Id).FirstOrDefault();

                    MCallerDataVM returnModel = new MCallerDataVM();
                    returnModel.Mobile = MCallerCandidate.Client != null ? MCallerCandidate.Client.ClientMobile : MCallerCandidate.Candidate.CandidatePhone;
                    returnModel.Name = MCallerCandidate.Client != null ? MCallerCandidate.Client.ClientName : MCallerCandidate.Candidate.CandidateName;
                    returnModel.CallDuration = MCallerCandidate.CallDuration;
                    returnModel.DateTime = MCallerCandidate.DateTime.ToString();
                    returnModel.CallType = MCallerCandidate.CallType;
                    returnModel.Status = MCallerCandidate.Status;
                    returnModel.Remarks = MCallerCandidate.Remarks;
                    returnModel.NextAction = MCallerCandidate.NextAction;
                    returnModel.SendSMSMessage = MCallerCandidate.SendSMSMessage;
                    returnModel.UserId = MCallerCandidate.AspNetUser.UserName;
                    returnModel.CandidateId = MCallerCandidate.CandidateId.ToString();
                    returnModel.ClientId = MCallerCandidate.ClientId.ToString();

                    Success Succ = new Success() { Code = "200", Message = "Data Fetched Successfully", Data = returnModel };
                    return new SuccessResult(Succ, Request);
                }
                else
                {
                    Error Err = new Error() { Code = "400", Message = "Bad Request, MCaller Data id should not be null" };
                    return new ErrorResult(Err, Request);
                }
            }
            catch (Exception e)
            {
                Error Err = new Error() { Code = "500", Message = "Server Error" };
                return new ErrorResult(Err, Request);
            }

        }

        [Authorize]
        // POST: odata/GetData
        [HttpPost]
        public IHttpActionResult GetCallHistory(ODataActionParameters parameters)
        {
            try
            {
                string id = parameters.Where(x => x.Key == "Id").Select(y => y.Value).FirstOrDefault().ToString();
                string ContactType = parameters.Where(x => x.Key == "ContactType").Select(y => y.Value).FirstOrDefault().ToString();
                string UserId = parameters.Where(x => x.Key == "UserId").Select(y => y.Value).FirstOrDefault().ToString();
                if (!string.IsNullOrEmpty(id))
                {
                    UserDetails = db.AspNetUsers.Where(x => x.UserName == UserId).FirstOrDefault();

                    MCallerData MCallerCandidate = new MCallerData();
                    int Id = Convert.ToInt16(id);

                    if (MCallerCandidate != null)
                    {
                        string flag = MCallerCandidate.Client != null ? "Client" : "Candidate";
                        List<MCallerDataVM> returnModel = new List<MCallerDataVM>();
                        if (ContactType.ToLower().Equals("client"))
                        {
                            returnModel = db.MCallerDatas.Where(x => x.ClientId == Id && x.UserId == UserDetails.Id).Select(x => new MCallerDataVM()
                            {
                                Id = x.Id.ToString(),
                                Name = x.Client.ClientName,
                                Mobile = x.Client.ClientMobile,
                                CallDuration = x.CallDuration.ToString(),
                                ContactType = "Client",
                                DateTime = x.DateTime.ToString(),
                                CallType = x.CallType,
                                Status = x.Status,
                                Remarks = x.Remarks,
                                NextAction = x.NextAction,
                                SendSMSMessage = x.SendSMSMessage,
                                UserId = x.AspNetUser.UserName,
                                CandidateId = x.CandidateId.ToString(),
                                ClientId = x.ClientId.ToString(),
                            }).OrderByDescending(x => x.DateTime).ToList();
                        }
                        else
                        {
                            returnModel = db.MCallerDatas.Where(x => x.CandidateId ==Id && x.UserId == UserDetails.Id).Select(x => new MCallerDataVM()
                            {
                                Id = x.Id.ToString(),
                                Name = x.Candidate.CandidateName,
                                Mobile = x.Candidate.CandidatePhone,
                                CallDuration = x.CallDuration.ToString(),
                                ContactType = "Candidate",
                                DateTime = x.DateTime.ToString(),
                                CallType = x.CallType,
                                Status = x.Status,
                                Remarks = x.Remarks,
                                NextAction = x.NextAction,
                                SendSMSMessage = x.SendSMSMessage,
                                UserId = x.AspNetUser.UserName,
                                CandidateId = x.CandidateId.ToString(),
                                ClientId = x.ClientId.ToString(),
                            }).OrderByDescending(x => x.DateTime).ToList();
                        }

                        Success Succ = new Success() { Code = "200", Message = "Data Fetched Successfully", Data = returnModel };
                        return new SuccessResult(Succ, Request);
                    }
                    else
                    {
                        Error Err = new Error() { Code = "400", Message = "Data not found" };
                        return new ErrorResult(Err, Request);
                    }
                }
                else
                {
                    Error Err = new Error() { Code = "400", Message = "Bad Request, MCaller Data id should not be null" };
                    return new ErrorResult(Err, Request);
                }
            }
            catch (Exception e)
            {
                Error Err = new Error() { Code = "500", Message = "Server Error" };
                return new ErrorResult(Err, Request);
            }
        }

        [Authorize]
        // POST: odata/GetClientAndCandidateData
        [HttpPost]
        public IHttpActionResult GetClientAndCandidateData(ODataActionParameters parameters)
        {
            try
            {
                string type = parameters.Where(x => x.Key == "Type").Select(y => y.Value).FirstOrDefault().ToString();
                string Name = parameters.Where(x => x.Key == "Name").Select(y => y.Value).FirstOrDefault().ToString();
                string userId = parameters.Where(x => x.Key == "UserId").Select(y => y.Value).FirstOrDefault().ToString();
                if (!string.IsNullOrWhiteSpace(userId))
                {
                    AspNetUser user = db.AspNetUsers.Where(x => x.UserName == userId).FirstOrDefault();
                    List<CandidateClientVM> ReturnData = new List<CandidateClientVM>();
                    if (type.ToLower().Equals("client"))
                    {

                        if (string.IsNullOrWhiteSpace(Name))
                        {
                            ReturnData = db.Clients.Where(t => t.IsActive == true && t.CompanyId == user.CompanyId).Select(x => new CandidateClientVM()
                            {
                                Name = x.ClientName,
                                Id = x.ClientId.ToString(),
                                Number = x.ClientPhone,

                            }).ToList();

                        }
                        else
                        {
                            ReturnData = db.Clients.Where(t => t.IsActive == true && t.CompanyId == user.CompanyId).Select(x => new CandidateClientVM()
                            {
                                Name = x.ClientName,
                                Id = x.ClientId.ToString(),
                                Number = x.ClientPhone,

                            }).Where(z => z.Name.StartsWith(Name)).ToList();
                        }

                        Success Succ = new Success() { Code = "200", Message = "Client Data Fetched Successfully", Data = ReturnData };
                        return new SuccessResult(Succ, Request);
                    }
                    else if (type.ToLower().Equals("candidate"))
                    {

                        if (string.IsNullOrWhiteSpace(Name))
                        {
                            ReturnData = db.Candidates.Where(z => z.CompanyId == user.CompanyId && z.IsActive == true).Select(x => new CandidateClientVM()
                            {
                                Name = x.CandidateName,
                                Id = x.CandidateId.ToString(),
                                Number = x.CandidatePhone
                            }).ToList();
                        }
                        else
                        {
                            ReturnData = db.Candidates.Where(z => z.CompanyId == user.CompanyId && z.IsActive == true).Select(x => new CandidateClientVM()
                            {
                                Name = x.CandidateName,
                                Id = x.CandidateId.ToString(),
                                Number = x.CandidatePhone
                            }).Where(z => z.Name.StartsWith(Name)).ToList();
                        }

                        Success Succ = new Success() { Code = "200", Message = "Candidate Data Fetched Successfully", Data = ReturnData };
                        return new SuccessResult(Succ, Request);
                    }
                    else
                    {
                        Error Err = new Error() { Code = "400", Message = "Bad Request, Please pass proper type" };
                        return new ErrorResult(Err, Request);
                    }
                }
                else
                {
                    Error Err = new Error() { Code = "400", Message = "Bad Request, User Id should not be null" };
                    return new ErrorResult(Err, Request);
                }
            }
            catch (Exception e)
            {
                Error Err = new Error() { Code = "500", Message = "Server Error" };
                return new ErrorResult(Err, Request);
            }
        }

        [Authorize]
        // POST: odata/GetMasterData
        [HttpPost]
        public IHttpActionResult GetMasterData()
        {
            try
            {
                lstMasterVM ReturnData = new lstMasterVM();

                List<MasterVM> nextAction = new List<MasterVM>();
                nextAction = db.NextActions.Select(x => new MasterVM()
                {
                    Id = x.Id.ToString(),
                    Name = x.Name
                }).ToList();

                List<MasterVM> template = new List<MasterVM>();
                template = db.SendMesssages.Select(x => new MasterVM()
                {
                    Id = x.Id.ToString(),
                    Name = x.Name
                }).ToList();

                ReturnData.NextAction = nextAction;
                ReturnData.Template = template;

                Success Succ = new Success() { Code = "200", Message = "Client Data Fetched Successfully", Data = ReturnData };
                return new SuccessResult(Succ, Request);

            }
            catch (Exception e)
            {
                Error Err = new Error() { Code = "500", Message = "Server Error" };
                return new ErrorResult(Err, Request);
            }
        }

        [Authorize]
        // POST: odata/GetRecentActivityData
        [HttpPost]
        public IHttpActionResult GetRecentActivityData(ODataActionParameters parameters)
        {
            try
            {
                List<MCallerDataVM> ReturnData = new List<MCallerDataVM>();

                string userId = parameters.Where(x => x.Key == "UserId").Select(y => y.Value).FirstOrDefault().ToString();
                if (!string.IsNullOrWhiteSpace(userId))
                {
                    var tenDaysAgo = DateTime.Today.AddDays(-10);
                    AspNetUser user = db.AspNetUsers.Where(x => x.UserName == userId).FirstOrDefault();
                    if (user != null)
                    {
                        List<MCallerDataVM> lstClientMCallerData = new List<MCallerDataVM>();
                        lstClientMCallerData = db.MCallerDatas.Where(x => x.DateTime >= tenDaysAgo && x.UserId == user.Id && (x.ClientId != null || x.CandidateId != null)).OrderByDescending(x => x.DateTime).Select(x => new MCallerDataVM()
                        {
                            Id = x.Id.ToString(),
                            Name = x.Client != null ? x.Client.ClientName : x.Candidate.CandidateName,
                            Mobile = x.Client != null ? x.Client.ClientMobile : x.Candidate.CandidatePhone,
                            CallDuration = x.CallDuration.ToString(),
                            ContactType = x.Client != null ? "Client" : "Candidate",
                            DateTime = x.DateTime.ToString(),
                            CallType = x.CallType,
                            Status = x.Status,
                            Remarks = x.Remarks,
                            NextAction = x.NextAction,
                            SendSMSMessage = x.SendSMSMessage,
                            UserId = x.AspNetUser.UserName,
                            CandidateId = x.CandidateId.ToString(),
                            ClientId = x.ClientId.ToString(),
                        }).ToList();

                        //List<MCallerDataVM> lstCandidateMCallerData = new List<MCallerDataVM>();
                        //lstCleintMCallerData = db.MCallerDatas.Where(x => x.Client != null && x.DateTime >= tenDaysAgo).Select(x => new MCallerDataVM()
                        //{
                        //    Id = x.Id.ToString(),
                        //    Name = x.Candidate.CandidateName,
                        //    Mobile = x.Candidate.CandidatePhone,
                        //    ContactType = "Candidate"

                        //}).ToList();
                        ReturnData.AddRange(lstClientMCallerData);
                        //ReturnData.AddRange(lstCandidateMCallerData);

                        Success Succ = new Success() { Code = "200", Message = "Recent Activatiy Data Fetched Successfully", Data = ReturnData };
                        return new SuccessResult(Succ, Request);
                    }
                    else
                    {
                        Error Err = new Error() { Code = "400", Message = "Bad Request, Not Found user against this user id" };
                        return new ErrorResult(Err, Request);
                    }
                }
                else
                {
                    Error Err = new Error() { Code = "400", Message = "Bad Request, User Id should not be null" };
                    return new ErrorResult(Err, Request);

                }

            }
            catch (Exception e)
            {
                Error Err = new Error() { Code = "500", Message = "Server Error" };
                return new ErrorResult(Err, Request);
            }
        }
    }
}
