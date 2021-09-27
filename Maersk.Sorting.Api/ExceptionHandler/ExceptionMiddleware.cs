﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Maersk.Sorting.Api.ExceptionHandler
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionMiddleware> logger;
        private readonly IHostEnvironment env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            this.next = next;
            this.logger = logger;
            this.env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = env.IsDevelopment() ? new ApiException((int)HttpStatusCode.InternalServerError, ex.Message, string.IsNullOrEmpty(ex.StackTrace) ? "" : ex.StackTrace.ToString()) :
                    new ApiResponse((int)HttpStatusCode.InternalServerError);

                var option = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };

                var json = JsonConvert.SerializeObject(response, option);
                await context.Response.WriteAsync(json);
            }
        }
    }
}
