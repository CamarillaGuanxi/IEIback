using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Text;

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
            string localidad = "",
            string codigoPostal = "",
            string provincia = "",
            string tipo = "")
        {
            try
            {

                Console.WriteLine(localidad);
                Console.WriteLine(codigoPostal);
                Console.WriteLine(provincia);
                Console.WriteLine(tipo);
                List<string> conditions = new List<string>();

                if (localidad != "null")
                {
                    conditions.Add($"l.nombre LIKE '%{localidad}%'");
                }

                if (codigoPostal!= "null")
                {
                    conditions.Add($"ce.codigo_postal = '{codigoPostal}'");
                }

                if (provincia != "null")
                {
                    conditions.Add($"p.nombre LIKE '%{provincia}%'");
                }

                if (tipo != "null")
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

                Console.WriteLine(query);

                var centros = this.ExecuteQueryAndGetStringResult(query);
                return Ok(centros);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        private string ExecuteQueryAndGetStringResult(string query)
        {
            StringBuilder result = new StringBuilder();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Ejemplo: concatenar cada columna seguida de una coma y un espacio
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                result.Append(reader[i].ToString() + ", ");
                            }
                            // Opcional: agregar un salto de línea después de cada fila
                            result.AppendLine();
                        }
                    }
                }
            }

            return result.ToString();
        }
    }
}