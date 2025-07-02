namespace BTGChallange.Service.Dtos
{
    public class AtualizarLimiteDto
    {
        public string Agencia { get; set; } = string.Empty;
        public string Conta { get; set; } = string.Empty;
        public decimal NovoLimitePix { get; set; }
    }
}
