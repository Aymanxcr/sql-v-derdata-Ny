using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Väderdata.DataAccess;
using Väderdata.Core;

namespace Väderdata.UI
{
    public class Menu
    {
        // Displays available dates in the database and prompts the user to search by date
        public static void ShowDates()
        {
            using (var context = new WeatherDataContext())
            {
                // Query to retrieve weather records sorted by date
                var dates = from weather in context.Weather
                            where weather.Date < DateTime.Now // Only include past dates
                            orderby weather.Date
                            select weather;

                // Prompt the user with the range of dates
                Console.WriteLine("Search for weather by day.");
                Console.WriteLine($"Enter a date between {dates.FirstOrDefault()!.Date.ToShortDateString()} - {dates.LastOrDefault()!.Date.ToShortDateString()}");

                // Call the TemperatureByDay method to fetch data for the selected date
                SQLTasks.TemperatureByDay(SelectDate());
            }
        }

        // Prompts the user to input a date and returns the selected date
        public static DateTime SelectDate()
        {
            DateTime date;
            var input = DateTime.TryParse(Console.ReadLine(), out date); // Validates user input
            return date; // Returns the parsed date
        }

        // Displays the main menu and allows the user to navigate through various options
        public static void DisplayAndSelectFromMenu()
        {
            bool menu = true; // Flag to keep the menu loop running
            while (menu)
            {
                // Display the menu options
                Console.WriteLine();
                Console.WriteLine("1. Search for temperature information by date.");
                Console.WriteLine("2. Sort by temperature in descending order");
                Console.WriteLine("3. Sort by humidity in descending order");
                Console.WriteLine("4. Sort by mold risk in ascending order");
                Console.WriteLine("5. Date of meteorological autumn and winter");
                Console.WriteLine("6. Exit program");
                Console.WriteLine();

                // Prompt the user to select a menu option
                Console.Write($"Select menu: ");
                string select = Console.ReadLine()!;
                Console.WriteLine();

                // Execute the corresponding task based on the user's selection
                switch (select)
                {
                    case "1":
                        ShowDates(); // Search by date
                        break;
                    case "2":
                        SQLTasks.SortByTemp(); // Sort by temperature
                        break;
                    case "3":
                        SQLTasks.SortByHumidity(); // Sort by humidity
                        break;
                    case "4":
                        SQLTasks.MoldIndex(); // Display mold risk
                        break;
                    case "5":
                        SQLTasks.AutumnOrWinter(); // Display meteorological autumn and winter dates
                        break;
                    case "6":
                        menu = false; // Exit the menu loop
                        break;
                    default:
                        Console.WriteLine("Select something between 1-6."); // Invalid input message
                        break;
                }
            }
        }
    }
}
