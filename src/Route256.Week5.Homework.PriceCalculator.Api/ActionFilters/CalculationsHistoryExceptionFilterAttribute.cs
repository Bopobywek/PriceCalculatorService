using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Route256.Week5.Homework.PriceCalculator.Bll.Exceptions;

namespace Route256.Week5.Homework.PriceCalculator.Api.ActionFilters;

public class CalculationsHistoryExceptionFilterAttribute: Attribute, IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        switch (context.Exception)
        {
            case OneOrManyCalculationsNotFoundException:
                var result = new ContentResult
                {
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
                context.Result = result;
                return;
            case OneOrManyCalculationsBelongsToAnotherUserException:
                var jsonResult = new JsonResult(context.Exception.Data)
                {
                    StatusCode = (int)HttpStatusCode.Forbidden
                };
                context.Result = jsonResult;
                return;
        }
    }
}