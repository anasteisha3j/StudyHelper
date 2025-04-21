using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudyApp.Models
{
    public class AdminModel
    {
        public int TotalUsers { get; set; }
        public int TotalAdmins { get; set; }
        public List<AdminUserViewModel> Users { get; set; } = new List<AdminUserViewModel>();
        public List<StorageViolation> StorageViolations { get; set; } = new List<StorageViolation>();
    }

    public class AdminUserViewModel
    {
        [Required]
        [Display(Name = "ID користувача")]
        public string Id { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Електронна пошта")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        [Display(Name = "Повне ім'я")]
        public string FullName { get; set; }

        [Required]
        [Display(Name = "Роль")]
        public string Role { get; set; }

        [Display(Name = "Остання активність")]
        [DataType(DataType.DateTime)]
        public DateTime LastActivity { get; set; }
    }

    public class StorageViolation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Display(Name = "ID користувача")]
        public string UserId { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Пошта користувача")]
        public string UserEmail { get; set; }

        [Required]
        [Display(Name = "Дата")]
        [DataType(DataType.DateTime)]
        public DateTime ViolationDate { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Тип файлу")]
        [StringLength(50)]
        public string AttemptedFileType { get; set; }

        [Required]
        [Display(Name = "Розмір файлу (байти)")]
        [Range(1, long.MaxValue)]
        public long AttemptedSize { get; set; }

        [Required]
        [Display(Name = "Максимально дозволений розмір")]
        [Range(1, long.MaxValue)]
        public long MaxAllowed { get; set; }
    }
}