using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskProTracker.MinimalAPI.Data;
using TaskProTracker.MinimalAPI.Dtos;
using TaskProTracker.MinimalAPI.Models;

namespace TaskProTracker.MinimalAPI.Endpoints
{
    public static class UserEndpoints
    {
        public static void MapUserEndpoints(this WebApplication app)
        {
            app.MapPost("/login", async (LoginUserDto userDto, AppDbContext db, IConfiguration config) =>
            {
                var validationResults = new List<ValidationResult>();
                var context = new ValidationContext(userDto);

                if (!Validator.TryValidateObject(userDto, context, validationResults, true))
                {
                    var errors = validationResults.Select(r => r.ErrorMessage);
                    return Results.BadRequest(errors);
                }

                var existingUser = await db.Users.FirstOrDefaultAsync(u => u.Email == userDto.Email);
                if (existingUser == null)
                {
                    return Results.NotFound("User not found.");
                }

                var passwordHasher = new PasswordHasher<User>();
                var result = passwordHasher.VerifyHashedPassword(existingUser, existingUser.PasswordHash, userDto.Password);
                if (result == PasswordVerificationResult.Failed)
                {
                    return Results.Unauthorized();
                }

                var claims = new[]
                {
        new Claim(ClaimTypes.Name, userDto.Email),
        new Claim(ClaimTypes.Role, existingUser.Role)
    };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetValue<string>("JwtSettings:Key")));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: config.GetValue<string>("JwtSettings:ValidIssuer"),
                    audience: config.GetValue<string>("JwtSettings:ValidAudience"),
                    claims: claims,
                    notBefore: DateTime.UtcNow,
                    expires: DateTime.UtcNow.AddHours(1),
                    signingCredentials: creds
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                return Results.Ok(new { token = tokenString });
            });

            app.MapPost("/register", async (RegisterUserDto dto, AppDbContext dbContext) =>
            {
                var validationResults = new List<ValidationResult>();
                var context = new ValidationContext(dto);

                if (!Validator.TryValidateObject(dto, context, validationResults, true))
                {
                    var errors = validationResults.Select(r => r.ErrorMessage);
                    return Results.BadRequest(errors);
                }

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
        }
    }
}
