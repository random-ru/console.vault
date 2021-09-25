using System;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using vault.commons;
using vault.exceptions;

namespace vault.controllers;

public static class WebApiExtensions
{
    public static IActionResult WithOk<T>(this T t) => new OkObjectResult(t);

    public static async ValueTask<IActionResult> WithAsyncOk<T>(this ValueTask<T> t) 
        => new OkObjectResult(await t);
    public static async Task<IActionResult> WithAsyncOk<T>(this Task<T> t) 
        => new OkObjectResult(await t);

    public static DocumentReference ValidateSpaceExist(this DocumentReference @ref)
    {
        var snapshot = @ref.GetSnapshotAsync().Result;

        if (!snapshot.Exists)
            throw new SpaceNotFoundException(@ref.Id);
        return @ref;
    }


    public static IServiceCollection AddWebApi(this IServiceCollection service)
    {
        service.AddControllers().AddNewtonsoftJson(x =>
        {
            x.SerializerSettings.ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            };
            x.SerializerSettings.Converters.Add(new TimestampJsonConverter());
            x.SerializerSettings.Converters.Add(new TimestampNullableJsonConverter());
            x.SerializerSettings.Converters.Add(new DocumentReferenceJsonConverter());
        });
        return service;
    }

    public static WebApplicationBuilder WithWebHost(this WebApplicationBuilder builder, Action<ConfigureWebHostBuilder> webHost)
    {
        webHost(builder.WebHost);
        return builder;
    }
}