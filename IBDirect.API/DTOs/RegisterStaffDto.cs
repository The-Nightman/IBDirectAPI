using System.ComponentModel.DataAnnotations;

namespace IBDirect.API.DTOs
{
    public class RegisterStaffDto
    {
        [Required]
        public string Name { get; set; }
        
        [Required]
        public string Password { get; set; }

        [Required]
        public int Role { get; set; }
    }
}