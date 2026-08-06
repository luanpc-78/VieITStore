using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using VieITStore.Models.Entities;

namespace VieITStore.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class AuthorizeRoleAttribute : ActionFilterAttribute
{
    private readonly VaiTro[] _allowedRoles;

    public AuthorizeRoleAttribute(params VaiTro[] roles) => _allowedRoles = roles;

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var session = context.HttpContext.Session;
        var userId = session.GetInt32("UserId");
        var roleValue = session.GetString("VaiTro");

        if (!userId.HasValue || !Enum.TryParse<VaiTro>(roleValue, out var role))
        {
            context.Result = new RedirectToActionResult("DangNhap", "TaiKhoan", new { area = "" });
            return;
        }

        if (!_allowedRoles.Contains(role))
        {
            context.Result = new RedirectToActionResult("AccessDenied", "Home", new { area = "" });
            return;
        }

        base.OnActionExecuting(context);
    }
}
