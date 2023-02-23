using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using eAndon_MVC.Models;



namespace eAndon_MVC.Controllers
{

    public class HomeController : Controller
    {

        private readonly MyDbContext _db;

        public HomeController(MyDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var workcenters = _db.WorkcenterList.ToList();
            return View(workcenters);
        }

        public IActionResult Overview()
        {
            var currentOverview = _db.WorkcenterList.ToList();
            var currentOverviewModel = new List<AndonTerminalModel>();

            foreach (var workcenter in currentOverview)
            {
                var statusDefinitions = _db.StatusDefinition.ToList();
                var statusValues = _db.WorkcenterList
                    .Where(w => w.WorkcenterID == workcenter.WorkcenterID)
                    .Select(w => new List<string> { w.Status1, w.Status2, w.Status3, w.Status4, w.Status5 })
                    .FirstOrDefault();

                // Create an instance of the custom model and populate it with the workcenter information and the status values
                var workcenterModel = new AndonTerminalModel
                {
                    WorkcenterID = workcenter.WorkcenterID,
                    WorkcenterName = workcenter.WorkcenterName,
                    StatusDefinitions = statusDefinitions,
                    StatusValues = statusValues
                };
                currentOverviewModel.Add(workcenterModel);
            }

            return View(currentOverviewModel);
        }


        public IActionResult AndonTerminal(string workcenterID, string workcenterName)
        {

            var statusDefinitions = _db.StatusDefinition.ToList();
            // Retrieve the status values for the workcenter from the database
            var statusValues = _db.WorkcenterList
                    .Where(w => w.WorkcenterID == workcenterID)
                    .Select(w => new List<string> { w.Status1, w.Status2, w.Status3, w.Status4, w.Status5 })
                    .FirstOrDefault();

                // Create an instance of the custom model and populate it with the workcenter information and the status values
                var model = new AndonTerminalModel
                {
                    WorkcenterID = workcenterID,
                    WorkcenterName = workcenterName,
                    StatusDefinitions = statusDefinitions,
                    StatusValues = statusValues
                };

                return View(model);

        }

        public IActionResult UpdateStatus(string workcenterID, int statusIndex)
        {
            var workcenter = _db.WorkcenterList.FirstOrDefault(w => w.WorkcenterID == workcenterID);

            var statusProperty = typeof(Workcenter).GetProperties().ElementAt(statusIndex + 3); // +3 because Status1 to Status5 are properties 3 to 7
            var currentStatus = (string)statusProperty.GetValue(workcenter);
            var newStatus = currentStatus == "green" ? "red" : "green";
            statusProperty.SetValue(workcenter, newStatus);

            var logEntry = new WorkcenterStatusLog
            {
                WorkcenterID = workcenterID,
                StatusIndex = statusIndex,
                OldStatus = currentStatus,
                NewStatus = newStatus,
                ChangeDateTime = DateTime.Now
            };

            _db.WorkcenterStatusLog.Add(logEntry);
            _db.SaveChanges();

            return RedirectToAction("AndonTerminal", new { workcenterID, workcenterName = workcenter.WorkcenterName });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}