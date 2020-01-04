using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TotallyNotJira.Models
{
    public class TaskItem
    {
        [Key]
        public int TaskId { get; set; }

        [Required(ErrorMessage = "Title required!")]
        public string Title { get; set; }

        public string Description { get; set; }

        public TaskStatus Status { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "You need to assign this task to a project!")]
        public int ProjectId { get; set; }
    }
    public class TaskDBContext : DbContext
    {
        public TaskDBContext() : base("DBConnectionString") { }
        public DbSet<TaskItem> TaskItems { get; set; }
    }

    public enum TaskStatus
    {
        ToDo,
        InProgress,
        Done
    }
}
