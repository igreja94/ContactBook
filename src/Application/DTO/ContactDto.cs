using System.ComponentModel.DataAnnotations;
using Application.DTO.Interfaces;

namespace Application.DTO
{
    public class ContactDto : IDto
    {
        public Guid? Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string? FirstName { get; set; }
        [Required]
        [MaxLength(100)]
        public string? LastName { get; set; }
        [Required]
        public DateTime? DateOfBirth { get; set; }
        [MaxLength(200)]
        public string? Email { get; set; }
        [MaxLength(15)]
        public string? PhoneNumber { get; set; }
        // Owner of the contact. Required on creation and immutable afterwards.
        public Guid? UserId { get; set; }
    }
}
