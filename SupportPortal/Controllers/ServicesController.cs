using Microsoft.Win32.TaskScheduler;
using SimpleImpersonation;
using SupportPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SupportPortal.Controllers
{
    public class ServicesController : Controller
    {
        #region Services
        // The "id" field is looking for an int to be returned from the view. either "1" for start or "0" for stop
        [Route("Services/{serviceNameParam}-service/{id?}")]
        public ActionResult Services(string serviceNameParam, int? id)
        {
            ServiceViewModel service = new ServiceViewModel(serviceNameParam);

            var credentials = new UserCredentials(service.Username, service.Password);
            Impersonation.RunAsUser(credentials, SimpleImpersonation.LogonType.Interactive, () =>
            {
                ServiceControllerPermission scp = new ServiceControllerPermission(ServiceControllerPermissionAccess.Control, service.Server, service.Name);
                scp.Assert();

                ServiceController sc = new ServiceController(service.Name, service.Server);
                TimeSpan timeout = new TimeSpan(0, 0, 30);
                switch (id)
                {
                    case 0:
                        sc.Stop();
                        sc.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                        break;
                    case 1:
                        sc.Start();
                        sc.WaitForStatus(ServiceControllerStatus.Running, timeout);
                        break;
                    default:
                        service.Status = sc.Status.ToString();
                        break;
                }
            });
            return View(service);
        }
        #endregion

        #region Scheduled Tasks
        [Route("Services/task-{task}/{id?}")]
        public ActionResult ScheduledTask(string task, int? id)
        {
            //TaskScheduleViewModel taskViewModel = new TaskScheduleViewModel();
            try
            {
                using (TaskService ts = new TaskService(@"\\valvcsgsw001vm"))
                {
                    Task[] tasks = ts.FindAllTasks(new Regex("\\.\\*"), true);
                    Task tsk = ts.GetTask(@"\Windows\System32\Tasks\Kmart Prose");
                    Version ver = TaskService.Instance.HighestSupportedVersion;
                    foreach (RunningTask rt in TaskService.Instance.GetRunningTasks(true))
                    {
                        if (rt != null)
                        {
                            Console.WriteLine("+ {0}, {1} ({2})", rt.Name, rt.Path, rt.State);
                            if (ver.Minor > 0)
                                Console.WriteLine("  Current Action: " + rt.CurrentAction);
                        }
                    }

                    TaskFolder tf = TaskService.Instance.RootFolder;
                    Console.WriteLine("\nRoot folder tasks ({0}):", tf.Tasks.Count);
                    foreach (Task t in tf.Tasks)
                    {
                        try
                        {
                            Console.WriteLine("+ {0}, {1} ({2})", t.Name,
                               t.Definition.RegistrationInfo.Author, t.State);
                            foreach (Trigger trg in t.Definition.Triggers)
                                Console.WriteLine(" + {0}", trg);
                            foreach (var act in t.Definition.Actions)
                                Console.WriteLine(" = {0}", act);
                        }
                        catch { }
                    }

                    Console.WriteLine("\n******Checking folder enum******");
                    TaskFolderCollection tfs = tf.SubFolders;
                    if (tfs.Count > 0)
                    {
                        Console.WriteLine("\nSub folders:");
                        try
                        {
                            foreach (TaskFolder sf in tfs)
                                Console.WriteLine("+ {0}", sf.Path);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return null;
        }

        #endregion
    }
}