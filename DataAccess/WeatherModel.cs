using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Väderdata.DataAccess
{
    // Represents a weather data model for the database
    public class WeatherModel
    {
        // Primary key for the Weather table
        public int Id { get; set; }

        // Date of the weather record
        public DateTime Date { get; set; }

        // Location of the weather measurement (nullable to handle missing data)
        public string? Location { get; set; }

        // Temperature recorded at the location
        public double Temperature { get; set; }

        // Humidity level recorded at the location
        public int Humidity { get; set; }
    }
}
