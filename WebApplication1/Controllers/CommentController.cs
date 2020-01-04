using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TotallyNotJira.Models;

namespace WebApplication1.Controllers
{
    public class CommentController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Comment
        public ActionResult Index()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }
            ViewBag.Projects = db.Projects.OrderBy(x => x.Name);

            return View();
        }

        [Authorize(Roles = "Member,Organizator,Administrator")]
        public ActionResult New()
        {
            Project project = new Project();

            ViewBag.Tasks = GetTasks();

            return View(project);

        }

        // POST: Project/Create
        [HttpPost]
        [Authorize(Roles = "Member, Organizator,Administrator")]
        public ActionResult New(Project project)
        {
            try
            {
                //project.Team = db.Teams.Find(project.TeamId);

                db.Projects.Add(project);
                db.SaveChanges();

                ApplicationDbContext context = new ApplicationDbContext();
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                if (!User.IsInRole("Administrator"))
                {
                    UserManager.RemoveFromRole(project.UserId, "Member");
                    UserManager.AddToRole(project.UserId, "Organizator");
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}