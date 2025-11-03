using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AnimeHub.Shared.Models.Dtos.User
{
    // Registration input
    public class RegistrationRequestDto
    {
        [Required(ErrorMessage = "Username is required.")]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; set; } = string.Empty;
    }
}
