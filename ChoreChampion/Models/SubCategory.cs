using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ChoreChampion.Models
{
    public class SubCategory
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Sub Category Name")]
        [Required]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        // This line tells the DB/asp.net that category id is a foreign key
        [ForeignKey("CategoryId")]
        // This line tells the DB/asp.net that the foreign key represents the category model/table
        public virtual Category Category { get; set; }
    }
}