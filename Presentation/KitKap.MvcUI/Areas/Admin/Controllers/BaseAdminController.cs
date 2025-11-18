using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KitKap.MvcUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class BaseAdminController : Controller
    {
        protected void SetSuccessMessage(string message)
        {
            TempData["Success"] = message;
        }

        protected void SetErrorMessage(string message)
        {
            TempData["Error"] = message;
        }

        protected void SetWarningMessage(string message)
        {
            TempData["Warning"] = message;
        }

        protected void SetInfoMessage(string message)
        {
            TempData["Info"] = message;
        }
    }
}