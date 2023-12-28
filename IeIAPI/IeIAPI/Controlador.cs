using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
namespace IeIAPI
{
    [ApiController]
    [Route("api/datos")]
    public class DatosController : ControllerBase
    {
        private readonly IeIAPI.DataAccess _dataAccess;

        public DatosController(IConfiguration configuration)
        {
            _dataAccess = new IeIAPI.DataAccess(configuration.GetConnectionString("DefaultConnection"));
        }

        [HttpGet]
        public ActionResult<IEnumerable<object>> Get()
        {
            var yourModels = _dataAccess.GetYourModels();
            return Ok(yourModels);
        }

    }
}
