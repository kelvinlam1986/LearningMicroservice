using Contracts.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Shared.SeedWork;
using System.Net;
using System.Text.Json;

namespace Ordering.Application.Common.Middleware
{
    public class ExceptionMiddleware : IMiddleware
    {
		private readonly ISerializeService _jsonSerializer;

		public ExceptionMiddleware(ISerializeService serializeService)
		{
			_jsonSerializer = serializeService;
		}
	
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
			try
			{
				await next(context);
			}
			catch (Exception ex)
			{

                var response = context.Response;
                switch (ex)
				{
					case Common.Exceptions.ValidationException:
                        var validationException = ex as Common.Exceptions.ValidationException;
                        List<string> errorMessages = new List<string>();
                        foreach (var error in validationException.Errors)
                        {
                            errorMessages.Add($"{error.Key}: {string.Join(",", error.Value)}");
                        }

                        var apiErrorResult = new ApiErrorResult<Exception>(errorMessages);

                        response.ContentType = "application/json";
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        await response.WriteAsync(_jsonSerializer.Serialize(apiErrorResult));
                        break;

                    default:
                        var apiInternalErrorResult = new ApiErrorResult<Exception>(ex.Message);
                        response.ContentType = "application/json";
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        await response.WriteAsync(_jsonSerializer.Serialize(apiInternalErrorResult));
                        break;
                }
			}
        }
    }
}
