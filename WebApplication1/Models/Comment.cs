using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebApplication1.Models;

namespace TotallyNotJira.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }

        [Required(ErrorMessage = "You need to write something in your comment")]
        public string Text { get; set; }

        public int TaskId { get; set; }
        public virtual Task1 Task { get; set; }
    }
}