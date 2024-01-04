
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using System.Xml.Serialization;
using IeIAPI.Entities;
using MySql.Data.MySqlClient;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Runtime.ConstrainedExecution;
using IeIAPI;
namespace IeIAPI
{


    internal class Extractor3JSON
    {
        public static string RemoveAccents(string input)
        {
            string normalized = input.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            foreach (char c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString();
        }
        public static String ExtractorJSON(int[] contador, string jsonData)
        {
            try
            {
                Boolean error = false;
                Boolean corregido = false;
                // Ruta al archivo JSON
               
                List<Centro_Educativo> cen = new List<Centro_Educativo>();
                List<Localidad> loc = new List<Localidad>();
                // Leer el contenido del archivo JSON
            
                int i = 0;
                // Deserializar el contenido del archivo JSON en una lista de objetos dinámicos
                // Si tienes una clase que coincida con la estructura JSON, usa esa clase en lugar de 'dynamic'
                List<dynamic> originalJsonData = JsonConvert.DeserializeObject<List<dynamic>>(jsonData);

                // Lista para almacenar los objetos transformados
                var transformedDataList = new List<object>();

                foreach (var originalDataObject in originalJsonData)
                {
                    error = false;
                    corregido = false;

                    string codPostal = "";
                    string regimen = "Otros";
                    ///Tipo
                    if (originalDataObject.tipo != null)
                    {
                        if (originalDataObject.tipo == "Colegio Público")
                        {
                            regimen = "Publico";
                        }
                        else if (originalDataObject.tipo == "Centro Privado")
                        {
                            regimen = "Privado";

                        }
                        else if (originalDataObject.tipo == "Centro Privado con varias Enseñanzas de Régimen General")
                        {
                            regimen = "Privado";
                        }
                        else if (originalDataObject.tipo == "Instituto de Educación Secundaria (IES)")
                        {
                            regimen = "Publico";
                        }
                    }
                    else
                    {
                        error = true;
                        Console.WriteLine("Error encontrado en el elemento " + i + " de los datos JSON al intentar transformar el tipo");
                    }
                    //codigos de localidad todos iguales
                    string lnom = originalDataObject.loccen;
                    if (!loc.Any(localidad => localidad.nombre == lnom))
                    {
                        contador[0] = contador[0] + 1;
                        loc.Add(new Localidad(contador[0], lnom));
                    }
                    //Error de codigo postal
                    string cp = originalDataObject.cpcen;
                    if (cp.Length != 5)
                    {
                        if (cp.Length == 4)
                        {
                            codPostal = 0 + originalDataObject.cpcen;
                            corregido = true;
                        }
                        else
                        {
                            error = true;
                            Console.WriteLine("Error encontrado en el elemento " + i + " de los datos JSON al intentar transformar el codigo postal");
                        }
                    }
                    else
                    {
                        codPostal = originalDataObject.cpcen;
                    }
                    ///Datos a string
                    String nombre = originalDataObject.dencen;
                    String dir = originalDataObject.domcen;
                    String telefono = originalDataObject.telcen;
                    String longitud = originalDataObject["geo-referencia"].lon;
                    String latitud = originalDataObject["geo-referencia"].lat;
                    String descipcion = originalDataObject.presentacionCorta;
                    if (!cen.Any(Centro => Centro.nombre == nombre && Centro.codigo_postal == codPostal))
                    {
                        cen.Add(new Centro_Educativo(nombre, codPostal));


                    }
                    else
                    {
                        error = true;
                        Console.WriteLine("Error encontrado en el elemento " + nombre + " de los datos CAT por centro repetido");

                    }
                    if (latitud.Length == 0 || longitud.Length == 0)
                    {
                        error = true;
                    }
                    var transformedObject = new
                    {
                        C_E = new
                        {
                            nombre = RemoveAccents(nombre),
                            tipo = regimen,
                            direccion = RemoveAccents(dir),
                            codigo_postal = cp,
                            telefono = telefono,
                            longitud = longitud, // Asumiendo que geo-referencia es un objeto con lat y lon
                            latitud = latitud,
                            descripcion = RemoveAccents(descipcion)
                        },
                        LOC = new
                        {
                            nombre = RemoveAccents(lnom),
                            codigo = loc.Find(localidad => localidad.nombre == lnom).codigo // Deberías obtener este valor según el mapeo requerido
                        },
                        PR = new
                        {
                            codigo = 999,
                            nombre = "Murcia", // Asignado directamente ya que el mapeo indica que es uniprovincial
                            // Este código debería ser determinado según corresponda
                        }
                    };

                    if (error == false)
                    {
                        if (corregido == false)
                        {
                            contador[1] += 1;
                        }
                        else
                        {
                            contador[2] += 1;
                        }

                        transformedDataList.Add(transformedObject);
                    }
                    else
                    {
                        contador[3] += 1;
                    }
                    i++;
                }

                // Convertir la lista transformada en JSON
                string newJson = JsonConvert.SerializeObject(transformedDataList, Newtonsoft.Json.Formatting.Indented);
                
                // Escribir el JSON transformado a un archivo
                //File.WriteAllText(@"C:\Users\Administrador.WIN-2O4P6U7CI32\source\repos\IEI\IEI\IEI\newfile.json", newJson);

                Console.WriteLine("Archivo JSON transformado generado exitosamente. (JSON)");


                return newJson;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return "error";
            }
        }
    }
}