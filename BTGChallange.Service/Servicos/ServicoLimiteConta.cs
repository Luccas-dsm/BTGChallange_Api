using AutoMapper;
using BTGChallange.Domain.Entidades;
using BTGChallange.Domain.Interfaces;
using BTGChallange.Service.Dtos;
using BTGChallange.Service.Interfaces;

namespace BTGChallange.Service.Servicos
{
    public class ServicoLimiteConta : IServicoLimiteConta
    {
        private readonly IRepositorioLimiteConta _repositorio;
        private readonly IMapper _mapper;

        public ServicoLimiteConta(IRepositorioLimiteConta repositorio, IMapper mapper)
        {
            _repositorio = repositorio;
            _mapper = mapper;
        }

        public Task<LimiteContaDto?> BuscarLimiteAsync(string agencia, string conta)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CadastrarLimiteAsync(CadastrarLimiteDto dto)
        {
            var limite = new LimiteContaCorrente(dto.Documento, dto.Agencia, dto.Conta, dto.LimitePix);
            return await _repositorio.CadastrarAsync(limite);
        }


    }
}
