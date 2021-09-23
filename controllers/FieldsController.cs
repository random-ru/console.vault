using Microsoft.AspNetCore.Mvc;

namespace vault.controllers;


[ApiController]
public class FieldsController : ControllerBase
{
    [HttpGet("/@/spaces/{space}/apps/{app}/fields")]
    public IActionResult GetFields(string space, string app) 
        => Ok(new [] {"app1", "app2"});
    [HttpPost("/@/spaces/{space}/apps/{app}/fields/{field}")]
    public IActionResult CreateField(string space, string app, string field) 
        => Ok(new { message = $"created '{field}' in '{app}' app in '{space}' space."});
    [HttpDelete("/@/spaces/{space}/apps/{app}/fields/{field}")]
    public IActionResult DeleteField(string space, string app, string field) 
        => Ok(new { message = $"deleted '{field}' in '{app}' app in '{space}' space."});
    [HttpPut("/@/spaces/{space}/apps/{app}/fields/{field}")]
    public IActionResult UpdateField(string space, string app, string field, [FromBody] object data) 
        => Ok(new { message = $"updated '{field}' in '{app}' app in '{space}' space."});
}

