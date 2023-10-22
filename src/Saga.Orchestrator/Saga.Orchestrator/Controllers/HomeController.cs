using Microsoft.AspNetCore.Mvc;

namespace Saga.Orchestrator.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return Redirect("~/swagger");
        }
    }
}
