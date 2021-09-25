using System.Threading.Tasks;
using console.vault.commons;
using Flurl.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace vault.controllers;

[ApiController]
[SwaggerResponse(403, "Not Authorized", typeof(NotAuthorizedDto))]
[SwaggerResponse(500, type: typeof(InternalErrorDto))]
public class IdentityController : ControllerBase
{
    private static string BASE = "https://random-ru.cloudflareaccess.com";

    [CloudflareAuthorize]
    [HttpGet("/@/users/{id}")]
    [SwaggerResponse(200, "Get identity by user id. For getting current user, use '@me'", 
        typeof(CloudflareIdentityDto))]
    public async ValueTask<IActionResult> GetIdentityAsync(string id)
    {
        if (id == "@me")
            return Ok(await GetMeAsync());
        return NotFound();
    }

    private Task<dynamic> GetMeAsync()
    {
        return $"{BASE}/cdn-cgi/access/get-identity"
            .WithHeader("Cf-Access-Authenticated-User-Email", this.Request.Headers["Cf-Access-Authenticated-User-Email"])
            .WithHeader("Cf-Access-Jwt-Assertion", this.Request.Headers["Cf-Access-Jwt-Assertion"])
            .WithHeader("CF-Access-Domain", BASE)
            .WithHeader("CF-Access-Aud", "dd5304dd2157284038b2a3cfe33c7d241923cb09e87ec53928eb6cffd853e851")
            .GetJsonAsync();
    }
}