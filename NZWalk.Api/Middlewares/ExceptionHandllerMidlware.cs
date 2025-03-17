using System.Net;    //Contains HttpStatusCode, used to set HTTP response codes.

namespace NZWalk.Api.Middlewares
{
    public class ExceptionHandllerMidlware
    {
        private readonly ILogger<ExceptionHandllerMidlware> logger;
        private readonly RequestDelegate next;  //Represents the next middleware in the request pipeline.

        public ExceptionHandllerMidlware(ILogger<ExceptionHandllerMidlware> logger,RequestDelegate next )
        {
            this.logger = logger;
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await next(httpContext);   //to pass the request to the next middleware.

            }
            catch (Exception ex)
            {

                var errorId= Guid.NewGuid();
                // logg this exception

                logger.LogError(ex, $"{errorId} : {ex.Message}");

                // return a custom error response

                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                httpContext.Response.ContentType = "application/json";

                var error = new
                {
                    Id = errorId,
                    ErrorMessage = "Somthing went  wrong! We are looking into resovlving this."
                };

               await httpContext.Response.WriteAsJsonAsync( error );
            }
        }
    }
}
