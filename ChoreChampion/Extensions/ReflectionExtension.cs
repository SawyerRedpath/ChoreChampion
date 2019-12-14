using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChoreChampion.Extensions
{
    // All extension methods must be static classes (and methods)
    public static class ReflectionExtension
    {
        /// <summary>
        /// A method that retrieves the value of a specified value for an item/object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item/object that we want to retrieve the value from</param>
        /// <param name="propertyName">The property name that we want to retrieve it's value from.</param>
        /// <returns>A string representing the value of the property of that object</returns>
        public static string GetPropertyValue<T>(this T item, string propertyName)
        {
            return item.GetType().GetProperty(propertyName).GetValue(item, null).ToString();
        }
    }
}