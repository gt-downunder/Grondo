---
title: Utilities
layout: default
nav_order: 4
---

# Utilities

---

## Environments

String constants for common environment names.

```csharp
using Grondo.Utilities;

string env = Environments.Local; // "local"
// Also: Environments.Dev, Environments.Test, Environments.Uat, Environments.Prod
```

Typically used with ASP.NET Core host configuration:

```csharp
if (builder.Environment.EnvironmentName == Environments.Prod)
    builder.Services.AddProductionLogging();
```

---

## StringFactory

Generates unique string identifiers.

```csharp
using Grondo.Utilities;

string id = StringFactory.UniqueString;
// e.g. "a3f5b2c8d1e94f6789abcdef01234567" (32 hex digits, no hyphens)

// Each access generates a new value
string id2 = StringFactory.UniqueString; // different from id

// Generate random strings of a specific length and character set
string code = StringFactory.CreateAlphabeticString(8); // "XjmnQpWe"
string pin  = StringFactory.CreateNumericString(6);    // "405291"
string hash = StringFactory.CreateHexString(16);       // "a3f5b2c8d1e94f67"
```

---

## JsonDefaults

Pre-configured `JsonSerializerOptions` instances for consistent serialization.

```csharp
using Grondo.Utilities;
using System.Text.Json;

// Standard presets (cached, reused across calls)
JsonSerializer.Serialize(model, JsonDefaults.CamelCase);   // camelCase property names
JsonSerializer.Serialize(model, JsonDefaults.SnakeCase);   // snake_case property names
JsonSerializer.Serialize(model, JsonDefaults.NoNulls);     // omit null values
JsonSerializer.Serialize(model, JsonDefaults.Indented);    // pretty-printed
JsonSerializer.Serialize(model, JsonDefaults.Web);         // camelCase + no-nulls + Web defaults

JsonSerializer.Deserialize<T>(json, JsonDefaults.CaseInsensitive); // loose property matching
```

Do not mutate the returned options; create your own `JsonSerializerOptions` if different
configuration is required.
