using BTGChallange.Service.Dtos;

namespace BTGChallange.Service.Interfaces
{
    public interface IServicoTransacaoPix
    {
        Task<ResultadoTransacaoDto> ProcessarTransacaoAsync(ProcessarPixDto dto);
    }
}
