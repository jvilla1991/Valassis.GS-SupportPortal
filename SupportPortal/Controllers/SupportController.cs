using System.Web.Mvc;
using System.Web.Routing;

namespace SupportPortal.Controllers
{
    public class SupportController : Controller
    {
        // GET: Support
        public ActionResult Index()
        {
            return View();
        }

        // Renders websites based on the action selected from the layout view
        #region WebSite Renders
        [Route("Support/DalimWebApp/{environment}")]
        public ActionResult DalimWebApp(string environment)
        {
            if (environment.Equals("Prod"))
            {
                ViewBag.URL = "http://valvcsgsw002vm:8090/DalimApp/";
            }
            else if (environment.Equals("Dev"))
            {
                ViewBag.URL = "http://vallomgsw003vm:8090/DalimApp/";
            }

            return View("IframeTemplate");
        }
        public ActionResult DalimAPITestDev()
        {
            ViewBag.URL = "http://vallomeweb5d1vm/SDKAPI/esapi/";

            return View("IframeTemplate");
        }

        [Route("Support/DalimService/{environment}")]
        public ActionResult DalimService(string environment)
        {
            if (environment.Equals("Prod"))
            {
                ViewBag.URL = "http://gswebservices/DalimService/DalimService.asmx";
            }
            else if (environment.Equals("Dev"))
            {
                ViewBag.URL = "http://valvcsmi001vm/DalimServiceESDev/DalimService.asmx";
            }

            return View("IframeTemplate");
        }

        public ActionResult EsCompanion()
        {
            return View();
        }

        public ActionResult GSWebPortal()
        {
            ViewBag.URL = "http://gsapphub/WebPortal/Default.aspx";

            return View("IframeTemplate");
        }

        public ActionResult KnowledgeBase()
        {
            ViewBag.URL = "http://valvcskb001vm/helpconsole6/";

            return View("IframeTemplate");
        }

        [Route("Support/ProcessReports/{id?}")]
        public ActionResult ProcessReports(int? id)
        {
            ViewBag.Parm = id + 1;
            switch (id)
            {
                case 1:
                    ViewBag.URL = "http://gswebservices/OnDemand/ProcessReports.asmx?op=ReloadWorksheetByInsertDate";
                    ViewBag.Message = "Used if a worksheet is not in the Spider Table. Should only be used in an Emergency situation where you are unable to get ahold of a Developer.";
                    ViewBag.Next = "/SupportPortal/Support/ProcessReports/2";
                    ViewBag.NextMessage = "Go to Reload WorkSheet by Worksheet";
                    break;
                case 2:
                    ViewBag.URL = "http://gswebservices/OnDemand/ProcessReports.asmx?op=ReloadWorksheetByWorksheet";
                    ViewBag.Next = "/SupportPortal/Support/ProcessReports/3";
                    ViewBag.NextMessage = "Go to View and Assign to Spider";
                    break;
                case 3:
                    ViewBag.URL = "http://gswebservices/OnDemand/ProcessReports.asmx?op=ViewAndAssignToSpider";
                    break;
                default:
                    ViewBag.URL = "http://gswebservicesqa/OnDemand/ProcessReports.asmx";
                    break;
            }

            return View("IframeTemplate");
        }

        public ActionResult DalimAPIDoc()
        {
            ViewBag.URL = "https://confluence.dalim.com/login.action?os_destination=%2Fdisplay%2FESAPI&permissionViolation=true";

            return View("IframeTemplate");

        }

        public ActionResult WebEIGSystem()
        {
            ViewBag.URL = "http://gsapphub/webeigsystem/default.aspx";

            return View("IframeTemplate");

        }
        #endregion

    }
}