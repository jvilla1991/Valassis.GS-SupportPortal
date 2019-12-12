using SupportPortal.Infrastructure;
using SupportPortal.Models;
using SupportPortal.Models.ES;
using System;
using System.IO;
using System.Net;
using System.Web.Mvc;

namespace SupportPortal.Controllers
{
    public class ESController : Controller
    {
        private static readonly ConfigSettings CONFIG = new ConfigSettings();
        private static readonly Common COMMON = new Common();
        public ActionResult EsCompanion()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UpdateJobMetadata(ESCompanionViewModel esViewModel)
        {
            string url = CONFIG.GetDalimRestServiceUrl(esViewModel.CurrentEnvironmment);

            url += String.Format("{0}/upsert/job/{1}/{2}",
                esViewModel.Customer,
                esViewModel.JobId,
                CONFIG.ApplicationName);
            
            ViewBag.Response = SendGetRequest(url);
            return View("EsCompanion");
        }

        [HttpPost]
        public ActionResult UpdateSectionMetadata(ESCompanionViewModel esViewModel)
        {
            string url = CONFIG.GetDalimRestServiceUrl(esViewModel.CurrentEnvironmment);

            if (!COMMON.IsEmptyOrNull(esViewModel.PageOrder))
            {
                if (esViewModel.PageOrder.Contains("-"))
                {
                    string[] Urlparams = esViewModel.PageOrder.Split('-');
                    esViewModel.UAVC = Urlparams[0];
                    esViewModel.IHD = Urlparams[1];
                }
            }
            url += String.Format("{0}/update/section/{1}/{2}/{3}", 
                esViewModel.Customer, 
                esViewModel.JobId, 
                esViewModel.UAVC + "-" + esViewModel.IHD, 
                CONFIG.ApplicationName);

            ViewBag.Response = SendGetRequest(url);
            return View("EsCompanion");
        }

        [HttpPost]
        public ActionResult UpdateBulkSectionMetadata(ESCompanionViewModel esViewModel)
        {
            string url = CONFIG.GetDalimRestServiceUrl(esViewModel.CurrentEnvironmment);

            url += String.Format("{0}/update/section/{1}/{2}",
                esViewModel.Customer,
                esViewModel.JobId,
                CONFIG.ApplicationName);

            ViewBag.Response = SendGetRequest(url);
            return View("EsCompanion");
        }

        [HttpPost]
        public ActionResult DMSectionToPrintReleaseRevision(ESCompanionViewModel esViewModel)
        {
            if (COMMON.IsEmptyOrNull(esViewModel.PageOrder))
            {
                esViewModel.PageOrder = esViewModel.UAVC + "-" + esViewModel.IHD;
            }

            ESRevision revision = new ESRevision(esViewModel);
            revision.command = REVISIONCOMMAND.RTPTOUCHCOMPLETED.ToString();
            revision.milestone = "RTP Touch Completed";
            revision.metadatas.Add(new string[] { "Valassis MetaData", "DirectMailAdStatus", "Completed Waiting For Print Release" });

            string url = CONFIG.GetDalimRestServiceUrl(esViewModel.CurrentEnvironmment);

            url += String.Format("{0}/{1}",
                "revision",
                CONFIG.ApplicationName);

            ViewBag.Response = SendPostRequest(url, revision);
            return View("EsCompanion");
        }

        private dynamic SendPostRequest(string url, object obj)
        {
            try
            {
                string data = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                request.Method = "POST";
                request.ContentType = "application/json";

                using (StreamWriter stream = new StreamWriter(request.GetRequestStream()))
                {
                    stream.Write(data);
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        private dynamic SendGetRequest(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}