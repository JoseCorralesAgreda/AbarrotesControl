using AbarrotesControl.Core.QueryFilters;
using System.Net;
using AutoMapper;
using AbarrotesControl.Core.DTOs;
using AbarrotesControl.Core.Entities;
using AbarrotesControl.Core.Exceptions;
using AbarrotesControl.Core.Interfaces;
using AbarrotesControl.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace AbarrotesControl.Services.Services;

public class StockService(IStockRepository repository, IConfiguration configuration, IMapper mapper) : IStockService
{
    private readonly int diasVencimiento = Math.Max(0, int.Parse(configuration["Alertas:DiasVencimiento"] ?? "15"));

    public List<AlertaStockDto> CalcularAlertas(Producto producto, DateOnly hoy)
    {
        var alertas = new List<AlertaStockDto>();
        if (!producto.Activo) return alertas;
        int? dias = producto.FechaVencimiento?.DayNumber - hoy.DayNumber;
        void Agregar(string tipo, string mensaje)
        {
            var alerta = mapper.Map<AlertaStockDto>(producto);
            alerta.Tipo = tipo;
            alerta.Mensaje = mensaje;
            alerta.DiasParaVencer = dias;
            alertas.Add(alerta);
        }
        if (producto.StockActual == 0) Agregar("SIN_STOCK", "Producto sin existencias.");
        if (producto.StockActual > 0 && producto.StockActual <= producto.StockMinimo)
            Agregar("STOCK_BAJO", "Stock igual o inferior al mínimo (RN-18).");
        if (dias is >= 0 && dias <= diasVencimiento)
            Agregar("POR_VENCER", $"El producto vence en {dias} días (RN-19).");
        if (dias is < 0) Agregar("VENCIDO", "El producto está vencido.");
        return alertas;
    }

    private StockDto ToDto(Producto producto, DateOnly hoy)
    {
        var dto = mapper.Map<StockDto>(producto);
        dto.DiasParaVencer = producto.FechaVencimiento?.DayNumber - hoy.DayNumber;
        dto.Alertas = CalcularAlertas(producto, hoy).Select(x => x.Tipo).ToList();
        return dto;
    }

    public async Task<List<StockDto>> GetAllAsync(StockQueryFilter filtro)
    {
        if (filtro.CategoriaId.HasValue && !await repository.ExisteCategoriaAsync(filtro.CategoriaId.Value))
            throw new BusinessException($"No existe la categoría con id {filtro.CategoriaId}.", HttpStatusCode.NotFound);
        var hoy = DateOnly.FromDateTime(DateTime.Today);
        var productos = await repository.GetAllAsync(filtro, hoy, diasVencimiento);
        return productos.Select(x => ToDto(x, hoy))
            .OrderByDescending(x => x.Alertas.Count > 0).ThenBy(x => x.Nombre).ThenBy(x => x.ProductoId).ToList();
    }

    public async Task<StockDto> GetByIdAsync(int productoId)
    {
        var producto = await repository.GetByIdAsync(productoId)
            ?? throw new BusinessException($"No existe el producto activo con id {productoId}.", HttpStatusCode.NotFound);
        return ToDto(producto, DateOnly.FromDateTime(DateTime.Today));
    }

    public async Task<ResumenAlertasDto> GetAlertasAsync(AlertaQueryFilter filtro)
    {
        var hoy = DateOnly.FromDateTime(DateTime.Today);
        var productos = await repository.GetAlertasAsync(filtro, hoy, diasVencimiento);
        var items = productos.OrderBy(x => x.Nombre).ThenBy(x => x.Id)
            .SelectMany(x => CalcularAlertas(x, hoy)).Where(x => filtro.Tipo is null || x.Tipo == filtro.Tipo).ToList();
        return new ResumenAlertasDto
        {
            Total = items.Count, SinStock = items.Count(x => x.Tipo == "SIN_STOCK"),
            StockBajo = items.Count(x => x.Tipo == "STOCK_BAJO"),
            PorVencer = items.Count(x => x.Tipo == "POR_VENCER"),
            Vencidos = items.Count(x => x.Tipo == "VENCIDO"), Items = items
        };
    }
}
