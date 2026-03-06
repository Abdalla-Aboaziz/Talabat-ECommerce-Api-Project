using ECommerce.ServiceAbstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Presentation.Attributes
{
    public class RedisCasheAttribute : ActionFilterAttribute
    {
        private readonly int _durationInMin;
        public RedisCasheAttribute(int DurationInMin=5)
        {
            _durationInMin = DurationInMin;
        }

        // This method runs before and after the action method execution.
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next) //context=> Take the context of the request and response and next => delegate to excute the controller action
        {

            // Get Cach Service from Di Container
            var cachService = context.HttpContext.RequestServices.GetRequiredService<ICashService>();
            // Create Cach Key based on Request Path and Query String
            var cachKey = CreateCachKey(context.HttpContext.Request);

            // check if data in cach or not 
            var CachValue = await cachService.GetAsync(cachKey);
            if (CachValue != null)
            {
                // if data in cach return data and skip the controller
                var contentResult = new ContentResult
                {
                    Content = CachValue,
                    ContentType = "application/json",
                    StatusCode = StatusCodes.Status200OK
                };
                context.Result = contentResult;
                return;
            }

            // If data is not in cache, execute the action and cache the result if it's 200 OK

            var executedContext = await next.Invoke(); // excute the controller
            if(executedContext.Result is OkObjectResult okObjectResult)
            {
                // store data in cach
                await cachService.SetAsync(cachKey, okObjectResult.Value!, TimeSpan.FromMinutes(_durationInMin));
            }

        }




        #region Helper
        private string CreateCachKey(HttpRequest request)
        {
            var keyBuilder = new StringBuilder();
            keyBuilder.Append($"{request.Path}"); //api/Products
            foreach (var (key, value) in request.Query.OrderBy(x => x.Key))
            {
                keyBuilder.Append($"|{key}-{value}"); // api/Products|page-1|size-10
            }
            return keyBuilder.ToString();

        }

        #endregion

    }
} 
