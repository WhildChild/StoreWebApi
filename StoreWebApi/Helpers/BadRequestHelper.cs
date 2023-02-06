using Microsoft.AspNetCore.Mvc;

namespace StoreWebApi.Helpers
{
    public static class BadRequestHelper
    {
        public static BadRequestObjectResult GetBadRequest(this ControllerBase controller, string errorReason)
        {
            return controller.BadRequest(new
            {
                ErrorReason = errorReason,
                ErrorMessage = new String(controller.ModelState.Values.SelectMany(x => x.Errors.SelectMany(y => y.ErrorMessage)).ToArray())
            });
        }
    }
}
