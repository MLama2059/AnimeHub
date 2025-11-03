using System;
using System.Collections.Generic;
using System.Text;

namespace AnimeHub.Shared.Models.Dtos.User
{
    public class AuthenticationResponseDto
    {
        public bool IsAuthSuccessful { get; set; } = false;
        public string ErrorMessage { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}
