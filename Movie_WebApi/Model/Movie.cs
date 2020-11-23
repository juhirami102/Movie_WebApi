using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Movie_WebApi.Model
{
    public class Movie
    {
        public  Int64 movieId { get; set; }

        
        public string title { get; set; }
       
        public string language { get; set; }
      
        public string duration { get; set; }
        
        public string releaseYear { get; set; }
    }
}
