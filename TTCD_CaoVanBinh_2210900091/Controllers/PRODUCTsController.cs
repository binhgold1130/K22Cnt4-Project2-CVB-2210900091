using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TTCD_CaoVanBinh_2210900091.Models;

namespace TTCD_CaoVanBinh_2210900091.Controllers
{
    public class PRODUCTsController : Controller
    {
        private TTCD_Cvb_2210900091Entities db = new TTCD_Cvb_2210900091Entities();

        // GET: PRODUCTs
        public ActionResult ProIndex()
        {
            return View(db.PRODUCTs.ToList());
        }

        // GET: PRODUCTs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PRODUCT pRODUCT = db.PRODUCTs.Find(id);
            if (pRODUCT == null)
            {
                return HttpNotFound();
            }
            return View(pRODUCT);
        }

        // GET: PRODUCTs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PRODUCTs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "product_id,product_name,product_price,product_size,product_color,product_quantity,description,product_image")] PRODUCT pRODUCT)
        {
            if (ModelState.IsValid)
            {
                db.PRODUCTs.Add(pRODUCT);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(pRODUCT);
        }

        // GET: PRODUCTs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PRODUCT pRODUCT = db.PRODUCTs.Find(id);
            if (pRODUCT == null)
            {
                return HttpNotFound();
            }
            return View(pRODUCT);
        }

        // POST: PRODUCTs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "product_id,product_name,product_price,product_size,product_color,product_quantity,description,product_image")] PRODUCT pRODUCT)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pRODUCT).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(pRODUCT);
        }

        // GET: PRODUCTs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PRODUCT pRODUCT = db.PRODUCTs.Find(id);
            if (pRODUCT == null)
            {
                return HttpNotFound();
            }
            return View(pRODUCT);
        }

        // POST: PRODUCTs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PRODUCT pRODUCT = db.PRODUCTs.Find(id);
            db.PRODUCTs.Remove(pRODUCT);
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
    }
}
