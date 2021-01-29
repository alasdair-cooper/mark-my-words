using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Group16SE.Frontend.Shared;

namespace Group16SE.Frontend.Server.Controllers
{
    public class AssignmentController : Controller
    {
        // GET: AssignmentController
        public ActionResult Index()
        {
            return View();
        }

        // GET: AssignmentController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AssignmentController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AssignmentController/Create
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

        // GET: AssignmentController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AssignmentController/Edit/5
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

        // GET: AssignmentController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AssignmentController/Delete/5
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
