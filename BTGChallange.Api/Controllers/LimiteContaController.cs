﻿using BTGChallange.Service.Dtos;
using BTGChallange.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BTGChallange.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LimiteContaController : ControllerBase
    {
        private readonly IServicoLimiteConta _servicoLimiteConta;

        public LimiteContaController(IServicoLimiteConta servicoLimiteConta)
        {
            _servicoLimiteConta = servicoLimiteConta;
        }

        /// <summary>
        /// Cadastra um novo limite para uma conta
        /// </summary>
        [HttpPost("cadastrar")]
        public async Task<IActionResult> CadastrarLimite([FromBody] CadastrarLimiteDto dto)
        {
            var sucesso = await _servicoLimiteConta.CadastrarLimiteAsync(dto);

            if (!sucesso)
            {
                return Conflict(new
                {
                    Sucesso = false,
                    Mensagem = "Conta já possui limite cadastrado",
                    StatusCode = 409
                });
            }

            return CreatedAtAction(
                nameof(BuscarLimite),
                new { agencia = dto.Agencia, conta = dto.Conta },
                new
                {
                    Sucesso = true,
                    Mensagem = "Limite cadastrado com sucesso",
                    StatusCode = 201
                });
        }

        /// <summary>
        /// Busca o limite de uma conta específica
        /// </summary>
        [HttpGet("{agencia}/{conta}")]
        public async Task<IActionResult> BuscarLimite(string agencia, string conta)
        {
            var limite = await _servicoLimiteConta.BuscarLimiteAsync(agencia, conta);

            if (limite == null)
            {
                return NotFound(new
                {
                    Sucesso = false,
                    Mensagem = "Limite não encontrado para esta conta",
                    StatusCode = 404
                });
            }

            return Ok(new
            {
                Sucesso = true,
                Dados = limite,
                StatusCode = 200
            });
        }

        /// <summary>
        /// Atualiza o limite de uma conta existente
        /// </summary>
        [HttpPut("atualizar")]
        public async Task<IActionResult> AtualizarLimite([FromBody] AtualizarLimiteDto dto)
        {
            var sucesso = await _servicoLimiteConta.AtualizarLimiteAsync(dto);

            if (!sucesso)
            {
                return NotFound(new
                {
                    Sucesso = false,
                    Mensagem = "Conta não encontrada para atualização",
                    StatusCode = 404
                });
            }

            return Ok(new
            {
                Sucesso = true,
                Mensagem = "Limite atualizado com sucesso",
                StatusCode = 200
            });
        }

        /// <summary>
        /// Remove o limite de uma conta
        /// </summary>
        [HttpDelete("{agencia}/{conta}")]
        public async Task<IActionResult> RemoverLimite(string agencia, string conta)
        {
            var sucesso = await _servicoLimiteConta.RemoverLimiteAsync(agencia, conta);

            if (!sucesso)
            {
                return NotFound(new
                {
                    Sucesso = false,
                    Mensagem = "Conta não encontrada para remoção",
                    StatusCode = 404
                });
            }

            return Ok(new
            {
                Sucesso = true,
                Mensagem = "Limite removido com sucesso",
                StatusCode = 200
            });
        }

    }
}
