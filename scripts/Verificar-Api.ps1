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
Write-Host "Resultado: $passed correctas; $failed fallidas."
if ($failed -gt 0) { exit 1 }
