using System.Collections.Generic;
using Newtonsoft.Json;

namespace vault;


public record SpaceDto(string CreateTime, string Id, string UpdateTime, string Description);
public record NotAuthorizedDto(string message, string details);
public record InternalErrorDto(string message, string rayID);

public record SuccessActionDto(string update_time);

public record ClearedStatusDto(string traceId, int status);


public class Idp
{
    [JsonConstructor]
    public Idp(
        [JsonProperty("id")] string id,
        [JsonProperty("type")] string type
    )
    {
        this.Id = id;
        this.Type = type;
    }

    [JsonProperty("id")]
    public readonly string Id;

    [JsonProperty("type")]
    public readonly string Type;
}

public class Geo
{
    [JsonConstructor]
    public Geo(
        [JsonProperty("country")] string country
    )
    {
        this.Country = country;
    }

    [JsonProperty("country")]
    public readonly string Country;
}

public class Org
{
    [JsonConstructor]
    public Org(
        [JsonProperty("id")] int id,
        [JsonProperty("name")] string name
    )
    {
        this.Id = id;
        this.Name = name;
    }

    [JsonProperty("id")]
    public readonly int Id;

    [JsonProperty("name")]
    public readonly string Name;
}

public class Team
{
    [JsonConstructor]
    public Team(
        [JsonProperty("name")] string name,
        [JsonProperty("org_id")] int orgId
    )
    {
        this.Name = name;
        this.OrgId = orgId;
    }

    [JsonProperty("name")]
    public readonly string Name;

    [JsonProperty("org_id")]
    public readonly int OrgId;
}

public class CloudflareIdentityDto
{
    [JsonConstructor]
    public CloudflareIdentityDto(
        [JsonProperty("id")] int id,
        [JsonProperty("name")] string name,
        [JsonProperty("email")] string email,
        [JsonProperty("idp")] Idp idp,
        [JsonProperty("geo")] Geo geo,
        [JsonProperty("user_uuid")] string userUuid,
        [JsonProperty("account_id")] string accountId,
        [JsonProperty("ip")] string ip,
        [JsonProperty("auth_status")] string authStatus,
        [JsonProperty("common_name")] string commonName,
        [JsonProperty("version")] int version,
        [JsonProperty("orgs")] List<Org> orgs,
        [JsonProperty("teams")] List<Team> teams
    )
    {
        this.Id = id;
        this.Name = name;
        this.Email = email;
        this.Idp = idp;
        this.Geo = geo;
        this.UserUuid = userUuid;
        this.AccountId = accountId;
        this.Ip = ip;
        this.AuthStatus = authStatus;
        this.CommonName = commonName;
        this.Version = version;
        this.Orgs = orgs;
        this.Teams = teams;
    }

    [JsonProperty("id")]
    public readonly int Id;

    [JsonProperty("name")]
    public readonly string Name;

    [JsonProperty("email")]
    public readonly string Email;

    [JsonProperty("idp")]
    public readonly Idp Idp;

    [JsonProperty("geo")]
    public readonly Geo Geo;

    [JsonProperty("user_uuid")]
    public readonly string UserUuid;

    [JsonProperty("account_id")]
    public readonly string AccountId;

    [JsonProperty("ip")]
    public readonly string Ip;

    [JsonProperty("auth_status")]
    public readonly string AuthStatus;

    [JsonProperty("common_name")]
    public readonly string CommonName;

    [JsonProperty("version")]
    public readonly int Version;

    [JsonProperty("orgs")]
    public readonly List<Org> Orgs;

    [JsonProperty("teams")]
    public readonly List<Team> Teams;
}