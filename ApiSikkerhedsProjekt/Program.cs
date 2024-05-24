using System.Threading.RateLimiting;
using ApiSikkerhedsProjekt.Security;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi.Models;
using RepoDb;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.Configure<RateLimitingOptions>(
  builder.Configuration.GetSection(RateLimitingOptions.MyRateLimit));

var rateLimitingOptions = new RateLimitingOptions();
builder.Configuration.GetSection(RateLimitingOptions.MyRateLimit).Bind(rateLimitingOptions);

builder.Services.AddRateLimiter(_ => _
  .AddFixedWindowLimiter(policyName: rateLimitingOptions.Policy, options =>
  {
    options.PermitLimit = rateLimitingOptions.PermitLimit;
    options.Window = TimeSpan.FromSeconds(rateLimitingOptions.Window);
    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    options.QueueLimit = rateLimitingOptions.QueueLimit;
  }));

//Registration of SecurityMiddleware
builder.Services.AddTransient<SecurityMiddleware>();
builder.Services.AddTransient<DatabaseHelperForSecurity>();

builder.Services.AddScoped<HeaderSecurity>();

// Makes sure that API key and secret has to be on all endpoints in swagger
builder.Services.AddSwaggerGen(c =>
{
  c.SwaggerDoc("v1", new OpenApiInfo { Title = "Api Key Auth", Version = "v1" });
  c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
  {
    Description = "ApiKey must appear in header",
    Type = SecuritySchemeType.ApiKey,
    Name = "api-key",
    In = ParameterLocation.Header,
    Scheme = "ApiKeyScheme"
  });
  OpenApiSecurityScheme key = new OpenApiSecurityScheme()
  {
    Reference = new OpenApiReference
    {
      Type = ReferenceType.SecurityScheme,
      Id = "api-key"
    },
    In = ParameterLocation.Header
  };
  OpenApiSecurityRequirement requirement = new OpenApiSecurityRequirement
  {
    { key, new List<string>() }
  };
  c.AddSecurityRequirement(requirement);
  c.OperationFilter<KeyFilter>();
  c.OperationFilter<SecretFilter>();
});

WebApplication app = builder.Build();

//app.Map("/", (HttpRequest request, HeaderSecurity headerSec) =>
//{
//  Microsoft.Extensions.Primitives.StringValues accept = request.Headers.Accept = "application/json";
//  IHeaderDictionary customHeader = headerSec.HeaderVerification(request).Headers;

//  return Results.Ok(new { accept, customHeader });
//});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
  app.UseDeveloperExceptionPage();
  app.UseStatusCodePages();
}
else
{
  app.UseExceptionHandler("/Error"); 
  app.UseHsts();
}
app.UseRouting();
app.UseRateLimiter();

app.UseMiddleware<SecurityMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers().RequireRateLimiting(rateLimitingOptions.Policy);

GlobalConfiguration
  .Setup()
  .UseSQLite();

app.Run();
