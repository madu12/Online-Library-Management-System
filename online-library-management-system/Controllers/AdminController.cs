using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace online_library_management_system.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

    }
}
