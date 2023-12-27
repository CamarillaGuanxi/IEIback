using Microsoft.AspNetCore.Mvc;

namespace IeIAPI
{
    [ApiController]
    [Route("api/datos")]
    public class DatosController : ControllerBase
    {
        [HttpGet]
        public ActionResult<string> ObtenerDatos()
        {
            // Lógica para obtener datos desde el backend
            return "Estos son los datos desde el backend";
        }
    }
}
