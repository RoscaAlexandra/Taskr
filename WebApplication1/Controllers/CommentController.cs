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
            ViewBag.Comments = db.Comments.OrderBy(x => x.CommentId);

            return View();
        }

        [Authorize(Roles = "Member,Organizator,Administrator")]
        public ActionResult New()
        {
            Comment comment = new Comment();

            ViewBag.Tasks = GetTasks();

            return View(comment);

        }

        // POST: Project/Create
        [HttpPost]
        [Authorize(Roles = "Member, Organizator,Administrator")]
        public ActionResult New(Comment comment)
        {
            try
            {
                //project.Team = db.Teams.Find(project.TeamId);

                db.Comments.Add(comment);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [NonAction]
        public IEnumerable<Task1> GetTasks()
        {
            // generam o lista goala
            var selectList = new List<Task1>();

            // Extragem toate categoriile din baza de date
            var teams = from team in db.Tasks
                        select team;

            // iteram prin categorii
            foreach (var team in teams)
            {
                // Adaugam in lista elementele necesare pentru dropdown
                selectList.Add(new Task1
                {
                    TaskId = team.TaskId,
                    TaskTitle = team.TaskTitle.ToString()
                });
            }

            // returnam lista de categorii
            return selectList;
        }
    }
}