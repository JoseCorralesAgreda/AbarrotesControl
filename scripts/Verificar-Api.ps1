param([string]$BaseUrl = 'https://localhost:7091')

# Requiere PowerShell 7. No crea ni modifica registros.
$ErrorActionPreference = 'Stop'
$passed = 0
$failed = 0

function Test-Endpoint {
    param([string]$Name, [string]$Path, [int]$Status = 200, [object]$Body = $null, [switch]$Validation)
    try {
        $request = @{
            Uri = "$BaseUrl$Path"
            Method = 'GET'
            SkipCertificateCheck = $true
            SkipHttpErrorCheck = $true
            TimeoutSec = 20
        }
        if ($null -ne $Body) {
            $request.Method = 'POST'
            $request.ContentType = 'application/json; charset=utf-8'
            $request.Body = $Body | ConvertTo-Json -Depth 10 -Compress
        }
        $response = Invoke-WebRequest @request
        if ([int]$response.StatusCode -ne $Status) {
            throw "Se esperaba HTTP $Status, se obtuvo $($response.StatusCode)."
        }
        if ($Status -ge 400) {
            $errorResponse = $response.Content | ConvertFrom-Json
            if ($errorResponse.status -ne $Status -or -not $errorResponse.title -or
                -not $errorResponse.message -or -not $errorResponse.traceId -or
                'errors' -notin $errorResponse.PSObject.Properties.Name) {
                throw 'La respuesta no cumple el contrato ErrorResponse.'
            }
        }
        if ($Status -eq 200 -and $Path.StartsWith('/api/')) {
            $success = $response.Content | ConvertFrom-Json
            if ('data' -notin $success.PSObject.Properties.Name) {
                throw 'La respuesta no cumple el contrato ApiResponse<T>.'
            }
        }
        if ($Validation) {
            $json = $response.Content | ConvertFrom-Json
            if ($json.message -ne 'Error de validación' -or $json.errors.Count -lt 1) {
                throw 'La respuesta no contiene el formato de validación esperado.'
            }
            foreach ($item in $json.errors) {
                if (-not $item.field -or -not $item.error) { throw 'Error de validación sin field/error.' }
            }
        }
        Write-Host "OK  $Name"
        $script:passed++
    }
    catch {
        Write-Host "FAIL $Name : $($_.Exception.Message)"
        $script:failed++
    }
}

Test-Endpoint 'Interfaz web' '/'
Test-Endpoint 'JavaScript' '/app.js'
Test-Endpoint 'Estilos' '/styles.css'
Test-Endpoint 'Listado de productos' '/api/productos'
Test-Endpoint 'Productos por nombre y categoría' '/api/productos?nombre=Arroz&categoriaId=1'
Test-Endpoint 'Productos activos' '/api/productos?activo=true'
Test-Endpoint 'Productos inactivos' '/api/productos?activo=false'
Test-Endpoint 'Stock por búsqueda' '/api/stock?buscar=Arroz'
Test-Endpoint 'Ventas por rango inclusivo' '/api/ventas?desde=2026-01-01&hasta=2026-12-31'
Test-Endpoint 'Ventas desde fecha' '/api/ventas?desde=2026-01-01'
Test-Endpoint 'Ventas hasta fecha' '/api/ventas?hasta=2026-12-31'
foreach ($ruta in @('productos', 'entradas', 'ventas', 'stock')) {
    Test-Endpoint "Id cero en $ruta" "/api/$ruta/0" 400 -Validation
    Test-Endpoint "Id negativo en $ruta" "/api/$ruta/-1" 400 -Validation
}
foreach ($ruta in @('productos', 'stock')) {
    Test-Endpoint "Categoría negativa en $ruta" "/api/${ruta}?categoriaId=-1" 400 -Validation
    Test-Endpoint "Categoría no convertible en $ruta" "/api/${ruta}?categoriaId=XYZ" 400 -Validation
}
$textoLargo = 'a' * 121
Test-Endpoint 'Nombre mayor a 120' "/api/productos?nombre=$textoLargo" 400 -Validation
Test-Endpoint 'Búsqueda mayor a 120' "/api/stock?buscar=$textoLargo" 400 -Validation
Test-Endpoint 'Estado no convertible' '/api/productos?activo=XYZ' 400 -Validation
Test-Endpoint 'Tipo XYZ' '/api/alertas?tipo=XYZ' 400 -Validation
Test-Endpoint 'Rango invertido' '/api/ventas?desde=2026-12-31&hasta=2026-01-01' 400 -Validation
Test-Endpoint 'Fecha no convertible' '/api/ventas?desde=XYZ' 400 -Validation
Test-Endpoint 'Listado de categorías' '/api/categorias'
Test-Endpoint 'Listado de proveedores' '/api/proveedores'
Test-Endpoint 'Producto inexistente' '/api/productos/2147483647' 404
Test-Endpoint 'Entrada inexistente' '/api/entradas/2147483647' 404

$product = @{ nombre = 'Validación sin escritura'; categoriaId = 1; precioVenta = 10; stockMinimo = 0; fechaVencimiento = $null }
foreach ($case in @(
    @{ Name = 'RN-02 precio cero'; Field = 'precioVenta'; Value = 0 },
    @{ Name = 'RN-03 stock negativo'; Field = 'stockMinimo'; Value = -1 },
    @{ Name = 'RN-04 vencimiento pasado'; Field = 'fechaVencimiento'; Value = '2000-01-01' },
    @{ Name = 'Nombre obligatorio'; Field = 'nombre'; Value = '' },
    @{ Name = 'Precio fuera de precisión'; Field = 'precioVenta'; Value = 100000000 }
)) {
    $body = $product.Clone()
    $body[$case.Field] = $case.Value
    Test-Endpoint $case.Name '/api/productos' 400 $body -Validation
}

$entry = @{ proveedorId = 1; fechaEntrada = '2026-01-01'; detalles = @(@{ productoId = 1; cantidad = 1; costoUnitario = 1 }) }
Test-Endpoint 'RN-06 detalles vacíos' '/api/entradas' 400 @{ proveedorId = 1; fechaEntrada = '2026-01-01'; detalles = @() } -Validation
Test-Endpoint 'RN-06 producto repetido' '/api/entradas' 400 @{ proveedorId = 1; fechaEntrada = '2026-01-01'; detalles = @($entry.detalles[0], $entry.detalles[0]) } -Validation
foreach ($case in @(
    @{ Name = 'RN-06 cantidad cero'; Field = 'cantidad'; Value = 0 },
    @{ Name = 'RN-06 cantidad fraccionaria'; Field = 'cantidad'; Value = 1.5 },
    @{ Name = 'RN-07 costo cero'; Field = 'costoUnitario'; Value = 0 },
    @{ Name = 'Costo fuera de precisión'; Field = 'costoUnitario'; Value = 100000000 }
)) {
    $detail = $entry.detalles[0].Clone()
    $detail[$case.Field] = $case.Value
    Test-Endpoint $case.Name '/api/entradas' 400 @{ proveedorId = 1; fechaEntrada = '2026-01-01'; detalles = @($detail) } -Validation
}
Test-Endpoint 'RN-11 fecha futura' '/api/entradas' 400 @{ proveedorId = 1; fechaEntrada = '9999-12-31'; detalles = $entry.detalles } -Validation
Test-Endpoint 'Fecha omitida' '/api/entradas' 400 @{ proveedorId = 1; detalles = $entry.detalles } -Validation
# CU-04/CU-05: consultas y rechazos seguros; no registra ventas exitosas.
Test-Endpoint 'Últimas ventas' '/api/ventas'
Test-Endpoint 'Venta inexistente' '/api/ventas/2147483647' 404
Test-Endpoint 'Stock activo' '/api/stock'
Test-Endpoint 'Stock con alertas' '/api/stock?soloAlertas=true'
Test-Endpoint 'Stock categoría inexistente' '/api/stock?categoriaId=2147483647' 404
Test-Endpoint 'Stock producto inexistente' '/api/stock/2147483647' 404
Test-Endpoint 'Stock filtro inválido' '/api/stock?soloAlertas=incorrecto' 400 -Validation
Test-Endpoint 'Todas las alertas' '/api/alertas'
foreach ($tipo in @('SIN_STOCK', 'STOCK_BAJO', 'POR_VENCER', 'VENCIDO')) {
    Test-Endpoint "Alertas $tipo" "/api/alertas?tipo=$tipo"
}
Test-Endpoint 'Tipo de alerta inválido' '/api/alertas?tipo=OTRO' 400 -Validation
Test-Endpoint 'RN-12 detalles vacíos' '/api/ventas' 400 @{ detalles = @() } -Validation
Test-Endpoint 'RN-12 detalles nulos' '/api/ventas' 400 @{ detalles = $null } -Validation
Test-Endpoint 'RN-12 línea nula' '/api/ventas' 400 @{ detalles = @($null) } -Validation
foreach ($cantidad in @(0, -1, 1.5)) {
    Test-Endpoint "RN-12 cantidad $cantidad" '/api/ventas' 400 @{
        detalles = @(@{ productoId = 1; cantidad = $cantidad })
    } -Validation
}
Test-Endpoint 'RN-12 producto repetido' '/api/ventas' 400 @{
    detalles = @(@{ productoId = 1; cantidad = 1 }, @{ productoId = 1; cantidad = 2 })
} -Validation
Test-Endpoint 'RN-14 producto inexistente' '/api/ventas' 404 @{
    detalles = @(@{ productoId = 2147483647; cantidad = 1 })
}
try {
    $stockResponse = Invoke-RestMethod "$BaseUrl/api/stock" -SkipCertificateCheck
    $productos = @($stockResponse.data)
    $activo = $productos | Select-Object -First 1
    if ($null -ne $activo) {
        Test-Endpoint 'Stock por producto' "/api/stock/$($activo.productoId)"
        Test-Endpoint 'Stock por categoría' "/api/stock?categoriaId=$($activo.categoriaId)"
    }
    # SIN_STOCK o VENCIDO garantizan rechazo aun si otra venta consume existencias.
    # RN-13 se prueba junto a un id inexistente para impedir cualquier escritura
    # incluso si hay reposiciones concurrentes; debe conservar el problema RN-13.
    $insuficiente = $productos | Where-Object { $_.stockActual -lt [int]::MaxValue } | Select-Object -First 1
    if ($null -ne $insuficiente) {
        $body = @{ detalles = @(
            @{ productoId = $insuficiente.productoId; cantidad = [int]::MaxValue },
            @{ productoId = 2147483647; cantidad = 1 }
        ) } | ConvertTo-Json -Depth 10
        $response = Invoke-WebRequest "$BaseUrl/api/ventas" -Method POST -Body $body `
            -ContentType 'application/json' -SkipCertificateCheck -SkipHttpErrorCheck
        $errorBody = $response.Content | ConvertFrom-Json
        if ([int]$response.StatusCode -ne 404 -or
            @($errorBody.errors | Where-Object { $_.motivo -like '*RN-13*' }).Count -eq 0) {
            throw 'No se acumularon los problemas de stock y producto inexistente.'
        }
        Write-Host 'OK  RN-13 y RN-14: acumulación de problemas sin escritura'
        $passed++
    } else { Write-Host 'SKIP RN-13: no hay productos activos con stock menor al máximo entero.' }
    $vencido = $productos | Where-Object { 'VENCIDO' -in $_.alertas } | Select-Object -First 1
    if ($null -ne $vencido) {
        Test-Endpoint 'RN-14 producto vencido' '/api/ventas' 409 @{
            detalles = @(@{ productoId = $vencido.productoId; cantidad = 1 })
        }
    } else { Write-Host 'SKIP RN-14 vencido: no hay productos activos vencidos.' }
    $ventas = Invoke-RestMethod "$BaseUrl/api/ventas" -SkipCertificateCheck
    if (@($ventas.data).Count -gt 50) { throw 'El listado devuelve más de 50 ventas.' }
    if (@($ventas.data).Count -gt 0) {
        Test-Endpoint 'Consulta de venta existente' "/api/ventas/$($ventas.data[0].id)"
    } else { Write-Host 'SKIP consulta individual de venta: no existen ventas.' }
} catch {
    Write-Host "FAIL Verificación CU-04/CU-05: $($_.Exception.Message)"
    $failed++
}
Write-Host "Resultado: $passed correctas; $failed fallidas."
if ($failed -gt 0) { exit 1 }
