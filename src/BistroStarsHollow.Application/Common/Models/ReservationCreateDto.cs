using System.ComponentModel.DataAnnotations;

namespace BistroStarsHollow.Application.Common.Models;

public class ReservationCreateDto
{
    [Required(ErrorMessage = "Jméno je povinné.")]
    [StringLength(50, ErrorMessage = "Jméno může mít maximálně 50 znaků.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Příjmení je povinné.")]
    [StringLength(50, ErrorMessage = "Příjmení může mít maximálně 50 znaků.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Telefon je povinný.")]
    [RegularExpression(@"^\+?[0-9\s]{9,15}$", ErrorMessage = "Zadejte platné telefonní číslo.")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Datum je povinné.")]
    public DateTime Date { get; set; }

    [Required(ErrorMessage = "Čas je povinný.")]
    public TimeSpan Time { get; set; }

    [Required(ErrorMessage = "Počet osob je povinný.")]
    [Range(1, 20, ErrorMessage = "Počet osob musí být mezi 1 a 20.")]
    public int NumberOfPeople { get; set; }

    [Required(ErrorMessage = "Vyberte stůl.")]
    public Guid TableId { get; set; }

    [StringLength(500, ErrorMessage = "Poznámka může mít maximálně 500 znaků.")]
    public string? Note { get; set; }
}
