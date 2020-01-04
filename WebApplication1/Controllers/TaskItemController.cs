using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TotallyNotJira.Models;

namespace TotallyNotJira.Controllers
{
    public class Task1Controller : Controller
    {
        private TaskDBContext db = new TaskDBContext();
        // GET: Task
        public ActionResult Index()
        {
            ViewBag.TaskItems = db.TaskItems.OrderBy(x => x.Title);
            return View();
        }

        public ActionResult New()
        {
            return View();
        }

        // POST: Project/Create
        [HttpPost]
        public ActionResult New(TaskItem task)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.TaskItems.Add(task);
                    db.SaveChanges();
                    TempData["message"] = "Task added!";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(task);
                }
            }
            catch (Exception e)
            {
                return View(task);
            }
        }

        // GET: Project/Edit/5
        public ActionResult Edit(int id)
        {
            ViewBag.Project = db.TaskItems.Find(id);

            return View();
        }

        // POST: Project/Edit/5
        [HttpPut]
        public ActionResult Edit(int id, TaskItem requiredTask)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    TaskItem task = db.TaskItems.Find(id);

                    if (TryUpdateModel(task))
                    {
                        task.Title = requiredTask.Title;
                        task.Description = requiredTask.Description;
                        task.StartDate = requiredTask.StartDate;
                        task.EndDate = requiredTask.EndDate;
                        task.Status = requiredTask.Status;
                        db.SaveChanges();
                    }

                    return RedirectToAction("Index");
                }
                else
                {
                    return View(requiredTask);
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: Project/Delete/5
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            TaskItem task = db.TaskItems.Find(id);
            db.TaskItems.Remove(task);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

    }

}