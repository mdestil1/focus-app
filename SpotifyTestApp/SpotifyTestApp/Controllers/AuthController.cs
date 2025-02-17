using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpotifyTestApp.Data;
using SpotifyTestApp.Models;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.Security.Claims;

namespace SpotifyTestApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // POST: api/Auth/signup
        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupDto signupDto)
{
    if (await _context.Users.AnyAsync(u => u.Email == signupDto.Email))
    {
        return BadRequest(new { message = "Email already in use." });
    }

    if (await _context.Users.AnyAsync(u => u.Username == signupDto.Username))
    {
        return BadRequest(new { message = "Username already taken." });
    }

    var user = new User
    {
        FirstName = signupDto.FirstName,
        LastName = signupDto.LastName,
        Username = signupDto.Username,
        Email = signupDto.Email,
        PasswordHash = ComputeSha256Hash(signupDto.Password)
    };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(user);

            return Ok(new { token });
        }

        // POST: api/Auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == loginDto.Identifier || u.Username == loginDto.Identifier);

            if (user == null || user.PasswordHash != ComputeSha256Hash(loginDto.Password))
            {
                return Unauthorized(new { message = "Invalid credentials." });
            }

            var token = GenerateJwtToken(user);

            return Ok(new { token });
        }

        // Helper methods and DTOs

        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("UserId", user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // DTOs

        public class SignupDto
        {
            public string FirstName { get; set; } // Optional
            public string LastName { get; set; }  // Optional
            public string Username { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class LoginDto
        {
            public string Identifier { get; set; } // Email or Username
            public string Password { get; set; }
        }
    }
}
