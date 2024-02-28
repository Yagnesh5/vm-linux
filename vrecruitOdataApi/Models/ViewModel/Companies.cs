using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vrecruitOdataApi.Models.ViewModel
{
    public class Companies
    {
        public int ID { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string CompanyEmail { get; set; }
        public string PhoneNumber { get; set; }
        public string Pincode { get; set; }
        public string UserID { get; set; }
        public string LastUpdateBy { get; set; }
        public string IsActive { get; set; }
    }
    public class QuestionGroupsVM
    {
        public int ID { get; set; }
        public string GroupName { get; set; }
        public int? CompanyId { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public string CompanyName { get; set; }
        public string CrntuserId { get; set; }
    }
}
