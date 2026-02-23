using Ocelot.DependencyInjection;
using Ocelot.Cache.CacheManager;
using ECommerce.SharedLibrary.DependencyInjection;
using ApiGateway.Presentation.Middleware;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot().AddCacheManager(x =>
{
    x.WithDictionaryHandle();
});

JwtAuthenticationScheme.AddJwtAuthenticationScheme(builder.Services, builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();
// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseCors();  
app.UseMiddleware<AttachSignatureToRequest>();
app.UseOcelot().Wait();

app.Run();