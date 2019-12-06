using SupportPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SupportPortal.Controllers
{
    public class ServicesController : Controller
    {
        #region Services
        // The "id" field is looking for an int to be returned from the view. either "1" for start or "0" for stop
        [Route("Services/{serviceNameParam}/{id?}")]
        public ActionResult Services(string serviceNameParam, int? id)
        {
            ServiceViewModel service = new ServiceViewModel(serviceNameParam);
            try
            {
                try
                {
                    service.Initialize();
                    service.GetStatus();
                }
                catch (Exception e)
                {
                    service.Exception = true;
                    service.Status = e.Message;
                }

                // This is skipped when the web page first renders. It will be up the user to determine the procedure.
                if (id == 1)
                {
                    ViewBag.Result = service.Start();
                    new DAO().AuditProcess("Services", serviceNameParam, "Start");
                    Thread.Sleep(5000);
                    service.GetStatus();

                }
                else if (id == 0)
                {
                    ViewBag.Result = service.Stop();
                    new DAO().AuditProcess("Services", serviceNameParam, "Stop");
                    Thread.Sleep(5000);
                    service.GetStatus();

                }
                return View(service);
            }
            catch (Exception e)
            {
                e.ToString();
            }
            return View();

        }

        #endregion
    }
}