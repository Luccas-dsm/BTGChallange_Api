namespace BTGChallange.Domain.Excecoes
{
    public class LimiteInsuficienteException : Exception
    {
        public LimiteInsuficienteException(string mensagem) : base(mensagem) { }
    }
}
