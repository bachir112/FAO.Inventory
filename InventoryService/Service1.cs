using Inventory.DataObjects.EDM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace InventoryService
{
    public partial class Service1 : ServiceBase
    {
        Timer Timer = new Timer();
        int Interval = 10000; // 10000 ms = 10 second  

        public Service1()
        {
            InitializeComponent();
            this.ServiceName = "WindowsService.NET";
        }

        protected override void OnStart(string[] args)
        {
            WriteLog("Service has been started");
            Timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            Timer.Interval = Interval;
            Timer.Enabled = true;
        }


        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            try
            {
                using (InventoryEntities db = new InventoryEntities())
                {
                    List<ReportSetting> reportSettings = db.ReportSettings.Select(x => x).ToList();
                    Inventory.WebApplication.Controllers.ReportsController reportsController = new Inventory.WebApplication.Controllers.ReportsController();
                    foreach (var report in reportSettings)
                    {
                        switch (report.ReportID)
                        {
                            case 1:
                                reportsController.DailyReport_Email();
                                break;
                            default:
                                break;
                        }
                    }
                    WriteLog("{0} connected to DB");
                    WriteLog("{0} : Found " + reportSettings.Count() + " reports");
                }
            }
            catch(Exception ex)
            {
                WriteLog("{0} Exception.");
            }
            WriteLog("{0} ms elapsed.");
        }

        protected override void OnStop()
        {
            Timer.Stop();
            WriteLog("Service has been stopped.");
        }

        private void WriteLog(string logMessage, bool addTimeStamp = true)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var filePath = String.Format("{0}\\{1}_{2}.txt",
                path,
                ServiceName,
                DateTime.Now.ToString("yyyyMMdd", CultureInfo.CurrentCulture)
                );

            if (addTimeStamp)
                logMessage = String.Format("[{0}] - {1}",
                    DateTime.Now.ToString("HH:mm:ss", CultureInfo.CurrentCulture),
                    logMessage);

            File.AppendAllText(filePath, logMessage);
        }
    }
}
