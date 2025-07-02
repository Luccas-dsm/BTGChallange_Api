using BTGChallange.Service.Dtos;

namespace BTGChallange.Service.Interfaces
{
    public interface IServicoLimiteConta
    {
        Task<bool> CadastrarLimiteAsync(CadastrarLimiteDto dto);
        Task<LimiteContaDto?> BuscarLimiteAsync(string agencia, string conta);
        Task<bool> AtualizarLimiteAsync(AtualizarLimiteDto dto);
        Task<bool> RemoverLimiteAsync(string agencia, string conta);

    }
}
