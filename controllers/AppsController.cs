using System.Linq;
using System.Threading.Tasks;
using console.vault.commons;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace vault.controllers;

[ApiController]
[SwaggerResponse(403, "Not Authorized", typeof(NotAuthorizedDto))]
[SwaggerResponse(500, type: typeof(InternalErrorDto))]
public class AppsController : ControllerBase
{
    private readonly IStoreAdapter _store;

    public AppsController(IStoreAdapter store) 
        => _store = store;

    [CloudflareAuthorize]
    [HttpGet("/@/spaces/{space}/apps")]
    [SwaggerResponse(200, type: typeof(string[]))]
    [SwaggerResponse(404, "Space not found.")]
    public async ValueTask<IActionResult> GetApps(string space)
    {
        var document = _store
            .Namespaces
            .Document(space);

        var snapshot = await document.GetSnapshotAsync();

        if (!snapshot.Exists)
            return NotFound();
        
        return await document
            .ListCollectionsAsync()
            .Select(x => x.Id)
            .ToArrayAsync()
            .WithAsyncOk();
    }

    [CloudflareAuthorize]
    [HttpPost("/@/spaces/{space}/apps/{app}")]
    [SwaggerResponse(208, "Already exist.")]
    [SwaggerResponse(200, type: typeof(SuccessActionDto))]
    public async ValueTask<IActionResult> CreateApp(string space, string app)
    {
        var document = _store
            .Namespaces
            .Document(space);

        var snapshot = await document.GetSnapshotAsync();

        if (!snapshot.Exists)
            return NotFound();

        document = document.Collection(app).Document("values");

        snapshot = await document.GetSnapshotAsync();

        if (snapshot.Exists)
            return StatusCode(208);

        return await document
            .SetAsync(Scaffold.EmptyApp)
            .WithAsyncOk();
    }

    [CloudflareAuthorize]
    [HttpDelete("/@/spaces/{space}/apps/{app}")]
    [SwaggerResponse(200, type: typeof(SuccessActionDto))]
    public IActionResult DeleteApp(string space, string app) 
        => Ok(new { message = $"deleted '{app}' in '{space}' space."});
}

