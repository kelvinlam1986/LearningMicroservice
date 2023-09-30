using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Product.API.Controllers
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
