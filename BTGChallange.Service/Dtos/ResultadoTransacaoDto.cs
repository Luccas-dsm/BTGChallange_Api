namespace BTGChallange.Service.Dtos
{
    public class ResultadoTransacaoDto
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public string? TransacaoId { get; set; }
        public decimal? LimiteRestante { get; set; }

        public static ResultadoTransacaoDto ContaNaoEncontrada()
        {
            return new ResultadoTransacaoDto
            {
                Sucesso = false,
                Mensagem = "Conta não encontrada no sistema"
            };
        }

        public static ResultadoTransacaoDto LimiteInsuficiente(decimal limiteDisponivel)
        {
            return new ResultadoTransacaoDto
            {
                Sucesso = false,
                Mensagem = $"Limite insuficiente. Disponível: R$ {limiteDisponivel:F2}",
                LimiteRestante = limiteDisponivel
            };
        }

        public static ResultadoTransacaoDto Aprovada(string transacaoId, decimal limiteRestante)
        {
            return new ResultadoTransacaoDto
            {
                Sucesso = true,
                Mensagem = "Transação aprovada com sucesso",
                TransacaoId = transacaoId,
                LimiteRestante = limiteRestante
            };
        }
    }
}
