using ECommerce.Service.Exceptions;

namespace ECommerceWeb.CustomeMiddleWare
{
    public class ExceptionHandlerMiddleWare                                         //:IMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleWare> _logger;

        public ExceptionHandlerMiddleWare(RequestDelegate next,ILogger<ExceptionHandlerMiddleWare> logger)
        {
           _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
                await HandelNotFoundEndPointAsync(context);
            }
            catch (Exception ex)
            {
                // Logging 
                _logger.LogError(ex,"Something Went Wrong");
               
                //return custom error response
                var problemDetails = new ProblemDetails
                {
                    Status =ex switch
                    {
                        NotFoundException => StatusCodes.Status404NotFound,
                        _=> StatusCodes.Status500InternalServerError,
                    } ,
                    Title = "Error While Processing http request",
                    Detail = ex.Message,
                    Instance = context.Request.Path
                };
                context.Response.StatusCode = problemDetails.Status.Value;
                await context.Response.WriteAsJsonAsync(problemDetails);
            }
        }

        private static async Task HandelNotFoundEndPointAsync(HttpContext context)
        {
            if (context.Response.StatusCode == StatusCodes.Status404NotFound && !context.Response.HasStarted)
            {
                // Log 404 errors(EndPoint Not Found)
                var response = new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Error While Processing Http request  -EndPoint Not Found",
                    Detail = $"The requested resource(EndPoint) '{context.Request.Path}' was not found on the server.",
                    Instance = context.Request.Path
                };
                await context.Response.WriteAsJsonAsync(response);

            }


        }
    }
}
