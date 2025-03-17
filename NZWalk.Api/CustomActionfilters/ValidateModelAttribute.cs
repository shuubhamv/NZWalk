using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters; //povides base classes for action filters like ActionFilterAttribute.


namespace NZWalk.Api.CustomActionfilters
{
    //Inherits from ActionFilterAttribute, making it a custom action filter.
   // Action filters allow execution of code before or after an action method runs.
    public class ValidateModelAttribute:ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid== false)
            {
                context.Result = new BadRequestResult();
            }
           
        }
    }
}
