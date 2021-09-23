using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace vault.controllers;

[ApiController]
public class AppsController : ControllerBase
{
    private readonly IStoreAdapter _store;

    public AppsController(IStoreAdapter store) 
        => _store = store;


    [HttpGet("/@/spaces/{space}/apps")]
    public async ValueTask<IActionResult> GetApps(string space) 
        => await _store
            .Namespaces
            .Document(space)
            .ValidateSpaceExist()
            .ListCollectionsAsync()
            .Select(x => x.Id)
            .ToArrayAsync()
            .WithAsyncOk();

    [HttpPost("/@/spaces/{space}/apps/{app}")]
    public IActionResult CreateApp(string space, string app) 
        => Ok(new { message = $"created '{app}' in '{space}' space."});
    [HttpDelete("/@/spaces/{space}/apps/{app}")]
    public IActionResult DeleteApp(string space, string app) 
        => Ok(new { message = $"deleted '{app}' in '{space}' space."});
}

