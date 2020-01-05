using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace TotallyNotJira.Models
{
    public class Project
    {
        [Key]
        public int ProjectId { get; set; }
        [Required(ErrorMessage = "You must give a name to your project")]
        public string Name { get; set; }
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public string UserId { get; set; }
        public string UserName { get; set; }
        public virtual ApplicationUser User { get; set; }


       // [Required(ErrorMessage = "You must add your project to a team")]
        public string TeamId { get; set; }
        public virtual Team Team { get; set; }
        // public virtual ICollection<Task> Task { get; set; }
        public IEnumerable<SelectListItem> Teams { get; set; }

    }
    /*
    public class ProjectDBContext : DbContext
    {
        public ProjectDBContext() : base("DBConnectionString") { }
        public DbSet<Project> Projects { get; set; }
    }
    */
}