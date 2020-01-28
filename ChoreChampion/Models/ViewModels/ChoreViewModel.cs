using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChoreChampion.Models.ViewModels
{
    public class ChoreViewModel
    {
        public Chore Chore { get; set; }

        public IEnumerable<Category> Category { get; set; }

        public IEnumerable<SubCategory> SubCategory { get; set; }

        public IEnumerable<ApplicationUser> User { get; set; }
    }
}