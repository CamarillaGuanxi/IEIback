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
        private static string host = "roundhouse.proxy.rlwy.net";
        private static int port = 56581;
        private static string database = "railway";
        private static string user = "root";
        private static string password = "gfG13156CABHDdcf151AbB6F1c212a1C";
        public DatosController(IConfiguration configuration)
        {
            string connectionString = $"Server={host};Port={port};Database={database};User Id={user};Password={password};CharSet=utf8mb4;";
            _dataAccess = new IeIAPI.DataAccess(connectionString);
        }

        [HttpGet]
        public ActionResult<IEnumerable<object>> Get()
        {
            var yourModels = _dataAccess.GetYourModels();
            return Ok(yourModels);
        }

    }
}
