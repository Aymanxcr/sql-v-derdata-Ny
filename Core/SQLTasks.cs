// Required namespaces for ChoETL, EF Core, and data access
using ChoETL;
using Microsoft.EntityFrameworkCore;
using Väderdata.DataAccess;

namespace Väderdata.Core
{
    public class SQLTasks
    {
        // Method to display temperature and humidity statistics for a specific day
        public static void TemperatureByDay(DateTime date)
        {
            using (var context = new WeatherDataContext())
            {
                // Check if data for the specified date exists
                var dateCheck = context.Weather!.Where(d => d.Date.Date == date).FirstOrDefault();

                if (dateCheck != null)
                {
                    // Calculate various statistics for temperature and humidity
                    var averageTemp = context.Weather!.Where(d => d.Date == date).Average(t => t.Temperature);
                    var averageHumidity = context.Weather!.Where(d => d.Date == date).Average(t => t.Humidity);
                    var lowestTemp = context.Weather!.Where(d => d.Date == date).Min(t => t.Temperature);
                    var highestTemp = context.Weather!.Where(d => d.Date == date).Max(t => t.Temperature);
                    var lowestHumidity = context.Weather!.Where(d => d.Date == date).Min(t => t.Humidity);
                    var highestHumidity = context.Weather!.Where(d => d.Date == date).Max(t => t.Humidity);

                    // Output the calculated statistics
                    Console.WriteLine($"Lowest Temperature: {lowestTemp}\t Average Temperature: {averageTemp}\t Highest Temperature: {highestTemp}");
                    Console.WriteLine($"Lowest Humidity: {lowestHumidity}\t Average Humidity: {averageHumidity}\t Highest Humidity: {highestHumidity}");
                }
                else
                {
                    // Message when no data exists for the specified date
                    Console.WriteLine("Your search doesn't exist in the database.");
                }
            }
        }

        // Method to sort and display average humidity (Top 10) for inside and outside locations
        public static void SortByHumidity()
        {
            using (var context = new WeatherDataContext())
            {
                // Group, calculate, and sort inside humidity by average
                var sortedByInsideHumidity = context.Weather!
                    .Where(l => l.Location == "Inne")
                    .GroupBy(w => w.Date.Date)
                    .Select(t => new { averageHumidity = t.Average(t => t.Humidity), t.Key.Date.Date })
                    .OrderByDescending(t => t.averageHumidity)
                    .Take(10);

                // Group, calculate, and sort outside humidity by average
                var sortedByOutsideHumidity = context.Weather!
                    .Where(l => l.Location == "Ute")
                    .GroupBy(w => w.Date.Date)
                    .Select(t => new { averageHumidity = t.Average(t => t.Humidity), t.Key.Date.Date })
                    .OrderByDescending(t => t.averageHumidity)
                    .Take(10);

                // Display results for inside humidity
                Console.WriteLine("Average inside humidity sorted by day (Top 10)");
                foreach (var item in sortedByInsideHumidity)
                {
                    Console.WriteLine($"{item.Date.Date}\t {item.averageHumidity}");
                }

                // Display results for outside humidity
                Console.WriteLine();
                Console.WriteLine("Average outside humidity sorted by day (Top 10)");
                foreach (var item in sortedByOutsideHumidity)
                {
                    Console.WriteLine($"{item.Date.Date}\t {item.averageHumidity}");
                }
            }
        }

        // Method to determine the first day of meteorological autumn and winter
        public static void AutumnOrWinter()
        {
            using (var context = new WeatherDataContext())
            {
                // Group data by date and calculate daily average temperature
                var list = context.Weather!
                    .Where(l => l.Location == "Ute")
                    .GroupBy(w => w.Date.Date)
                    .Select(t => new { t.Key.Date.Date, averageTemp = t.Average(t => t.Temperature) })
                    .OrderBy(w => w.Date.Date)
                    .ToList();

                DateTime autumnDay;
                DateTime winterDay;
                bool autumnFound = false;
                bool winterFound = false;

                // Loop through the list to find autumn and winter days
                for (int i = 0; i < list.Count(); i++)
                {
                    if (list[i].averageTemp < 10)
                    {
                        autumnDay = list[i].Date;
                        winterDay = list[i].Date;

                        // Check for consecutive days below 10°C for autumn
                        for (int j = 0; j < 5; j++)
                        {
                            if (list[i].averageTemp < 10 && i < list.Count && !autumnFound)
                            {
                                if (j == 4)
                                {
                                    Console.WriteLine($"Meteorological autumn: {autumnDay.Date.Day}/{autumnDay.Date.Month}/{autumnDay.Date.Year}");
                                    autumnFound = true;
                                }
                            }
                        }

                        // Check for consecutive days below 0°C for winter
                        for (int k = 0; k < 5; k++)
                        {
                            if (list[i].averageTemp < 0 && i < list.Count && !winterFound)
                            {
                                if (k == 4)
                                {
                                    Console.WriteLine($"Meteorological winter: {winterDay.Date.Day}/{winterDay.Date.Month}/{winterDay.Date.Year}");
                                    winterFound = true;
                                }
                            }
                        }

                        // Break loop if both autumn and winter are found
                        if (autumnFound && winterFound)
                            break;
                    }
                }

                // Output messages if no autumn or winter found
                if (!autumnFound)
                {
                    Console.WriteLine("There was no meteorological autumn this year");
                }
                if (!winterFound)
                {
                    Console.WriteLine("There was no meteorological winter this year");
                }
            }
        }

        // Method to sort and display average temperatures (Top 10) for inside and outside locations
        public static void SortByTemp()
        {
            using (var context = new WeatherDataContext())
            {
                // Group, calculate, and sort inside temperatures by average
                var sortedByInsideTemperature = context.Weather!
                    .Where(l => l.Location == "Inne")
                    .GroupBy(w => w.Date.Date)
                    .Select(t => new { averageTemp = t.Average(t => t.Temperature), t.Key.Date.Date })
                    .OrderByDescending(t => t.averageTemp)
                    .Take(10);

                // Group, calculate, and sort outside temperatures by average
                var sortedByOutsideTemperature = context.Weather!
                    .Where(l => l.Location == "Ute")
                    .GroupBy(w => w.Date.Date)
                    .Select(t => new { averageTemp = t.Average(t => t.Temperature), t.Key.Date.Date })
                    .OrderByDescending(t => t.averageTemp)
                    .Take(10);

                // Display results for inside temperature
                Console.WriteLine("Average inside temperature sorted by day (Top 10)");
                foreach (var item in sortedByInsideTemperature)
                {
                    Console.WriteLine($"{item.Date.Date}\t {item.averageTemp}");
                }

                // Display results for outside temperature
                Console.WriteLine();
                Console.WriteLine("Average outside temperature sorted by day (Top 10)");
                foreach (var item in sortedByOutsideTemperature)
                {
                    Console.WriteLine($"{item.Date}\t {item.averageTemp}");
                }
            }
        }

        // Method to calculate mold index based on temperature and humidity
        
public static void MoldIndex()
        {

            double[,] mtab = {
                { 0, 0, 0, 0},     // # 0°
                { 0,97,98,100 }, // # 1°
                {0,95,97,100}, // # 2°
                {0,93,95,100 }, // # 3°
                {0,91,93,98},  // # 4°
                {0,88,92,97},  // # 5°
                {0,87,91,96},  // # 6°  
                {0,86,91,95},  // # 7°  
                {0,84,90,95},  // # 8°  
                {0,83,89,94},  // # 9°  
                {0,82,88,93},  // # 10°  
                {0,81,88,93},  // # 11°  
                {0,81,88,92},  // # 12°  
                {0,80,87,92},  // # 13°  
                {0,79,87,92},  // # 14°  
                {0,79,87,91},  // # 15°  
                {0,79,86,91},  // # 16°  
                {0,79,86,91},  // # 17°  
                {0,79,86,90},  // # 18°  
                {0,79,85,90},  // # 19°  
                {0,79,85,90},  // # 20°  
                {0,79,85,90},  // # 21°  
                {0,79,85,89},  // # 22°  
                {0,79,84,89},  // # 23°  
                {0,79,84,89},  // # 24°
                {0,79,84,89},  // # 25°  
                {0,79,84,89},  // # 26°  
                {0,79,83,88},  // # 27°  
                {0,79,83,88},  // # 28°  
                {0,79,83,88},  // # 29°  
                {0,79,83,88},  // # 30°  
                {0,79,83,88},  // # 31°  
                {0,79,83,88},  // # 32°  
                {0,79,82,88},  // # 33°  
                {0,79,82,87},  // # 34°  
                {0,79,82,87},  // # 35°  
                {0,79,82,87},  // # 36°  
                {0,79,82,87},  // # 37°  
                {0,79,82,87},  // # 38°  
                {0,79,82,87},  // # 39°  
                {0,79,82,87},  // # 40°  
                {0,79,81,87},  // # 41°  
                {0,79,81,87},  // # 42°  
                {0,79,81,87},  // # 43°  
                {0,79,81,87},  // # 44°  
                {0,79,81,86},  // # 45°  
                {0,79,81,86},  // # 46°  
                {0,79,81,86},  // # 47°  
                {0,79,80,86},  // # 48°  
                {0,79,80,86 },  // # 49°  
                { 0,79,80,86 }   // # 50°
};

            using (var context = new WeatherDataContext())
            {
                var mold = context.Weather!.GroupBy(w => w.Date.Date).Select(a => new { averageTemp = a.Average(a => a.Temperature), averageHumidity = a.Average(a => a.Humidity), a.Key.Date.Date }).OrderBy(a => a.Date.Date);
                var mindex = 0;
                foreach (var item in mold)
                {
                    if (item.averageTemp <= 0 || item.averageTemp > 50)
                    {
                        mindex = 0;
                        Console.WriteLine($"Date: {item.Date.Date} Average Temp: {(int)item.averageTemp} Average Humidity: {(int)item.averageHumidity} Mold rating: {mindex}");
                    }
                    else
                    {
                        for (int i = 1; i < 4; i++)
                        {
                            if (item.averageHumidity < mtab[(int)item.averageTemp, i])
                            {
                                mindex = i-1;
                                Console.WriteLine($"Date: {item.Date.Date} Average Temp: {(int)item.averageTemp} Average Humidity: {(int)item.averageHumidity} Mold rating: {mindex}");
                                break;
                            }
                        }
                    }

                }
            }


        }
    }
}
