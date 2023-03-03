using System.Security.Claims;
using Gymr.Common;
using Gymr.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Octokit;

namespace Gymr.Pages;

public class GitHubModel : PageModel
{
    private readonly GitHubClient client;
    private GitHubOptions options;

    public GitHubModel(GitHubClient client, IOptions<GitHubOptions> options)
    {
        this.client = client;
        this.options = options.Value;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
        catch { }

        string? code = Request.Query["code"].SingleOrDefault();
        if (code is not { Length: > 0 })
        {
            return BadRequest("Invalid value for parameter 'code'");
        }

        OauthToken oauthToken = await client.Oauth.CreateAccessToken(new(options.ClientId, options.ClientSecret, code));
        if (oauthToken.Error is not null)
        {
            return BadRequest($"Error response from GitHub: {oauthToken.Error}");
        }

        client.Credentials = new(oauthToken.AccessToken, AuthenticationType.Bearer);
        User user = await client.User.Current();

        List<Claim> claims = new()
        {
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.NameIdentifier, $"github://{user.Id}"),
        };

        if (user.Id == options.AdministratorId)
        {
            claims.Add(new(ClaimTypes.Role, Roles.Administrator));
        }

        ClaimsIdentity identity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        ClaimsPrincipal principal = new(identity);
        AuthenticationProperties properties = new() { IsPersistent = true };
        try
        {
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);
        }
        catch
        {

        }
        return Redirect("/");
    }
}