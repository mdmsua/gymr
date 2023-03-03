using Azure.Identity;
using Gymr.Options;
using Gymr.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Azure.SignalR;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using Octokit;

ConventionRegistry.Register("CamelCase", new ConventionPack { new CamelCaseElementNameConvention() }, type => true);

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
builder.Services.AddAuthorization();
builder.Services.AddRazorPages();
builder.Services.AddHealthChecks().AddMongoDb(builder.Configuration.GetConnectionString("Mongo")!);
builder.Services.AddSingleton<IMongoDatabase>(_ => new MongoClient(builder.Configuration.GetConnectionString("Mongo")!).GetDatabase("Gymr"));
builder.Services.AddSingleton<IExerciseRepository, ExerciseRepository>();
builder.Services.AddSingleton<IWorkoutRepository, WorkoutRepository>();
builder.Services.AddServerSideBlazor();
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddHttpContextAccessor();
builder.Services.Configure<GitHubOptions>(builder.Configuration.GetSection("GitHub"));
builder.Services.AddSingleton(new GitHubClient(new ProductHeaderValue("gymr", "1.0")));

if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddSignalR().AddAzureSignalR(options =>
    {
        options.Endpoints = new ServiceEndpoint[] { new(new(builder.Configuration.GetConnectionString("SignalR")!), new ManagedIdentityCredential()) };
        options.ServerStickyMode = ServerStickyMode.Required;
        options.ApplicationName = "Gymr";
    });
}

var app = builder.Build();

// if (!app.Environment.IsDevelopment())
// {
//     app.UseHsts();
// }

// app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseHealthChecks("/healthz");

app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
