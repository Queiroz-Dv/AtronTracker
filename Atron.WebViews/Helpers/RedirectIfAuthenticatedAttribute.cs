using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Shared.Extensions;

namespace Atron.WebViews.Helpers
{
    /// <summary>
    /// This attribute checks if the user is authenticated by looking for an authentication token
    /// in the session or cookies. If the token is found, the user is redirected to the Home page.
    /// </summary>
    public class RedirectIfAuthenticatedAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Called before the action method is executed.
        /// </summary>
        /// <param name="context">The context for the action.</param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Retrieve the session from the HTTP context
            var session = context.HttpContext.Session;

            // Try to get the authentication token from the session or cookies
            var token = session.GetString("AuthToken") ?? context.HttpContext.Request.Cookies["AuthToken"];

            // If the token is not null or empty, redirect to the Home page
            if (!token.IsNullOrEmpty())
            {
                context.Result = new RedirectToActionResult("Index", "Home", null);
            }

            // Call the base method to continue the action execution pipeline
            base.OnActionExecuting(context);
        }
    }
}