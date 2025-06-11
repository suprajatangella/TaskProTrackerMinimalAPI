using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskProTracker.MinimalAPI.Data;
using TaskProTracker.MinimalAPI.Dtos;
using TaskProTracker.MinimalAPI.Models;
using TaskProTracker.MinimalAPI.Options;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
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

var app = builder.Build();

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

RouteGroupBuilder taskItems = app.MapGroup("/tasks");

taskItems.MapGet("/", GetAllTasks);
taskItems.MapGet("/complete", GetCompletedTasks);
taskItems.MapGet("/{id}", GetTask);
taskItems.MapPost("/", CreateTask).RequireAuthorization();
taskItems.MapPut("/{id}", UpdateTask).RequireAuthorization();
taskItems.MapDelete("/{id}", DeleteTask).RequireAuthorization();

app.MapPost("/login", async (User user, AppDbContext db) =>
{
    var existingUser = await db.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
    if (existingUser == null)
    {
        return Results.NotFound("User not found.");
    }
    var passwordHasher = new PasswordHasher<User>();
    var result = passwordHasher.VerifyHashedPassword(existingUser, existingUser.PasswordHash, user.PasswordHash);
    if (result == PasswordVerificationResult.Failed)
    {
        return Results.Unauthorized();
    }
    var claims = new[]
   {
        new Claim(ClaimTypes.Name, user.Email),
        new Claim(ClaimTypes.Role, user.Role)
    };
   
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: jwtSettings["ValidIssuer"],
        audience: jwtSettings["ValidAudience"],
        claims: claims,
        expires: DateTime.Now.AddHours(1),
        signingCredentials: creds
    );

    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

    return Results.Ok(new { token = tokenString });
});

app.MapPost("/register", async (RegisterUserDto dto, AppDbContext dbContext) =>
{
    var existingUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
    if (existingUser != null)
    {
        return Results.BadRequest("User already exists.");
    }

    var user = new User
    {
        Name = dto.Name,
        Email = dto.Email,
        Role = dto.Role ?? "User"    // or "Admin"
    };

    var hasher = new PasswordHasher<User>();
    user.PasswordHash = hasher.HashPassword(user, dto.Password);

    dbContext.Users.Add(user);
    await dbContext.SaveChangesAsync();

    return Results.Ok("User registered successfully.");
});
app.Run();

static async Task<IResult> GetAllTasks(AppDbContext db)
{
    return TypedResults.Ok(await db.Tasks.Select(x => new TaskItemDTO(x)).ToArrayAsync());
}

static async Task<IResult> GetCompletedTasks(AppDbContext db)
{
    return TypedResults.Ok(await db.Tasks.Where(t => t.IsCompleted).Select(x => new TaskItemDTO(x)).ToListAsync());
}

static async Task<IResult> GetTask(int id, AppDbContext db)
{
    return await db.Tasks.FindAsync(id)
        is TaskItem task
            ? TypedResults.Ok(new TaskItemDTO(task))
            : TypedResults.NotFound();
}

static async Task<IResult> CreateTask(TaskItemDTO taskItemDTO, AppDbContext db)
{
    var taskItem = new TaskItem
    {
        IsCompleted = taskItemDTO.IsCompleted,
        Title = taskItemDTO.Title,
        ProjectId = taskItemDTO.ProjectId
    };

    db.Tasks.Add(taskItem);
    await db.SaveChangesAsync();

    taskItemDTO = new TaskItemDTO(taskItem);

    return TypedResults.Created($"/taskitems/{taskItem.Id}", taskItemDTO);
}

static async Task<IResult> UpdateTask(int id, TaskItemDTO taskItemDTO, AppDbContext db)
{
    var task = await db.Tasks.FindAsync(id);

    if (task is null) return TypedResults.NotFound();

    task.Title = taskItemDTO.Title;
    task.IsCompleted = taskItemDTO.IsCompleted;
    task.ProjectId = taskItemDTO.ProjectId;

    await db.SaveChangesAsync();

    return TypedResults.NoContent();
}

static async Task<IResult> DeleteTask(int id, AppDbContext db)
{
    if (await db.Tasks.FindAsync(id) is TaskItem task)
    {
        db.Tasks.Remove(task);
        await db.SaveChangesAsync();
        return TypedResults.NoContent();
    }

    return TypedResults.NotFound();
}

