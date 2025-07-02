namespace BTGChallange.Service.Dtos
{
    public class CadastrarLimiteDto
    {
        public string Documento { get; set; } = string.Empty;
        public string Agencia { get; set; } = string.Empty;
        public string Conta { get; set; } = string.Empty;
        public decimal LimitePix { get; set; }
    }
}
