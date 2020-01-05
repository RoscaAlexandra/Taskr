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
    public class Task1
    {
        [Key]
        public int TaskId { get; set; }

        [Required(ErrorMessage = "Title required!")]
        public string TaskTitle { get; set; }

        public string TaskDescription { get; set; }

        public string TaskStatus {get; set;}

        public DateTime TaskStartDate { get; set; }

        public DateTime TaskEndDate { get; set; }

        //public int TeamId { get; set; }
        public int ProjectId { get; set; }

        //public virtual Team Team { get; set; }
        public virtual Project Project { get; set; }

        // public IEnumerable<SelectListItem> Teams { get; set; }

        public IEnumerable<SelectListItem> Projects { get; set; }

        //public virtual ICollection<Comments> Comment { get; set; }

        public virtual ApplicationUser Member { get; set; }

        public virtual string MemberId { get; set; }
    }
    /*
    public class TaskDBContext : DbContext
    {
        public TaskDBContext() : base("DBConnectionString") { }
        public DbSet<Task1> Tasks { get; set; }
    }*/
}
