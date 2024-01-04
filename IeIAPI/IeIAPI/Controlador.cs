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
        [Route("buscarCentros")]
        public ActionResult<IEnumerable<object>> BuscarCentros(
            string localidad = null,
            string codigoPostal = null,
            string provincia = null,
            string tipo = null)
        {
            try
            {
                List<string> conditions = new List<string>();

                if (!string.IsNullOrWhiteSpace(localidad))
                {
                    conditions.Add($"l.nombre LIKE '%{localidad}%'");
                }

                if (!string.IsNullOrWhiteSpace(codigoPostal))
                {
                    conditions.Add($"ce.codigo_postal = '{codigoPostal}'");
                }

                if (!string.IsNullOrWhiteSpace(provincia))
                {
                    conditions.Add($"p.nombre LIKE '%{provincia}%'");
                }

                if (!string.IsNullOrWhiteSpace(tipo))
                {
                    conditions.Add($"ce.tipo = '{tipo}'");
                }

                string query = "SELECT ce.* FROM Centro_Educativo ce " +
                            "INNER JOIN Localidad l ON ce.en_localidad = l.codigo " +
                            "INNER JOIN Provincia p ON l.en_provincia = p.codigo";

                if (conditions.Count > 0)
                {
                    query += " WHERE " + string.Join(" AND ", conditions);
                }

                var centros = this.GetYourModels(query);
                return Ok(centros);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        private IEnumerable<object> GetYourModels(string query)
        {
            List<object> result = new List<object>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
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
    }
}