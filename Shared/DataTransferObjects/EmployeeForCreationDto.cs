using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects
{
    public record EmployeeForCreationDto
    {
        [Required(ErrorMessage = "Employee name is a required field")]
        [MaxLength(30, ErrorMessage = "Maximum length for the name is 30 charachters.")]
        public string? Name {  get; init; }
        [Required(ErrorMessage = "Age is a required field")]
        [Range(18, int.MaxValue, ErrorMessage = "Age is required and cannot be lower than 18")]
        public int Age { get; init; }
        [Required(ErrorMessage = "Position is a required field")]
        [MaxLength(20, ErrorMessage = "Maximum length for the position is 20 charachters.")]
        public string? Position {  get; init; }
    }
}
