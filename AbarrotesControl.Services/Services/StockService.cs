using System.Net;
using AbarrotesControl.Core.DTOs;
using AbarrotesControl.Core.Entities;
using AbarrotesControl.Core.Exceptions;
using AbarrotesControl.Core.Interfaces;
using AbarrotesControl.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace AbarrotesControl.Services.Services;

public class StockService(IStockRepository repository, IConfiguration configuration) : IStockService
{
    private static readonly string[] Tipos = ["SIN_STOCK", "STOCK_BAJO", "POR_VENCER", "VENCIDO"];
    private readonly int diasVencimiento = Math.Max(0, int.Parse(configuration["Alertas:DiasVencimiento"] ?? "15"));

    public List<AlertaStockDto> CalcularAlertas(Producto producto, DateOnly hoy)
    {
        var alertas = new List<AlertaStockDto>();
        if (!producto.Activo) return alertas;
        int? dias = producto.FechaVencimiento?.DayNumber - hoy.DayNumber;
        void Agregar(string tipo, string mensaje) => alertas.Add(new AlertaStockDto
        {
            ProductoId = producto.Id, ProductoNombre = producto.Nombre, Tipo = tipo,
            StockActual = producto.StockActual, StockMinimo = producto.StockMinimo,
            FechaVencimiento = producto.FechaVencimiento, DiasParaVencer = dias, Mensaje = mensaje
        });
        if (producto.StockActual == 0) Agregar("SIN_STOCK", "Producto sin existencias.");
        if (producto.StockActual > 0 && producto.StockActual <= producto.StockMinimo)
            Agregar("STOCK_BAJO", "Stock igual o inferior al mínimo (RN-18).");
        if (dias is >= 0 && dias <= diasVencimiento)
            Agregar("POR_VENCER", $"El producto vence en {dias} días (RN-19).");
        if (dias is < 0) Agregar("VENCIDO", "El producto está vencido.");
        return alertas;
    }

    private StockDto ToDto(Producto producto, DateOnly hoy) => new()
    {
        ProductoId = producto.Id, Nombre = producto.Nombre, CategoriaId = producto.CategoriaId,
        Categoria = producto.Categoria.Nombre, PrecioVenta = producto.PrecioVenta,
        StockActual = producto.StockActual, StockMinimo = producto.StockMinimo,
        FechaVencimiento = producto.FechaVencimiento,
        DiasParaVencer = producto.FechaVencimiento?.DayNumber - hoy.DayNumber,
        Alertas = CalcularAlertas(producto, hoy).Select(x => x.Tipo).ToList()
    };

    public async Task<List<StockDto>> GetAllAsync(int? categoriaId = null, string? buscar = null, bool soloAlertas = false)
    {
        if (categoriaId.HasValue && !await repository.ExisteCategoriaAsync(categoriaId.Value))
            throw new BusinessException($"No existe la categoría con id {categoriaId}.", HttpStatusCode.NotFound);
        var productos = await repository.GetAllAsync(categoriaId, buscar);
        var hoy = DateOnly.FromDateTime(DateTime.Today);
        return productos.Select(x => ToDto(x, hoy)).Where(x => !soloAlertas || x.Alertas.Count > 0)
            .OrderByDescending(x => x.Alertas.Count > 0).ThenBy(x => x.Nombre).ThenBy(x => x.ProductoId).ToList();
    }

    public async Task<StockDto> GetByIdAsync(int productoId)
    {
        var producto = await repository.GetByIdAsync(productoId)
            ?? throw new BusinessException($"No existe el producto activo con id {productoId}.", HttpStatusCode.NotFound);
        return ToDto(producto, DateOnly.FromDateTime(DateTime.Today));
    }

    public async Task<ResumenAlertasDto> GetAlertasAsync(string? tipo = null)
    {
        if (tipo is not null && !Tipos.Contains(tipo))
            throw new BusinessException("Tipo inválido. Use SIN_STOCK, STOCK_BAJO, POR_VENCER o VENCIDO.");
        var productos = await repository.GetAllAsync(null, null);
        var hoy = DateOnly.FromDateTime(DateTime.Today);
        var items = productos.OrderBy(x => x.Nombre).ThenBy(x => x.Id)
            .SelectMany(x => CalcularAlertas(x, hoy)).Where(x => tipo is null || x.Tipo == tipo).ToList();
        return new ResumenAlertasDto
        {
            Total = items.Count, SinStock = items.Count(x => x.Tipo == "SIN_STOCK"),
            StockBajo = items.Count(x => x.Tipo == "STOCK_BAJO"),
            PorVencer = items.Count(x => x.Tipo == "POR_VENCER"),
            Vencidos = items.Count(x => x.Tipo == "VENCIDO"), Items = items
        };
    }
}
