using Microsoft.AspNetCore.Identity;
using System;

namespace StudyApp.Models
{
    public class User : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public DateTime LastActivity { get; set; } = DateTime.Now;
    }
}