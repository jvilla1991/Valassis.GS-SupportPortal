using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using SupportPortal.Infrastructure;
using SupportPortal.Models;

namespace SupportPortal.Services
{
    public class WebService
    {

        public async Task<int> KickSpider(string jobid)
        {
            var url = String.Format("http://vallomeapp5d1vm:8080/EspritEngine/IPSImport.html?type=ips&action=launch&misName=SPIDER&misHierarchy=0,{0},es-mistral", jobid);
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            var content = String.Empty;

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    using (var sr = new StreamReader(stream))
                    {
                        content = sr.ReadToEnd();
                    }
                }
            }
            return await Task.FromResult(1);
        }

        public dynamic SendGetRequest(string url)
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

        public dynamic SendPostRequest(string url, object obj)
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


    }
}