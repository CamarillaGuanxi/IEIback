using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.IO;
using System.Xml.Linq;
using System.Security.Policy;
using Org.BouncyCastle.Asn1.Tsp;

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

       [HttpPost]
       [Route("CSV")]
       public IActionResult ProcesarDatos([FromBody] string lines)
       {
           try
           {
               using (MySqlConnection connection = new MySqlConnection(connectionString))
               {
                   connection.Open();

                   



                   try
                   {
                       List<string> c_e = new List<string>();
                       List<int> pr = new List<int>();
                       List<int> loc = new List<int>();
                       dynamic[] dataArray = JsonConvert.DeserializeObject<dynamic[]>(lines);

                       Console.WriteLine("Conexión exitosa!");
                       foreach (dynamic data in dataArray)
                       {
                           InsertIntoProvincia(connection, data);
                           InsertIntoLocalidad(connection, data);
                           InsertIntoCentroEducativo(connection, data);
                       }
                   }
                   catch (SqlException ex)
                   {
                       // Manejar la excepción de SQL
                   }

                   return Ok(lines);
               }
           }
           catch (Exception ex)
           {
               Console.WriteLine($"Error: {ex.Message}");
               return StatusCode(500, $"Internal Server Error: {ex.Message}");
           }
       }
      /*  [HttpGet]
        [Route("CSV")]
        public IActionResult ProcesarDatos()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    int[] numeros = new int[4];
                    numeros[0] = 0; // Código localidad
                    numeros[1] = 0; // Buenos
                    numeros[2] = 0; // Corregidos
                    string url = "https://raw.githubusercontent.com/CamarillaGuanxi/IEIback/main/IeIAPI/IeIAPI/CV.csv";

                    using (HttpClient client = new HttpClient())
                    using (HttpResponseMessage response = client.GetAsync(url).Result)
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            using (HttpContent content = response.Content)
                            {
                                // Descargar el contenido como una cadena
                                string csvContent = content.ReadAsStringAsync().Result;

                                // Separar las líneas del CSV
                                string[] lines = csvContent.Split('\n');
                                Console.WriteLine(lines[2]);
                                Console.WriteLine(System.Runtime.InteropServices.RuntimeInformation.OSDescription);
                                // Procesar el CSV
                                string json = Extractor1CSV.Extractor1(numeros, lines);

                                try
                                {
                                    List<string> c_e = new List<string>();
                                    List<int> pr = new List<int>();
                                    List<int> loc = new List<int>();
                                    dynamic[] dataArray = JsonConvert.DeserializeObject<dynamic[]>(json);

                                    Console.WriteLine("Conexión exitosa!");
                                    foreach (dynamic data in dataArray)
                                    {
                                        InsertIntoProvincia(connection, data);
                                        InsertIntoLocalidad(connection, data);
                                        InsertIntoCentroEducativo(connection, data);
                                    }
                                }
                                catch (SqlException ex)
                                {
                                    // Manejar la excepción de SQL
                                    return StatusCode(500, $"Error al procesar datos SQL: {ex.Message}");
                                }
                            }
                        }
                        else
                        {
                            // Manejar el caso cuando la solicitud no es exitosa
                            return StatusCode((int)response.StatusCode, $"Error al descargar el archivo. Código de estado: {response.StatusCode}");
                        }
                    }

                    return Ok(new { Mensaje = "Datos procesados desde la ruta 'api/carga/CSV'" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }*/
        [HttpGet]
    [Route("CAT")]
    public IActionResult ProcesarCATDatos()
    {
        try
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();



                int[] numeros = new int[4];
                numeros[0] = 50; // COdigo localidad
                numeros[1] = 50; // Buenardos
                numeros[2] = 0; // Corregidos
                Console.WriteLine("\n-------------------------------");
                Console.WriteLine("Inicio de extraccion 1");
                string url = "https://raw.githubusercontent.com/CamarillaGuanxi/IEIback/main/IeIAPI/IeIAPI/CAT.xml";

                XDocument doc = XDocument.Load(url);

                string json = Extractor2XML.Extractor2(numeros, doc);


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

                return Ok(json);
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

        

      

        [HttpGet]
        [Route("MUR")]

        public IActionResult ProcesarMURDatos()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();



                    int[] numeros = new int[4];
                    numeros[0] = 20; // COdigo localidad
                    numeros[1] = 20; // Buenardos
                    numeros[2] = 0; // Corregidos
                    Console.WriteLine("\n-------------------------------");
                    Console.WriteLine("Inicio de extraccion 1");
                    string jsonFilePath = "./MUR.json";
                    string jsonData = System.IO.File.ReadAllText(jsonFilePath);





                    string json = Extractor3JSON.ExtractorJSON(numeros, jsonData);
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

                    return Ok(json);
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
        [HttpGet]
        [Route("BorrarDatos")]
        public IActionResult BorrarDatos()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // Eliminar todos los registros de las tablas
                    BorrarRegistros(connection, "Centro_Educativo");
                    BorrarRegistros(connection, "Localidad");
                    BorrarRegistros(connection, "Provincia");

                    return Ok(new { Mensaje = "Datos borrados exitosamente." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al borrar datos: {ex.Message}");
            }
        }

        private void BorrarRegistros(MySqlConnection connection, string tableName)
        {
            try
            {
                string deleteQuery = $"DELETE FROM {tableName}";

                using (MySqlCommand command = new MySqlCommand(deleteQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al borrar registros de la tabla {tableName}: {ex.Message}");
            }
        }
    }
}
