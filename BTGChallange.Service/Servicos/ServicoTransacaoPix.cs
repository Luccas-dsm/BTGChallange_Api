using BTGChallange.Domain.Entidades;
using BTGChallange.Domain.Excecoes;
using BTGChallange.Domain.Interfaces;
using BTGChallange.Service.Dtos;
using BTGChallange.Service.Interfaces;

namespace BTGChallange.Service.Servicos
{
    public class ServicoTransacaoPix : IServicoTransacaoPix
    {
        private readonly IRepositorioLimiteConta _repositorioLimite;
        private readonly IRepositorioTransacaoPix _repositorioTransacao;

        public ServicoTransacaoPix(
            IRepositorioLimiteConta repositorioLimite,
            IRepositorioTransacaoPix repositorioTransacao)
        {
            _repositorioLimite = repositorioLimite;
            _repositorioTransacao = repositorioTransacao;
        }

        public async Task<ResultadoTransacaoDto> ProcessarTransacaoAsync(ProcessarPixDto dto)
        {
            
            var limite = await _repositorioLimite.ObterPorContaAsync(dto.Agencia, dto.Conta);
            if (limite == null)
            {
                return ResultadoTransacaoDto.ContaNaoEncontrada();
            }

            
            var transacao = new TransacaoPix(dto.Agencia, dto.Conta, dto.Valor);

            
            if (!limite.PodeProcessarTransacao(dto.Valor))
            {
                transacao.Rejeitar("Limite insuficiente");
                await _repositorioTransacao.SalvarTransacaoAsync(transacao);
                return ResultadoTransacaoDto.LimiteInsuficiente(limite.LimiteDisponivel);
            }

            try
            {
                
                limite.ConsumirLimite(dto.Valor);
                
                var atualizacaoSucesso = await _repositorioLimite.AtualizarAsync(limite);

                if (atualizacaoSucesso)
                {
                    transacao.Aprovar();
                    await _repositorioTransacao.SalvarTransacaoAsync(transacao);
                    return ResultadoTransacaoDto.Aprovada(transacao.Id, limite.LimiteDisponivel);
                }
                else
                {
                    transacao.Rejeitar("Erro interno do sistema");
                    await _repositorioTransacao.SalvarTransacaoAsync(transacao);
                    return new ResultadoTransacaoDto
                    {
                        Sucesso = false,
                        Mensagem = "Erro interno ao processar transação"
                    };
                }
            }
            catch (LimiteInsuficienteException ex)
            {
                transacao.Rejeitar(ex.Message);
                await _repositorioTransacao.SalvarTransacaoAsync(transacao);
                return ResultadoTransacaoDto.LimiteInsuficiente(limite.LimiteDisponivel);
            }
        }
    }
}
