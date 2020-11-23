using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using Movie_WebApi.Model;

namespace Movie_WebApi.Mappers
{

    public sealed class Moviesmap : ClassMap<Movie>
    {
        public Moviesmap()
        {
            Map(x => x.title).Name("Title");
            Map(x => x.language).Name("Language");
            Map(x => x.releaseYear).Name("ReleaseYear");
            Map(x => x.duration).Name("Duration");
         
        }
    }
}
