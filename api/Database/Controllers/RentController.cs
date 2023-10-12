using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.Database.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentController : ControllerBase
    {
        SimbirGoContext db;
        public RentController(SimbirGoContext db) 
        {
            this.db = db;
        }

        



    }
}
