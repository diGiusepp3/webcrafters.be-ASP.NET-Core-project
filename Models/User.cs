using System.ComponentModel.DataAnnotations.Schema;

namespace webcrafters.be_ASP.NET_Core_project.Models
{
    [Table("users")]  // tabelnaam exact zoals in MySQL
    public class User
    {
        [Column("user_id")]
        public int UserId { get; set; }

        [Column("first_name")]
        public string FirstName { get; set; } = null!;

        [Column("last_name")]
        public string LastName { get; set; } = null!;

        [Column("username")]
        public string Username { get; set; } = null!;

        [Column("password")]
        public string Password { get; set; } = null!;

        [Column("email")]
        public string Email { get; set; } = null!;

        [Column("confirmed")]
        public bool Confirmed { get; set; }

        [Column("registered")]
        public DateTime Registered { get; set; }

        [Column("updated")]
        public DateTime? Updated { get; set; }

        [Column("user_project_id")]
        public int? UserProjectId { get; set; }

        [Column("credit_count")]
        public int CreditCount { get; set; }

        [Column("zipcode")]
        public string? Zipcode { get; set; }

        [Column("city")]
        public string? City { get; set; }

        [Column("street")]
        public string? Street { get; set; }

        [Column("house_number")]
        public string? HouseNumber { get; set; }

        [Column("country")]
        public string? Country { get; set; }

        [Column("last_online")]
        public DateTime? LastOnline { get; set; }
    }
}