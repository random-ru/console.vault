using System;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using vault;
using vault.commons;
using vault.controllers;
using vault.exceptions;

var builder = WebApplication.CreateBuilder(args)
    .WithWebHost(x =>
        x.UseUrls($"http://*.*.*.*:{Environment.GetEnvironmentVariable("PORT") ?? "8080"}"));

builder.Services
    .AddWebApi()
    .AddScoped<IStoreAdapter, FireStoreAdapter>()
    .AddSwaggerGen(c =>
        c.SwaggerDoc("v1", new() { Title = "Console Vault", Version = "v1" }))
    .AddRouting(options => options.LowercaseUrls = true);

var app = builder.Build();

app.UseMiddleware<CloudflareMiddleware>();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI(c => 
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Console Vault v1"));

app.UseExceptionHandler(x => x.Run(context =>
{
    var exception = context.Features
        .Get<IExceptionHandlerPathFeature>()?
        .Error;

    context.Response.StatusCode = 200;

    if (exception is VaultException s1)
        return context.Response.WriteAsJsonAsync(new { message = s1.Message });

    context.Response.StatusCode = 500;
    return context.Response.WriteAsJsonAsync(new { });
}));

app.Run();
