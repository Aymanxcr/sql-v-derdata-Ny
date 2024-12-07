// Required namespaces for CsvHelper, file handling, globalization, and data access
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using System.IO;
using System.Globalization;
using Väderdata.DataAccess;

namespace Väderdata.Core
{
    public class ExcelData
    {
        // List to store weather data models
        public List<WeatherModel> weatherList { get; set; }

        // Constructor initializing the weather list
        public ExcelData()
        {
            weatherList = new List<WeatherModel>();
        }

        // Reads weather data from a file and uploads it to the database
        public void ReadFileAndUploadToDB(string path)
        {
            // Ensure the database is created and check if data already exists
            using (var context = new WeatherDataContext())
            {
                context.Database.EnsureCreated();
                var empty = context.Weather.FirstOrDefault(i => i.Id == 1);

                // If database is empty, read data from the file
                if (empty == null)
                {
                    var streamReader = new StreamReader(path);

                    // Read and skip the first line (header)
                    string firstLine = streamReader.ReadLine()!;
                    string secondLine = "";

                    // Read each line of the file and parse the data
                    while ((secondLine = streamReader.ReadLine()!) != null)
                    {
                        var line = secondLine.Split(',');

                        // Validate temperature and date, and parse humidity
                        double temp = IsValidIntOrDouble(line);
                        DateTime dateTime = IsValidDateTime(line);
                        var humidity = int.Parse(line[3]);

                        // Create a weather model and add it to the list
                        var myClass = new WeatherModel
                        {
                            Date = dateTime,
                            Location = line[1],
                            Temperature = temp,
                            Humidity = humidity
                        };
                        weatherList.Add(myClass);
                    }

                    // Remove duplicate entries from the list
                    Duplicates();

                    // Save the data to the database
                    AddToDatabase();
                }
            }
        }

        // Saves the weather list to the database
        private void AddToDatabase()
        {
            using (var context = new WeatherDataContext())
            {
                context.AddRange(weatherList); // Add all items from the list to the database
                context.SaveChanges(); // Commit changes to the database
            }
        }

        // Validates and parses a date from a line
        static DateTime IsValidDateTime(string[] line)
        {
            bool dateTime = DateTime.TryParse(line[0], out DateTime result);
            return result;
        }

        // Validates and parses a temperature value from a line
        double IsValidIntOrDouble(string[] line)
        {
            double.TryParse(line[2], out double result);
            return result;
        }

        // Removes duplicate weather entries from the list
        void Duplicates()
        {
            for (int i = 0; i < weatherList.Count; i++)
            {
                for (int j = i + 1; j < weatherList.Count; j++)
                {
                    // Compare two weather entries for equality
                    if (weatherList[i].Date == weatherList[j].Date &&
                        weatherList[i].Location == weatherList[j].Location &&
                        weatherList[i].Temperature == weatherList[j].Temperature &&
                        weatherList[i].Humidity == weatherList[j].Humidity)
                    {
                        weatherList.RemoveAt(j); // Remove duplicate entry
                    }
                    else
                    {
                        break; // Exit inner loop if no duplicate is found
                    }
                }
            }
        }
    }
}
