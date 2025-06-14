// using Microsoft.AspNetCore.Authentication.JwtBearer;   // ← auth removed

var builder = WebApplication.CreateBuilder(args);

// ── CORS for local React dev ────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactDev", policy =>
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
});

// ── (optional) MVC controllers you might still have ─────────────────────────
builder.Services.AddControllers();

// ── Swagger (dev only) ──────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

///////////////////////////////////////////////////////////////////////////////
//  Authentication & Authorization  ❌  (commented out for now)               //
///////////////////////////////////////////////////////////////////////////////

// builder.Services
//     .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options =>
//     {
//         options.Authority = builder.Configuration["Jwt:Authority"];
//         options.Audience  = builder.Configuration["Jwt:Audience"];
//         options.RequireHttpsMetadata = true;
//     });

// builder.Services.AddAuthorization(options =>
// {
//     options.AddPolicy("CasePolicy", policy =>
//         policy.RequireClaim("scope", "case.read", "case.write"));
//     options.AddPolicy("ConsultantPolicy", policy =>
//         policy.RequireClaim("scope", "consultant.read", "consultant.write"));
// });

///////////////////////////////////////////////////////////////////////////////

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// ── Pipeline ────────────────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowReactDev");

// app.UseAuthentication();   // ← commented out
// app.UseAuthorization();    // ← commented out

app.MapReverseProxy();
app.MapControllers();

app.Run();
