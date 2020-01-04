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
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Task
        public ActionResult Index()
        {
            ViewBag.Tasks = db.Tasks.OrderBy(x => x.TaskTitle);
            return View();
        }

        public ActionResult Show(int id)
        {
            Task1 task = db.Tasks.Find(id);
            return View(task);

        }
        [Authorize(Roles = "Organizator,Administrator")]
        public ActionResult New()
        {
            Task1 task = new Task1();
            var selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem
            {
                Value = "1",
                Text = "Not started"
            });
            selectList.Add(new SelectListItem
            {
                Value = "2",
                Text = "In progress"
            });
            selectList.Add(new SelectListItem
            {
                Value = "3",
                Text = "Done"
            });
            ViewBag.Statuses = selectList;

            task.Projects = GetAllProjects();

            return View(task);
           
        }
        [Authorize(Roles = "Organizator,Administrator")]
        [HttpPost]
        public ActionResult New(Task1 task)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Tasks.Add(task);
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

       public ActionResult Edit(int id)
        {
            Task1 task = db.Tasks.Find(id);
            ViewBag.Task = task;

            //status dropdown
            var selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem
            {
                Value = "1",
                Text = "Not started"
            });
            selectList.Add(new SelectListItem
            {
                Value = "2",
                Text = "In progress"
            });
            selectList.Add(new SelectListItem
            {
                Value = "3",
                Text = "Done"
            });
            ViewBag.Statuses = selectList;

            task.Projects = GetAllProjects();

            return View(task);
        }

        [HttpPut]
        public ActionResult Edit(int id, Task1 requiredTask)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Task1 task = db.Tasks.Find(id);

                    if (TryUpdateModel(task))
                    {
                        task.TaskTitle = requiredTask.TaskTitle;
                        task.TaskDescription = requiredTask.TaskDescription;
                        task.TaskStartDate = requiredTask.TaskStartDate;
                        task.TaskEndDate = requiredTask.TaskEndDate;
                        task.TaskStatus = requiredTask.TaskStatus;
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

 
        public ActionResult Delete(int id)
        {
            Task1 task = db.Tasks.Find(id);
            db.Tasks.Remove(task);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllProjects()
        {
            // generam o lista goala
            var selectList = new List<SelectListItem>();

            // Extragem toate categoriile din baza de date
            var teams = from project in db.Projects
                             select project;

            // iteram prin categorii
            foreach (var team in teams)
            {
                // Adaugam in lista elementele necesare pentru dropdown
                selectList.Add(new SelectListItem
                {
                    Value = team.ProjectId.ToString(),
                    Text = team.Name.ToString()
                });
            }

            // returnam lista de proiecte
            return selectList;
        }

    }

}