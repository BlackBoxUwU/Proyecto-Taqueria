using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Proyecto_Taqueria.Data.Modelos
{
    public class Producto : IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La categoría es obligatoria.")]
        [StringLength(100, ErrorMessage = "La categoría no puede exceder los 100 caracteres.")]
        public string Categoria { get; set; } = string.Empty;

        [Required(ErrorMessage = "La cantidad en stock es obligatoria.")]
        [Range(0, int.MaxValue, ErrorMessage = "La cantidad en stock debe ser un número positivo o cero.")]
        public int CantidadStock { get; set; }

        [Required(ErrorMessage = "El stock mínimo es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock mínimo debe ser un número positivo o cero.")]
        public int MinimoStock { get; set; }

        [Required(ErrorMessage = "El stock máximo es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock máximo debe ser un número positivo o cero.")]
        public int MaximoStock { get; set; }

        [Required(ErrorMessage = "La fecha de caducidad es obligatoria.")]
        public DateTime? FechaCaducidad { get; set; }

        [Required(ErrorMessage = "La fecha de llegada es obligatoria.")]
        public DateTime? FechaLlegada { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Validaciones de Stock
            if (CantidadStock < MinimoStock)
            {
                yield return new ValidationResult(
                    "La cantidad en stock no puede ser menor que el stock mínimo.",
                    new[] { nameof(CantidadStock), nameof(MinimoStock) }
                );
            }

            if (CantidadStock > MaximoStock)
            {
                yield return new ValidationResult(
                    "La cantidad en stock no puede ser mayor que el stock máximo.",
                    new[] { nameof(CantidadStock), nameof(MaximoStock) }
                );
            }

            if (MinimoStock > MaximoStock)
            {
                yield return new ValidationResult(
                    "El stock mínimo no puede ser mayor que el stock máximo.",
                    new[] { nameof(MinimoStock), nameof(MaximoStock) }
                );
            }

            // Validaciones de Fecha de Caducidad
            if (FechaCaducidad.HasValue)
            {
                DateTime today = DateTime.Today;

                if (FechaCaducidad.Value.Date < today)
                {
                    yield return new ValidationResult(
                        "La fecha de caducidad no puede ser anterior a la fecha actual.",
                        new[] { nameof(FechaCaducidad) }
                    );
                }
                else
                {
                    int diasCercaExpiracion = 3;
                    DateTime fechaLimiteAdvertencia = today.AddDays(diasCercaExpiracion);

                    if (FechaCaducidad.Value.Date <= fechaLimiteAdvertencia)
                    {
                        yield return new ValidationResult(
                            "Advertencia: La fecha de caducidad está muy cerca. ¿Está seguro de querer esta fecha?",
                            new[] { nameof(FechaCaducidad) }
                        );
                    }
                }
            }

            // Validaciones de Fecha de Llegada
            if (FechaLlegada.HasValue)
            {
                DateTime today = DateTime.Today;
                // Calcular el inicio del mes anterior (Junio de 2025)
                DateTime firstDayOfCurrentMonth = new DateTime(today.Year, today.Month, 1);
                DateTime firstDayOfPreviousMonth = firstDayOfCurrentMonth.AddMonths(-1);

                // Validación 1: La fecha de llegada no puede ser futura (posterior a la fecha actual)
                if (FechaLlegada.Value.Date > today)
                {
                    yield return new ValidationResult(
                        "La fecha de llegada no puede ser una fecha futura.",
                        new[] { nameof(FechaLlegada) }
                    );
                }
                // Validación 2: La fecha de llegada no puede ser anterior al primer día del mes anterior (Mayo de 2025)
                else if (FechaLlegada.Value.Date < firstDayOfPreviousMonth)
                {
                    yield return new ValidationResult(
                        $"La fecha de llegada no puede ser anterior al mes de {firstDayOfPreviousMonth:MMMM 'de' yyyy}.",
                        new[] { nameof(FechaLlegada) }
                    );
                }
            }
        }
    }
}