using NLog;
using System.Web.Http.Filters;
using System.Net.Http;
using System.Net;

public class GlobalExceptionFilter : ExceptionFilterAttribute
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
   
    public override void OnException(HttpActionExecutedContext context)
    {
        _logger.Error(context.Exception, $"Unhandled exception occurred: {context.Exception.Message}");

        _logger.Error($"Stack Trace: {context.Exception.StackTrace}");

        var errorMessage = context.Exception.Message ?? context.Exception.InnerException.Message;
        context.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent($"An error occurred: {errorMessage}"),
            ReasonPhrase = "Internal Server Error"
        };
    }

}
