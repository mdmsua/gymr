namespace Gymr.Options;

public sealed class GitHubOptions
{
    public string ClientId { get; init; } = null!;

    public string ClientSecret { get; init; } = null!;

    public int AdministratorId { get; set; }
}
