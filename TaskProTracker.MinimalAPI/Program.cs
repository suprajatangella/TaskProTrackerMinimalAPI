using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;
using TaskProTracker.MinimalAPI;
using TaskProTracker.MinimalAPI.Data;
using TaskProTracker.MinimalAPI.Endpoints;
using TaskProTracker.MinimalAPI.Middlewares;
using TaskProTracker.MinimalAPI.Transformers;

var builder = WebApplication.CreateBuilder(args);

// Configure Logging
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Error()
    .WriteTo.File("Logs/errors.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Configure Services
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
 var jwtSettings = builder.Configuration.GetSection("JwtSettings");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
          .AddJwtBearer(options =>
          {
              options.TokenValidationParameters = new TokenValidationParameters
              {
                  ValidateIssuer = true,
                  ValidateAudience = true,
                  ValidateLifetime = true,
                  ValidateIssuerSigningKey = true,
                  ValidIssuer = jwtSettings["ValidIssuer"],
                  ValidAudience = jwtSettings["ValidAudience"],
                  IssuerSigningKey = new SymmetricSecurityKey(
                      Encoding.UTF8.GetBytes(jwtSettings["Key"])
                  )
              };
          });
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
// Configure Swagger
builder.Services.AddOpenApi("v1", options =>
{
    options.AddHeader("Version", "1.0");
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
    options.AddDocumentTransformer((doc, ctx, token) => {
        doc.Info.Contact = new OpenApiContact { Name = "Supraja", Email = "supraja,tangella@gmail.com" };
        return Task.CompletedTask;
    });
});
//builder.Services.AddAntiforgery();
var app = builder.Build();

// Configure Middleware
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
//app.UseAntiforgery();
// Use the global error handler middleware
app.UseGlobalExceptionHandler();
// Enable Swagger middleware
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    //app.MapOpenApi("/openapi/{documentName}.yaml");
    app.MapSwaggerUi();
    //app.UseSwaggerUI(
    //options =>
    //{
    //    options.SwaggerEndpoint("/openapi/v1.json", "v1");
    //});
}

// Map endpoints
app.MapTaskEndpoints();
app.MapProjectEndpoints();
app.MapCommentEndpoints();
app.MapUserEndpoints();

//app.UseRouting();

app.Run();


