using System.Threading.RateLimiting;
using ApiSikkerhedsProjekt.Security;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.OpenApi.Models;
using RepoDb;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHsts(options =>
{
  options.Preload = true;
  options.IncludeSubDomains = true;
  options.MaxAge = TimeSpan.FromDays(60);

});

if (!builder.Environment.IsDevelopment())
{
  builder.Services.AddHttpsRedirection(options =>
  {
    options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
    options.HttpsPort = 443;
  });
}

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

//Registration of Logging
Log.Logger = new LoggerConfiguration()
  .Enrich.FromLogContext()
  .WriteTo.Console(
    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}")
  .WriteTo.File($@"C:\log.txt",
    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] ({SourceContext:l}) {Message:lj}{NewLine}{Exception} {Properties:j}{NewLine}")
  .CreateLogger();

builder.Services.AddLogging(loggingBuilder =>
  loggingBuilder.AddSerilog(dispose: true));

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

app.UseRouting();
app.UseRateLimiter();

app.MapControllers().RequireRateLimiting(rateLimitingOptions.Policy);

app.Map("/*", (HttpResponse response, HeaderSecurity headerSec) =>
{
  var customHeader = headerSec.HeaderSanitization(response);

  return Results.Ok(new { customHeader });
});

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
app.UseHttpsRedirection();

app.UseMiddleware<SecurityMiddleware>();

app.UseAuthorization();

GlobalConfiguration
  .Setup()
  .UseSQLite();

app.Run();
