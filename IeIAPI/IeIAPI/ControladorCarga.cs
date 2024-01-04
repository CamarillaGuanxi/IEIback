using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;


namespace IeIAPI
{
    [ApiController]
    [Route("api/carga")]
    public class ControladorCarga : ControllerBase
    {
        private static string host = "monorail.proxy.rlwy.net";
        private static int port = 25026;
        private static string database = "railway";
        private static string user = "root";
        private static string password = "HA2A2baGAEH2B1f-4A42b1g6c2EbGaB4";
        private static string connectionString = $"Server={host};Port={port};Database={database};User Id={user};Password={password};CharSet=utf8mb4;";
        
        [HttpGet]
        [Route("CSV")]
            public IActionResult ProcesarDatos()
             {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    int[] numeros = new int[4];
                    numeros[0] = 0; // COdigo localidad
                    numeros[1] = 0; // Buenardos
                    numeros[2] = 0; // Corregidos
                    Console.WriteLine("\n-------------------------------");
                    Console.WriteLine("Inicio de extraccion 1");
                    numeros = Extractor1CSV.Extractor1(numeros, connection);
                }

                return Ok(new { Mensaje = "Datos procesados desde la ruta 'api/carga/CSV'" });
            }
            catch (Exception ex)
            {
                // Imprimir detalles de la excepción en la consola
                Console.WriteLine($"Error en el método PostDatos: {ex.GetType().FullName}");
                Console.WriteLine($"Mensaje de la excepción: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");

                // Retornar un código de estado 500 con un mensaje de error genérico
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
 
    }
}
