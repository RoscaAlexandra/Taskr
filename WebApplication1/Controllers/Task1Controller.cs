using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TotallyNotJira.Models;
using WebApplication1.Models;

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

            var comments = db.Comments;
            var goodComments = new List<Comment>();
            foreach(var comment in comments)
            {
                if (comment.TaskId == task.TaskId)
                    goodComments.Add(comment);
            }
            ViewBag.Comments = goodComments;

            
            
           // ViewBag.Members = GetAllUsers();

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
                    var projectId = task.ProjectId;
                    task.Project = db.Projects.Find(projectId);
                    var id = Int32.Parse(task.Project.TeamId); 
                    db.Tasks.Add(task);
                    db.SaveChanges();
                    TempData["message"] = "Task added!";

                     return RedirectToAction("Show","Team",new { id,TempData});
                    //return new RedirectResult(@"~\Team\Show\" + id);

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
       // [Authorize(Roles = "Organizator,Administrator")]
        public ActionResult Edit(int id)
        {
            if (User.IsInRole("Member"))
            {
                TempData["message"] = "You don't have the permission to edit more details!";
                return RedirectToAction("Show", "Task1", new { id });
            }

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

            Project project = db.Projects.Find(task.ProjectId);
            Team team = db.Teams.Find(Int32.Parse(project.TeamId));
            var members = team.Members;

            var members1 = new List<SelectListItem>();
            foreach (var member in members)
            {
                members1.Add(new SelectListItem
                {
                    Value = member.Id.ToString(),
                    Text = member.UserName.ToString()
                });
            }
            ViewBag.Members = members1;

            return View(task);
        }

        [HttpPut]
        [Authorize(Roles = "Organizator,Administrator")]
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
                        task.MemberId = requiredTask.MemberId;
                        task.Member = db.Users.Find(requiredTask.MemberId);
                        db.SaveChanges();
                        id = task.TaskId;
                    }

                    return RedirectToAction("Show","Task1",new { id });
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

        public ActionResult ChangeStatus(int id, int newTaskStatus)
        {
            try
            {
                Task1 task = db.Tasks.Find(id);
                switch (newTaskStatus)
                {
                    case 1:
                        task.TaskStatus = "Not started";
                        break;
                    case 2:
                        task.TaskStatus = "In progress";
                        break;
                    case 3:
                        task.TaskStatus = "Done";
                        break;
                }
                db.SaveChanges();
                TempData["message"] = "Status changed!";

                return RedirectToAction("Show", new { id = id });
            }
            catch
            {
                return RedirectToAction("Show", new { id = id });
            }
        }

        [Authorize(Roles = "Organizator,Administrator")]
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
        [NonAction]
        public IEnumerable<SelectListItem> GetAllUsers()
        {
            var selectList = new List<SelectListItem>();

            var users = db.Users.ToList();

            foreach (var user in users)
            {
                selectList.Add(new SelectListItem
                {
                    Value = user.Id.ToString(),
                    Text = user.UserName.ToString()
                });
            }

            return selectList;
        }
 
    }

}