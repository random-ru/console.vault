using System;
using console.vault.commons;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.Annotations;

namespace vault.controllers;

[ApiController]
[SwaggerResponse(403, "Not Authorized", typeof(NotAuthorizedDto))]
[SwaggerResponse(500, type: typeof(InternalErrorDto))]
public class SystemController : ControllerBase
{
    private readonly IConfiguration _config;

    public SystemController(IConfiguration config) 
        => _config = config;
    
    [SwaggerResponse(418, "418 - health is ok, otherwise service is down.", typeof(ClearedStatusDto))]
    [HttpGet("/sys/health")]
    public IActionResult Health() 
        => StatusCode(418);


    [CloudflareAuthorize]
    [HttpGet("/sys/env/{key}")]
    public IActionResult GetEnvByKey(string key) 
        => Ok(Environment.GetEnvironmentVariable(key));

    [CloudflareAuthorize]
    [HttpGet("/sys/conf/{key}")]
    public IActionResult GetConfigByKey(string key)
        => Ok(_config[key]);
}