using SimpleImpersonation;
using SupportPortal.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceProcess;
using System.Web;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using OfficeOpenXml;
using static System.Net.Mime.MediaTypeNames;
using System.IO;
using Ionic.Zip;
using System.Threading;

namespace SupportPortal.Controllers
{
    public class SupportController : Controller
    {
        // GET: Support
        public ActionResult Index()
        {
            return View();
        }

        #region Services
        // The "id" field is looking for an int to be returned from the view. either "1" for start or "0" for stop
        [Route("Support/Services/{serviceNameParam}/{id?}")]
        public ActionResult Services(string serviceNameParam, int? id)
        {
            DAO dao = new DAO();
            Service service = new Service();

            // Based on the string returned from the view, we'll choose the correct service name as it is on the server
            service.SetName(serviceNameParam);
            service.server = "valvcsgsw001vm";

            var credentials = new UserCredentials("valvcsgsw001vm", "GSappSwAutoProcess", "GsAppSAP2010!");

            try
            {

                string ex = String.Empty;
                try
                {
                    service.Initialize();
                    ViewBag.Status = service.GetStatus();
                }
                catch (Exception e)
                {
                    ViewBag.Status = e.ToString();
                }

                ViewBag.Message1 = service.GetName();

                // This is skipped when the web page first renders. It will be up the user to determine the procedure.
                if (id == 1)
                {
                    // service.Start() will return a string depending on the response from the server that the service is one
                    ViewBag.Result = "Result: " + service.Start();
                    Thread.Sleep(3000);
                    // We'll be passing the status of the service as well
                    ViewBag.Status = service.GetStatus();

                    dao.AuditProcess("Services", serviceNameParam, "Start");

                    return View();
                }
                else if (id == 0)
                {
                    ViewBag.Result = service.Stop();
                    Thread.Sleep(3000);
                    ViewBag.Status = service.GetStatus();

                    dao.AuditProcess("Services", serviceNameParam, "Stop");

                    return View();
                }
                return View();
            }
            catch (Exception e)
            {
                e.ToString();
            }
            return View();

        }

        #endregion


        #region DataAccess
        [Route("Support/QuerySelect/{query}")]
        public ActionResult QuerySelect(string query)
        {
            DAO dao = new DAO();
            DataTable dataTable;

            dao.query = query;
            ViewBag.Query = query;

            // These queries will return the DAO results view - they do not require the user to enter any parameters
            switch (query)
            {
                case "checkimportspider":
                    dataTable = dao.GetData();
                    return View("DBResults", dataTable);

                case "turnkeystatus":
                    dataTable = dao.GetData();
                    return View("DBResults", dataTable);

                case "wrapdbstatus":
                    dataTable = dao.GetData();
                    return View("DBResults", dataTable);

                case "processexec":
                    dataTable = dao.GetData();
                    return View("DBResults", dataTable);

            }

            // Otherwise, we'll pass the dao object to a view where the user can enter parameters to certain queries
            return View("DBParams", dao);
        }

        // The Results only apply to queries that reuqire the user to enter parameters
        [HttpPost]
        public ActionResult QueryRead(DAO dao)
        {
            DataTable dataTable;

            if (dao == null)
            {
                throw new ArgumentNullException(nameof(dao));
            }
            else
            {
                dataTable = dao.GetData();

                dao.AuditProcess("Queries", dao.query, "");

                return View("DBResults", dataTable);
            }
        }

        [HttpPost]
        public JsonResult QueryDelete(string[] ids)
        {
            
            if (ids != null)
            {
                try
                {
                    DAO dao = new DAO();

                    // Removes null elements from ids
                    ids = ids.Where(x => x != null).ToArray();

                    dao.deleteData(ids);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            


            return Json(new { success = true, JsonRequestBehavior.AllowGet });
        }
        #endregion

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