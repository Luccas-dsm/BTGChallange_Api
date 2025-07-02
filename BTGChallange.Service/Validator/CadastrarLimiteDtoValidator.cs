using BTGChallange.Service.Dtos;
using FluentValidation;
using Integrador.Service.Validator.Base;

namespace BTGChallange.Service.Validator
{
    public class CadastrarLimiteDtoValidator : ValidatorBase<CadastrarLimiteDto>
    {
        public CadastrarLimiteDtoValidator()
        {
            RuleFor(x => x.Documento)
                .NotEmpty()
                .WithMessage("Documento é obrigatório")
                .Must(ValidarCpf)
                .WithMessage("CPF inválido");

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

            RuleFor(x => x.LimitePix)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Limite PIX deve ser maior ou igual a zero")
                .LessThanOrEqualTo(1000000)
                .WithMessage("Limite PIX não pode exceder R$ 1.000.000,00");
        }

        private static bool ValidarCpf(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return false;

            // Remove caracteres não numéricos
            cpf = cpf.Replace(".", "").Replace("-", "").Replace(" ", "");

            // Verifica se tem 11 dígitos
            if (cpf.Length != 11)
                return false;

            // Verifica se todos os dígitos são iguais
            if (cpf.Distinct().Count() == 1)
                return false;

            // Validação dos dígitos verificadores
            int[] multiplicadores1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicadores2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            string tempCpf = cpf.Substring(0, 9);
            int soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicadores1[i];

            int resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;

            string digito = resto.ToString();
            tempCpf += digito;

            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicadores2[i];

            resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;
            digito += resto.ToString();

            return cpf.EndsWith(digito);
        }
    }
}
