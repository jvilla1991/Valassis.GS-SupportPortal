using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.ServiceProcess;
using System.Web;

namespace SupportPortal.Models
{
    public class Service
    {
        private ServiceController sc;
        private string name { get; set; }
        public String server { get; set; }
        private string status { get; set; }

        // initializes an object that uses the service name and server to connect to a server
        internal String Initialize()
        {
            try
            {
                sc = new ServiceController(name, server);
            } catch (Exception e)
            {
                return e.ToString();
            }
            return "good";
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
                    return "Could not start, unidentified error";
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
                    return "Service already stopped";
                }
                else
                {
                    return "Could not stop, unidentified error";
                }
            }
            return "Service stopped successfully";
        }

        // Gets status of targeted service
        internal dynamic GetStatus()
        {
            return "Status: " + sc.Status;
        }

        // Takes a service name param from a url and returns the name of the service on the server
        internal void SetName(string serviceNameParam)
        {
            if (serviceNameParam.Equals("turnkey"))
            {
                name = "Valassis.Turnkey";
            }
            else if (serviceNameParam.Equals("swautopagebuild"))
            {
                name = "Valassis.SWAutoPageBuild";
            }
        }

        internal dynamic GetName()
        {
            return name;
        }
    }
}