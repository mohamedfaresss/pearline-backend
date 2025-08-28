using Microsoft.AspNetCore.Identity;

namespace AuthApi.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Personal Info
        public required string FullName { get; set; }
        public string? MobileNumber { get; set; }

        // Company Info
        public required string CompanyName { get; set; }
        public string? CompanyWebsite { get; set; }
        public string? VatNumber { get; set; }
        public string? StreetAddress { get; set; }
        public string? City { get; set; }
        public required string Country { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
    }
}
