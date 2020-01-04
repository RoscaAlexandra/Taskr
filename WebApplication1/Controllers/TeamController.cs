using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TotallyNotJira.Models;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class TeamController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Team
        public ActionResult Index()
        {
            var teams = db.Teams.Include("Project");

            ViewBag.Teams = db.Teams.OrderBy(x => x.Name);
            return View();
        }

        [Authorize(Roles = "Administrator, Organizator")]
        public ActionResult Show(int id)
        {
            Team team = db.Teams.Find(id);

            var selectList = new List<SelectListItem>();

            // Extragem toate categoriile din baza de date
            var tasks = from task in db.Tasks
                        select task;
            // var projects = team.Projects;
            var allProjects = from project in db.Projects
                           select project;
            var projects = new List<Project>();
            foreach(var project in allProjects)
            {
                if (project.TeamId == team.TeamId)
                    projects.Add(project);
            }

            foreach (var task in tasks)
                foreach(var project in projects)
                    {
                        if(task.ProjectId == project.ProjectId)
                        {
                            selectList.Add(new SelectListItem
                            {
                                Value = task.TaskId.ToString(),
                                Text = task.TaskTitle.ToString()
                            });
                        }
                    }

            team.Tasks = selectList;

            return View(team);
        }

        [Authorize(Roles = "Administrator,Organizator")]
        public ActionResult New()
        {
            Team team = new Team();

            ViewBag.AllUsers = GetAllUsers();

            return View(team);

        }

        [HttpPost]
        [Authorize(Roles = "Administrator,Organizator")]
        public ActionResult New(Team team)
        {
            UserStore<ApplicationUser> userStore = new UserStore<ApplicationUser>(db);
            ApplicationUserManager userManager = new ApplicationUserManager(userStore);

            team.Members = new List<ApplicationUser>();

            foreach (var memberId in team.MemberIds)
            {
                var member = userManager.FindById(memberId);
                team.Members.Add(member);
            }

            try
            {
                db.Teams.Add(team);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [Authorize(Roles = "Administrator,Organizator")]
        public ActionResult Edit(int id)
        {
            Team team = db.Teams.Find(id);

            ViewBag.AllUsers = GetAllUsers();
            ViewBag.CurrentMembers = GetCurrentMembers(team);

            return View(team);

        }

        [HttpPut]
        [Authorize(Roles = "Administrator,Organizator")]
        public ActionResult Edit(int id, Team requestTeam)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Team team = db.Teams.Find(id);

                    UserStore<ApplicationUser> userStore = new UserStore<ApplicationUser>(db);
                    ApplicationUserManager userManager = new ApplicationUserManager(userStore);

                    requestTeam.Members = new List<ApplicationUser>();

                    foreach (var memberId in requestTeam.MemberIds)
                    {
                        var member = userManager.FindById(memberId);
                        requestTeam.Members.Add(member);
                    }

                    if (TryUpdateModel(requestTeam))
                    {
                        team.Name = requestTeam.Name;
                        team.MemberIds = requestTeam.MemberIds;
                        team.Members = requestTeam.Members;
                        db.SaveChanges();
                        TempData["message"] = "Team edited!";
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(requestTeam);
                }
            }
            catch (Exception e)
            {
                return View(requestTeam);
            }
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(int id)
        {
            Team team = db.Teams.Find(id);

            if (User.IsInRole("Administrator"))
            {
                db.Teams.Remove(team);
                db.SaveChanges();
                TempData["message"] = "Team deleted!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "You can not delete that team!";
                return RedirectToAction("Index");
            }
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

        [NonAction]
        public IEnumerable<SelectListItem> GetCurrentMembers(Team team)
        {
            var selectList = new List<SelectListItem>();

            foreach (var member in team.Members)
            {
                var user = db.Users.Find(member.Id);

                selectList.Add(new SelectListItem
                {
                    Value = user.Id.ToString(),
                    Text = user.UserName.ToString()
                });
            }

            return selectList;
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllProjects()
        {
            // generam o lista goala
            var selectList = new List<SelectListItem>();

            // Extragem toate categoriile din baza de date
            var projects = from pr in db.Projects
                           select pr;

            // iteram prin categorii
            foreach (var project in projects)
            {
                // Adaugam in lista elementele necesare pentru dropdown
                selectList.Add(new SelectListItem
                {
                    Value = project.ProjectId.ToString(),
                    Text = project.Name.ToString()
                });
            }

            // returnam lista de categorii
            return selectList;
        }
    }
}