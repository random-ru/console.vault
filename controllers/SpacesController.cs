using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using vault.commons;

namespace vault.controllers;


[ApiController]
public class SpacesController : ControllerBase
{
    private readonly IStoreAdapter _store;

    public SpacesController(IStoreAdapter store) => _store = store;

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
                description = x.FetchValue<string>("description")
            })
            .ToArrayAsync()
            .WithAsyncOk();

    [HttpPost("/@/spaces/{space}")]
    public async ValueTask<IActionResult> CreateSpaces(string space) 
        => await _store
            .Namespaces
            .Document(space)
            .CreateAsync(Scaffold.EmptySpace)
            .WithAsyncOk();

    [HttpDelete("/@/spaces/{space}")]
    public async ValueTask<IActionResult> DeleteSpaces(string space)
        => await _store
            .Namespaces
            .Document(space)
            .DeleteAsync()
            .WithAsyncOk();
}