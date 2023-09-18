using System.ComponentModel.DataAnnotations;

namespace IBDirect.API.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string Name { get; set; }
        
        [Required]
        public string Password { get; set; }
    }
}