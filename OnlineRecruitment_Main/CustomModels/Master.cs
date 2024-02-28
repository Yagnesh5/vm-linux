using vrecruit.DataBase.EntityDataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using vrecruit.DataBase.ViewModel;

namespace OnlineRecruitment_Main.CustomModels
{
    public class Master
    {
    }

    public class UserViewModel
    {
        public List<AspNetRole> Roleslist { get; set; }
        public List<Company> CompanyList { get; set; }
        public string Id { get; set; }
        public string RoleName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public int CompanyId { get; set; }
        public string PhoneNumber { get; set; }
        public string Image { get; set; }
        public string Password2 { get; set; }
        public string ConfirmPassword2 { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public string ClientAddress { get; set; }
        public string ClientWWW { get; set; }
        public string ClientGST { get; set; }
        public string ClientPhone { get; set; }
        public string ClientEmail { get; set; }
        public string ClientMobile { get; set; }
        public bool ClientStatus { get; set; }
        public virtual Company Company { get; set; }

        public bool IsAdd { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
        public bool IsPrint { get; set; }
        public bool IsExport { get; set; }

        public string LastUpdateDate { get; set; }
    }
    public class RolesViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string LastUpdatedBy { get; set; }
        public bool IsAdd { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
        public bool IsPrint { get; set; }
        public bool IsExport { get; set; }

    }
    public class CompaniesVM : Company
    {
        public bool IsAdd { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
        public bool IsPrint { get; set; }
        public bool IsExport { get; set; }
        public string LastUpdateDateString { get; set; }
    }
    public class CandidateTestViewModel
    {
        public string SubmitDate { get; set; }
        public int ID { get; set; }
        public int QuestionId { get; set; }
        public int CandidateAnswerId { get; set; }
        public bool IsMulti { get; set; }
        public int? TestGroupId { get; set; }
        public string TestLink { get; set; }
        public int? CandidateId { get; set; }
        public string AssignBy { get; set; }
        public string TestName { get; set; }
        public string TestScore { get; set; }
        public string TestStatus { get; set; }

    }

    public class ClientCandidateMappingVM : ClientCandidateMapping
    {
        public bool IsAdd { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
        public bool IsPrint { get; set; }
        public bool IsExport { get; set; }
        public string LastUpdateDateString { get; set; }
    }

    public class ClientCandidateMappingModel
    {
        public int Id { get; set; }
        public Nullable<int> ClientId { get; set; }
        public Nullable<int> CandidateId { get; set; }
    }

    public class InterviewVM
    {
        public int InterviewId { get; set; }
        public string TestName { get; set; }
        public string InterviewVideo { get; set; }
        public List<InterviewEmotionVM> lstEmotions { get; set; }
        public List<InterviewQuestionsTimeVM> lstQuestions { get; set; }
        public List<InterviewFeedbackVM> lstComments { get; set; }
    }

    public class InterviewEmotionVM
    {
        public int Id { get; set; }
        public int? InterviewId { get; set; }
        public double? EmotionTime { get; set; }
        public string EmotionName { get; set; }
        public double? EmotionScore { get; set; }
    }

    public class InterviewCandidatesVM
    {
        public int Id { get; set; }
        public int InterviewId { get; set; }
        public string CandidateName { get; set; }
        public string TestName { get; set; }
    }

    public class InterviewQuestionsTimeVM
    {
        public int Id { get; set; }
        public int InterviewId { get; set; }
        public int? QuestionId { get; set; }
        public double? QuestionTime { get; set; }
        public string QuestionName { get; set; }
        public bool? IsAnswerCorrect { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedDate { get; set; }
    }

    public class InterviewFeedbackVM
    {
        public string Comment { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedByName { get; set; }
        public string CommentDate { get; set; }
    }

}