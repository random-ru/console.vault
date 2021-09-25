using System.Linq;
using System.Threading.Tasks;
using console.vault.commons;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace vault.controllers;


[ApiController]
[SwaggerResponse(403, "Not Authorized", typeof(NotAuthorizedDto))]
[SwaggerResponse(500, type: typeof(InternalErrorDto))]
public class FieldsController : ControllerBase
{
    [CloudflareAuthorize]
    [HttpGet("/@/spaces/{space}/apps/{app}/fields")]
    [SwaggerResponse(200, type: typeof(string[]))]
    [SwaggerResponse(404, "Space or Application not found.")]
    public async ValueTask<IActionResult> GetFields(string space, string app)
    {
        var snapshot = await _store.Namespaces
            .Document(space)
            .Collection(app)
            .Document("values")
            .GetSnapshotAsync();

        if (!snapshot.Exists)
            return NotFound();
        return snapshot
            .ToDictionary()
            .Select(x => x.Key)
            .WithOk();
    }

    [CloudflareAuthorize]
    [HttpPost("/@/spaces/{space}/apps/{app}/fields/{field}")]
    [SwaggerResponse(200, type: typeof(SuccessActionDto))]
    [SwaggerResponse(404, "Space or Application not found.")]
    [SwaggerResponse(208, "Already exist.")]
    public async ValueTask<IActionResult> CreateField(string space, string app, string field, [FromBody] object data)
    {
        var document = _store.Namespaces
            .Document(space)
            .Collection(app)
            .Document("values");
        var snapshot = await document
            .GetSnapshotAsync();

        if (!snapshot.Exists)
            return NotFound();

        if (snapshot.ContainsField(field))
            return StatusCode(404);

        return await document.UpdateAsync(field, data).WithAsyncOk();
    }

    [CloudflareAuthorize]
    [HttpDelete("/@/spaces/{space}/apps/{app}/fields/{field}")]
    [SwaggerResponse(200, type: typeof(SuccessActionDto))]
    public IActionResult DeleteField(string space, string app, string field) 
        => Ok(new { message = "ok" });

    [CloudflareAuthorize]
    [HttpPut("/@/spaces/{space}/apps/{app}/fields/{field}")]
    [SwaggerResponse(200, type: typeof(SuccessActionDto))]
    [SwaggerResponse(404, "Space, Application or Field not found.")]
    public async ValueTask<IActionResult> UpdateField(string space, string app, string field, [FromBody] object data)
    {
        var document = _store.Namespaces
            .Document(space)
            .Collection(app)
            .Document("values");
        var snapshot = await document
            .GetSnapshotAsync();

        if (!snapshot.Exists)
            return NotFound();

        if (!snapshot.ContainsField(field))
            return StatusCode(404);

        return await document.UpdateAsync(field, data).WithAsyncOk();
    }

    #region ctor

    private readonly IStoreAdapter _store;

    public FieldsController(IStoreAdapter store) 
        => _store = store;

    #endregion
}

