using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using console.vault.commons;
using Flurl.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace vault.commons;
public class CloudflareMiddleware
{
    // Cf-Access-Jwt-Assertion
    private static string BASE = "https://random-ru.cloudflareaccess.com";
    private readonly RequestDelegate _next;
    private ILogger logger;
    public CloudflareMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
    {
        this._next = next;
        this.logger = loggerFactory.CreateLogger(nameof(CloudflareMiddleware));
    }

    private static List<SecurityKey> Keys = new ();

    public async Task InvokeAsync(HttpContext context)
    {
        #region DEBUG
        foreach (var header in context.Request.Headers)
            logger.LogInformation($"HEADER: '{header.Key}' - '{header.Value}'");
        foreach (var header in context.Request.Cookies)
            logger.LogInformation($"COOKIES: '{header.Key}' - '{header.Value}'");
        #endregion
        
        #region DEBUG

        if ($"{context.Request.Headers.Referer}".Contains("swagger"))
        {
            await _next(context);
            return;
        }
        #endregion

        var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
        var attribute = endpoint?.Metadata.GetMetadata<CloudflareAuthorizeAttribute>();

        if (attribute is null)
        {
            await _next(context);
            return;
        }


        if (!context.Request.Headers.ContainsKey("Cf-Access-Jwt-Assertion"))
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsJsonAsync(new { message = "Not Authorized", details = "no token." });
            logger.LogError($"No 'Cf-Access-Jwt-Assertion' found.");
            return;
        }

        var validateParams = await GetValidationParametersAsync();
        

        var isValid = ValidateToken(context.Request.Headers["Cf-Access-Jwt-Assertion"], 
            validateParams, out var msg, out var token);

        if (isValid && token is not null)
        {
            context.User = token;
            await _next(context);
            return;
        }
        context.Response.StatusCode = 403;
        await context.Response.WriteAsJsonAsync(new { message = "Not Authorized", details = msg });
    }

    private bool ValidateToken(string authToken, TokenValidationParameters @params, out string message, out ClaimsPrincipal? token)
    {
        token = null;
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
        
            var principal = tokenHandler.ValidateToken(authToken, @params, out _);
            message = "ok";
            if (principal.Identity is null)
                return false;
            token = principal;
            return principal.Identity.IsAuthenticated;
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "Failed validate JWT token.");
            message = e.Message;
            token = null;
            return false;
        }
    }


    private static async Task<TokenValidationParameters> GetValidationParametersAsync() => new ()
    {
        ValidateLifetime = true, 
        ValidateAudience = false,
        ValidateIssuer = false,
        ValidIssuer = BASE,
        ValidAudience = "dd5304dd2157284038b2a3cfe33c7d241923cb09e87ec53928eb6cffd853e851",
        IssuerSigningKeys = await GetKeys()
    };

    private static async Task<SecurityKey[]> GetKeys()
    {
        if (Keys.Any())
            return Keys.ToArray();
        var result = await $"{BASE}/cdn-cgi/access/certs"
            .GetJsonAsync<ExpectedJwksResponse>();
        Keys.AddRange(result.Keys);
        return Keys.ToArray();
    }
    public class ExpectedJwksResponse
    {
        [JsonProperty(PropertyName = "keys")]
        public List<JsonWebKey> Keys { get; set; }
    }
}