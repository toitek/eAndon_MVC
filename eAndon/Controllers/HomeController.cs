using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eAndon.Models;

namespace eAndon.Controllers
{

    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Overview()
        {
            var _eAndonDB = new eAndonDB_Entities();
            var currentOverview = _eAndonDB.WorkcenterList;
            var currentOverviewModel = new List<AndonTerminalModel>();;
            
            foreach (var workcenter in currentOverview)
            {
                var statusValues = _eAndonDB.WorkcenterList.Where(w => w.WorkcenterID == workcenter.WorkcenterID)
                    .Select(w => new { w.Status1, w.Status2, w.Status3, w.Status4, w.Status5 })
                    .FirstOrDefault();

                // Create an instance of the custom model and populate it with the workcenter information and the status values
                var workcenterModel = new AndonTerminalModel
                {
                    WorkcenterID = workcenter.WorkcenterID,
                    WorkcenterName = workcenter.WorkcenterName,
                    StatusValues = new List<string>
                    {
                        statusValues.Status1, statusValues.Status2, statusValues.Status3, statusValues.Status4,
                        statusValues.Status5
                    }
                };
                currentOverviewModel.Add(workcenterModel);

            }

            return View(currentOverviewModel);
        }

        public ActionResult AndonTerminal(string workcenterID, string workcenterName)
        {
            if (workcenterID == null || workcenterName == null) return View();
            
            var _eAndonDB = new eAndonDB_Entities();

            // Retrieve the status values for the workcenter from the database
            var statusValues = _eAndonDB.WorkcenterList.Where(w => w.WorkcenterID == workcenterID)
                .Select(w => new { w.Status1, w.Status2, w.Status3, w.Status4, w.Status5 })
                .FirstOrDefault();

            // Create an instance of the custom model and populate it with the workcenter information and the status values
            var model = new AndonTerminalModel
            {
                WorkcenterID = workcenterID,
                WorkcenterName = workcenterName,
                StatusValues = new List<string>
                {
                    statusValues.Status1, statusValues.Status2, statusValues.Status3, statusValues.Status4,
                    statusValues.Status5
                }
            };

            return View(model);
        }

        public ActionResult UpdateStatus(string workcenterID, int statusIndex)
        {
            var _eAndonDB = new eAndonDB_Entities();
            
            var workcenter = _eAndonDB.WorkcenterList.FirstOrDefault(w => w.WorkcenterID == workcenterID);

            var statusProperty = typeof(WorkcenterList).GetProperties().ElementAt(statusIndex + 3); // +3 because Status1 to Status5 are properties 3 to 7
            var currentStatus = (string)statusProperty.GetValue(workcenter);
            var newStatus = currentStatus == "green" ? "red" : "green";
            statusProperty.SetValue(workcenter, newStatus);

            _eAndonDB.SaveChanges();

            return RedirectToAction("AndonTerminal", new { workcenterID, workcenterName = workcenter.WorkcenterName });
        }

    }
}