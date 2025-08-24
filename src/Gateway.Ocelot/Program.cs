using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("ocelot.json");
builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
await app.UseOcelot();
