using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Group16SE.Frontend.Shared;

namespace Group16SE.Frontend.Server.Controllers
{
    public class AttemptController : Controller
    {
        // GET: AttemptController
        public ActionResult Index()
        {
            return View();
        }

        // GET: AttemptController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AttemptController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AttemptController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AttemptController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AttemptController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AttemptController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AttemptController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
