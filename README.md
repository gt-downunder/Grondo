# Grondo

[![NuGet](https://img.shields.io/nuget/v/Grondo.svg)](https://www.nuget.org/packages/Grondo)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A C# 14 / .NET 10 library providing functional programming types, extension methods, utilities, and custom exception types for ASP.NET Core applications.

## Why Grondo?

.NET lacks built-in functional types like `Result<T>` or `Maybe<T>`, and common operations on strings, collections, dates, and tasks often require repetitive boilerplate. Grondo fills these gaps with a single, cohesive package:

- **Functional types** — `Result<T>`, `Maybe<T>`, `Either<L,R>`, and `Validation<T>` with LINQ query syntax, full interop between types, and async support
- **30 extension method classes** — Strings, collections, dates, JSON, HTTP, tasks, guard clauses, async LINQ, memoization, and more
- **13 HTTP-mapped exceptions** — Domain exceptions with status codes and message headers, ready for API error handling

## Installation

```bash
dotnet add package Grondo
```

> **Note:** Grondo references the ASP.NET Core shared framework (`Microsoft.AspNetCore.App`), which is included with the .NET SDK — not an external NuGet dependency. See [Requirements](#requirements) for details.

## Quick Start

```csharp
using Grondo;                // Result<T>, Maybe<T>, Either<L,R>, Validation<T>
using Grondo.Extensions;     // All extension methods
using Grondo.Exceptions;     // Domain exceptions
using Grondo.Utilities;      // Environments, StringFactory
```

### Railway-Oriented Pipelines

```csharp
var result = await GetUserAsync(id)
    .EnsureAsync(u => u.IsActive, "User inactive")
    .MapAsync(u => u.Email)
    .BindAsync(email => SendEmailAsync(email))
    .TapAsync(r => LogAsync("Email sent"))
    .MapErrorAsync(e => $"Pipeline failed: {e}");

result.Match(
    onSuccess: val => Console.WriteLine($"OK: {val}"),
    onFailure: err => Console.WriteLine($"Error: {err}"));
```

### LINQ Query Syntax

```csharp
var order =
    from userId in ValidateUserId(request.UserId)
    from user in GetUser(userId)
    from product in GetProduct(request.ProductId)
    select new Order { UserId = user.Id, ProductId = product.Id };
```

### Type Conversions

All functional types convert seamlessly between each other:

```csharp
Maybe<User> maybeUser = FindUser(42);

Result<User> result       = maybeUser.ToResult("User not found");
Either<string, User> either = result.ToEither();
Validation<User> validation = result.ToValidation();
Maybe<User> backToMaybe     = result.ToMaybe();
```

### Extension Methods

```csharp
"user@example.com".IsWellFormedEmailAddress();    // true
"Hello World!".Truncate(8);                       // "Hello..."
"MyPropertyName".ToSnakeCase();                   // "my_property_name"
1024L.ToHumanByteSize();                          // "1 KB"
3.ToOrdinal();                                    // "3rd"
new[] { 1, 2, 3, 4, 5 }.Batch(2);                // [[1,2], [3,4], [5]]
name.ThrowIfNullOrWhiteSpace();                   // guard clause
```

### Custom Exceptions

```csharp
throw new EntityNotFoundException("User not found");
// .StatusCode = 404, .MessageHeader = "Not found"
```

> 📖 **[Explore the full documentation with code examples →](https://gt-downunder.github.io/Grondo/)**

## Build and Test

```bash
dotnet build
dotnet test
```

### Benchmarks

```bash
dotnet run --project benchmarks -c Release -- --filter "*StringEx*"
```

## Requirements

- .NET 10.0 SDK or later
- C# 14 (uses extension blocks, `readonly struct`, `INumber<T>`, `[GeneratedRegex]`)
- **ASP.NET Core shared framework** — included with the .NET SDK, not an external package. A handful of extensions (`ServiceProviderEx`, `EnvironmentEx`, `ConfigurationEx`, `HttpEx`) use ASP.NET Core types; all other extensions and functional types work in any .NET 10 application.

## For Maintainers

Grondo uses an automated release process with GitHub Actions. See [.github/RELEASE_GUIDE.md](.github/RELEASE_GUIDE.md) for complete release documentation.

```bash
# Update CHANGELOG.md first, then:
./RELEASE.sh <version>    # e.g. ./RELEASE.sh 1.2.0
```

## Documentation

> 📖 **[gt-downunder.github.io/Grondo](https://gt-downunder.github.io/Grondo/)** — Full API reference with practical code examples for every public method.

- **[Extension Methods](https://gt-downunder.github.io/Grondo/extensions)** — All 30 extension method classes with examples
- **[Types](https://gt-downunder.github.io/Grondo/types)** — `Result<T>`, `Maybe<T>`, `Either<L,R>`, `Validation<T>`, LINQ syntax
- **[Custom Exceptions](https://gt-downunder.github.io/Grondo/exceptions)** — HTTP-mapped domain exceptions
- **[Utilities](https://gt-downunder.github.io/Grondo/utilities)** — `Environments`, `StringFactory`, `JsonDefaults`
- **[CONTRIBUTING.md](CONTRIBUTING.md)** — Guidelines for contributing

## License

This project is licensed under the [MIT](LICENSE.md) license.
