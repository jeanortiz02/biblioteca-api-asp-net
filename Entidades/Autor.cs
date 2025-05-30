using System;
using System.ComponentModel.DataAnnotations;
using BibliotecaAPI.Validaciones;

namespace BibliotecaAPI.Entidades;

public class Autor : IValidatableObject
{
    public int Id { get; set; }
    [Required(ErrorMessage = "El campo {0} es requerido")]
    [StringLength(10, ErrorMessage = "El campo {0} debe tener {1} caracteres o menos")]
    [PrimeraLetraMayuscula]
    public required string Nombre { get; set; }
    public List<Libro> Libros { get; set; } = new List<Libro>();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrEmpty(Nombre))
        {
            var primeraLetra = Nombre[0].ToString();

            if (primeraLetra != primeraLetra.ToUpper())
            {
                yield return new ValidationResult("La primera letra debe ir en mayuscula - por modelo",
                    new String[] { nameof(Nombre) }
                );
            }
        }
    }

    // [Range(18, 65, ErrorMessage = "el campo {0} debe ser entre {1} y {2}")]
    // public int? Edad { get; set; }

    // [CreditCard]
    // public string? CreditCard { get; set; }

    // [Url]
    // public string? Url { get; set; }
}
