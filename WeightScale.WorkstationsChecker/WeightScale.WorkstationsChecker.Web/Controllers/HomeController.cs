using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using WeightScale.WorkstationsChecker.Data;
using WeightScale.WorkstationsChecker.Model;
using WeightScale.WorkstationsChecker.Web.Models;

namespace WeightScale.WorkstationsChecker.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext context;

        public HomeController(ApplicationDbContext contextParam)
        {
            this.context = contextParam;
        }

        public HomeController()
        {
            this.context = new ApplicationDbContext();
        }

        public ActionResult Index()
        {
            var scales = context.WeightScales.OrderBy(p=>p.ScreenPosition).ToList();
            var scalesView = Mapper.Map<IEnumerable<WeightScaleWorkStationViewModel>>(scales);
            return View((object)scalesView);
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

        [HttpPost]
        public ActionResult GetStatistics(DateTime? startPeriod, DateTime? endPeriod, int id = 3)
        {
            startPeriod = startPeriod ?? DateTime.Now.AddHours(-1);
            endPeriod = endPeriod ?? DateTime.Now;
            var result = context.Pings.Where(p => p.WeightScaleWorkStationId == id && (p.TimeStamp > startPeriod && endPeriod > p.TimeStamp));
            var model = Mapper.Map<IEnumerable<PingStatisticsViewModel>>(result.ToList());
            return Json(model);
        }
    }
}