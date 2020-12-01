using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using Movie_WebApi.Mappers;
using Movie_WebApi.Model;

namespace Movie_WebApi.Services
{
    public class MovieInfoService : IMovieInfoService
    {
        //public int Add(Movie placeInfo)
        //{
        //    throw new NotImplementedException();
        //}

    

        //public int AddRange(IEnumerable<Movie> places)
        //{
        //    throw new NotImplementedException();
        //}

        //public Movie Find(int id)
        //{
        //    throw new NotImplementedException();
        //}

        //public IEnumerable<Movie> GetAll()
        //{
        //    throw new NotImplementedException();
        //}

        //public int Remove(int id)
        //{
        //    throw new NotImplementedException();
        //}

 

        //public int Update(Movie placeInfo)
        //{
        //    throw new NotImplementedException();
        //}

      

        public void WriteCSVFile(string path, List<Movie> mv)
        {
            using (StreamWriter sw = new StreamWriter(path, false, new UTF8Encoding(true)))

            {
                using (var cw = new CsvHelper.CsvWriter(sw, System.Globalization.CultureInfo.CurrentCulture))
                {
                    cw.WriteHeader<Movie>();
                    cw.NextRecord();
                    foreach (Movie stu in mv)
                    {
                        cw.WriteRecord<Movie>(stu);
                        cw.NextRecord();
                    }
                }

            }
            
        }
        public async Task<Movie> GetMovieById(int id)
        {
            var Movie = await _repository.GetAsync(id);
            if (Movie == null)
                return null;
            return _mapperFactory.Get<BrandMaster, BrandMasterEntity>(Movie);
        }
        public List<Movie> ReadCSVFile(string location)
        {
            try
            {
                using (var reader = new StreamReader(location, Encoding.Default))
                {
                   
                    using (var csv = new CsvHelper.CsvReader(reader, System.Globalization.CultureInfo.CurrentCulture))
                    {
                        csv.Configuration.RegisterClassMap<Moviesmap>();
                        var records = csv.GetRecords<Movie>().ToList();
                        return records;
                    }
                }
                
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

    }
}
