using System.ComponentModel.DataAnnotations;

namespace RoleManagementBackend.Application.DTOs;

public class VisitorCreateDto
{
    [Required(ErrorMessage = "Name is required")]
    [MinLength(3, ErrorMessage = "Name must be at least 3 characters")]
    [MaxLength(150, ErrorMessage = "Name cannot exceed 150 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [MaxLength(200, ErrorMessage = "Email cannot exceed 200 characters")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required")]
    [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Phone number must be exactly 10 digits")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Company is required")]
    [MaxLength(200, ErrorMessage = "Company cannot exceed 200 characters")]
    public string Company { get; set; } = string.Empty;

    [Required(ErrorMessage = "Purpose is required")]
    [MaxLength(500, ErrorMessage = "Purpose cannot exceed 500 characters")]
    public string Purpose { get; set; } = string.Empty;

    [Required(ErrorMessage = "Visit date is required")]
    public DateTime VisitDate { get; set; }
}