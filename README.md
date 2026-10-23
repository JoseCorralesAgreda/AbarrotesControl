# AbarrotesControl

Aplicación local de registro y consulta de productos (CU-02) y entradas de mercadería (CU-03), con actualización transaccional del stock. Incluye una interfaz web adaptable a escritorio y móvil, servida por la misma API. Usa .NET 10, EF Core 9, Pomelo MySQL, AutoMapper y FluentValidation, organizada en Api, Core, Infrastructure y Services con repositorios.

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

El script crea la base y las seis tablas con sus índices y claves foráneas. No elimina datos ni modifica tablas existentes. No se utilizan migraciones ni creación automática del esquema desde la API.

El script no carga datos de ejemplo. Antes de registrar productos debe existir el usuario indicado por `UsuarioSemillaId` (por defecto, id 1) y una categoría. Para registrar entradas también deben existir un proveedor activo y productos activos.

## Configuración

En `AbarrotesControl.Api/appsettings.json`, configura estos valores conservando las demás opciones:

```json
{
  "ConnectionStrings": {
    "ConnectionMySql": "Server=localhost;Port=3306;Database=abarrotes_control;User=root;Password=CAMBIAR;"
  },
  "UsuarioSemillaId": 1
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

La aplicación usa el usuario semilla configurado; no incluye autenticación y está destinada al uso local del alcance CU-02/CU-03. Categorías, proveedores y usuario deben existir previamente en la base. Las pantallas avisan cuando la API no puede cargar los datos y permiten reintentar.

## Verificación

Con la aplicación en ejecución, utiliza PowerShell 7 desde la raíz:

```powershell
pwsh -File scripts/Verificar-Api.ps1
```

El script comprueba los archivos de la interfaz, las consultas y las respuestas de validación, sin crear ni modificar registros. Para comprobar altas exitosas y RN-01/RN-08 utiliza las peticiones HTTP con los ids adecuados de tu base. Una entrada confirmada incrementa el stock; ejecuta las pruebas de escritura en una base de pruebas.

Abre `AbarrotesControl.Api/AbarrotesControl.Api.http` en un cliente compatible para ejecutar las peticiones. Ajusta los ids a registros de tu base; usa los ids devueltos por los POST para las consultas individuales. El caso RN-01 debe ejecutarse después de crear el producto con el mismo nombre. El archivo incluye peticiones válidas por endpoint y casos inválidos para RN-01, RN-02, RN-03, RN-04, RN-06, RN-07, RN-08 y RN-11.
