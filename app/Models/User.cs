using System.Security.Claims;

namespace Gymr.Models;

internal class User
{

    public string Id { get; init; } = null!;

    public string Name { get; init; } = null!;

    public string Provider => Id.Split("://")[0];

    internal static User? FromClaims(IEnumerable<Claim>? claims)
    {
        string? id = claims?.SingleOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
        string? name = claims?.SingleOrDefault(claim => claim.Type == ClaimTypes.Name)?.Value;

        if (id is null || name is null)
        {
            return null;
        }

        return new() { Id = id, Name = name };
    }
}