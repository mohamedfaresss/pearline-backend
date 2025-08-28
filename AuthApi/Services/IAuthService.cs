﻿using AuthApi.DTOs;

namespace AuthApi.Services
{
    public interface IAuthService
    {
        Task<AuthResultDto> RegisterAsync(RegisterDto dto);
        Task<AuthResultDto> LoginAsync(LoginDto dto);
        Task<AuthResultDto> ForgotPasswordAsync(ForgotPasswordDto dto);
        Task<AuthResultDto> ResetPasswordAsync(ResetPasswordDto dto);
    }
}
