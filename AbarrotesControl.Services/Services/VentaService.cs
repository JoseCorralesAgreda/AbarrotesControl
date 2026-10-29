using System.Net;
using AbarrotesControl.Core.DTOs;
using AbarrotesControl.Core.Entities;
using AbarrotesControl.Core.Exceptions;
using AbarrotesControl.Core.Interfaces;
using AbarrotesControl.Services.Interfaces;
using AutoMapper;
using FluentValidation;

namespace AbarrotesControl.Services.Services;

public class VentaService(IVentaRepository ventas, IProductoRepository productos, IStockService stock,
    IValidator<CrearVentaDto> validator, IMapper mapper) : IVentaService
{
    private VentaDto ToDto(Venta venta)
    {
        var dto = mapper.Map<VentaDto>(venta);
        var hoy = DateOnly.FromDateTime(DateTime.Today);
        dto.Alertas = venta.Detalles.SelectMany(x => stock.CalcularAlertas(x.Producto, hoy))
            .Where(x => x.Tipo is "STOCK_BAJO" or "SIN_STOCK").ToList();
        return dto;
    }

    public async Task<VentaDto> GetByIdAsync(int id) => ToDto(await ventas.GetByIdAsync(id)
        ?? throw new BusinessException($"No existe la venta con id {id}.", HttpStatusCode.NotFound));

    public async Task<List<VentaDto>> GetAllAsync() => (await ventas.GetAllAsync()).Select(ToDto).ToList();

    public async Task<VentaDto> InsertAsync(Venta venta)
    {
        var validation = await validator.ValidateAsync(mapper.Map<CrearVentaDto>(venta));
        if (!validation.IsValid)
            throw new BusinessException("Error de validación", details:
                validation.Errors.Select(e => new { field = e.PropertyName, error = e.ErrorMessage }));

        var problemas = new List<object>();
        var idsInvalidos = new List<int>();
        var hoy = DateOnly.FromDateTime(DateTime.Today);
        foreach (var detalle in venta.Detalles)
        {
            var producto = await productos.GetByIdAsync(detalle.ProductoId);
            void Problema(string motivo) => problemas.Add(new
            {
                productoId = detalle.ProductoId, producto = producto?.Nombre, motivo,
                disponible = producto?.StockActual ?? 0, solicitado = detalle.Cantidad
            });
            if (producto is null || !producto.Activo)
            {
                idsInvalidos.Add(detalle.ProductoId);
                Problema("Producto inexistente o inactivo (RN-14)");
                continue;
            }
            if (producto.StockActual < detalle.Cantidad) Problema("Stock insuficiente (RN-13)");
            if (producto.FechaVencimiento < hoy) Problema("Producto vencido (RN-14)");
            detalle.PrecioUnitario = producto.PrecioVenta;
            detalle.Subtotal = detalle.Cantidad * producto.PrecioVenta;
        }
        // Los ids inexistentes/inactivos tienen prioridad 404, conservando todos los problemas.
        if (idsInvalidos.Count > 0)
            throw new BusinessException($"Productos inexistentes o inactivos: {string.Join(", ", idsInvalidos)}.",
                HttpStatusCode.NotFound, problemas);
        if (problemas.Count > 0)
            throw new BusinessException("No se puede registrar la venta.", HttpStatusCode.Conflict, problemas);
        venta.Total = venta.Detalles.Sum(x => x.Subtotal);
        return ToDto(await ventas.InsertAsync(venta));
    }
}
