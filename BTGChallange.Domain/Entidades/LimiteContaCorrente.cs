using BTGChallange.Domain.Excecoes;

namespace BTGChallange.Domain.Entidades
{
    public class LimiteContaCorrente
    {
        public string Documento { get; private set; }
        public string Agencia { get; private set; }
        public string Conta { get; private set; }
        public decimal LimitePix { get; private set; }
        public decimal LimiteDisponivel { get; private set; }
        public DateTime CriadoEm { get; private set; }
        public DateTime? AtualizadoEm { get; private set; }

        protected LimiteContaCorrente() { }

        public LimiteContaCorrente(string documento, string agencia, string conta, decimal limitePix)
        {
            ValidarEntradas(documento, agencia, conta, limitePix);

            Documento = documento;
            Agencia = agencia;
            Conta = conta;
            LimitePix = limitePix;
            LimiteDisponivel = limitePix;
            CriadoEm = DateTime.UtcNow;
        }

        public bool PodeProcessarTransacao(decimal valor)
        {
            return LimiteDisponivel >= valor && valor > 0;
        }

        public void ConsumirLimite(decimal valor)
        {
            if (!PodeProcessarTransacao(valor))
                throw new LimiteInsuficienteException($"Limite insuficiente. Disponível: {LimiteDisponivel}, Solicitado: {valor}");

            LimiteDisponivel -= valor;
            AtualizadoEm = DateTime.UtcNow;
        }

        public void AtualizarLimite(decimal novoLimite)
        {
            if (novoLimite < 0)
                throw new InvalidOperationException("Limite não pode ser negativo");

            var diferenca = novoLimite - LimitePix;
            LimitePix = novoLimite;
            LimiteDisponivel += diferenca;
            AtualizadoEm = DateTime.UtcNow;
        }

        public string ObterChaveConta()
        {
            return $"{Agencia}#{Conta}";
        }

        private static void ValidarEntradas(string documento, string agencia, string conta, decimal limitePix)
        {
            if (string.IsNullOrWhiteSpace(documento))
                throw new ArgumentException("Documento é obrigatório", nameof(documento));

            if (string.IsNullOrWhiteSpace(agencia))
                throw new ArgumentException("Agência é obrigatória", nameof(agencia));

            if (string.IsNullOrWhiteSpace(conta))
                throw new ArgumentException("Conta é obrigatória", nameof(conta));

            if (limitePix < 0)
                throw new ArgumentException("Limite PIX deve ser maior ou igual a zero", nameof(limitePix));
        }
    }
}
