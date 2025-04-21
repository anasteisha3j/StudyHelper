using System;

namespace StudyApp.Models
{
    public class UserDownloadStats
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public long TotalDownloadSize { get; set; } // in bytes
        public int TotalFiles { get; set; }
        public DateTime LastDownloadDate { get; set; }
    }
} 