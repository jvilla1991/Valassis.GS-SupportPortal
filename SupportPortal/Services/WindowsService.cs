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
            public static string SWAutoPageBuild { get { return ("Valassis.SWAutoPageBuild"); } }
        }

        public ServiceViewModel(string serviceNameParam)
        {
            this.NameParam = serviceNameParam;
            SetName(serviceNameParam);
            Server = "valvcsgsw001vm";
        }
        private ServiceController sc;
        public string Name { get; set; }
        public string NameParam { get; set; }
        public string Server { get; set; }
        public string Status { get; set; }
        public bool Exception { get; internal set; }

        // initializes an object that uses the service name and server to connect to a server
        internal void Initialize()
        {
            sc = new ServiceController(Name, Server);
        }

        // Starts a service
        public string Start()
        {
            try
            {
                sc.Start();
            }
            catch (InvalidOperationException ex)
            {
                if (ex.InnerException.ToString().Contains("already running"))
                {
                    return "Service already running";
                }
                else
                {
                    return "Could not start: " + ex.Message;
                }
            }
            return "Service started successfully";
        }

        // Stops a service
        public string Stop()
        {
            try
            {
                sc.Stop();
            }
            catch (InvalidOperationException ex)
            {
                if (ex.InnerException.ToString().Contains("already stopped"))
                {
                    return "Service not currently running";
                }
                else
                {
                    return "Could not stop: " + ex.Message;
                }
            }
            return "Service stopped successfully";
        }

        // Gets status of targeted service
        internal void GetStatus()
        {
            sc.Refresh();
            this.Status = sc.Status.ToString();
        }

        // Takes a service name param from a url and returns the name of the service on the server
        internal void SetName(string serviceNameParam)
        {
            switch (serviceNameParam)
            {
                case "turnkey":
                    Name = ServiceName.Turnkey;
                    break;
                case "swautopagebuild":
                    Name = ServiceName.SWAutoPageBuild;
                    break;
                case "cfumanager":
                    Name = ServiceName.CFUManager;
                    break;
                case "mtwatcher":
                    Name = ServiceName.MTWatcher;
                    break;
            }
        }

        internal dynamic GetName()
        {
            return Name;
        }
    }
}