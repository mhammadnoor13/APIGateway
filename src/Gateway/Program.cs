using Gateway.Clients;
using Gateway.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var env = builder.Environment;

string authBaseUrl;
var authDestinations = builder.Configuration
    .GetSection("ReverseProxy:Clusters:authCluster:Destinations")
    .GetChildren();


authBaseUrl = authDestinations
  .Select(destionation => destionation.GetValue<string>("Address"))
  .FirstOrDefault(address => !string.IsNullOrWhiteSpace(address))
  ?? throw new InvalidOperationException(
      "No destination configured for authCluster");


builder.Services.AddControllers();

builder.Services
    .AddHttpClient<IAuthServiceClient, AuthServiceClient>(c =>
    {
        c.BaseAddress = new Uri(authBaseUrl);
        c.Timeout = TimeSpan.FromSeconds(10);
    });


builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// 3️⃣ Swagger & CORS
builder.Services.AddCors(p => p.AddDefaultPolicy(pb =>
    pb.AllowAnyOrigin()
      .AllowAnyMethod()
      .AllowAnyHeader()));

// Documentation and Testing and adding a Bearer button in Swagger UI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API Gateway", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        Description = "Enter ‘Bearer {token}’"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// 4️⃣ Pipeline: CORS, Swagger, Health
app.UseCors();
app.UseSwagger();
app.UseSwaggerUI();

// 5️⃣ Public paths whitelist
var publicPaths = new[]
{
    "/auth/login",
    "/auth/register",
    "/swagger",
    "/swagger/index.html",
    "/swagger/v1/swagger.json",
    "/health",
};

// 6️⃣ Global JWT‐validation middleware
app.Use(async (ctx, next) =>
{
    var path = ctx.Request.Path.Value ?? "";
    var method = ctx.Request.Method;

    if (method == HttpMethods.Post
    && path.Equals("/cases", StringComparison.OrdinalIgnoreCase))
    {
        await next();
        return;
    }


    if (publicPaths.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase)))
    {
        await next();
        return;
    }

    if (!ctx.Request.Headers.TryGetValue("Authorization", out var authHdr) ||
        !authHdr.ToString().StartsWith("Bearer "))
    {
        ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return;
    }

    var token = authHdr.ToString()["Bearer ".Length..].Trim();
    var client = ctx.RequestServices.GetRequiredService<IAuthServiceClient>();
    var user = await client.ValidateTokenAsync(token);
    if (user is null)
    {
        ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return;
    }



    ctx.Request.Headers["X-User-Id"] = user.UserId.ToString();
    ctx.Request.Headers["X-User-Email"] = user.Email;
    ctx.Request.Headers["X-User-Roles"] = string.Join(",", user.Roles);

    await next();
});

// 7️⃣ Map controllers, then proxy all other routes
app.MapControllers();
app.MapReverseProxy();

app.Run();
