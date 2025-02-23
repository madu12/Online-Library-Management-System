using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.AspNetCore.Mvc;

namespace online_library_management_system.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class ErrorController : Controller
    {
        [Route("Admin/404")]
        public IActionResult PageNotFound()
        {
            return View();
        }
    }

}
