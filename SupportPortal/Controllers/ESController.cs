using SupportPortal.Infrastructure;
using SupportPortal.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SupportPortal.Controllers
{
    public class ESController : Controller
    {
        private static readonly ConfigSettings CONFIG = new ConfigSettings(); 
        public ActionResult EsCompanion()
        {
            return View();
        }
        // GET: ES
        [HttpPost]
        public ActionResult CallDalimRestService(ESCompanionViewModel esViewModel)
        {
            return View("EsCompanion");
        }

        [HttpPost]
        public ActionResult UpdateSectionMetadata(ESCompanionViewModel esViewModel)
        {
            if(!IsEmptyOrNull(esViewModel.PageOrder))
            {
                if (esViewModel.PageOrder.Contains("-"))
                {
                    string[] Urlparams = esViewModel.PageOrder.Split('-');
                    esViewModel.UAVC = Urlparams[0];
                    esViewModel.IHD = Urlparams[1];
                }
            }
            
            string urlParams = String.Format("{0}/update/section/{1}/{2}/{3}", 
                esViewModel.Customer, 
                esViewModel.JobId, 
                esViewModel.UAVC + "-" + esViewModel.IHD, 
                CONFIG.ApplicationName);
            try
            {
                string url = CONFIG.GetDalimRestServiceUrl(esViewModel.CurrentEnvironmment) + urlParams;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    ViewBag.Response = reader.ReadToEnd();
                }
            } catch (Exception e)
            {
                ViewBag.Response = e.Message;
            }

            return View("EsCompanion");
        }

        [HttpPost]
        public ActionResult DMSectionToPrintRelease(ESCompanionViewModel esViewModel)
        {
            return View("EsCompanion");
        }

        public static bool IsEmptyOrNull(string s)
        {
            return (s == null || s == String.Empty) ? true : false;
        }
    }
}