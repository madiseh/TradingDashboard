using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

var options = ParseArgs(args);

if (!File.Exists(options.OpenApiPath))
{
    throw new FileNotFoundException($"OpenAPI file not found: {options.OpenApiPath}");
}

Directory.CreateDirectory(options.OutputDirectory);

var preserveSet = new HashSet<string>(options.PreserveFiles, StringComparer.OrdinalIgnoreCase);
foreach (var file in Directory.GetFiles(options.OutputDirectory, "*.cs", SearchOption.TopDirectoryOnly))
{
    var name = Path.GetFileName(file);
    if (!preserveSet.Contains(name))
    {
        File.Delete(file);
    }
}

using var document = JsonDocument.Parse(File.ReadAllText(options.OpenApiPath));
if (!document.RootElement.TryGetProperty("components", out var components) ||
    !components.TryGetProperty("schemas", out var schemas))
{
    throw new InvalidOperationException($"No components.schemas found in: {options.OpenApiPath}");
}

var schemaNames = schemas.EnumerateObject().Select(x => x.Name).OrderBy(x => x, StringComparer.Ordinal).ToArray();
foreach (var schemaName in schemaNames)
{
    var schema = schemas.GetProperty(schemaName);
    var typeName = ToPascalCase(schemaName);
    var targetPath = Path.Combine(options.OutputDirectory, $"{typeName}.cs");

    var lines = new List<string>
    {
        "using System.Text.Json.Serialization;",
        string.Empty,
        $"namespace {options.Namespace};",
        string.Empty
    };

    if (schema.TryGetProperty("enum", out var enumValues) && enumValues.ValueKind == JsonValueKind.Array)
    {
        lines.Add($"public enum {typeName}");
        lines.Add("{");
        var values = enumValues.EnumerateArray().Select(x => x.ToString()).ToArray();
        for (var i = 0; i < values.Length; i++)
        {
            var suffix = i < values.Length - 1 ? "," : string.Empty;
            lines.Add($"    {ToPascalCase(values[i])}{suffix}");
        }
        lines.Add("}");
    }
    else
    {
        lines.Add($"public sealed class {typeName}");
        lines.Add("{");

        var required = new HashSet<string>(StringComparer.Ordinal);
        if (schema.TryGetProperty("required", out var requiredArray) && requiredArray.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in requiredArray.EnumerateArray())
            {
                var value = item.GetString();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    required.Add(value);
                }
            }
        }

        if (!schema.TryGetProperty("properties", out var properties) || properties.ValueKind != JsonValueKind.Object)
        {
            lines.Add("}");
        }
        else
        {
            var props = properties.EnumerateObject().OrderBy(x => x.Name, StringComparer.Ordinal).ToArray();
            for (var i = 0; i < props.Length; i++)
            {
                var prop = props[i];
                var propertyLines = BuildPropertyLines(prop.Name, prop.Value, required);
                lines.AddRange(propertyLines);

                if (i < props.Length - 1)
                {
                    lines.Add(string.Empty);
                }
            }

            lines.Add("}");
        }
    }

    File.WriteAllText(targetPath, string.Join(Environment.NewLine, lines) + Environment.NewLine, Encoding.UTF8);
}

return;

static (string OpenApiPath, string OutputDirectory, string Namespace, string[] PreserveFiles) ParseArgs(string[] args)
{
    string? openApiPath = null;
    string? outputDirectory = null;
    string? ns = null;
    var preserve = new List<string>();

    for (var i = 0; i < args.Length; i++)
    {
        var key = args[i];
        if (!key.StartsWith("--", StringComparison.Ordinal))
        {
            continue;
        }

        var value = i + 1 < args.Length ? args[++i] : string.Empty;
        switch (key)
        {
            case "--openapi":
                openApiPath = value;
                break;
            case "--output":
                outputDirectory = value;
                break;
            case "--namespace":
                ns = value;
                break;
            case "--preserve":
                if (!string.IsNullOrWhiteSpace(value))
                {
                    preserve.Add(value);
                }
                break;
        }
    }

    if (string.IsNullOrWhiteSpace(openApiPath) ||
        string.IsNullOrWhiteSpace(outputDirectory) ||
        string.IsNullOrWhiteSpace(ns))
    {
        throw new ArgumentException("Required arguments: --openapi <path> --output <dir> --namespace <name>");
    }

    return (Path.GetFullPath(openApiPath), Path.GetFullPath(outputDirectory), ns, preserve.ToArray());
}

static IEnumerable<string> BuildPropertyLines(string jsonName, JsonElement schema, HashSet<string> required)
{
    var propName = ToPascalCase(jsonName);
    var (typeName, isValueType) = GetTypeName(schema);

    var isRequired = required.Contains(jsonName);
    var isNullable = schema.TryGetProperty("nullable", out var nullableNode) &&
                     nullableNode.ValueKind == JsonValueKind.True;

    var finalType = typeName;
    var initializer = string.Empty;

    if (isValueType)
    {
        if (isNullable)
        {
            finalType = $"{typeName}?";
        }
    }
    else
    {
        if (!isRequired || isNullable)
        {
            finalType = $"{typeName}?";
        }
        else
        {
            initializer = " = default!;";
        }
    }

    return
    [
        $"    [JsonPropertyName(\"{jsonName}\")]",
        $"    public {finalType} {propName} {{ get; set; }}{initializer}"
    ];
}

static (string TypeName, bool IsValueType) GetTypeName(JsonElement schema)
{
    if (schema.TryGetProperty("$ref", out var refNode) && refNode.ValueKind == JsonValueKind.String)
    {
        var value = refNode.GetString() ?? string.Empty;
        var refName = value.Split('/').LastOrDefault() ?? "object";
        return (refName, false);
    }

    var type = schema.TryGetProperty("type", out var typeNode) ? typeNode.GetString() : null;
    var format = schema.TryGetProperty("format", out var formatNode) ? formatNode.GetString() : null;

    return type switch
    {
        "integer" when string.Equals(format, "int64", StringComparison.OrdinalIgnoreCase) => ("long", true),
        "integer" => ("int", true),
        "number" when string.Equals(format, "float", StringComparison.OrdinalIgnoreCase) => ("float", true),
        "number" => ("double", true),
        "boolean" => ("bool", true),
        "string" when string.Equals(format, "uuid", StringComparison.OrdinalIgnoreCase) => ("Guid", true),
        "string" when string.Equals(format, "date-time", StringComparison.OrdinalIgnoreCase) => ("DateTime", true),
        "string" => ("string", false),
        "array" => GetArrayType(schema),
        "object" => GetDictionaryType(schema),
        _ => ("object", false)
    };
}

static (string TypeName, bool IsValueType) GetArrayType(JsonElement schema)
{
    if (!schema.TryGetProperty("items", out var items))
    {
        return ("IReadOnlyList<object>", false);
    }

    var (itemType, _) = GetTypeName(items);
    return ($"IReadOnlyList<{itemType}>", false);
}

static (string TypeName, bool IsValueType) GetDictionaryType(JsonElement schema)
{
    if (!schema.TryGetProperty("additionalProperties", out var additional))
    {
        return ("object", false);
    }

    if (additional.ValueKind == JsonValueKind.True)
    {
        return ("IReadOnlyDictionary<string, object>", false);
    }

    if (additional.ValueKind == JsonValueKind.Object)
    {
        var (valueType, _) = GetTypeName(additional);
        return ($"IReadOnlyDictionary<string, {valueType}>", false);
    }

    return ("object", false);
}

static string ToPascalCase(string value)
{
    if (string.IsNullOrWhiteSpace(value))
    {
        return "Unnamed";
    }

    var normalized = Regex.Replace(value, "[^A-Za-z0-9]+", " ");
    var parts = Regex.Matches(normalized, "[A-Z]+(?![a-z])|[A-Z]?[a-z]+|[0-9]+")
        .Select(x => x.Value)
        .Where(x => !string.IsNullOrWhiteSpace(x))
        .ToArray();

    if (parts.Length == 0)
    {
        return "Unnamed";
    }

    var result = string.Concat(parts.Select(x => x.Length == 1
        ? x.ToUpperInvariant()
        : char.ToUpperInvariant(x[0]) + x[1..]));
    return char.IsDigit(result[0]) ? $"N{result}" : result;
}
