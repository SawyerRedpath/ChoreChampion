using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChoreChampion.Models.ViewModels
{
    public class SubCategoryAndCategoryViewModel
    {
        // The list of existing categories
        public IEnumerable<Category> CategoryList { get; set; }

        // The subcategory that the user is creating via text input
        public SubCategory SubCategory { get; set; }

        // List of names of all subcategories that exist
        public List<string> SubCategoryList { get; set; }

        // The status message to be displayed on the page
        public string StatusMessage { get; set; }
    }
}