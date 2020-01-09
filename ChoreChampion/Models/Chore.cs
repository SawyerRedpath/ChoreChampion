using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ChoreChampion.Models
{
    public class Chore
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsComplete { get; set; }

        [Display(Name = "Before Picture")]
        public string BeforeImage { get; set; }

        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [ForeignKey("SubCategoryId")]
        public virtual SubCategory SubCategory { get; set; }

        [Display(Name = "Sub Category")]
        public int SubCategoryId { get; set; }
    }
}