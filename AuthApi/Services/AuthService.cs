using AuthApi.DTOs;
using AuthApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;

        public AuthService(UserManager<ApplicationUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }

        // Register new user
        public async Task<AuthResultDto> RegisterAsync(RegisterDto dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = $"{dto.FirstName} {dto.LastName}",
                MobileNumber = dto.MobileNumber,
                CompanyName = dto.CompanyName,
                CompanyWebsite = dto.CompanyWebsite,
                VatNumber = dto.VatNumber,
                StreetAddress = dto.StreetAddress,
                City = dto.City,
                Country = dto.Country,
                State = dto.State,
                ZipCode = dto.ZipCode
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                return new AuthResultDto
                {
                    Success = false,
                    Errors = result.Errors.Select(e => e.Description)
                };
            }

            return new AuthResultDto
            {
                Success = true,
                Message = "User registered successfully."
            };
        }

        // Login user and generate JWT
        public async Task<AuthResultDto> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                return new AuthResultDto
                {
                    Success = false,
                    Errors = new[] { "Invalid credentials" }
                };
            }

            // User claims
            var userRoles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            // Create JWT token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Secret"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["JWT:Issuer"],
                audience: _config["JWT:Audience"],
                expires: DateTime.UtcNow.AddHours(2),
                claims: claims,
                signingCredentials: creds
            );

            return new AuthResultDto
            {
                Success = true,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Message = "Login successful"
            };
        }

        // Generate password reset token
        public async Task<AuthResultDto> ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return new AuthResultDto
                {
                    Success = false,
                    Errors = new[] { "User not found" }
                };
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            return new AuthResultDto
            {
                Success = true,
                Message = "Password reset token generated.",
                Token = token
            };
        }

        // Reset password with token
        public async Task<AuthResultDto> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return new AuthResultDto
                {
                    Success = false,
                    Errors = new[] { "User not found" }
                };
            }

            var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
            return result.Succeeded
                ? new AuthResultDto { Success = true, Message = "Password reset successful" }
                : new AuthResultDto { Success = false, Errors = result.Errors.Select(e => e.Description) };
        }
    }
}
