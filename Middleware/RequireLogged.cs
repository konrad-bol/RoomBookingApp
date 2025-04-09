using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RoombookingApp.Middleware
{
    public class RequireLoggedInAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userId = context.HttpContext.Session.GetInt32("user_id");

            if (userId == null)
            {
                context.Result = new UnauthorizedObjectResult(new { error = "Musisz być zalogowany, aby wykonać tę funkcję." });
            }

            base.OnActionExecuting(context);
        }     
    }
    public class RequireLoggedOutAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userId = context.HttpContext.Session.GetInt32("user_id");
            if(userId !=null)
            {
                context.Result = new BadRequestObjectResult(new { error = "Jesteś już zalogowany." });
            }
            base.OnActionExecuting(context);
        }
    }
}
