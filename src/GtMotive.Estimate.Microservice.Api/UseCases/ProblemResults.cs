using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GtMotive.Estimate.Microservice.Api.UseCases
{
    /// <summary>
    /// Builds RFC 7807 error responses so every error has the same shape as the ones produced by BusinessExceptionFilter.
    /// </summary>
    internal static class ProblemResults
    {
        public static IActionResult NotFound(string detail)
        {
            return new NotFoundObjectResult(Create(StatusCodes.Status404NotFound, "Not Found", detail));
        }

        public static IActionResult Conflict(string detail)
        {
            return new ConflictObjectResult(Create(StatusCodes.Status409Conflict, "Conflict", detail));
        }

        private static ProblemDetails Create(int status, string title, string detail)
        {
            return new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = detail,
            };
        }
    }
}
