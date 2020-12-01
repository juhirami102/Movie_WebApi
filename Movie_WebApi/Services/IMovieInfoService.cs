using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Movie_WebApi.Model;

namespace Movie_WebApi.Services
{
    public interface IMovieInfoService
    {
        List<Movie> ReadCSVFile(string path);
        void WriteCSVFile(string path, List<Movie> mv);
        Task<Movie> GetMovieById(int id);
        List<Movie> GetAllMovie();
        Task<Movie> AddMovie(Movie entityBrand);
        Task<Movie> UpdateMovie(Movie entityBrand);
        Task<Movie> DeleteMovie(int id);
    }
}
