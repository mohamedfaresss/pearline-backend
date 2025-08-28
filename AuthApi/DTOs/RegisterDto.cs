using System.ComponentModel.DataAnnotations;

public class RegisterDto
{
    // Personal Information
    [Required, StringLength(50)]
    public required string FirstName { get; set; }

    [Required, StringLength(50)]
    public required string LastName { get; set; }

    [Required, EmailAddress]
    public required string Email { get; set; }

    [Required, StringLength(100, MinimumLength = 6)]
    public required string Password { get; set; }

    [Phone]
    public string? MobileNumber { get; set; }

    // Company Information
    [Required, StringLength(100)]
    public required string CompanyName { get; set; }

    [Url]
    public string? CompanyWebsite { get; set; }

    [Phone]
    public string? PhoneNumber { get; set; }

    public string? VatNumber { get; set; }
    public string? StreetAddress { get; set; }
    public string? City { get; set; }
    public required string Country { get; set; }
    public string? State { get; set; }

    [StringLength(10)]
    public string? ZipCode { get; set; }
}
