using Microsoft.AspNet.Identity;
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
            
            // ViewBag.Tasks = GetTasks();

            return View(comment);

        }

        // POST: Project/Create
        [HttpPost]
        [Authorize(Roles = "Member, Organizator,Administrator")]
        public ActionResult New(int id, Comment comment)
        {
            try
            {
                //project.Team = db.Teams.Find(project.TeamId);
                // comment.TaskId = TaskId;
                comment.Task = db.Tasks.Find(id);
                comment.TaskId = id;
                db.Comments.Add(comment);
                db.SaveChanges();

                return RedirectToAction("Show", "Task1", new {id});
            }
            catch
            {
                return View();
            }
        }

        [Authorize(Roles = "Member,Organizator,Administrator")]
        public ActionResult Edit(int id)
        {
            Comment comment = db.Comments.Find(id);
            ViewBag.Comment = comment;
           // comment.Members = GetMembers();
            if (comment.UserId == User.Identity.GetUserId() || User.IsInRole("Administrator"))
            {
                return View(comment);
            }
            else
            {
                TempData["message"] = "You can not modify this comment!";
                return RedirectToAction("Index");
            }

        }

        [HttpPut]
        [Authorize(Roles = "Member,Organizator,Administrator")]
        public ActionResult Edit(int id, Comment requestComment)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    Comment comment = db.Comments.Find(id);
                    if (comment.UserId == User.Identity.GetUserId() ||
                        User.IsInRole("Administrator"))
                    {
                        if (TryUpdateModel(comment))
                        {
                            comment.Text = requestComment.Text;
                            db.SaveChanges();
                            TempData["message"] = "Comment edited!";
                        }
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["message"] = "You can not modify this comment!";
                        return RedirectToAction("Index");
                    }

                }
                else
                {
                    return View(requestComment);
                }

            }
            catch (Exception e)
            {
                return View(requestComment);
            }
        }

        // GET: Project/Delete/5
        [Authorize(Roles = "Member,Organizator,Administrator")]
        public ActionResult Delete(int id)
        {
            Comment comment = db.Comments.Find(id);
            if (comment.UserId == User.Identity.GetUserId() ||
            User.IsInRole("Administrator"))
            {
                db.Comments.Remove(comment);
                db.SaveChanges();
                TempData["message"] = "Comment deleted!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "You can not delete that comment!";
                return RedirectToAction("Index");
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