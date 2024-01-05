using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Xml;
using Newtonsoft.Json;
using System.Xml.Serialization;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Threading;
using OpenQA.Selenium.Interactions;
using IeIAPI.Entities;
using MySql.Data.MySqlClient;

using System.Globalization;
using System.Text;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;

namespace IeIAPI
{
    internal class Extractor1CSV
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
        public static string Extractor1(int[] contador, string[] lines)
        {

            try
            {

                // Ruta al archivo CSV
              



                // Crear una lista para almacenar los objetos mapeados
                var dataList = new List<object>();
                var options = new ChromeOptions();
                Boolean error = false;
                Boolean corregido = false;
                options.AddArgument("--headless"); // Modo sin interfaz gráfica
                List<Localidad> loc = new List<Localidad>();
                List<Provincia> prov = new List<Provincia>();
                List<Centro_Educativo> cen = new List<Centro_Educativo>();
                using (IWebDriver driver = new FirefoxDriver())
                {
                    driver.Manage().Timeouts().PageLoad.Add(TimeSpan.FromSeconds(90000));
                    driver.Manage().Timeouts().AsynchronousJavaScript.Add(TimeSpan.FromSeconds(900000));


                    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                    for (int i = 1; i < lines.Length; i++)
                    {
                        driver.Navigate().GoToUrl("https://www.geoplaner.com/");
                        error = false;
                        corregido = false;
                        var values = lines[i].Split(';');
                        string direccion = values[10] + " " + values[6] + " " + values[7] + " " + values[8];


                        // Esperar hasta que el elemento sea visible
                        IWebElement direccionInput = driver.FindElement(By.Name("ad"));


                        // Enviar datos al elemento input
                        direccionInput.SendKeys(direccion);

                        // Enviar el formulario (puede ser un clic en un botón de búsqueda, depende de tu página)
                        direccionInput.Submit();
                        IWebElement botonOk = driver.FindElement(By.CssSelector("input.butt3[value='ok']"));

                        // Hacer clic en el botón.
                        botonOk.Click();

                        // Esperar un momento para que la página responda (puedes ajustar el tiempo según sea necesario)
                        System.Threading.Thread.Sleep(2000);

                        // Obtener los elementos de salida (longitud y latitud)
                        IWebElement latitudInput = driver.FindElement(By.Name("da1"));
                        IWebElement longitudInput = driver.FindElement(By.Name("do1"));

                        // Obtener los valores de longitud y latitud.
                        string lat = latitudInput.GetAttribute("value");
                        string lon = longitudInput.GetAttribute("value");
                        // Mapear los valores a un objeto
                        //Tipos
                        string regimen = "Otros";
                        if (values[5] is string && values[5] != null)
                        {
                            if (values[5] == "PÃšB.")
                            {
                                regimen = "Publico";
                            }
                            else if (values[5] == "PRIV.")
                            {
                                regimen = "Privado";
                            }
                            else if (values[5] == "PRIV. CONC.")
                            {
                                regimen = "Concertado";
                            }
                        }
                        else
                        {
                            error = true;
                            Console.WriteLine("Error encontrado en el elemento " + i + " de los datos CSV al intentar transformar el tipo");
                        }

                        //Codigo postal
                        string codPostal = "";
                        if (values[9].Length != 5)
                        {
                            if (values[9].Length == 4)
                            {
                                codPostal = 0 + values[9];
                                corregido = true;
                            }
                            else
                            {
                                error = true;
                                Console.WriteLine("Error encontrado en el elemento " + i + " de los datos CSV al intentar transformar el codigo postal");
                            }
                        }
                        else
                        {
                            codPostal = values[9];
                        }

                        //Codigo de localidad no repetido
                        string lnom = values[10];
                        if (!loc.Any(localidad => localidad.nombre == lnom))
                        {
                            contador[0] = contador[0] + 1;
                            loc.Add(new Localidad(contador[0], lnom));
                        }

                        //Codigo de provincia no repetido
                        string pnom = values[11];
                        if (!prov.Any(Provincia => Provincia.nombre == pnom))
                        {
                            prov.Add(new Provincia(int.Parse(string.Join("", values[9].Take(3).ToArray())), pnom));
                        }
                        String nombre = values[3];
                        if (!cen.Any(Centro => Centro.nombre == nombre && Centro.codigo_postal == codPostal))
                        {
                            cen.Add(new Centro_Educativo(nombre, codPostal));


                        }
                        else
                        {
                            error = true;
                            Console.WriteLine("Error encontrado en el elemento " + nombre + " de los datos CSV por centro repetido");

                        }
                        ///String de los elementos

                        String dir = values[6] + " " + values[7] + " " + values[8];
                        String telefono = values[12];
                        var dataObject = new
                        {
                            C_E = new
                            {
                                nombre = RemoveAccents(nombre),
                                tipo = regimen,
                                direccion = RemoveAccents(direccion),
                                codigo_postal = codPostal,
                                telefono = telefono,
                                longitud = lon,
                                latitud = lat,
                                descripcion = "",
                            },
                            LOC = new
                            {
                                codigo = loc.Find(localidad => localidad.nombre == lnom).codigo,
                                nombre = RemoveAccents(lnom),
                            },
                            PR = new
                            {
                                codigo = prov.Find(provincia => provincia.nombre == pnom).codigo,
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
                        direccionInput.Clear();

                    }

                    string json = JsonConvert.SerializeObject(dataList, Newtonsoft.Json.Formatting.Indented);
                    Console.WriteLine(json);
                    return json;

                }
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.Message);
                return "error";

            }
        }
    }
}

