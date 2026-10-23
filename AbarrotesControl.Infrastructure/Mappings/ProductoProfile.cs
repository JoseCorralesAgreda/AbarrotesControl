using AbarrotesControl.Core.DTOs;
using AbarrotesControl.Core.Entities;
using AutoMapper;

namespace AbarrotesControl.Infrastructure.Mappings;

public class ProductoProfile : Profile
{
    public ProductoProfile()
    {
        CreateMap<CrearProductoDto, Producto>(MemberList.Source);
        CreateMap<Producto, ProductoDto>()
            .ForMember(x => x.CategoriaNombre, o => o.MapFrom(x => x.Categoria.Nombre));
    }
}
