using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using SupportPortal.Models;

namespace SupportPortal.Infrastructure
{
    public class ConfigSettings
    {
        internal string GetDalimRestServiceUrl(EsEnvironment currentEnvironmment)
        {
            return currentEnvironmment.Equals(EsEnvironment.Production) ? DalimRestServiceUrl : DalimRestServiceUrlDev;
        }
        public string ApplicationName
        {
            get { return GetValue("ApplicationNameUrlParam"); }
            
        }
        public string DalimRestServiceUrl
        {
            get { return GetValue("DalimRestService"); }
        }

        public string DalimRestServiceUrlDev
        {
            get { return GetValue("DalimRestServiceDev");  }
        }

        private static string GetValue(string key)
        {
            string result = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrEmpty(result))
            {
                throw new KeyNotFoundException(key);
            }
            return result;
        }
    }
}