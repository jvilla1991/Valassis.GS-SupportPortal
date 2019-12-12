using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SupportPortal.Models.ES
{
    public class ESRevision
    {
        public ESRevision(ESCompanionViewModel esViewModel)
        {
            this.pageOrder = esViewModel.PageOrder;
            this.jobId = esViewModel.JobId;
            this.customer = esViewModel.Customer;
            this.force = esViewModel.Force;
        }

        public string pageOrder { get; set; }
        public string jobId { get; set; }
        public string customer { get; set; }
        public string command { get; set; }
        public string milestone { get; set; }
        public bool force { get; set; }
        public List<String[]> metadatas = new List<string[]> { };
    }

    public enum REVISIONCOMMAND
    {
        RTPTOUCHCOMPLETED,
        NEWTOUCH,
        DELETETOUCH,
        EDITPAGEORDER,
        MOVEPAGEORDER
    }

}