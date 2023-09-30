using Microsoft.AspNetCore.Mvc;

namespace Basket.API.Controllers
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
