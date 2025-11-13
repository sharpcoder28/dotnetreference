using DotnetReference.Api.Data;
using DotnetReference.Api.Endpoints;
using DotnetReference.Api.Queries;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IOrderQueries, OrderQueries>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapOrderEndpoints();

app.Run();