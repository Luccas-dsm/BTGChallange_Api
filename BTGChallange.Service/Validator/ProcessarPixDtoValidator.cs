using BTGChallange.Service.Dtos;
using FluentValidation;
using Integrador.Service.Validator.Base;

namespace BTGChallange.Service.Validator
{
    public class ProcessarPixDtoValidator : ValidatorBase<ProcessarPixDto>
    {
        public ProcessarPixDtoValidator()
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

            RuleFor(x => x.Valor)
                .GreaterThan(0)
                .WithMessage("Valor da transação deve ser maior que zero")
                .LessThanOrEqualTo(100000)
                .WithMessage("Valor da transação não pode exceder R$ 100.000,00")
                .Must(TerCasasDecimaisValidas)
                .WithMessage("Valor deve ter no máximo 2 casas decimais");
        }

        private static bool TerCasasDecimaisValidas(decimal valor)
        {
            // Verifica se o valor tem no máximo 2 casas decimais
            return valor == Math.Round(valor, 2);
        }
    }
}
