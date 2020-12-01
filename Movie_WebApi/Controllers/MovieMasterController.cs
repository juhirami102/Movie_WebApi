using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Movie_WebApi.Services;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Movie_WebApi.Resources.Services;
using Movie_WebApi.Model;

namespace Movie_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieMasterController : ControllerBase
    {
       
        #region Properties
        private readonly IBrandService _BrandService;
        private readonly IConfiguration _configuration;
        private readonly IObjectResponseHandler<BrandMasterEntity> _ObjectResponse;
        private readonly IDataTableResponseHandler<BrandMasterEntity> _DatatableResponse;
        private readonly IListResponseHandler<BrandMasterEntity> _ListResponse;
        private readonly IResourceService _messageService;
        private readonly IHostingEnvironment _ihostingEnvironment;
        private readonly IMovieInfoService _fileUploader;
        private readonly IWebHostEnvironment _hostingEnvironment;
        #endregion

        #region Constructor
        public MovieMasterController(IMovieInfoService MovieService, IConfiguration configuration, IListResponseHandler<Movie> ListResponse,
            IObjectResponseHandler<Movie> ObjectResponse,IResourceService messageService, IHostingEnvironment hostingEnvironment)
        {
            _MovieService = IMovieInfoService;
            _configuration = configuration;
            _ObjectResponse = ObjectResponse;
            _ListResponse = ListResponse;
            _messageService = messageService;
            _hostingEnvironment = hostingEnvironment;
        }
        #endregion
        public MovieMasterController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        
        [HttpPost("UploadFile")]
        public async Task Upload(IFormFile file)
        {
            var response = _ObjectResponse;
            var csvPath = Path.Combine(_hostingEnvironment.ContentRootPath, "Files\\"+ Request.Form.Files[0].FileName);
           
            _fileUploader.ReadCSVFile(csvPath);
           response = _ObjectResponse.Create(OMovie, _messageService.GetString("SaveMessage"));
        }

        [HttpGet, Route("GetMovieById/{id}")]
        public async Task<IActionResult> GetMovieById([FromRoute] int id)
        {
            var response = _ObjectResponse;
            var OMovie = new Movie();
            OMovie = await _MovieService.GetMovieById(id);
            if (OMovie != null)
            {
                if (OMovie.BrandImage != null)
                response = _ObjectResponse.Create(OMovie, _messageService.GetString("GetById"));
            }
            else
            {
                response = _ObjectResponse.Create(OMovie, _messageService.GetString("NotFound"));
                return response.ToHttpResponse(404);
            }
            return response.ToHttpResponse();
        }

        [HttpGet, Route("GetAllMovie")]
        public async Task<IActionResult> GetAllMovie()
        {
            var response = _ObjectResponse;
            var OMovie = new Movie();
            OMovie = await _MovieService.GetAllMovie(id);
            if (OMovie != null)
            {
                if (OMovie.movieId != null)
                response = _ObjectResponse.Create(OBrand, _messageService.GetString("GetAllList"));
            }
            else
            {
                response = _ObjectResponse.Create(OBrand, _messageService.GetString("NotFound"));
                return response.ToHttpResponse(404);
            }
            return response.ToHttpResponse();
        }
    }
}
