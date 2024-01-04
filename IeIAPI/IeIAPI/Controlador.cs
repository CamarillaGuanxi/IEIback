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
        private static string host = "monorail.proxy.rlwy.net";
        private static int port = 25026;
        private static string database = "railway";
        private static string user = "root";
        private static string password = "HA2A2baGAEH2B1f-4A42b1g6c2EbGaB4";
        private static string connectionString = $"Server={host};Port={port};Database={database};User Id={user};Password={password};CharSet=utf8mb4;";


        [HttpGet]
        [Route("test")]
        public IEnumerable<object> GetYourModels()
        {
            List<object> result = new List<object>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (MySqlCommand command = new MySqlCommand("SELECT * FROM Localidad", connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Mapear los datos del lector a un objeto YourModel
                            var model = new
                            {
                                Id = (int)reader["Codigo"],
                                nombre = (string)reader["Nombre"],
                                en_provincia = (int)reader["en_provincia"],
                            };

                            result.Add(model);
                        }
                    }
                }
            }

            return result;
        }

        [HttpGet]
        public ActionResult<IEnumerable<object>> Get()
        {
            try
            {
                var yourModels = this.GetYourModels();
                return Ok(yourModels);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

    }
}
