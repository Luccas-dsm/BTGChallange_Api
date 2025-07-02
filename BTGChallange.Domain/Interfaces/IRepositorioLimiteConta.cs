using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BTGChallange.Domain.Entidades;

namespace BTGChallange.Domain.Interfaces
{
    public interface IRepositorioLimiteConta
    {
        Task<LimiteContaCorrente?> ObterPorContaAsync(string agencia, string conta);
        Task<bool> CadastrarAsync(LimiteContaCorrente limite);
        Task<bool> AtualizarAsync(LimiteContaCorrente limite);
        Task<bool> RemoverAsync(string agencia, string conta);
    }
}
