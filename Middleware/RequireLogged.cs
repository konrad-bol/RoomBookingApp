using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

// Filtr do ustalania czy uzytkownik ma dostep do funkcji
// sluzy do zabezpieczenie uzytkownika przed uzyciem metod do ktorych
// nie ma dostepu bo jest zalogowany lub nie
namespace RoombookingApp.Middleware
{
    public class RequireLoggedInAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            
            var userId = context.HttpContext.Session.GetInt32("user_id");
            //jezeli w sesji jest id uzytkownika to oznacza ze jest zalogowany
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
            //jezeli w sesji jest id uzytkownika to oznacza ze jest zalogowany
            var userId = context.HttpContext.Session.GetInt32("user_id");
            if(userId !=null)
            {
                context.Result = new BadRequestObjectResult(new { error = "Jesteś już zalogowany." });
            }
            base.OnActionExecuting(context);
        }
    }
}
