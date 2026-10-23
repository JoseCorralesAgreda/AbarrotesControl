using AbarrotesControl.Infrastructure.Data;
using AbarrotesControl.Core.Interfaces;
using AbarrotesControl.Infrastructure.Repositories;
using AbarrotesControl.Services.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("ConnectionMySql");
builder.Services.AddDbContext<AbarrotesControlContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddControllers(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true)
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context => new BadRequestObjectResult(new
        {
            message = "Error de validación",
            errors = context.ModelState
                .SelectMany(entry => entry.Value!.Errors.Select(_ => new
                {
                    field = entry.Key,
                    error = "El valor enviado no es válido para este campo."
                }))
        });
    });
builder.Services.AddOpenApi();
builder.Services.AddAutoMapper(typeof(AbarrotesControlContext).Assembly);
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IEntradaRepository, EntradaRepository>();
builder.Services.AddScoped<IProveedorRepository, ProveedorRepository>();
builder.Services.AddValidatorsFromAssemblyContaining<CrearProductoDtoValidator>();

var app = builder.Build();

app.UseExceptionHandler(handler => handler.Run(async context =>
{
    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
    await context.Response.WriteAsJsonAsync(new { message = "Ocurrió un error interno al procesar la solicitud." });
}));

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

app.Run();
