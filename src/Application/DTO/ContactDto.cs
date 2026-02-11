using System.ComponentModel.DataAnnotations;
using Application.DTO.Interfaces;

namespace Application.DTO
{
    public class ContactDto : IDto
    {
        public string? Id { get; set; }
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        [Required]
        public DateTime? DateOfBirth { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
