using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vrecruitOdataApi.Models.ViewModel
{
    public class MCallerDataVM
    {
        public string Id { get; set; }
        public string Mobile { get; set; }
        public string Name { get; set; }
        public string CallDuration { get; set; }
        public string DateTime { get; set; }
        public string CallType { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public string NextAction { get; set; }
        public string SendSMSMessage { get; set; }
        public string UserId { get; set; }
        public string CandidateId { get; set; }
        public string ClientId { get; set; }
        public string ContactType { get; set; }
    }

    public class CandidateClientVM
    {
        public string Name { get; set; }
        public string Number { get; set; }
        public string Id { get; set; }
    }

    public class lstMasterVM
    {
        public List<MasterVM> NextAction { get; set; }
        public List<MasterVM> Template { get; set; }

    }
    public class MasterVM
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
    public class RecentActivityVM
    {

    }
}
