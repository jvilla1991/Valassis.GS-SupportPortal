using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

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
    }
}