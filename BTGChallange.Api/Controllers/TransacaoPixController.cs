using BTGChallange.Service.Dtos;
using BTGChallange.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BTGChallange.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransacaoPixController : ControllerBase
    {
        private readonly IServicoTransacaoPix _servicoTransacaoPix;

        public TransacaoPixController(IServicoTransacaoPix servicoTransacaoPix)
        {
            _servicoTransacaoPix = servicoTransacaoPix;
        }

        /// <summary>
        /// Processa uma transação PIX validando o limite disponível
        /// </summary>
        [HttpPost("processar")]
        public async Task<IActionResult> ProcessarTransacao([FromBody] ProcessarPixDto dto)
        {
            var resultado = await _servicoTransacaoPix.ProcessarTransacaoAsync(dto);

            if (!resultado.Sucesso)
            {
                var statusCode = resultado.Mensagem.Contains("não encontrada") ? 404 : 400;

                return StatusCode(statusCode, new
                {
                    Sucesso = false,
                    Mensagem = resultado.Mensagem,
                    LimiteRestante = resultado.LimiteRestante,
                    StatusCode = statusCode
                });
            }

            return Ok(new
            {
                Sucesso = true,
                Mensagem = resultado.Mensagem,
                TransacaoId = resultado.TransacaoId,
                LimiteRestante = resultado.LimiteRestante,
                StatusCode = 200
            });
        }
    }
}
