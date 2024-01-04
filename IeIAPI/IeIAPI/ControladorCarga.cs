using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.IO;
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
                    /*string jsonFilePath = "./resultadoXML.json";
                    string json = System.IO.File.ReadAllText(jsonFilePath);
                    */
                      string filePath = "./CV.csv";

                    // Leer el archivo CSV
                     string[] lines = System.IO.File.ReadAllLines(filePath);
                    string json = Extractor1CSV.Extractor1(numeros, lines);
                    Console.WriteLine("data" + json);
                    try
                    {
                        List<string> c_e = new List<string>();
                        List<int> pr = new List<int>();
                        List<int> loc = new List<int>();
                        dynamic[] dataArray = JsonConvert.DeserializeObject<dynamic[]>(json);

                        Console.WriteLine("Connection successful!");
                        foreach (dynamic data in dataArray)
                        {


                            InsertIntoProvincia(connection, data);

                            InsertIntoLocalidad(connection, data);

                            InsertIntoCentroEducativo(connection, data);



                        }
                    }
                    catch (SqlException ex)
                    {
                        //Console.WriteLine("Error: " + ex.Message);
                    }

                    return Ok(new { Mensaje = "Datos procesados desde la ruta 'api/carga/CSV'" });
                }
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

        private static void InsertIntoProvincia(MySqlConnection connection, dynamic data)
        {
            try
            {
                string insertQuery = "INSERT INTO Provincia (codigo, nombre) VALUES (@Codigo, @Nombre)";

                using (MySqlCommand command = new MySqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@Codigo", data.PR.codigo);
                    command.Parameters.AddWithValue("@Nombre", data.PR.nombre);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine("Error inserting into Provincia: " + ex.Message);
            }
        }

        private static void InsertIntoLocalidad(MySqlConnection connection, dynamic data)
        {
            try
            {
                string insertQuery = "INSERT INTO Localidad (codigo, nombre, en_provincia) VALUES (@Codigo, @Nombre, @EnProvincia)";

                using (MySqlCommand command = new MySqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@Codigo", data.LOC.codigo);
                    command.Parameters.AddWithValue("@Nombre", data.LOC.nombre);
                    command.Parameters.AddWithValue("@EnProvincia", data.PR.codigo);
                    command.ExecuteNonQuery();

                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine("Error inserting into Localidad: " + ex.Message);
            }
        }

        private static void InsertIntoCentroEducativo(MySqlConnection connection, dynamic data)
        {
            try
            {
                string insertQuery = "INSERT INTO Centro_Educativo (nombre, tipo, direccion, codigo_postal, latitud, longitud, telefono, descripcion, en_localidad) " +
                                     "VALUES (@Nombre, @Tipo, @Direccion, @CodigoPostal, @Latitud, @Longitud, @Telefono, @Descripcion, @EnLocalidad)";

                using (MySqlCommand command = new MySqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@Nombre", data.C_E.nombre);
                    command.Parameters.AddWithValue("@Tipo", data.C_E.tipo);
                    command.Parameters.AddWithValue("@Direccion", data.C_E.direccion);
                    command.Parameters.AddWithValue("@CodigoPostal", data.C_E.codigo_postal);
                    command.Parameters.AddWithValue("@Latitud", data.C_E.latitud);
                    command.Parameters.AddWithValue("@Longitud", data.C_E.longitud);
                    command.Parameters.AddWithValue("@Telefono", data.C_E.telefono);
                    command.Parameters.AddWithValue("@Descripcion", data.C_E.descripcion);
                    command.Parameters.AddWithValue("@EnLocalidad", data.LOC.codigo);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine("Error inserting into Centro_Educativo: " + data.C_E.nombre + " " + ex.Message);
            }
        }
    }
}
