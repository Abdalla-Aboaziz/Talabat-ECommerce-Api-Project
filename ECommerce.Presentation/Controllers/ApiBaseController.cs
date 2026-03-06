using ECommerce.Shared.CommonResult;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApiBaseController : ControllerBase
    {
        // HandelResult Without Value 
        // If Result is Success => return No Content (204)
        // If Result is Failure => return Problem With Status Code and Errors Details

        protected IActionResult HandelResult(Result result)
        {
            if (result.IsSuccess)
            {
                return NoContent(); // 204 No Content
            }
            else
            {
                return HandelProblem(result.Errors); // Handle the errors and return appropriate response

            }
        }
        // Handel Result With Value
        protected ActionResult<Tvalue> HandelResult<Tvalue>(Result<Tvalue> result)
        {
            if (result.IsSuccess)
            {
                return Ok(result.Value); // 200 OK with the value
            }
            else
            {
                return HandelProblem(result.Errors); // Handle the errors and return appropriate response
            }

        }


        // Handel Problems
        private ActionResult HandelProblem(IReadOnlyList<Error> errors)
        {
            // if No Errors Are Provided => return 500 Internal Server Error
            if (errors.Count == 0) return Problem(statusCode: StatusCodes.Status500InternalServerError, title: "Un Expected Error");

           

            // if all errors are validation errors => handel as validation problem
            if (errors.All(e => e.Type == ErrorType.Validation)) return HandelValidationProblems(errors);
            // if only one errot => handel as single error problem
            return HandelSingleError(errors[0]);




        }

        // Handel Single Error
        private ActionResult HandelSingleError(Error error)
        {
            return Problem
            (
                title: error.Code,
                detail: error.Description,
                type: error.Type.ToString(),
                statusCode: MapErrorTypeStatusCode(error.Type)


            );
        }

        // Map ErrorType to corresponding HTTP status code
        private static int MapErrorTypeStatusCode(ErrorType errorType) => errorType switch
        {
            ErrorType.Failure => StatusCodes.Status500InternalServerError,
            ErrorType.InvalidCredentials => StatusCodes.Status401Unauthorized,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        };

        // Handel Validation Problems
        private ActionResult HandelValidationProblems(IReadOnlyList<Error> errors)
        {
            var modelState = new ModelStateDictionary();
            foreach (var error in errors)
            {
                modelState.AddModelError(error.Code, error.Description);
            }
            return ValidationProblem(modelState);

        }


    }
}
