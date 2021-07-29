using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CarInsurance.Models;

namespace CarInsurance.Controllers
{
    public class InsureeController : Controller
    {
        private InsuranceEntities db = new InsuranceEntities();

        // GET: Insuree
        public ActionResult Index()
        {
            return View(db.Insurees.ToList());
        }

        // GET: Insuree/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // GET: Insuree/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Insuree/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,CoverageType,Quote")] Insuree insuree)
        {
            if (ModelState.IsValid)
            {
                insuree.Quote = QuoteLogic(insuree);
                db.Insurees.Add(insuree);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(insuree);
        }

        // GET: Insuree/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // POST: Insuree/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,CoverageType,Quote")] Insuree insuree)
        {
            if (ModelState.IsValid)
            {
                insuree.Quote = QuoteLogic(insuree);
                db.Entry(insuree).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(insuree);
        }

        // GET: Insuree/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Insuree insuree = db.Insurees.Find(id);
            if (insuree == null)
            {
                return HttpNotFound();
            }
            return View(insuree);
        }

        // POST: Insuree/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Insuree insuree = db.Insurees.Find(id);
            db.Insurees.Remove(insuree);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult Admin()
        {
            List<Insuree> listInsuree = new List<Insuree>();
            foreach (var insuree in db.Insurees.ToList())
            {
                Insuree theInsured = new Insuree();
                theInsured.FirstName = insuree.FirstName;
                theInsured.LastName = insuree.LastName;
                theInsured.EmailAddress = insuree.EmailAddress;
                theInsured.Quote = insuree.Quote;

                listInsuree.Add(theInsured);
            }

            return View(listInsuree);
        }

        private decimal QuoteLogic(Insuree insuree)
        {
            decimal quote = 50;
            int age = new DateTime(DateTime.Now.Subtract(Convert.ToDateTime(insuree.DateOfBirth)).Ticks).Year - 1;

            if (age < 19)
            {
                quote += 100;
            }
            else if (age >= 19 && age <= 25)
            {
                quote += 50;
            }
            else if (age > 25)
            {
                quote += 25;
            }

            if (insuree.CarYear < 2000 || insuree.CarYear > 2015)
            {
                quote += 25;
            }

            if (insuree.CarMake == "Porsche")
            {
                quote += 25;
            }

            if (insuree.CarMake == "Porsche" && insuree.CarModel == "911 Carrera")
            {
                quote += 25;
            }

            quote += Convert.ToInt32(insuree.SpeedingTickets) * 10;

            if (Convert.ToBoolean(insuree.DUI))
            {
                quote += quote * .25m;
            }

            if (Convert.ToBoolean(insuree.CoverageType))
            {
                quote += quote * .5m;
            }

            return quote;
        }
    }
}
