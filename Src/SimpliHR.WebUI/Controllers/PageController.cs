using Microsoft.AspNetCore.Mvc;

namespace SimpliHR.WebUI.Controllers
{
    public class PageController : Controller
    {
        public IActionResult Error()
        {
            return View();
        }
    }
}
