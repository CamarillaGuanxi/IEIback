using Microsoft.AspNetCore.Mvc;

namespace IeIAPI
{
    [ApiController]
    [Route("api/datos")]
    public class DatosController : ControllerBase
    {
        private readonly DataAccess _dataAccess;

        public DatosController(IConfiguration configuration)
        {
            _dataAccess = new DataAccess(configuration.GetConnectionString("DefaultConnection"));
        }

        [HttpGet]
        public ActionResult<IEnumerable<object>> Get()
        {
            var yourModels = _dataAccess.GetYourModels();
            return Ok(yourModels);
        }

    }
}
