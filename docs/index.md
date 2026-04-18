---
title: Home
layout: default
nav_order: 1
---

# Grondo — Comprehensive Usage Guide

A complete reference for every public API in the Grondo library with practical code examples.
{: .fs-6 .fw-300 }

> **Requirements:** .NET 10 · C# 14 · ASP.NET Core

[Get started](#getting-started){: .btn .btn-primary .fs-5 .mb-4 .mb-md-0 .mr-2 }
[View on GitHub](https://github.com/gt-downunder/Grondo){: .btn .fs-5 .mb-4 .mb-md-0 }

---

## Getting Started

### Installation

```shell
dotnet add package Grondo
```

### Using Directives

```csharp
using Grondo;                // Result<T>, Result, Maybe<T>, Either<L,R>, Validation<T>
using Grondo.Extensions;     // All extension methods
using Grondo.Exceptions;     // ExceptionBase, ErrorResponse, and all domain exceptions
using Grondo.Utilities;      // Environments, StringFactory
```

### ASP.NET Core Shared Framework

Grondo uses a `<FrameworkReference>` to the ASP.NET Core shared framework (`Microsoft.AspNetCore.App`), which is installed with the .NET SDK. This is **not** an external NuGet package dependency.

The library provides extension methods for:

- **`IServiceProvider`** (ServiceProviderEx) — Scoped service resolution (sync and async)
- **`IHostEnvironment`** (EnvironmentEx) — Environment name checks (IsLocal, IsTest, IsUat, IsProduction, IsDevelopment, IsStaging)
- **`IConfiguration`** (ConfigurationEx) — Typed configuration value retrieval with GetValue<T>, GetRequiredValue<T>, GetSection<T>
- **`HttpRequest`** (HttpEx) — Raw body extraction, query params, form data, client IP, AJAX detection

This makes Grondo primarily suitable for **ASP.NET Core applications** (web APIs, MVC, Blazor). Most extension methods (strings, collections, dates, Result<T>, Maybe<T>, etc.) work in any .NET 10 application, but the ASP.NET Core framework is always referenced.

---

## What's Inside

### [Extension Methods]({{ site.baseurl }}{% link extensions.md %})

30 extension method classes covering strings, collections, dates, JSON, HTTP, tasks, guards, async LINQ, memoization, and more.

### [Custom Exceptions]({{ site.baseurl }}{% link exceptions.md %})

13 HTTP-aware domain exceptions with status codes and message headers, including ValidationException with field-level errors.

### [Utilities]({{ site.baseurl }}{% link utilities.md %})

Environment name constants and unique string generation.

### [Types]({{ site.baseurl }}{% link types.md %})

`Result<T>`, `Result`, `Maybe<T>`, `Either<L,R>`, and `Validation<T>` — comprehensive functional programming types with LINQ query syntax support.

