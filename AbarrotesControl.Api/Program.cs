using AbarrotesControl.Core.QueryFilters;
using AbarrotesControl.Api.Filters;
using AbarrotesControl.Core.DTOs;
using AbarrotesControl.Core.Interfaces;
using AbarrotesControl.Infrastructure.Data;
using AbarrotesControl.Infrastructure.Mappings;
using AbarrotesControl.Infrastructure.Repositories;
using AbarrotesControl.Services.Interfaces;
using AbarrotesControl.Services.Services;
using AbarrotesControl.Services.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AbarrotesControl.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        #region "Configurar la BD MySql"
        var connectionString = builder.Configuration.GetConnectionString("ConnectionMySql");
        builder.Services.AddDbContext<AbarrotesControlContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
        #endregion

        builder.Services.AddTransient<IProductoService, ProductoService>();
        builder.Services.AddTransient<ICategoriaService, CategoriaService>();
        builder.Services.AddTransient<IProveedorService, ProveedorService>();
        builder.Services.AddTransient<IEntradaService, EntradaService>();
        builder.Services.AddTransient<IVentaService, VentaService>();
        builder.Services.AddTransient<IStockService, StockService>();
        builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
        builder.Services.AddScoped<IEntradaRepository, EntradaRepository>();
        builder.Services.AddScoped<IVentaRepository, VentaRepository>();
        builder.Services.AddScoped<IStockRepository, StockRepository>();

        builder.Services.AddControllers(options =>
        {
            options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
            options.Filters.Add<ModelStateValidationFilter>();
        }).ConfigureApiBehaviorOptions(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

        builder.Services.AddOpenApi();
        builder.Services.AddAutoMapper(typeof(ProductoProfile).Assembly);

        builder.Services.AddScoped<IValidator<CrearProductoDto>, CrearProductoDtoValidator>();
        builder.Services.AddScoped<IValidator<CrearEntradaDto>, CrearEntradaDtoValidator>();
        builder.Services.AddScoped<IValidator<CrearDetalleEntradaDto>, CrearDetalleEntradaDtoValidator>();
        builder.Services.AddScoped<IValidator<CrearVentaDto>, CrearVentaDtoValidator>();
        builder.Services.AddScoped<IValidator<CrearDetalleVentaDto>, CrearDetalleVentaDtoValidator>();

        builder.Services.AddScoped<IValidator<int>, IdParametroValidator>();
        builder.Services.AddScoped<IValidator<ProductoQueryFilter>, ProductoQueryFilterValidator>();
        builder.Services.AddScoped<IValidator<StockQueryFilter>, StockQueryFilterValidator>();
        builder.Services.AddScoped<IValidator<AlertaQueryFilter>, AlertaQueryFilterValidator>();
        builder.Services.AddScoped<IValidator<VentaQueryFilter>, VentaQueryFilterValidator>();

        var app = builder.Build();

        app.UseMiddleware<ExceptionHandlingMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
        app.UseDefaultFiles();
        app.UseStaticFiles();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}
