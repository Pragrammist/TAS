using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TreatyAutomateSystem.Filters;

public class CustomExceptionFilterAttribute : Attribute, IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        string exceptionMessage = context.Exception.Message;
        new BadRequestResult();
        context.Result = new ContentResult{
            Content = exceptionMessage,
            StatusCode = 400
        };

        context.ExceptionHandled = true;
    }
}