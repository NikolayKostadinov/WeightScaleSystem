using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Newtonsoft.Json;
using WeightScale.WorkstationsChecker.Data;
using WeightScale.WorkstationsChecker.Model;
using WeightScale.WorkstationsChecker.Web.Models;

namespace WeightScale.WorkstationsChecker.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IUowData context;

        public HomeController(IUowData contextParam)
        {
            this.context = contextParam;
        }

        public ActionResult Index()
        {
            var scales = context.WeightScales.All().OrderBy(p => p.ScreenPosition).Where(x => x.IsStopped == false).ToList();
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
        public async Task<ActionResult> GetStatistics(DateTime? startPeriod, DateTime? endPeriod, int id = 3)
        {
            startPeriod = startPeriod ?? DateTime.Now.AddHours(-1);
            endPeriod = endPeriod ?? DateTime.Now;
            IEnumerable<PingStatisticsViewModel> model;
            if (endPeriod - startPeriod <= TimeSpan.FromMinutes(10))
            {
                var result = await context.Pings.All().Where(p => p.WeightScaleWorkStationId == id && (p.TimeStamp >= startPeriod && endPeriod >= p.TimeStamp)).ToListAsync();
                model = Mapper.Map<IEnumerable<PingStatisticsViewModel>>(result);
            }
            else
            {
                var modelList = await context.Pings.All()
                     .Where(p => p.WeightScaleWorkStationId == id && (p.TimeStamp >= startPeriod && endPeriod >= p.TimeStamp)).ToListAsync();
                model = modelList 
                     .GroupBy(sel => new { sel.TimeStamp.Date, sel.TimeStamp.Hour, sel.TimeStamp.Minute })
                     .Select(gr => new PingStatisticsViewModel() 
                     {
                         TimeStamp = new DateTime(
                            gr.Key.Date.Year,
                            gr.Key.Date.Month,
                            gr.Key.Date.Day,
                            gr.Key.Hour,
                            gr.Key.Minute,
                            0
                            ),
                         RoundtripTime = gr.Max(grp => grp.PingReply.RoundtripTime)
                     });
            }
            var content = await JsonConvert.SerializeObjectAsync(model);
            return Content(content,"application/json");
        }
    }
}