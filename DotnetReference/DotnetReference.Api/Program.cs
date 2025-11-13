using DotnetReference.Api.Data;
using DotnetReference.Api.Endpoints;
using DotnetReference.Api.Queries;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IOrderQueries, OrderQueries>();

// Add Problem Details support
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.

// Add global exception handler
// I'm iffy on this, there is probably a more elegant way to do it
if (app.Environment.IsDevelopment())
{
    // Show stack traces in development
    app.UseExceptionHandler(exceptionHandlerApp =>
    {
        exceptionHandlerApp.Run(async context =>
        {
            var exceptionHandlerFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
            var exception = exceptionHandlerFeature?.Error;

            var problemDetails = new Microsoft.AspNetCore.Mvc.ProblemDetails
            {
                Title = "An error occurred while processing your request.",
                Status = StatusCodes.Status500InternalServerError,
                Detail = exception?.Message,
                Instance = context.Request.Path
            };

            if (exception != null)
            {
                problemDetails.Extensions["exception"] = exception.GetType().Name;
                problemDetails.Extensions["stackTrace"] = exception.StackTrace;
            }

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/problem+json";

            await context.Response.WriteAsJsonAsync(problemDetails);
        });
    });
}
else
{
    // No stack traces in production
    app.UseExceptionHandler(exceptionHandlerApp
        => exceptionHandlerApp.Run(async context
            => await Results.Problem()
                         .ExecuteAsync(context)));
}

app.MapOrderEndpoints();

app.Run();