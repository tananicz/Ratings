using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Ratings.Filters
{
    public class RoleAdderFilter : Attribute, IResultFilter
    {
        public void OnResultExecuted(ResultExecutedContext context)
        {
            //do nothing
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is ViewResult result)
            { 
                if (context.HttpContext.User.IsInRole("admin"))
                {
                    result.ViewData["userRoleIndicator"] = 3;
                }
                else if (context.HttpContext.User.IsInRole("moderator"))
                {
                    result.ViewData["userRoleIndicator"] = 2;
                }
                else if (context.HttpContext.User.IsInRole("user"))
                {
                    result.ViewData["userRoleIndicator"] = 1;
                }
                else
                {
                    result.ViewData["userRoleIndicator"] = 0;
                }
            }
        }
    }
}
