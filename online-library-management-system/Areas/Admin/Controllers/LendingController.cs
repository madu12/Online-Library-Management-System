using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.AspNetCore.Mvc;

namespace online_library_management_system.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class LendingController : Controller
    {
        public ActionResult Issue()
        {
            return View();
        }

        public ActionResult Return()
        {
            return View();
        }
    }
}
