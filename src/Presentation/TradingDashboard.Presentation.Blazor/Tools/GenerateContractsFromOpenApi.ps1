param(
    [Parameter(Mandatory = $true)]
    [string]$OpenApiPath,
    [Parameter(Mandatory = $true)]
    [string]$OutputDirectory,
    [Parameter(Mandatory = $true)]
    [string]$Namespace,
    [string[]]$PreserveFiles = @()
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Has-Prop {
    param($Object, [string]$Name)
    if ($null -eq $Object) { return $false }
    if ($Object -is [hashtable]) { return $Object.ContainsKey($Name) }
    return $null -ne $Object.PSObject.Properties[$Name]
}

function Get-Prop {
    param($Object, [string]$Name)
    if ($null -eq $Object) { return $null }
    if ($Object -is [hashtable]) {
        if ($Object.ContainsKey($Name)) { return $Object[$Name] }
        return $null
    }
    if ($null -eq $Object.PSObject.Properties[$Name]) { return $null }
    return $Object.$Name
}

function Get-ObjectPropertyNames {
    param($Object)
    if ($null -eq $Object) { return @() }
    if ($Object -is [hashtable]) { return @($Object.Keys) }
    return @($Object.PSObject.Properties.Name)
}

function Get-ObjectPropertyValue {
    param($Object, [string]$Name)
    if ($Object -is [hashtable]) { return $Object[$Name] }
    return $Object.$Name
}

function Convert-ToPascalCase {
    param([string]$Value)

    if ([string]::IsNullOrWhiteSpace($Value)) { return "Unnamed" }

    $parts = @([System.Text.RegularExpressions.Regex]::Split($Value, "[^a-zA-Z0-9]+") | Where-Object { $_ -ne "" })
    if (@($parts).Count -eq 0) { return "Unnamed" }

    $result = ($parts | ForEach-Object {
        if ($_.Length -eq 1) { $_.ToUpperInvariant() } else { $_.Substring(0,1).ToUpperInvariant() + $_.Substring(1) }
    }) -join ""

    if ([char]::IsDigit($result[0])) { return "N$result" }
    return $result
}

function Get-TypeName {
    param($Schema)

    if ($null -eq $Schema) { return @{ TypeName = "object"; IsValueType = $false } }

    if (Has-Prop $Schema '$ref') {
        $refName = ((Get-Prop $Schema '$ref') -split "/")[-1]
        return @{ TypeName = $refName; IsValueType = $false }
    }

    $schemaType = [string](Get-Prop $Schema "type")
    $schemaFormat = [string](Get-Prop $Schema "format")

    switch ($schemaType) {
        "integer" {
            if ($schemaFormat -eq "int64") { return @{ TypeName = "long"; IsValueType = $true } }
            return @{ TypeName = "int"; IsValueType = $true }
        }
        "number" {
            if ($schemaFormat -eq "float") { return @{ TypeName = "float"; IsValueType = $true } }
            return @{ TypeName = "double"; IsValueType = $true }
        }
        "boolean" { return @{ TypeName = "bool"; IsValueType = $true } }
        "string" {
            if ($schemaFormat -eq "uuid") { return @{ TypeName = "Guid"; IsValueType = $true } }
            if ($schemaFormat -eq "date-time") { return @{ TypeName = "DateTime"; IsValueType = $true } }
            return @{ TypeName = "string"; IsValueType = $false }
        }
        "array" {
            $itemType = Get-TypeName -Schema (Get-Prop $Schema "items")
            return @{ TypeName = "IReadOnlyList<$($itemType.TypeName)>"; IsValueType = $false }
        }
        "object" {
            if (Has-Prop $Schema "additionalProperties") {
                $additionalProperties = Get-Prop $Schema "additionalProperties"
                if ($additionalProperties -and $additionalProperties -ne $true) {
                    $valueType = Get-TypeName -Schema $additionalProperties
                    return @{ TypeName = "IReadOnlyDictionary<string, $($valueType.TypeName)>"; IsValueType = $false }
                }
            }
            return @{ TypeName = "object"; IsValueType = $false }
        }
        default {
            return @{ TypeName = "object"; IsValueType = $false }
        }
    }
}

function Get-PropertyLine {
    param(
        [string]$JsonName,
        $PropertySchema,
        [string[]]$RequiredPropertyNames
    )

    $propertyName = Convert-ToPascalCase -Value $JsonName
    $typeMeta = Get-TypeName -Schema $PropertySchema
    $typeName = [string]$typeMeta.TypeName
    $isValueType = [bool]$typeMeta.IsValueType

    $isRequired = $RequiredPropertyNames -contains $JsonName
    $isNullable = $false
    if (Has-Prop $PropertySchema "nullable") {
        $isNullable = [bool](Get-Prop $PropertySchema "nullable")
    }

    $finalTypeName = $typeName
    $initializer = ""

    if ($isValueType) {
        if ($isNullable) {
            $finalTypeName = "${typeName}?"
        }
    }
    else {
        if (-not $isRequired -or $isNullable) {
            $finalTypeName = "${typeName}?"
        }
        else {
            $initializer = " = default!;"
        }
    }

    return @(
        "    [JsonPropertyName(`"$JsonName`")]",
        "    public $finalTypeName $propertyName { get; set; }$initializer"
    )
}

if (-not (Test-Path $OpenApiPath)) {
    throw "OpenAPI file not found: $OpenApiPath"
}

New-Item -ItemType Directory -Force -Path $OutputDirectory | Out-Null

$preserveSet = @{}
foreach ($preserve in $PreserveFiles) {
    $preserveSet[$preserve] = $true
}

Get-ChildItem -Path $OutputDirectory -Filter *.cs -File | ForEach-Object {
    if (-not $preserveSet.ContainsKey($_.Name)) {
        Remove-Item $_.FullName -Force
    }
}

$openApi = Get-Content -Path $OpenApiPath -Raw | ConvertFrom-Json
if (-not (Has-Prop $openApi "components") -or -not (Has-Prop (Get-Prop $openApi "components") "schemas")) {
    throw "No components.schemas found in: $OpenApiPath"
}

$schemas = Get-Prop (Get-Prop $openApi "components") "schemas"
$schemaNames = Get-ObjectPropertyNames $schemas | Sort-Object

foreach ($schemaName in $schemaNames) {
    $schema = Get-ObjectPropertyValue -Object $schemas -Name $schemaName
    $typeName = Convert-ToPascalCase -Value ([string]$schemaName)

    $lines = New-Object System.Collections.Generic.List[string]
    $lines.Add("using System.Text.Json.Serialization;")
    $lines.Add("")
    $lines.Add("namespace $Namespace;")
    $lines.Add("")

    if (Has-Prop $schema "enum") {
        $lines.Add("public enum $typeName")
        $lines.Add("{")
        $enumValues = @((Get-Prop $schema "enum"))
        for ($i = 0; $i -lt @($enumValues).Count; $i++) {
            $memberName = Convert-ToPascalCase -Value ([string]$enumValues[$i])
            $suffix = if ($i -lt (@($enumValues).Count - 1)) { "," } else { "" }
            $lines.Add("    $memberName$suffix")
        }
        $lines.Add("}")
    }
    else {
        $lines.Add("public sealed class $typeName")
        $lines.Add("{")

        $properties = $null
        if (Has-Prop $schema "properties") {
            $properties = Get-Prop $schema "properties"
        }

        $requiredProps = @()
        if (Has-Prop $schema "required") {
            $requiredProps = @((Get-Prop $schema "required") | ForEach-Object { [string]$_ })
        }

        $propNames = @(Get-ObjectPropertyNames $properties | Sort-Object)
        if (@($propNames).Count -eq 0) {
            $lines.Add("}")
        }
        else {
            for ($i = 0; $i -lt @($propNames).Count; $i++) {
                $propName = [string]$propNames[$i]
                $propSchema = Get-ObjectPropertyValue -Object $properties -Name $propName
                $propLines = Get-PropertyLine -JsonName $propName -PropertySchema $propSchema -RequiredPropertyNames $requiredProps
                foreach ($propLine in $propLines) {
                    $lines.Add($propLine)
                }
                if ($i -lt (@($propNames).Count - 1)) {
                    $lines.Add("")
                }
            }
            $lines.Add("}")
        }
    }

    $targetPath = Join-Path $OutputDirectory "$typeName.cs"
    $content = ($lines -join [Environment]::NewLine) + [Environment]::NewLine
    Set-Content -Path $targetPath -Value $content -Encoding UTF8
}
