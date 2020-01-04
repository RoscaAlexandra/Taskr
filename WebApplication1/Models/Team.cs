using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TotallyNotJira.Models;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace WebApplication1.Models
{
    public class Team
    {
        [Key]
        public int TeamId { get; set; }

        public virtual ICollection<string> MemberIds { get; set; }
        public virtual ICollection<ApplicationUser> Members { get; set; }

        [Required(ErrorMessage = "You must name your team")]
        public string Name { get; set; }

        //public string ProjectId { get; set; }

        //public IEnumerable<SelectListItem> Projects { get; set; }

       // public IEnumerable<SelectListItem> Users { get; set; }
    }
}