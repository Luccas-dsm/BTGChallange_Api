using BTGChallange.Domain.Entidades;

namespace BTGChallange.Domain.Interfaces
{
    public interface IRepositorioTransacaoPix
    {
        Task<bool> SalvarTransacaoAsync(TransacaoPix transacao);
        Task<TransacaoPix?> ObterPorIdAsync(string id);
        Task<List<TransacaoPix>> ObterTransacoesPorContaAsync(string agencia, string conta);
    }
}
