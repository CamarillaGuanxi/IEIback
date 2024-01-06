using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
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
        public IActionResult BuscarCentros(
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

                if (codigoPostal != "null")
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

                var centros = this.ExecuteQueryAndGetResult(query);
                Console.WriteLine(JsonConvert.SerializeObject(centros));
                return Ok(centros);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        private List<Dictionary<string, object>> ExecuteQueryAndGetResult(string query)
        {
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Dictionary<string, object> row = new Dictionary<string, object>();

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[reader.GetName(i)] = reader[i];
                            }

                            result.Add(row);
                        }
                    }
                }
            }

            return result;
        }
    }
}