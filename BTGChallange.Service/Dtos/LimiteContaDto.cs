namespace BTGChallange.Service.Dtos
{
    public class LimiteContaDto
    {
        public string Documento { get; set; } = string.Empty;
        public string Agencia { get; set; } = string.Empty;
        public string Conta { get; set; } = string.Empty;
        public decimal LimitePix { get; set; }
        public decimal LimiteDisponivel { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime? AtualizadoEm { get; set; }
    }
}
