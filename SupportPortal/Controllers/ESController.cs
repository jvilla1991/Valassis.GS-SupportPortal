using SupportPortal.Infrastructure;
using SupportPortal.Models;
using SupportPortal.Models.ES;
using SupportPortal.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SupportPortal.Controllers
{
    public class ESController : Controller
    {
        private static readonly ConfigSettings CONFIG = new ConfigSettings();
        private static readonly Common COMMON = new Common();
        private static WebService WEBSERVICE = new WebService();

        public ActionResult EsCompanion()
        {
            return View();
        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult UpdateJobMetadata(ESCompanionViewModel esViewModel)
        {
            string url = CONFIG.GetDalimRestServiceUrl(esViewModel.CurrentEnvironmment);
            url += String.Format("{0}/upsert/job/{1}/{2}",
                esViewModel.Customer,
                esViewModel.JobId,
                CONFIG.ApplicationName);

            string response = WEBSERVICE.SendGetRequest(url);
            //object obj = new JavaScriptSerializer().Deserialize<object>(response);
            return Json(response, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult UpdateSectionMetadata(ESCompanionViewModel esViewModel)
        {
            string url = CONFIG.GetDalimRestServiceUrl(esViewModel.CurrentEnvironmment);
            url += COMMON.IsEmptyOrNull(esViewModel.PageOrder) ? 
            String.Format("{0}/update/section/{1}/{2}",
                esViewModel.Customer,
                esViewModel.JobId,
                CONFIG.ApplicationName) :
            String.Format("{0}/update/section/{1}/{2}/{3}", 
                esViewModel.Customer, 
                esViewModel.JobId, 
                esViewModel.PageOrder, 
                CONFIG.ApplicationName);

            if(esViewModel.Customer.Equals("fsi"))
            {
                url = url.Replace("section", esViewModel.FSIUpdateTarget.ToString());
            }

            string response = WEBSERVICE.SendGetRequest(url);
            //object obj = new JavaScriptSerializer().Deserialize<object>(response);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult ReleaseToPrintJob(ESCompanionViewModel esViewModel)
        {
            string url = CONFIG.GetDalimRestServiceUrl(esViewModel.CurrentEnvironmment);
            url += String.Format("{0}/{1}/{2}",
                esViewModel.Customer,
                "release",
                CONFIG.ApplicationName);

            string response = WEBSERVICE.SendGetRequest(url);
            //object obj = new JavaScriptSerializer().Deserialize<object>(response);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult DMSectionToPrintReleaseRevision(ESCompanionViewModel esViewModel)
        {
            string url = CONFIG.GetDalimRestServiceUrl(esViewModel.CurrentEnvironmment);

            url += String.Format("{0}/{1}",
                "revision",
                CONFIG.ApplicationName);

            ESRevision revision = new ESRevision(esViewModel);
            revision.command = REVISIONCOMMAND.RTPTOUCHCOMPLETED.ToString();
            revision.milestone = "RTP Touch Completed";
            revision.metadatas.Add(new string[] { "Valassis MetaData", "DirectMailAdStatus", "Completed Waiting For Print Release" });
            List<ESRevision> revisions = new List<ESRevision> { };
            revisions.Add(revision);

            string response = WEBSERVICE.SendPostRequest(url, revisions);
            return Json(response, JsonRequestBehavior.AllowGet);
        }


    }
}