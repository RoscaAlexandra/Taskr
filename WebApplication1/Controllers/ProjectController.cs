using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using TotallyNotJira.Models;
using WebApplication1.Models;

namespace TotallyNotJira.Controllers
{
    public class ProjectController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Project
        // [Authorize(Roles = "Member,Organizator,Administrator")]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Index()
        {
            var projects = db.Projects.Include("Teams").Include("User");

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }
            ViewBag.Projects = db.Projects.OrderBy(x => x.Name);

            return View();
        }
        [Authorize(Roles = "Member,Organizator,Administrator")]
        public ActionResult Show(int id)
        {
            Project project = db.Projects.Find(id);
            return View(project);

        }
        // GET: Project/Create
        [Authorize(Roles = "Member,Organizator,Administrator")]
        public ActionResult New()
        {
            Project project = new Project();

           // project.Teams = GetTeams();
            // Preluam ID-ul utilizatorului curent
            project.UserId = User.Identity.GetUserId();
            project.UserName = User.Identity.GetUserName();
            project.User = db.Users.Find(project.UserId);

            ViewBag.Teams = GetTeams();
            project.Teams = GetTeams();

            return View(project);

        }

        // POST: Project/Create
        [HttpPost]
        [Authorize(Roles = "Member, Organizator,Administrator")]
        public ActionResult New(Project project)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    project.Team = db.Teams.Find(Int32.Parse(project.TeamId));

                    project.Teams = GetTeams();
                    db.Projects.Add(project);
                    db.SaveChanges();

                    ApplicationDbContext context = new ApplicationDbContext();
                    var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                    var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                    if (User.IsInRole("Member"))
                    {
                        UserManager.RemoveFromRole(project.UserId, "Member");
                        UserManager.AddToRole(project.UserId, "Organizator");
                    }

                   // project.Teams = GetTeams();
                    return RedirectToAction("Index");
                }
                else
                {
                    project.Teams = GetTeams();
                    return View(project);
                }
            }
            catch (Exception e)
            {
                project.Teams = GetTeams();
                return View(project);
            }
        }
        [Authorize(Roles = "Organizator,Administrator")]
        public ActionResult Edit(int id)
        {
            Project project = db.Projects.Find(id);
            ViewBag.Project = project;
            ViewBag.Teams = GetTeams();
            project.Teams = GetTeams();
            if (project.UserId == User.Identity.GetUserId() || User.IsInRole("Administrator"))
            {
                return View(project);
            }
            else
            {
                TempData["message"] = "You can not modify this project!";
                return RedirectToAction("Index");
            }

        }

        [HttpPut]
        [Authorize(Roles = "Organizator,Administrator")]
        public ActionResult Edit(int id, Project requestProject)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    Project project = db.Projects.Find(id);
                    if (project.UserId == User.Identity.GetUserId() ||
                        User.IsInRole("Administrator"))
                    {
                        if (TryUpdateModel(project))
                        {
                            project.Name = requestProject.Name;
                            project.Description = requestProject.Description;
                            db.SaveChanges();
                            TempData["message"] = "Project edited!";
                        }
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["message"] = "You can not modify this project!";
                        return RedirectToAction("Index");
                    }

                }
                else
                {
                    return View(requestProject);
                }

            }
            catch (Exception e)
            {
                return View(requestProject);
            }
        }

        // GET: Project/Delete/5
        [Authorize(Roles = "Organizator,Administrator")]
        public ActionResult Delete(int id)
        {
            Project project = db.Projects.Find(id);
            if (project.UserId == User.Identity.GetUserId() ||
            User.IsInRole("Administrator"))
            {
                db.Projects.Remove(project);
                db.SaveChanges();
                TempData["message"] = "Project deleted!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "You can not delete that project!";
                return RedirectToAction("Index");
            }
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetTeams()
        {
            // generam o lista goala
            var selectList = new List<SelectListItem>();

            // Extragem toate echipele din baza de date
            var teams = from team in db.Teams
                             select team;

            // iteram prin echipe
            foreach (var team in teams)
            {
                // Adaugam in lista elementele necesare pentru dropdown
                selectList.Add(new SelectListItem
                {
                    Value = team.TeamId.ToString(),
                    Text = team.Name.ToString()
                });
            }

            // returnam lista de echipe
            return selectList;
        }
    }
}
