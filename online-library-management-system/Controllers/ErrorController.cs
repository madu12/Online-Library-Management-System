using Microsoft.AspNetCore.Mvc;

namespace online_library_management_system.Controllers
{
    public class ErrorController : Controller
    {
        [Route("404")]
        public IActionResult PageNotFound()
        {
            return View();
        }
    }
}
