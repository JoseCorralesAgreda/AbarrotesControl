using AbarrotesControl.Core.DTOs;
using AbarrotesControl.Core.Entities;
using AutoMapper;

namespace AbarrotesControl.Infrastructure.Mappings;

public class EntradaProfile : Profile
{
    public EntradaProfile()
    {
        CreateMap<CrearEntradaDto, EntradaMercaderia>(MemberList.Source);
        CreateMap<CrearDetalleEntradaDto, DetalleEntrada>(MemberList.Source);
        CreateMap<EntradaMercaderia, EntradaDto>()
            .ForMember(x => x.ProveedorNombre, o => o.MapFrom(x => x.Proveedor.Nombre));
        CreateMap<DetalleEntrada, DetalleEntradaDto>()
            .ForMember(x => x.ProductoNombre, o => o.MapFrom(x => x.Producto.Nombre))
            .ForMember(x => x.StockActual, o => o.MapFrom(x => x.Producto.StockActual));
        CreateMap<Proveedor, ProveedorDto>();
    }
}
