using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Movie_WebApi.Services;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Movie_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieMasterController : ControllerBase
    {
        private readonly IMovieInfoService _fileUploader;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public MovieMasterController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        //[HttpPost("single-file")]
        //public FileUploadResult UploadFile(IFormFile file)
        //{
        //    return _FileUploader.UploadFile(Request.Form.Files[0]);
        //}

        [HttpPost("single-file")]
        public async Task Upload(IFormFile file)
        {
            var csvPath = Path.Combine(_hostingEnvironment.ContentRootPath, "Files\\"+ Request.Form.Files[0].FileName);

            //string csvPath = server.MapPath("~/Files/") + Path.GetFileName(Request.Form.Files[0].FileName);
            _fileUploader.ReadCSVFile(csvPath);
            //Here We are calling function to read CSV file  

            // validate the file, scan virus, save to a file storage
        }
    }
}
