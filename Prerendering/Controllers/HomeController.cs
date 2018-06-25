using Microsoft.AspNetCore.Mvc;

namespace prerendering.Controllers
{
    public class HomeController: Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}