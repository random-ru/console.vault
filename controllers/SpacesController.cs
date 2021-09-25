using System.Linq;
using System.Threading.Tasks;
using console.vault.commons;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using vault.commons;

namespace vault.controllers;



[ApiController]
[SwaggerResponse(403, "Not Authorized", typeof(NotAuthorizedDto))]
[SwaggerResponse(500, type: typeof(InternalErrorDto))]
public class SpacesController : ControllerBase
{
    [SwaggerResponse(200, "The Space", typeof(SpaceDto[]))]
    [CloudflareAuthorize]
    [HttpGet("/@/spaces")]
    public async ValueTask<IActionResult> GetSpaces()
        => await _store
            .Namespaces
            .ListDocumentsAsync()
            .SelectAwait(async x => await x.GetSnapshotAsync())
            .Select(x => new
            {
                x.CreateTime, 
                x.Id, 
                x.UpdateTime, 
                Description = x.FetchValue<string>("description")
            })
            .ToArrayAsync()
            .WithAsyncOk();

    [CloudflareAuthorize]
    [HttpPost("/@/spaces/{space}")]
    [SwaggerResponse(200, type: typeof(SuccessActionDto))]
    public async ValueTask<IActionResult> CreateSpaces(string space) 
        => await _store
            .Namespaces
            .Document(space)
            .CreateAsync(Scaffold.EmptySpace)
            .WithAsyncOk();

    [CloudflareAuthorize]
    [HttpDelete("/@/spaces/{space}")]
    [SwaggerResponse(200, type: typeof(SuccessActionDto))]
    public async ValueTask<IActionResult> DeleteSpaces(string space)
        => await _store
            .Namespaces
            .Document(space)
            .DeleteAsync()
            .WithAsyncOk();


    #region ctor

    private readonly IStoreAdapter _store;

    public SpacesController(IStoreAdapter store) => _store = store;

    #endregion
}