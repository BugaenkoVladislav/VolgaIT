using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.Database.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AcountController : ControllerBase
    {
        SimbirGoContext db;
        public AcountController(SimbirGoContext db)
        {
            this.db = db;
        }


        /*[HttpPost("/SignIn")]
        public IActionResult SignIn(string username, string password)
        {

        }*/
    }
}
