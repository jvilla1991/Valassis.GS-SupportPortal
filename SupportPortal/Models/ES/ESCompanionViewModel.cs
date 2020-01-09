using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SupportPortal.Models
{
    public class ESCompanionViewModel
    {
        public EsEnvironment CurrentEnvironmment { get; set; }
        public FSIUpdateTarget FSIUpdateTarget { get; set; }
        public string Customer { get; set; }
        public string JobId { get; set; }
        public string PageOrder { get; set; }
        public bool Force { get; set; }
        public string InHomeWeek { get; set; }
    }
    public enum EsEnvironment
    {
        Production,
        Development
    }

    public enum FSIUpdateTarget
    {
        Section,
        Form,
        Ad
    }
}