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
            ViewBag.TaskItems = db.TaskItems.OrderBy(x => x.Title);
            return View();
        }

<<<<<<< HEAD:WebApplication1/Controllers/TaskItemController.cs
=======
        public ActionResult Show(int id)
        {
            Task1 task = db.Tasks.Find(id);
            return View(task);

        }
>>>>>>> abd6868e1193bb1e69869936fa332d6b15fb8083:WebApplication1/Controllers/Task1Controller.cs
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

            task.Teams = GetAllTeams();

            return View(task);
           
        }

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

       public ActionResult Edit(int id)
        {
            ViewBag.Project = db.TaskItems.Find(id);

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

            task.Teams = GetAllTeams();

            return View(task);
        }

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

 
        public ActionResult Delete(int id)
        {
            TaskItem task = db.TaskItems.Find(id);
            db.TaskItems.Remove(task);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllTeams()
        {
            // generam o lista goala
            var selectList = new List<SelectListItem>();

            // Extragem toate categoriile din baza de date
            var teams = from team in db.Teams
                             select team;

            // iteram prin categorii
            foreach (var team in teams)
            {
                // Adaugam in lista elementele necesare pentru dropdown
                selectList.Add(new SelectListItem
                {
                    Value = team.TeamId.ToString(),
                    Text = team.Name.ToString()
                });
            }

            // returnam lista de categorii
            return selectList;
        }

    }

}