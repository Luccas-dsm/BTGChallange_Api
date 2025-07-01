namespace BTGChallange.Domain.Entidades
{
    public class TransacaoPix
    {
        public string Id { get; private set; }
        public string Agencia { get; private set; }
        public string Conta { get; private set; }
        public decimal Valor { get; private set; }
        public DateTime DataTransacao { get; private set; }
        public bool Aprovada { get; private set; }
        public string? MotivoRejeicao { get; private set; }

        protected TransacaoPix() { } // Para ORMs

        public TransacaoPix(string agencia, string conta, decimal valor)
        {
            Id = Guid.NewGuid().ToString();
            Agencia = agencia;
            Conta = conta;
            Valor = valor;
            DataTransacao = DateTime.UtcNow;
            Aprovada = false;
        }

        public void Aprovar()
        {
            Aprovada = true;
            MotivoRejeicao = null;
        }

        public void Rejeitar(string motivo)
        {
            Aprovada = false;
            MotivoRejeicao = motivo;
        }
    }
}
