namespace BTGChallange.Domain.Excecoes
{
    public class ContaNaoEncontradaException : Exception
    {
        public ContaNaoEncontradaException(string mensagem) : base(mensagem) { }
    }
}
