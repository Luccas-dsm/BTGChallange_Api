using BTGChallange.Service.Dtos;
using FluentValidation;
using Integrador.Service.Validator.Base;

namespace BTGChallange.Service.Validator
{
    public class AtualizarLimiteDtoValidator : ValidatorBase<AtualizarLimiteDto>
    {
        public AtualizarLimiteDtoValidator()
        {
            RuleFor(x => x.Agencia)
                .NotEmpty()
                .WithMessage("Agência é obrigatória")
                .Length(4)
                .WithMessage("Agência deve ter exatamente 4 dígitos")
                .Matches(@"^\d{4}$")
                .WithMessage("Agência deve conter apenas números");

            RuleFor(x => x.Conta)
                .NotEmpty()
                .WithMessage("Conta é obrigatória")
                .Length(1, 10)
                .WithMessage("Conta deve ter entre 1 e 10 caracteres")
                .Matches(@"^\d+$")
                .WithMessage("Conta deve conter apenas números");

            RuleFor(x => x.NovoLimitePix)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Novo limite PIX deve ser maior ou igual a zero")
                .LessThanOrEqualTo(1000000)
                .WithMessage("Novo limite PIX não pode exceder R$ 1.000.000,00");
        }
    }
}
