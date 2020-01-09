using System;
using System.ServiceProcess;

namespace SupportPortal.Models
{
    public class ServiceViewModel
    {
        public class ServiceName
        {
            private ServiceName(string value) { Value = value; }

            public string Value { get; set; }

            public static string CFUManager { get { return ("Valassis CFU.CentralFileManager"); } }
            public static string MTWatcher { get { return ("Valassis CFU.MassTransitWatcher"); } }
            public static string Turnkey { get { return ("Valassis.Turnkey"); } }
            public static string MassTransit { get { return ("MassTransit"); } }
            public static string SWAutoPageBuild { get { return ("Valassis.SWAutoPageBuild"); } }
            public static string Tomcat9 { get { return ("Tomcat9"); } }
        }

        public ServiceViewModel(string serviceNameParam)
        {
            this.NameParam = serviceNameParam;
            Initialize(serviceNameParam);
        }
        private ServiceController sc;
        public string Name { get; set; }
        public string Description { get; set; }
        public string NameParam { get; set; }
        public string Server { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Status { get; set; }
        public bool Exception { get; internal set; }

        // Takes a service name param from a url and returns the name, description, and credentials of the service on the server
        internal void Initialize(string serviceNameParam)
        {
            switch (serviceNameParam)
            {
                case "turnkey":
                    Name = ServiceName.Turnkey;
                    Description = "(Obsolete) Service used to move Turnkey files to their Print Jobs";
                    Server = "valvcsgsw001vm";
                    break;
                case "swautopagebuild":
                    Name = ServiceName.SWAutoPageBuild;
                    Description = "(Obsolete) Service used to automate the life cycle of incoming Wrap Files from AX";
                    break;
                case "cfumanager":
                    Name = ServiceName.CFUManager;
                    Description = "Primary Service responsible for routing and auditing messages and tasks within the CFU MicroService Architecture";
                    Username = @"VAL\gswebfilecentral";
                    Password = @"GsWebFC2013!";
                    Server = "valvcsgsw001vm";
                    break;
                case "mtwatcher":
                    Name = ServiceName.MTWatcher;
                    Description = "Central File Upload Service responsible for reading incoming data from Mass Transit";
                    Username = @"VAL\gswebfilecentral";
                    Password = @"GsWebFC2013!";
                    Server = "valvcsgsw001vm";
                    break;
                case "mt":
                    Name = ServiceName.MassTransit;
                    Description = "MassTransit Managed File Transfer Service by Acronis, Inc.";
                    Username = @"VAL\masstransit";
                    Password = @"MassTran2006*!";
                    Server = "valvcsmt003ph";
                    break;
                case "java-prod":
                    Name = ServiceName.Tomcat9;
                    Description = "Apache Tomcat 9.0.0.M17 Server - http://tomcat.apache.org/";
                    Username = @"val\GSwebHHSAPI";
                    Password = @"GsWebHHS2018!";
                    Server = "valvcsgsw002vm";
                    break;
                case "java-dev":
                    Name = ServiceName.Tomcat9;
                    Description = "Apache Tomcat 9.0.0.M17 Server - http://tomcat.apache.org/";
                    Username = @"val\GSwebHHSAPI";
                    Password = @"GsWebHHS2018!";
                    Server = "vallomgsw003vm";
                    break;
            }
        }

    }
}