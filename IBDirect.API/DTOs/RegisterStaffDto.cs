using System.ComponentModel.DataAnnotations;

namespace IBDirect.API.DTOs
{
    public class RegisterStaffDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public int Role { get; set; }

        [Required]
        public string Specialty { get; set; }

        [Required]
        public string Practice { get; set; }
    }
}
