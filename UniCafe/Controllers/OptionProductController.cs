﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UniCafe.Controllers
{
    public class OptionProductController : Controller
    {
        // GET: OptionProduct
        public ActionResult Index()
        {
            return View();
        }

        // GET: OptionProduct/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: OptionProduct/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: OptionProduct/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: OptionProduct/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: OptionProduct/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: OptionProduct/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: OptionProduct/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
