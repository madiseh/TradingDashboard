param(
    [Parameter(Mandatory = $true)]
    [string]$ProjectPath,
    [Parameter(Mandatory = $true)]
    [string]$OpenApiPath,
    [Parameter(Mandatory = $true)]
    [int]$Port,
    [int]$StartupTimeoutSeconds = 45
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$projectFullPath = [System.IO.Path]::GetFullPath($ProjectPath)
$openApiFullPath = [System.IO.Path]::GetFullPath($OpenApiPath)
$openApiDirectory = [System.IO.Path]::GetDirectoryName($openApiFullPath)
if ([string]::IsNullOrWhiteSpace($openApiDirectory)) {
    throw "Invalid OpenApiPath: $OpenApiPath"
}

New-Item -ItemType Directory -Force -Path $openApiDirectory | Out-Null

$baseUrl = "http://127.0.0.1:$Port"
$swaggerUrl = "$baseUrl/swagger/v1/swagger.json"
$runArguments = @(
    "run",
    "--no-build",
    "--project", $projectFullPath,
    "--urls", $baseUrl
)

$process = $null
try {
    $process = Start-Process -FilePath "dotnet" -ArgumentList $runArguments -PassThru

    $deadline = (Get-Date).AddSeconds($StartupTimeoutSeconds)
    $isReady = $false

    while ((Get-Date) -lt $deadline) {
        if ($process.HasExited) {
            throw "API process exited before swagger endpoint became available."
        }

        try {
            $probe = Invoke-WebRequest -UseBasicParsing -Uri $swaggerUrl -TimeoutSec 3
            if ($probe.StatusCode -ge 200 -and $probe.StatusCode -lt 300) {
                $isReady = $true
                break
            }
        }
        catch {
            Start-Sleep -Milliseconds 500
        }
    }

    if (-not $isReady) {
        throw "Swagger endpoint did not become ready within $StartupTimeoutSeconds seconds: $swaggerUrl"
    }

    $swaggerContent = (Invoke-WebRequest -UseBasicParsing -Uri $swaggerUrl -TimeoutSec 10).Content
    Set-Content -Path $openApiFullPath -Value $swaggerContent -Encoding UTF8
}
finally {
    if ($null -ne $process -and -not $process.HasExited) {
        Stop-Process -Id $process.Id -Force
    }
}
