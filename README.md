# AbarrotesControl

Aplicación local de registro y consulta de productos (CU-02), entradas de mercadería (CU-03), ventas (CU-04) y stock y alertas (CU-05), con actualización transaccional del stock. Incluye una interfaz web para productos y entradas. Usa .NET 10, EF Core 9, Pomelo MySQL, AutoMapper y FluentValidation.

## Estructura en capas

- **Api:** controllers delgados, respuestas `ApiResponse<T>`, configuración de dependencias y middleware de excepciones. El controller de ventas asigna usuario semilla y fecha del servidor.
- **Core:** entidades basadas en `BaseEntity`, DTOs, contratos de repositorios y `BusinessException`.
- **Infrastructure:** contexto y mapeos EF, perfiles AutoMapper y repositorios basados en `BaseRepository<T>`/`IBaseRepository<T>`. Venta y detalles, descuento condicional de stock y movimientos VENTA se confirman en una sola transacción.
- **Services:** interfaces, servicios con reglas de negocio y validators. `StockService` centraliza todas las alertas; las consultas de stock usan `AsNoTracking`.

## Base de datos

Se requiere MySQL y el SDK de .NET 10. Si la base `abarrotes_control` ya existe con las tablas del proyecto, utiliza esa base directamente.

Para una instalación nueva, abre una terminal en la raíz del repositorio:

```sh
mysql -u root -p
```

Dentro del cliente MySQL ejecuta:

```sql
SOURCE abarrotes_control.sql;
```

El script disponible contiene seis tablas del alcance inicial. La base existente del proyecto ya tiene las diez tablas, incluidas `ventas`, `detalle_venta`, `ajustes_stock` y `movimientos_stock`; CU-04 requiere ese esquema completo. El script inicial por sí solo no prepara una instalación nueva para CU-04. Si la base ya existe, no vuelvas a ejecutarlo. No se utilizan migraciones ni creación automática del esquema desde la API.

El script no carga datos de ejemplo. Antes de registrar productos debe existir el usuario indicado por `UsuarioSemillaId` (por defecto, id 1) y una categoría. Para registrar entradas también deben existir un proveedor activo y productos activos.

## Configuración

En `AbarrotesControl.Api/appsettings.json`, configura estos valores conservando las demás opciones:

```json
{
  "ConnectionStrings": {
    "ConnectionMySql": "Server=localhost;Port=3306;Database=abarrotes_control;User=root;Password=CAMBIAR;"
  },
  "UsuarioSemillaId": 1,
  "Alertas": { "DiasVencimiento": 15 }
}
```

Sustituye `CAMBIAR` por tu contraseña de MySQL. No es necesario modificar `appsettings.Development.json`. MySQL debe estar disponible al utilizar los endpoints que acceden a la base, porque la configuración usa `ServerVersion.AutoDetect`.

## Ejecutar la API

Desde la raíz:

```sh
dotnet restore AbarrotesControl.slnx
dotnet dev-certs https --trust
dotnet run --project AbarrotesControl.Api --launch-profile https
```

La API escucha en `https://localhost:7091`. En Development, el documento OpenAPI está disponible en `https://localhost:7091/openapi/v1.json`.

Abre **https://localhost:7091/** para usar la aplicación:

- **Inventario:** búsqueda por nombre, filtro de categoría, productos con stock bajo y resumen de existencias.
- **Nuevo producto:** registro con categoría, precio, stock mínimo y vencimiento opcional.
- **Registrar entrada:** proveedor, fecha, documento opcional y varios productos con cantidad y costo. Se muestra el total y el comprobante de la entrada guardada.
- **Consultar entrada:** consulta por el número generado al guardar, con el stock actual de los productos.

La aplicación usa el usuario semilla configurado; no incluye autenticación y está destinada al uso local del alcance CU-02 a CU-05. Categorías, proveedores y usuario deben existir previamente en la base. Las pantallas avisan cuando la API no puede cargar los datos y permiten reintentar. CU-04 y CU-05 se consultan mediante los endpoints siguientes.

## Endpoints

| Método | Ruta | Función |
| --- | --- | --- |
| GET / POST | `/api/productos` | Listar / registrar productos |
| GET | `/api/productos/{id}` | Consultar producto |
| GET | `/api/categorias` | Listar categorías |
| GET | `/api/proveedores` | Listar proveedores activos |
| POST | `/api/entradas` | Registrar entrada |
| GET | `/api/entradas/{id}` | Consultar entrada |
| POST | `/api/ventas` | Registrar venta (201) |
| GET | `/api/ventas/{id}` | Consultar venta |
| GET | `/api/ventas` | Últimas 50 ventas, más recientes primero |
| GET | `/api/stock?categoriaId=&buscar=&soloAlertas=false` | Stock de productos activos, primero los que tienen alertas |
| GET | `/api/stock/{productoId}` | Stock y alertas de un producto activo |
| GET | `/api/alertas?tipo=SIN_STOCK` | Resumen; tipo opcional: SIN_STOCK, STOCK_BAJO, POR_VENCER, VENCIDO |

La venta recibe únicamente `detalles: [{ productoId, cantidad }]`; los precios se toman del producto. No admite modificación ni eliminación. RN-12 rechaza listas vacías, cantidades no enteras o no positivas y productos repetidos (400). RN-13/RN-14 acumulan todos los problemas de stock y vencimiento (409); si hay ids inexistentes o inactivos, prevalece 404 y se conservan los demás problemas. El middleware publica los `details` de `BusinessException` en `errors`, con objetos `{ productoId, producto, motivo, disponible, solicitado }`.

SIN_STOCK significa cero existencias; STOCK_BAJO, stock positivo menor o igual al mínimo. POR_VENCER incluye hoy y el día límite configurado; VENCIDO significa fecha anterior a hoy. Un producto puede generar varias alertas. Los contadores del resumen cuentan alertas del filtro aplicado, no productos únicos. Las alertas de una venta reflejan el stock consultado al devolverla; no son un historial de alertas.

## Verificación

Con la aplicación en ejecución, utiliza PowerShell 7 desde la raíz:

```powershell
pwsh -File scripts/Verificar-Api.ps1
```

El script comprueba los archivos de la interfaz, consultas CU-02 a CU-05 y respuestas de validación, sin crear registros. RN-13 se verifica junto con un producto inexistente para garantizar el rechazo sin escritura; las verificaciones que requieren productos vencidos o ventas existentes se omiten explícitamente si faltan esos datos. Para comprobar altas exitosas y el 409 aislado de RN-13 utiliza las peticiones HTTP con los ids adecuados en una base de pruebas. Una entrada confirmada incrementa stock y una venta lo descuenta.

Abre `AbarrotesControl.Api/AbarrotesControl.Api.http` en un cliente compatible para ejecutar las peticiones. Ajusta los ids a registros de tu base; usa los ids devueltos por los POST para las consultas individuales. El caso RN-01 debe ejecutarse después de crear el producto con el mismo nombre. El archivo incluye peticiones válidas por endpoint y casos inválidos para RN-01, RN-02, RN-03, RN-04, RN-06, RN-07, RN-08, RN-11, RN-12, RN-13, RN-14 y filtros de stock/alertas.
