using AbarrotesControl.Core.DTOs;
using AbarrotesControl.Core.Entities;
using AutoMapper;

namespace AbarrotesControl.Infrastructure.Mappings;

public class VentaProfile : Profile
{
    public VentaProfile()
    {
        CreateMap<CrearVentaDto, Venta>();
        CreateMap<CrearDetalleVentaDto, DetalleVenta>();
        CreateMap<Venta, CrearVentaDto>();
        CreateMap<DetalleVenta, CrearDetalleVentaDto>();
        CreateMap<Venta, VentaDto>().ForMember(x => x.Alertas, opt => opt.Ignore());
        CreateMap<DetalleVenta, DetalleVentaDto>()
            .ForMember(x => x.ProductoNombre, opt => opt.MapFrom(x => x.Producto.Nombre));
    }
}
