using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Customer.API.Controllers
{
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Redirect("~/swagger");
        }
    }
}
