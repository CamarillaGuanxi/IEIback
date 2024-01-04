using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace IeIAPI
{
    internal class Railway
    {
        private static string host = "roundhouse.proxy.rlwy.net";
        private static int port = 56581;
        private static string database = "railway";
        private static string user = "root";
        private static string password = "gfG13156CABHDdcf151AbB6F1c212a1C";

        public static MySqlConnection GetConnection()
        {
            string connectionString = $"Server={host};Port={port};Database={database};User Id={user};Password={password};CharSet=utf8mb4;";

            MySqlConnection connection = new MySqlConnection(connectionString);


            connection.Open();

            return connection;
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

        public static void InsertCentro(string json, MySqlConnection connection)
        {
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
        }


    }
}
