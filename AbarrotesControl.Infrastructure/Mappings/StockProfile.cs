using AbarrotesControl.Core.DTOs;
using AbarrotesControl.Core.Entities;
using AutoMapper;

namespace AbarrotesControl.Infrastructure.Mappings;

public class StockProfile : Profile
{
    public StockProfile()
    {
        CreateMap<Producto, StockDto>()
            .ForMember(x => x.ProductoId, o => o.MapFrom(x => x.Id))
            .ForMember(x => x.Categoria, o => o.MapFrom(x => x.Categoria.Nombre))
            .ForMember(x => x.Alertas, o => o.Ignore())
            .ForMember(x => x.DiasParaVencer, o => o.Ignore());
        CreateMap<Producto, AlertaStockDto>()
            .ForMember(x => x.ProductoId, o => o.MapFrom(x => x.Id))
            .ForMember(x => x.ProductoNombre, o => o.MapFrom(x => x.Nombre))
            .ForMember(x => x.Tipo, o => o.Ignore())
            .ForMember(x => x.Mensaje, o => o.Ignore())
            .ForMember(x => x.DiasParaVencer, o => o.Ignore());
    }
}
