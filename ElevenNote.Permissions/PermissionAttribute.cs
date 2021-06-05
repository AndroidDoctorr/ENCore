using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ElevenNote.Permissions
{
    public class PermissionAttribute : ActionFilterAttribute, IActionFilter//, IAuthorizationFilter
    {
        private IPermissionService _service;
        public PermissionAttribute(IPermissionService service)
        {
            _service = service;
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ControllerActionDescriptor actionDescriptor = ((ControllerBase)filterContext.Controller).ControllerContext.ActionDescriptor;
            string controllerName = actionDescriptor.ControllerName;
            string actionName = actionDescriptor.ActionName.ToUpper();
            string roleName = ((ClaimsIdentity)filterContext.HttpContext.User.Identity).FindFirst(ClaimTypes.Role)?.Value;

            Action action;
            switch (actionName)
            {
                case "POST":
                    action = Action.Create;
                    break;
                case "PUT":
                    action = Action.Edit;
                    break;
                case "DELETE":
                    action = Action.Remove;
                    break;
                default:
                    action = Action.Get;
                    break;
            }

            Role role;
            switch (roleName)
            {
                case "SuperAdmin":
                    role = Role.SuperAdmin;
                    break;
                case "Admin":
                    role = Role.Admin;
                    break;
                case "Instructor":
                    role = Role.Instructor;
                    break;
                case "LA":
                    role = Role.LA;
                    break;
                default:
                    role = Role.Student;
                    break;
            }

            int accessLevel = _service.GetAccessLevel(controllerName, action, role).Result;

            filterContext.ActionArguments["access"] = accessLevel;

            if (accessLevel == 0)
                filterContext.Result = new UnauthorizedObjectResult("This action is not authorized");

            base.OnActionExecuting(filterContext);
        }
    }
}