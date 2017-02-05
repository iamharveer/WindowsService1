using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace WindowsService1
{
    public partial class Service1 : ServiceBase
    {
        private System.Diagnostics.EventLog eventLog1;

        public Service1(string[] args)
        { 
            InitializeComponent();
            eventLog1 = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists("MySource"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "MySource", "MyNewLog");
            }
            eventLog1.Source = "MySource";
            eventLog1.Log = "MyNewLog";
        }

        protected override void OnStart(string[] args)
        {
            eventLog1.WriteEntry("In OnStart ");
            // Set up a timer to trigger every minute.  
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 60000 * 60 * 6; // checks every 6 hours. 
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            timer.Start();

            if (args == null || args.Length != 1)
            {
                throw new Exception("args should only contain the abs path to key file.");
            }
            cmdArgs = args;
        }

        public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
        {
            // TODO: Insert monitoring activities here.
            eventLog1.WriteEntry("Monitoring the System"); //, "Information", EventLogEntryType.Information);

            var CheckUpdates = new Updates("http://www.ontarioimmigration.ca/en/pnp/OI_PNPNEW.html");
            if (CheckUpdates.PageIsDifferent)
            {
                CheckUpdates.SendMail(cmdArgs[0]);
            }
        }

        protected override void OnStop()
        {
            eventLog1.WriteEntry("In onStop.");
        }

        string[] cmdArgs = null;
    }
}
