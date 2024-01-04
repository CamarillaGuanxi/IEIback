using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Xml.Linq;
using IeIAPI.Entities;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace IeIAPI
{
  
    internal static class Extractor2XML
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
        public static string Provincias(String comarca) {
         List<string> comarcasBarcelona = new List<string>
        {
            "Alt Penedès", "Anoia", "Bages", "Baix Llobregat", "Barcelonès", "Berguedà", "Garraf",
            "Maresme", "Moianès", "Osona", "Vallès Occidental", "Vallès Oriental"
        }; 
         List<string> comarcasGirona = new List<string>
        {
            "Alt Empordà", "Baix Empordà", "Cerdanya", "Garrotxa", "Gironès", "Pla de l'Estany",
            "Ripollès", "Selva"
        }; 
         List<string> comarcasLleida = new List<string>
        {
            "Alta Ribagorça", "Alt Urgell", "Cerdanya", "Garrigues", "Noguera", "Pallars Jussà",
            "Pallars Sobirà", "Pla d'Urgell", "Segarra", "Segrià", "Solsonès", "Urgell", "Val d'Aran"
        }; 
        List<string> comarcasTarragona = new List<string>
        {
             "Alt Camp", "Baix Camp", "Baix Ebre", "Baix Penedès", "Conca de Barberà", "Montsià",
            "Priorat", "Ribera d'Ebre", "Tarragonès", "Terra Alta"

        };
            if (comarcasBarcelona.Contains(comarca)) return "Barcelona";
            if (comarcasGirona.Contains(comarca)) return "Girona";
            if (comarcasLleida.Contains(comarca)) return "Lleida";
            if (comarcasTarragona.Contains(comarca)) return "Tarragona";

            return "Comarca no encontrada";
    }
    public static string Extractor2(int[] contador,XDocument doc )
        {

            try
            {
                // Rutas predefinidas
                
                string resultsFolderPath ="./";
                Console.WriteLine(resultsFolderPath);
                Boolean error = false;
                Boolean corregido = false;
                // Completar la ruta completa del archivo XML
                //xmlFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, xmlFilePath);
                List<Centro_Educativo> cen = new List<Centro_Educativo>();
                // Comprobar si la carpeta de resultados existe, si no, crearla
                if (!Directory.Exists(resultsFolderPath))
                {
                    Directory.CreateDirectory(resultsFolderPath);
                }

                // Cargar el archivo XML
                
                int i = 0;
                // Obtener todos los elementos 'row' del archivo XML
                var rows = doc.Descendants("row");

                // Crear una lista para almacenar los objetos mapeados
                var dataList = new List<object>();
                List<Localidad> loc = new List<Localidad>();
                List<Provincia> prov = new List<Provincia>();
                // Iterar sobre los elementos 'row'
                foreach (var row in rows)
                {
                    error = false;
                    corregido = false;
                    string codPostal = "";
                    //Esto es importante si no da un valor nullo no se porque
                    if (row.Element("denominaci_completa")?.Value != null)
                    {
                        // Mapear los valores a un objeto


                        //Codigo de localidad no repetido
                        String lnom = row.Element("nom_municipi")?.Value;
                        if (!loc.Any(localidad => localidad.nombre == lnom))
                        {
                            contador[0] += 1;
                            loc.Add(new Localidad(contador[0], lnom));
                        }


                        //Codigo de provincia no repetido
                        string nomcomark = row.Element("nom_comarca")?.Value;
                        string pnom = Provincias(nomcomark);
                        if (pnom == "Comarca no encontrada")
                        {
                            error = true;
                        }
                        if (!prov.Any(Provincia => Provincia.nombre == pnom))
                        {
                            prov.Add(new Provincia(int.Parse(row.Element("codi_postal")?.Value?.Substring(0, 3)), pnom));
                        }
                       

                        //CP errores
                        if (row.Element("codi_postal")?.Value.Length != 5)
                        {
                            if (row.Element("codi_postal")?.Value.Length == 4)
                            {
                                codPostal = 0 + row.Element("codi_postal")?.Value;
                                corregido = true;
                            }
                            else
                            {
                                error = true;
                                Console.WriteLine("Error encontrado en el elemento " + i + " de los datos XML al intentar transformar el codigo postal");

                            }
                        }
                        else
                        {
                            codPostal = row.Element("codi_postal")?.Value;
                        }


                        //Tipos
                        string regimen = "Otros";
                        if (row.Element("nom_naturalesa")?.Value is string && row.Element("nom_naturalesa")?.Value != null)
                        {
                            if (row.Element("nom_naturalesa")?.Value == "Public")
                            {
                                regimen = "Publico";
                            }
                            else if (row.Element("nom_naturalesa")?.Value == "Privat")
                            {
                                regimen = "Privado";
                            }
                        }
                        else
                        {
                            error = true;
                            Console.WriteLine("Error encontrado en el elemento " + i + " de los datos XML al intentar transformar el tipo");
                        }
                        //Pasar datos a string
                        String nombre = row.Element("denominaci_completa")?.Value;
                        String direccion = row.Element("adre_a")?.Value;
                        String longitud = row.Element("coordenades_geo_x")?.Value;
                        String latitud = row.Element("coordenades_geo_y")?.Value;
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
                        var dataObject = new
                        {
                            C_E = new
                            {
                                nombre = RemoveAccents(nombre),
                                tipo = regimen,
                                direccion = RemoveAccents(direccion),
                                codigo_postal = codPostal,
                                telefono = "",
                                longitud = longitud,
                                latitud = latitud,
                                descripcion = "",
                            },
                            LOC = new
                            {
                                codigo = loc.Find(localidad => localidad.nombre == lnom).codigo,
                                nombre = RemoveAccents(lnom),
                            },
                            PR = new
                            {
                                codigo = prov.Find(Provincia => Provincia.nombre == pnom).codigo,
                                nombre = RemoveAccents(pnom),
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
                            dataList.Add(dataObject);
                        }
                        else
                        {
                            contador[3] += 1;
                        }
                        i++;
                    }
                }


                // Crear la ruta para guardar el archivo JSON en la carpeta "Results"
                string jsonFilePath = Path.Combine(resultsFolderPath, "resultadoXML.json");

                string json = JsonConvert.SerializeObject(dataList, Newtonsoft.Json.Formatting.Indented);
                // Escribir JSON a un archivo
               // File.WriteAllText(jsonFilePath, json);

                Console.WriteLine("Archivo JSON generado exitosamente. (XML)");

                // Devolver el código municipal actualizado
                return json;
            }

            catch (Exception e)
            {
                //Console.WriteLine("Error: " + e.Message);
                return "erroe";
            }
        }

        // Método para obtener el tipo de centro educativo

    }
}

