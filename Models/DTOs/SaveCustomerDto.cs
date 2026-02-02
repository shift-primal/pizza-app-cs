using System.ComponentModel.DataAnnotations;

public record SaveCustomerDto(
    [Required, StringLength(100)] string Name,
    [Required, EmailAddress] string Email,
    [Required, Phone] string Phone
);
