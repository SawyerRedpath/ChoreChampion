using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChoreChampion.Extensions
{
    // All extension methods must be static classes (and methods)
    public static class IEnumerableExtension
    {
        /// <summary>
        /// A method that takes an IEnumerable object and creates an IEnumerable of SelectListItem. It is used to create a dropdown list from any list passed in.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The list of items to be turned into a select list/dropdown</param>
        /// <param name="selectedValue">The selected index so we can know which item is selected</param>
        /// <returns>A list of SelectListItems</returns>
        // The first argument of an extension method should be of the extended class preceded by the "this" keyword
        public static IEnumerable<SelectListItem> ToSelectListItem<T>(this IEnumerable<T> items, int selectedValue)
        {
            return from item in items
                   select new SelectListItem
                   {
                       Text = item.GetPropertyValue("Name"),
                       Value = item.GetPropertyValue("Id"),
                       // If the id of this item matches the id selected value id passed ot this method then mark this item as selected
                       Selected = item.GetPropertyValue("Id").Equals(selectedValue.ToString())
                   };
        }

        public static IEnumerable<SelectListItem> ToSelectListItem<T>(this IEnumerable<T> items, string selectedValue)
        {
            return from item in items
                   select new SelectListItem
                   {
                       Text = item.GetPropertyValue("Name"),
                       Value = item.GetPropertyValue("Id"),
                       // If the id of this item matches the id selected value id passed ot this method then mark this item as selected
                       Selected = item.GetPropertyValue("Id").Equals(selectedValue)
                   };
        }
    }
}