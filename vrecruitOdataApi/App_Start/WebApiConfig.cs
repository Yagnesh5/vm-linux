using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.OData.Builder;
using System.Web.Http.OData.Extensions;
using vrecruit.DataBase.EntityDataModel;
using vrecruitOdataApi.Models.ViewModel;

namespace vrecruitOdataApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
           
            // Web API configuration and services
            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<Company>("Companies");
            builder.EntitySet<AspNetUser>("AspNetUsers");
            builder.EntitySet<AspNetUserLogin>("AspNetUserLogins");
            builder.EntitySet<Company>("Companies");
            builder.EntitySet<QuestionGroup>("QuestionGroups");
            builder.EntitySet<Question>("Questions");
            builder.EntitySet<Answer>("Answers");
            builder.EntitySet<Customer>("Customers");
            builder.EntitySet<Client>("Clients");
            builder.EntitySet<Customer>("Customers");
            builder.EntitySet<UserMapping>("UserMappings");
            builder.EntitySet<Job>("Jobs");
            builder.EntitySet<CandidateExam>("CandidateExams");
            builder.EntitySet<Candidate>("Candidates");
            builder.EntitySet<Activity>("Activities");
            builder.EntitySet<Country>("Countries");
            builder.EntitySet<State>("States");
            builder.EntitySet<City>("Cities");
            builder.EntitySet<Test>("Tests");
            builder.EntitySet<CandidatesResume>("CandidatesResumes");
            builder.EntitySet<SkillMaster>("SkillMasters");
            builder.EntitySet<SendMesssage>("SendMesssages");
            builder.EntitySet<NextAction>("NextActions");
            builder.EntitySet<MCallerDataVM>("MCallerDatas");
            var action = builder.Entity<MCallerDataVM>().Collection.Action("GetData");
            action.Parameter<string>("Id");

            var GetCallHistory = builder.Entity<MCallerDataVM>().Collection.Action("GetCallHistory");
            GetCallHistory.Parameter<string>("Id");
            GetCallHistory.Parameter<string>("ContactType");
            GetCallHistory.Parameter<string>("UserId");

            var getClientAndCandidateActionData = builder.Entity<MCallerDataVM>().Collection.Action("GetClientAndCandidateData");
            getClientAndCandidateActionData.Parameter<string>("Type");
            getClientAndCandidateActionData.Parameter<string>("Name");
            getClientAndCandidateActionData.Parameter<string>("UserId");

            var getMasterDataAction = builder.Entity<MCallerDataVM>().Collection.Action("GetMasterData");

            var getRecentActivityData = builder.Entity<MCallerDataVM>().Collection.Action("GetRecentActivityData");
            getRecentActivityData.Parameter<string>("UserId");

            //builder.EntitySet<AccessPermission>("AccessPermissions");
            //builder.EntitySet<Entity>("Entities");
            config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
        }
    }
}
