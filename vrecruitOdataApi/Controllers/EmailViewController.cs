using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using vrecruit.DataBase.ViewModel;

namespace vrecruitOdataApi.Controllers
{
    public class EmailViewController : Controller
    {
        // GET: EmailView
        [HttpPost]
        public string EmailTemplate(string viewName, ActivityVM model)
        {
            var routeData = new RouteData();
            routeData.Values.Add("controller", "EmailView");
            var ControllerContext = new ControllerContext(new HttpContextWrapper(new HttpContext(new HttpRequest(null, "http://google.com", null), new HttpResponse(null))), routeData, new FakeController());

            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext,viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View,
                                             ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }

        }
        
        public ControllerContext getControllerContext()
        {
            var routeData = new RouteData();
            routeData.Values.Add("controller", "EmailView");
            var fakeControllerContext = new ControllerContext(new HttpContextWrapper(new HttpContext(new HttpRequest(null, "http://google.com", null), new HttpResponse(null))), routeData, new FakeController());
            return fakeControllerContext;
        }   
    }

}