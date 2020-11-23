using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Movie_WebApi.Model;

namespace Movie_WebApi.Services
{
    public interface IMovieInfoService
    {
        int Add(Movie placeInfo);
        int AddRange(IEnumerable<Movie> places);
        IEnumerable<Movie> GetAll();
        Movie Find(int id);
        int Remove(int id);
        int Update(Movie placeInfo);
        List<Movie> ReadCSVFile(string path);
        void WriteCSVFile(string path, List<Movie> mv);
    }
}
