using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace vault.controllers;

[ApiController]
public class SystemController : ControllerBase
{
    private readonly IConfiguration _config;

    public SystemController(IConfiguration config) 
        => _config = config;

    [HttpGet("/sys/health")]
    public IActionResult Health() 
        => StatusCode(418);


    [HttpGet("/sys/env/{key}")]
    public IActionResult GetEnvByKey(string key) 
        => Ok(Environment.GetEnvironmentVariable(key));

    [HttpGet("/sys/conf/{key}")]
    public IActionResult GetConfigByKey(string key)
        => Ok(_config[key]);
}