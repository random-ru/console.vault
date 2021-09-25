using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace vault.controllers;


[ApiController]
[SwaggerResponse(403, "Not Authorized", typeof(NotAuthorizedDto))]
[SwaggerResponse(500, type: typeof(InternalErrorDto))]
public class AuditController : ControllerBase
{
    [HttpGet("@/audit")]
    [SwaggerResponse(200, "Not Authorized", typeof(List<AuditEntity>))]
    public IActionResult GetEntities() => Ok(new { });
}


public record AuditEntity
{
    public string ID { get; set; }
    public string UserID { get; set; }
    public string ScopeAction { get; set; }
}