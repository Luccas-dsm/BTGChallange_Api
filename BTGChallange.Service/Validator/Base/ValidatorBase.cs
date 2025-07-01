using FluentValidation;
using FluentValidation.Results;

namespace Integrador.Service.Validator.Base
{
    public class ValidatorBase<T> : AbstractValidator<T>
    {
        protected override void RaiseValidationException(ValidationContext<T> context, ValidationResult result)
        {
            var ex = new ValidationException(result.Errors);
            throw new ArgumentException("Falha ao validar " + GetType() + "; " + ex.Message, ex);
        }
    }
}
