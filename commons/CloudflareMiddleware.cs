using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Flurl.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace vault.commons;
public class CloudflareMiddleware
{
    // Cf-Access-Jwt-Assertion
    private static string BASE = "https://random-ru.cloudflareaccess.com";
    private readonly RequestDelegate _next;
    public CloudflareMiddleware(RequestDelegate next) 
        => this._next = next;

    private static List<SecurityKey> Keys = new ();

    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.Headers["Server"] = "Yori/1.64.226 (cluster)";

        if (Environment.GetEnvironmentVariable("NO_AUTH") is not null)
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.ContainsKey("Cf-Access-Jwt-Assertion"))
        {
            context.Response.Redirect(BASE);
            await _next(context);
        }

        var isAuthed = await ValidateToken(context.Request.Headers["Cf-Access-Jwt-Assertion"]);

        if (isAuthed)
        {
            await _next(context);
            return;
        }
        
        context.Response.Redirect(BASE);
        await _next(context);
    }

    private static async Task<bool> ValidateToken(string authToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = await GetValidationParameters();
        
        var principal = tokenHandler.ValidateToken(authToken, validationParameters, out var validatedToken);

        if (principal.Identity is null)
            return false;
        
        return principal.Identity.IsAuthenticated;
    }


    private static async Task<TokenValidationParameters> GetValidationParameters() => new ()
    {
        ValidateLifetime = true, 
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidIssuer = BASE,
        ValidAudience = "dd5304dd2157284038b2a3cfe33c7d241923cb09e87ec53928eb6cffd853e851",
        IssuerSigningKeys = await GetKeys()
    };

    private static async Task<SecurityKey[]> GetKeys()
    {
        if (Keys.Any())
            return Keys.ToArray();
        var result = await $"{BASE}/cdn-cgi/access/certs".GetJsonAsync<ExpectedJwksResponse>();
        Keys.AddRange(result.Keys);
        return Keys.ToArray();
    }

    public class ExpectedJwksResponse
    {
        [JsonProperty(PropertyName = "keys")]
        public List<JsonWebKey> Keys { get; set; }
    }
}