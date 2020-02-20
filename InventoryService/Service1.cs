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
        int Interval = 60 * 1000;

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
                        report.LastSent = report.LastSent == null ? DateTime.Now : report.LastSent;
                        TimeSpan timeDiff = DateTime.Now - Convert.ToDateTime(report.LastSent);

                        bool sendReport = false;
                        if ((timeDiff.Hours > 24) && report.DailyBasis)
                        {
                            sendReport = true;
                        }
                        if ((timeDiff.Days >= 7) && report.WeeklyBasis)
                        {
                            sendReport = true;
                        }
                        if ((timeDiff.Days >= 30) && report.MonthlyBasis)
                        {
                            sendReport = true;
                        }
                        if ((timeDiff.Days >= 356) && report.YearlyBasis)
                        {
                            sendReport = true;
                        }

                        string queryReport = null;
                        List<ReportQuery> queries = db.ReportQueries.Where(x => x.ReportID == report.Id).Select(x => x).ToList();
                        foreach(var query in queries)
                        {
                            var items = db.Items.Where(x => x.Name.Trim() == query.ItemName.Trim() && x.AvailabilityStatusID == query.AvailabilityStatusID).Select(x => x).ToList();
                            var quantity = items.Count();
                            var price = items.Sum(x => x.Price);

                            query.MinimumQuantity = query.MinimumQuantity == null ? 0 : query.MinimumQuantity;
                            query.MaximumQuantity = query.MaximumQuantity == null ? int.MaxValue : query.MaximumQuantity;
                            query.MinimumPrice = query.MinimumPrice == null ? 0 : query.MinimumPrice;
                            query.MaximumPrice = query.MaximumPrice == null ? int.MaxValue : query.MaximumPrice;
                            if (quantity != 0 
                                && (query.MinimumQuantity >= quantity || query.MaximumQuantity <= quantity)
                                && (query.MinimumPrice >= price || query.MaximumPrice <= price))
                            {
                                sendReport = true;
                                queryReport = "Urgent: " + query.ItemName + " has a total quantity of " + quantity + " and a total price of L.L." + price;
                            }
                        }

                        switch (report.ReportID)
                        {
                            case 1:
                                reportsController.InventoryGeneralReport_Email(report.ReceivedByUsers, queryReport);
                                break;
                            case 2:
                                reportsController.ItemsInReport_Email(report.ReceivedByUsers, queryReport);
                                break;
                            case 3:
                                reportsController.SearchForNonConsumableReport_Email(report.ReceivedByUsers, queryReport);
                                break;
                            case 4:
                                reportsController.DailyReport_Email(report.ReceivedByUsers, queryReport);
                                break;
                            case 5:
                                reportsController.ConsumableItemsReport_Email(report.ReceivedByUsers, queryReport);
                                break;
                            case 6:
                                reportsController.NonConsumableItemsReport_Email(report.ReceivedByUsers, queryReport);
                                break;
                            case 7:
                                reportsController.FullInventoryGeneralReport_Email(report.ReceivedByUsers, queryReport);
                                break;
                            case 8:
                                reportsController.BudgetLineStatementOfAccountReport_Email(report.ReceivedByUsers, queryReport);
                                break;
                            case 9:
                                reportsController.QuantityReport_Email(report.ReceivedByUsers, queryReport);
                                break;
                            case 10:
                                reportsController.SchoolTransferReport_Email(report.ReceivedByUsers, queryReport);
                                break;
                            default:
                                break;
                        }
                        report.LastSent = DateTime.Now;
                        db.SaveChanges();
                    }
                }
            }
            catch(Exception ex)
            {
                WriteLog("{0} Exception: " + ex.ToString());
            }
            //WriteLog("{0} ms elapsed.");
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
