using AutoMapper;
using BTGChallange.Domain.Entidades;
using BTGChallange.Service.Dtos;

namespace Integrador.Service.Mapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {            
            CreateMap<CadastrarLimiteDto, LimiteContaCorrente>()
                .ConstructUsing(src => new LimiteContaCorrente(src.Documento, src.Agencia, src.Conta, src.LimitePix));

        }
    }
}
