using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SupportPortal.Infrastructure
{
    public class Common
    {
        public bool IsEmptyOrNull(string s)
        {
            return (s == null || s == String.Empty) ? true : false;
        }
    }
}