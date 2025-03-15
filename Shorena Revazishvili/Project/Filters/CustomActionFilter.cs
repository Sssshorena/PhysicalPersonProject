using Microsoft.AspNetCore.Mvc.Filters;

namespace Project.Filters
{
    public class CustomActionFilter : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                throw new ArgumentException("Invalid model state");
            }

            await next();
        }
    }
}
