using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using WeightScale.WorkstationsChecker.Data;
using WeightScale.WorkstationsChecker.Model;
using WeightScale.WorkstationsChecker.Web.Models;

namespace WeightScale.WorkstationsChecker.Web.Areas.PowerUser.Controllers
{
    public class ObserverController : PowerUserController
    {
        private readonly IUowData context;
        public ObserverController(IUowData contextParam)
        {
            this.context = contextParam;
        }

        // GET: PowerUser/Observer
        public ActionResult Index()
        {
            var observedWorkstations = this.context.WeightScales.All();
            return View(Mapper.Map<IEnumerable<WorkstationViewModel>>(observedWorkstations));
        }

        // GET: PowerUser/Observer/Edit
        public ActionResult Edit(int id)
        {
            var workstation = context.WeightScales.All().FirstOrDefault(x => x.Id == id);
            ViewBag.Title = string.Format("Edit workstation {0}", workstation.Name);

            return View(Mapper.Map<WorkstationViewModel>(workstation));
        }

        // GET: PowerUser/Observer/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(WorkstationViewModel workstation)
        {
            if (ModelState.IsValid)
            {
                var dbWorkStation = context.WeightScales.GetById(workstation.Id);
                Mapper.Map(workstation, dbWorkStation);
                context.WeightScales.Update(dbWorkStation);
                context.SaveChanges();
            }
            else
            {
                return View(workstation);
            }

            return RedirectToAction("Index");
        }

        // GET: PowerUser/Observer/Edit
        public ActionResult Create()
        {
            ViewBag.Title = string.Format("Create new workstation observer");
            return View("Edit");
        }

        // GET: PowerUser/Observer/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(WorkstationViewModel workstation)
        {
            if (ModelState.IsValid)
            {
                var dbWorkStation = Mapper.Map<WeightScaleWorkStation>(workstation);
                context.WeightScales.Add(dbWorkStation);
                context.SaveChanges();
            }
            else
            {
                ViewBag.Title = string.Format("Create new workstation observer");
                return View("Edit", (object)workstation);
            }

            return RedirectToAction("Index");
        }

        // GET: PowerUser/Observer/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            context.WeightScales.Delete(id);
            context.SaveChanges();

            return RedirectToAction("Index");
        }

    }
}