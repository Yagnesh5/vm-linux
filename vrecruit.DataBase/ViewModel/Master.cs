using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using vrecruit.DataBase.EntityDataModel;

namespace vrecruit.DataBase.ViewModel
{
    public class QuestionAnswerVM
    {
        public QuestionAnswerVM()
        {
            this.questionList = new List<Question>();
        }
        public string SelectedAnswer { get; set; }
        public string CorrectAnswer { get; set; }
        public List<string> SelectedAnswerList { get; set; }
        public List<string> CorrectAnswerList { get; set; }
        public List<QuestionGroup> QuegroupList { get; set; }
        public ICollection<Answer> Answerlst { get; set; }
        public List<Company> CompanyList { get; set; }
        public List<Answer> optionAnswersliist { get; set; }
        public List<QuestionImages> QuestionImagelist { get; set; }
        public int QuestionId { get; set; }
        public string Question { get; set; }
        public int? QuestionGroupId { get; set; }
        public string QuestionGroupName { get; set; }
        public bool? QuestionStatus { get; set; }
        public string QuestionType { get; set; }
        public string QuestionSkill { get; set; }
        public SelectList QuestionSkillList { get; set; }
        public int? CompanyId { get; set; }
        public string CompanyName { get; set; }
        public int? CustomerId { get; set; }
        public Guid? LastUpdateBy { get; set; }
        public int AnswerId { get; set; }
        public string Answer { get; set; }
        public bool AnswerCorrect { get; set; }
        public string Option1 { get; set; }
        public string Option2 { get; set; }
        public string Option3 { get; set; }
        public string Option4 { get; set; }
        public string Option5 { get; set; }
        public string Option6 { get; set; }
        public string Option7 { get; set; }
        public string Option8 { get; set; }
        public string Option9 { get; set; }
        public string Option10 { get; set; }
        public bool AnsChk1 { get; set; }
        public bool AnsChk2 { get; set; }
        public bool AnsChk3 { get; set; }
        public bool AnsChk4 { get; set; }
        public bool AnsChk5 { get; set; }
        public bool AnsChk6 { get; set; }
        public bool AnsChk7 { get; set; }
        public bool AnsChk8 { get; set; }
        public bool AnsChk9 { get; set; }
        public bool AnsChk10 { get; set; }
        public string Createdby { get; set; }
        public Guid? CrntuserId { get; set; }
        public string SearchQuestionType { get; set; }

        public List<Question> questionList { get; set; }
        public string Candidateemail { get; set; }
        public string useremail { get; set; }
        public int testSecond { get; set; }
        public List<QuestionAnswerImage> imagelist { get; set; }

        public bool IsAdd { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
        public bool IsPrint { get; set; }
        public bool IsExport { get; set; }
    }
    public class OptionAns
    {
        public int Id { get; set; }
        public string Value { get; set; }
    }

    public class ClientVM
    {
        public List<Company> CompanyList { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public string ClientAddress { get; set; }
        public string ClientGST { get; set; }
        public string ClientPhone { get; set; }
        public string ClientEmail { get; set; }
        public string ClientMobile { get; set; }
        public bool? ClientStatus { get; set; }
        public int? CustomerId { get; set; }
        public int? CompanyId { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string ClientWWW { get; set; }
        public Guid? LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string LastUpdateDateString { get; set; }
        public string UserId { get; set; }
        public string Password2 { get; set; }
        public string ConfirmPassword2 { get; set; }
        public string Contact_Person { get; set; }
        public string Client_PAN_No { get; set; }
        public string UserName { get; set; }
        public string ClientIsactive { get; set; }
        public bool IsAdd { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
        public bool IsPrint { get; set; }
        public bool IsExport { get; set; }
        public string ClientCity { get; set; }
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public List<Country> CountryList { get; set; }
    }
    public class JobVM
    {
        public int JobId { get; set; }
        public string JobName { get; set; }
        public string JobDescription { get; set; }
        public bool JobStatus { get; set; }
        public int? JobCount { get; set; }
        public string JobTargetDate { get; set; }
        public int? ClientId { get; set; }
        public string LastUpdatedBy { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public int Years_of_Exp { get; set; }
        public string Location { get; set; }
        public string Contact_Person { get; set; }
        public string ClientName { get; set; }
        public string uniqueJobid { get; set; }
        public string LastUpdateDateString { get; set; }
        public string JobStatusString { get; set; }
        public string LastUpdatedByName { get; set; }
    }
    public class JobViewModel
    {
        public JobVM JM { get; set; }
        public List<JobVM> joblst { get; set; }
        public List<Client> ClientList { get; set; }
        public int? JobCount { get; set; }
        public string JobDescription { get; set; }
        public string ClientName { get; set; }
        public string CompanyName { get; set; }
        public string LastUpdatedBy { get; set; }
        public int JobId { get; set; }
        public int CompanyId { get; set; }
        public string JobName { get; set; }
        public string JobStatus { get; set; }
        public bool IsAdd { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
        public bool IsPrint { get; set; }
        public bool IsExport { get; set; }
        public bool IsView { get; set; }
        public string LastUpdateDateString { get; set; }

    }
    public class CandidateVM
    {
        public int CandidateId { get; set; }
        public string CandidateName { get; set; }
        public string CandidateDOB { get; set; }
        public string CandidateSkill { get; set; }
        public string CandidateResume { get; set; }
        public int? CustomerId { get; set; }
        public Guid? LastUpdateBy { get; set; }
        public string CandidatePhone { get; set; }
        public string CandidateEmail { get; set; }
        public string CandidatePhone2 { get; set; }
        public string CandidateLocation { get; set; }
        public string CandidateLastJobChange { get; set; } //Previous Organisation
        public string PreviousDesignation { get; set; }
        public string CurrentOrganisation { get; set; }
        public string CandidateExperience { get; set; }
        public string CandidateExperienceYears { get; set; }
        public string CandidateExperienceMonths { get; set; }
        public string SkypeId { get; set; }
        public decimal CurrentCTC { get; set; }
        public decimal ExpectedCTC { get; set; }
        public bool BuyoutOption { get; set; }
        public string Education { get; set; }
        public decimal OffersInHand { get; set; }
        public int NoticePeriod { get; set; }
        public string ClientDesignation { get; set; }
        public string Remarks { get; set; }
        public List<Client> ClientList { get; set; }
        public int ClientId { get; set; }
        public List<ActivityVM> ActivityList { get; set; }
        public int CompanyId { get; set; }
        public List<ActivityType> ActivityTypeList { get; set; }
        public string ActivityType1 { get; set; }
        public string TestName { get; set; }
        public List<Test> TestList { get; set; }
        public string Activity1 { get; set; }
        public string Relevant_Exp { get; set; }
        public string Preferred_Location { get; set; }
        public string Last_Job_change_reason { get; set; }
        public string LastUpdateDateString { get; set; }
        public List<CandidatesResume> CandidateResumesList { get; set; }
        public int EntityId { get; set; }
        public bool IsAdd { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
        public bool IsPrint { get; set; }
        public bool IsExport { get; set; }
        public string Entityname { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public string CandidateVideo { get; set; }
        public string ClientCurrentRole { get; set; }
        public int InterviewId { get; set; }
    }
    public class ActivityVM
    {
        public ActivityVM()
        {
            this.Activities = new List<Activity>();
        }

        public int ActivityId { get; set; }
        public string ActivityType { get; set; }
        public string Activity1 { get; set; }
        public string ActivityDate { get; set; }
        public string ActivityRemark { get; set; }
        public string ActivityComplete { get; set; }
        public int CandId { get; set; }
        public int JobId { get; set; }
        public string UserId { get; set; }
        public int CustomerId { get; set; }
        public int ClientId { get; set; }
        public string JobName { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public List<ActivityType> ActivityTypeList { get; set; }
        public List<Activity> Activities { get; set; }
        public string day { get; set; }
        public string ClientIds { get; set; }
        public string TestLink { get; set; }
        public string UserEmail { get; set; }
        public string JobDescription { get; set; }
        public int JobCount { get; set; }
        public string CandidateName { get; set; }
        public string JobLocation { get; set; }
        public string ContactPerson { get; set; }
    }
    public class Testlist
    {
        public string GroupId { get; set; }
        public string QueId { get; set; }
        public string TestName { get; set; }
        public int CompanyId { get; set; }
        public List<int> AnsId { get; set; }
        public int TotalCount { get; set; }
        public string canEmail { get; set; }
        public string useremail { get; set; }
        public bool isMulti { get; set; }
    }
    public class IndexVM
    {
        public int TotalComapnies { get; set; }
        public int TotalCandidates { get; set; }
        public int TotalClients { get; set; }
        public int TotalActiveCandidates { get; set; }
        public int TotalJobs { get; set; }
        public int TotalUser { get; set; }
    }
    public class CountryVM
    {
        public int Id { get; set; }
        public string County { get; set; }
    }
    public class StateVM
    {
        public int Id { get; set; }
        public string State { get; set; }
        public int CountyId { get; set; }
    }
    public class CityVM
    {
        public int Id { get; set; }
        public string City { get; set; }
        public int StateId { get; set; }
    }
    public class QuestionImages
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string Images { get; set; }
    }
    public class EmailViewModels
    {
        public string Subject { get; set; }
        public string email { get; set; }
        public string passwordlink { get; set; }
        public string body { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public string Smtpemail { get; set; }
        public string Smtppassword { get; set; }
        public string Smtp { get; set; }
        public int Port { get; set; }
    }
    public class AccessPermissionVM
    {
        public int Id { get; set; }
        public string RoleId { get; set; }
        public int EntityId { get; set; }
        public bool IsAdd { get; set; }
        public bool IsView { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
        public bool IsPrint { get; set; }
        public bool IsExport { get; set; }
        public string Entityname { get; set; }
        public List<Entity> EntityLst { get; set; }
    }
    public class QuestionGroupVM
    {
        public int ID { get; set; }
        public string GroupName { get; set; }
        public int CompanyId { get; set; }
        public string LastUpdateBy { get; set; }
        public List<Company> CompanyList { get; set; }
        public bool IsAdd { get; set; }
        public bool IsView { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
        public bool IsPrint { get; set; }
        public bool IsExport { get; set; }
    }

    public class CandidateTestStatusVM : Candidate
    {
        public string TestName { get; set; }
        public string TestLink { get; set; }
        public string TestStatus { get; set; }
        public string TestScore { get; set; }
        public bool IsAdd { get; set; }
        public bool IsView { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
        public bool IsPrint { get; set; }
        public bool IsExport { get; set; }
    }

    public class SkillMasterModel : SkillMaster
    {
        public string LastUpdateDateString { get; set; }
        public bool IsAdd { get; set; }
        public bool IsView { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
        public bool IsPrint { get; set; }
        public bool IsExport { get; set; }
    }

    public class NextActionVM : NextAction
    {
        public bool IsAdd { get; set; }
        public bool IsView { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
        public bool IsPrint { get; set; }
        public bool IsExport { get; set; }
    }

    public class SendMesssageVM : SendMesssage
    {
        public bool IsAdd { get; set; }
        public bool IsView { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
        public bool IsPrint { get; set; }
        public bool IsExport { get; set; }
    }   

    public class MCallDataVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ContactType { get; set; }
        public string Mobile { get; set; }
        public string CallType { get; set; }
        public string Stauts { get; set; }
        public string CallDuration { get; set; }
        public string DateTime { get; set; }
        public string Remarks { get; set; }
        public string NextAction { get; set; }
        public string CalledBy { get; set; }
    }
}