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

        public async Task<bool> AtualizarLimiteAsync(AtualizarLimiteDto dto)
        {
            var limite = await _repositorio.ObterPorContaAsync(dto.Agencia, dto.Conta);
            if (limite == null)
                return false;

            limite.AtualizarLimite(dto.NovoLimitePix);
            return await _repositorio.AtualizarAsync(limite);
        }

        public async Task<LimiteContaDto?> BuscarLimiteAsync(string agencia, string conta)
        {
            var limite = await _repositorio.ObterPorContaAsync(agencia, conta);
            return limite != null ? _mapper.Map<LimiteContaDto>(limite) : null;
        }

        public async Task<bool> CadastrarLimiteAsync(CadastrarLimiteDto dto)
        {
            var limite = new LimiteContaCorrente(dto.Documento, dto.Agencia, dto.Conta, dto.LimitePix);
            return await _repositorio.CadastrarAsync(limite);
        }

        public async Task<bool> RemoverLimiteAsync(string agencia, string conta)
        {
            return await _repositorio.RemoverAsync(agencia, conta);
        }
    }
}
