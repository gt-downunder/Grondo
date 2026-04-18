# Contributing to Grondo

Thank you for your interest in contributing! This guide covers everything you need to get started.

## Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download) or later
- An editor with C# 14 support (Visual Studio 2025+, VS Code with C# Dev Kit, Rider 2025+)

## Getting Started

1. **Fork and clone** the repository:

   ```bash
   git clone https://github.com/<your-username>/dotnet-library.git
   cd dotnet-library
   ```

2. **Build** the solution:

   ```bash
   dotnet build
   ```

3. **Run tests** to verify your setup:

   ```bash
   dotnet test
   ```

   All 664+ tests should pass with 0 warnings and 0 errors.

## Project Structure

```
Grondo.sln
├── src/                        # Main library (Grondo)
│   ├── Extensions/             # Extension method classes (C# 14 extension blocks)
│   ├── Exceptions/             # HTTP-mapped custom exception types, ProblemDetails middleware
│   ├── Utilities/              # JsonDefaults, StringFactory, Environments
│   ├── Result.cs               # Result<T> and Result readonly structs
│   ├── ResultTE.cs             # Result<T, TError> typed-error variant
│   ├── Maybe.cs                # Maybe<T> optional readonly struct
│   ├── Either.cs               # Either<TLeft, TRight> readonly struct
│   ├── Validation.cs           # Validation<T> accumulative errors
│   ├── Error.cs                # Error record for Result<T, TError>
│   ├── Combinators.cs          # Sequence / Traverse for all container types
│   └── OneOf2.cs / OneOf3.cs / OneOf4.cs  # Discriminated unions (2–4 arities)
├── tests/                      # MSTest unit tests (mirrors src/ layout)
└── benchmarks/                 # BenchmarkDotNet performance benchmarks
```

## Design Principles

- **Zero external dependencies** — the library must not add any NuGet package dependencies. Only `Microsoft.AspNetCore.App` framework reference and build-time-only tools (e.g., SourceLink) are permitted.
- **`readonly struct` for value types** — types like `Result<T>`, `Result`, and `Maybe<T>` are `readonly struct` for zero-allocation and value semantics.
- **C# 14 extension blocks** — all extension methods use the new `extension(Type param) { }` syntax, not the traditional `this` parameter style.
- **Railway-oriented programming** — `Result<T>` follows ROP conventions with `Map`, `Bind`, `Tap`, `Ensure`, `Match`, and async variants.
- **Warnings as errors** — the build treats all warnings as errors. Your code must compile with 0 warnings.

## Coding Conventions

### General

| Rule | Convention |
|---|---|
| **Indentation** | 4 spaces (no tabs) |
| **Line endings** | CRLF |
| **Charset** | UTF-8 with BOM |
| **Nullable** | Enabled — all reference types are nullable-aware |
| **Implicit usings** | Enabled |

### Naming

| Symbol | Convention | Example |
|---|---|---|
| Public members | PascalCase | `IsEmpty()`, `ToJson()` |
| Private fields | _camelCase | `_value`, `_isSuccess` |
| Interfaces | IPascalCase | `IAsyncDisposable` |
| Type parameters | TPascalCase | `TResult`, `TAccumulate` |
| Async methods | Suffix with `Async` | `MapAsync()`, `RetryAsync()` |
| Local variables | camelCase | `result`, `index` |
| Constants | PascalCase | `MaxRetries` |

### Extension Methods

All extension methods must use C# 14 extension blocks:

```csharp
public static partial class MyEx
{
    extension(string value)
    {
        /// <summary>XML doc comment required.</summary>
        public string MyMethod() => value.ToUpper();
    }
}
```

**Do not** use the traditional `this` parameter style:

```csharp
// ❌ Don't do this
public static string MyMethod(this string value) => value.ToUpper();
```

### Locale Sensitivity

The project enforces CA1305. Any call to `ToString(string format)` or similar locale-sensitive methods must pass `CultureInfo.InvariantCulture`:

```csharp
// ✅ Correct
value.ToString("F1", CultureInfo.InvariantCulture);

// ❌ Will fail the build
value.ToString("F1");
```

### ConfigureAwait

All `await` calls must use `ConfigureAwait(false)`:

```csharp
var result = await SomeAsync().ConfigureAwait(false);
```

### XML Documentation

Every public method and type must have an XML doc comment with at least a `<summary>`:

```csharp
/// <summary>
/// Returns the string with its characters in reverse order.
/// </summary>
/// <returns>The reversed string.</returns>
public string Reverse() { ... }
```

## Writing Tests

### Framework

- **Test framework:** MSTest (`[TestClass]`, `[TestMethod]`, `[DataRow]`)
- **Assertions:** [FluentAssertions](https://fluentassertions.com/)
- **Base classes:** Extension tests inherit from `BaseExtensionTest`; type tests inherit from `BaseTest`

### Conventions

- **One test class per source class** — e.g., `StringExTests.cs` tests `StringEx.cs`
- **Test naming:** `MethodName_Scenario_ExpectedResult` with underscores
- **Use `[DataRow]`** for parameterized tests when testing multiple inputs:

```csharp
[TestMethod]
[DataRow("PascalCase", "pascal_case")]
[DataRow("camelCase", "camel_case")]
[DataRow("", "")]
public void ToSnakeCase_ConvertsCorrectly(string input, string expected) =>
    input.ToSnakeCase().Should().Be(expected);
```

- **Test edge cases:** null inputs, empty collections, boundary values
- **Test exception throwing** using FluentAssertions:

```csharp
[TestMethod]
public void ThrowIfNegative_NegativeValue_Throws()
{
    Func<int> act = () => (-1).ThrowIfNegative("n");
    act.Should().Throw<ArgumentOutOfRangeException>().WithParameterName("n");
}
```

### Running Tests

```bash
# Run all tests
dotnet test

# Run with Release configuration
dotnet build -c Release
dotnet test -c Release --no-build

# Run a specific test class
dotnet test --filter "FullyQualifiedName~StringExTests"
```

## Benchmarks

If your change affects a hot path (e.g., collection or string operations), add or update benchmarks:

```bash
dotnet run --project benchmarks -c Release -- --filter "*YourBenchmark*"
```

Benchmark classes live in the `benchmarks/` directory and use [BenchmarkDotNet](https://benchmarkdotnet.org/).

## Submitting Changes

1. **Create a branch** from `main`:

   ```bash
   git checkout -b feature/my-new-feature
   ```

2. **Make your changes** following the conventions above.

3. **Ensure the build passes** with zero warnings and zero errors:

   ```bash
   dotnet build -c Release
   ```

4. **Ensure all tests pass:**

   ```bash
   dotnet test -c Release --no-build
   ```

5. **Commit** with a clear, descriptive message:

   ```
   Add ToTitleCase extension to StringEx
   ```

6. **Open a pull request** against `main` with:
   - A description of what the change does and why
   - Any new public API surfaces documented in the PR description
   - Confirmation that all tests pass

### Merge Requirements

All pull requests targeting `main` must pass the following checks before merging:

| Requirement | Description |
|---|---|
| **Build & tests** | The CI build must succeed with zero warnings, zero errors, and all tests passing |
| **Code scanning** | CodeQL must report no new security vulnerabilities |
| **Code quality** | CodeQL must report no new code quality issues |
| **Code owner approval** | A project maintainer must approve the PR |

Direct pushes to `main` are not allowed — all changes must go through a pull request.

## What Makes a Good Contribution

- **Fills a genuine gap** in the library's utility surface
- **Follows existing patterns** — look at similar extension classes for style guidance
- **Includes tests** — every new public method needs tests covering happy path, edge cases, and error cases
- **Has XML documentation** on all public members
- **Zero external dependencies** — no new NuGet packages
- **Respects the scope** — this is a utility/extension library, not an application framework

## Documentation

> 📖 **[gt-downunder.github.io/Grondo](https://gt-downunder.github.io/Grondo/)** — Full API reference with practical code examples for every public method.

When adding new public APIs, ensure they are documented in the `docs/` directory as well.

## Questions?

Open an issue if you have questions about whether a feature fits the library's scope before investing time in implementation.

