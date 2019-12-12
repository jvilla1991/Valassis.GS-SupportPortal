using SupportPortal.Models;
using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace SupportPortal.Controllers
{
    public class DataController : Controller
    {
        #region DataAccess
        

        // The Results only apply to queries that require the user to enter parameters
        [HttpPost]
        [Route("Data/QueryRead")]
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
        [Route("Data/QueryDelete")]
        public JsonResult QueryDelete(string[] ids)
        {
            if (ids != null)
            {
                try
                {
                    DAO dao = new DAO();
                    ids = ids.Where(x => x != null).ToArray(); // Removes null elements from ids
                    dao.deleteData(ids);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return Json(new { success = true, JsonRequestBehavior.AllowGet });
        }

        [Route("Data/{query}")]
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
            // Otherwise, we'll pass the dao model to a view where the user can enter parameters to certain queries
            return View("DBParams", dao);
        }
        #endregion

    }
}