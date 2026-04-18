# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.0.1] - 2026-04-18

### Fixed

- **NuGet packaging:** restored the `<ItemGroup>` that includes `README.md` and
  `LICENSE.md` in the `.nupkg`. `dotnet pack` was failing with `NU5039` because
  the readme referenced by `<PackageReadmeFile>` was no longer being packed.
- **Resource leak in `FuncEx.Debounce` / `FuncEx.Debounce<T>`:** the last
  scheduled `CancellationTokenSource` was never disposed (previous CTS was
  disposed on the next invocation, leaving the final one dangling). Disposal is
  now tied to the delay task's continuation via a `try`/`finally`, so each CTS
  is disposed exactly once whether the delay completes or is cancelled.
- **Floating-point equality in `NumericEx.PercentageOf(double)`:** replaced the
  exact `total == 0` check with `Math.Abs(total) < double.Epsilon`. IEEE-754
  equivalent for detecting zero (no non-zero double is smaller than
  `double.Epsilon`), but silences the static-analysis warning.

### Changed

- **`GuardEx.ThrowIfTooShort`** no longer includes a dead `value is null ? 0 : value.Length`
  branch in its error message — the surrounding `value?.Length < minLength`
  guard already ensures `value` is non-null on that path.
- **`ConfigurationEx.GetRequiredValue<T>`** narrows its `catch` to
  `InvalidCastException`, `FormatException`, and `OverflowException`, matching
  the sibling `GetValue<T>`. Unrelated exceptions (`OutOfMemoryException`,
  custom `TypeConverter` exceptions, etc.) now propagate instead of being
  wrapped in `InvalidOperationException`.
- **`Combinators.Traverse`** (for `Result<T>`, `Maybe<T>`, `Validation<T>`)
  simplified to `Sequence(source.Select(selector))` — same behavior, less code.
- **`EnumerableEx.HasDuplicates`** simplified to `source.Any(item => !seen.Add(item))`.
- **`StringEx.ToPascalCase`** and **`StringEx.RemoveDiacritics`** use LINQ
  `Where(...)` for filtering inside their `StringBuilder` loops.
- **README** now documents both release scripts (`RELEASE.sh` for bash/WSL and
  `RELEASE.ps1` for PowerShell) and notes that they are equivalent.
- **Tests:** minor hygiene — `var _ = ...` discards changed to `_ = ...`,
  `StreamReader` in middleware tests is now disposed, and a redundant
  `Guid.Empty.Equals(Guid.Empty)` in a `OneOf3` test was removed.

---

## [2.0.0] - 2026-04-18

### Breaking Changes

- **`ValidationException.Errors`** type changed from `IDictionary<string, string[]>` to
  `IReadOnlyDictionary<string, IReadOnlyList<string>>`. The dictionary and inner lists are now
  deeply immutable and the supplied errors are defensively copied by the constructor. A new
  constructor overload accepting `IReadOnlyDictionary<string, IReadOnlyList<string>>` is available
  for consumers that already work with read-only collections; the existing
  `IDictionary<string, string[]>` constructor continues to work but copies its input.
- **`ExceptionBaseEx`** (with the `ToProblemDetails` extension on `ExceptionBase`) moved from the
  `Grondo.Exceptions` namespace/folder to `Grondo.Extensions`, matching all other `*Ex`
  extension classes. Consumers that use `using Grondo.Extensions;` (already typical) need no
  change; consumers that imported only `Grondo.Exceptions` must add `using Grondo.Extensions;`.

### Added

#### Types
- `Result<T, TError>` — typed-error Result monad with `Map`, `MapError`, `Bind`, `Match`, `Tap`,
  `TapError`, `Ensure`, `Recover`, LINQ query syntax, async overloads, and equality.
- `Error` record with common factory helpers: `NotFound`, `Validation`, `Unauthorized`,
  `Forbidden`, `Conflict`, `Unexpected`.
- `Combinators` static class: `Sequence`/`Traverse` for `Result<T>`, `Maybe<T>`, `Validation<T>`,
  and `Result<T, TError>`.
- `OneOf<T1, T2>`, `OneOf<T1, T2, T3>`, `OneOf<T1, T2, T3, T4>` — discriminated unions for values
  that are one of N peer types, with `Match`, `Switch`, `IsTN`/`AsTN`, implicit conversions,
  `FromTN` factories, and value equality. `OneOf<T1, T2>` additionally has `MapT0`/`MapT1`.
- `Either<L, R>`: `Swap`, `BiMap`, `TapBoth`, `ToValidation`, typed `ToResult()`, `Select`,
  `SelectMany` (LINQ query syntax).
- `Maybe<T>`: `AsEnumerable`, `ToArray`, `ToList`.
- `Validation<T>`: `Apply` (applicative style), `ToMaybe`, `ToEither`.

#### Extensions
- `StringEx`: `ToPascalCase`, `ToTitleCase`, `StripHtml`, `WordCount`, `RemoveDiacritics`,
  `Repeat`, `EnsureStartsWith`, `EnsureEndsWith`, `ReplaceMultiple`.
- `DateTimeEx` / `DateTimeOffsetEx`: `StartOfWeek`, `EndOfWeek`, `StartOfYear`, `EndOfYear`,
  `Age`, `AgeAt`, `ToUnixTimeSeconds`, `ToUnixTimeMilliseconds`, `IsToday`, `IsInPast`,
  `IsInFuture`, `DaysUntil`, `IsLeapYear`.
- `TimeSpanEx`: `ToClockString`, `IsPositive`, `IsNegative`.
- `NumericEx`: `ClampTo`, `IsBetween` (for `INumber<T>`); `RoundTo`, `PercentageOf`
  (for `double` and `decimal`).
- `EnumerableEx`: `HasDuplicates`, `None`, `Cycle`, `Flatten`.
- `DictionaryEx`: `AddOrUpdate`, `RemoveWhere`.
- `TaskEx`: `WithCancellationAsync` on both `Task` and `Task<T>`.
- `GuardEx`: `ThrowIfLessThan`, `ThrowIfGreaterThan` for `INumber<T>`.
- `UriEx`: `AppendPath`, `GetQueryParameters`, `WithQueryParameter`.
- New `RegexEx` class: `RegexIsMatch`, `RegexMatch`, `RegexMatches`, `RegexReplace`.
- New `CancellationTokenEx` class: `AsTask`, `WithTimeout`.

#### Exceptions & API
- `PaymentRequiredException` (HTTP 402).
- `ExceptionBase.ToProblemDetails()` — converts any `ExceptionBase` (including
  `ValidationException`) to an RFC 7807 `ProblemDetails` response.
- `ExceptionHandlingMiddleware` with `UseGrondoExceptionHandling()` extension for automatic
  conversion of exceptions to `application/problem+json` responses. Supports a configurable
  `ProblemTypeUriFormatter` on `ExceptionHandlingOptions`.

#### Utilities
- `JsonDefaults` is now `public` and adds presets: `CamelCase`, `SnakeCase`, `NoNulls`, `Web`.
- `StringFactory`: `CreateAlphabeticString`, `CreateNumericString`, `CreateHexString`.

#### Tooling & Infrastructure
- `.editorconfig` with repo-wide formatting rules.
- Community health files: `SECURITY.md`, issue and PR templates.
- PR CI: build-and-publish workflow now runs on `pull_request` (publish job remains gated).
- Weekly mutation-testing workflow (Stryker.NET) and PR-triggered benchmark workflow.
- Property-based tests for monad laws (`Maybe`, `Result`, `Either`) via FsCheck 3.

### Changed
- Fixed `DebuggerDisplay` expression syntax in `Maybe<T>`, `Result<T>`, `Result`, and
  `OneOf<T1, T2>` / `OneOf<T1, T2, T3>` / `OneOf<T1, T2, T3, T4>`.
- `ProblemDetails.Type` now defaults to the MDN documentation URI for the status code
  (e.g. `https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/404`) instead of
  `https://httpstatuses.io/{code}`. The formatter is configurable via
  `ExceptionHandlingOptions.ProblemTypeUriFormatter` (middleware) or the new
  `typeUriFormatter` parameter on `ExceptionBase.ToProblemDetails(...)`.
- `Error` factory methods (`NotFound`, `Validation`, `Unauthorized`, `Forbidden`, `Conflict`,
  `Unexpected(string)`) now throw `ArgumentNullException` on a null `message`, matching the
  existing `Unexpected(Exception)` overload.
- Dependabot updates scheduled `monthly` instead of `weekly`.
- Pages workflow bumped to `actions/configure-pages@v6`, `actions/upload-pages-artifact@v5`,
  `actions/deploy-pages@v5`.
- Documentation "What's Inside" links now correctly resolve under `/Grondo`.

---

## [1.2.0] - 2026-03-24

### Added

#### Type Conversions
- **`Result<T>`** — `ToMaybe()`, `ToEither()`, `ToValidation()` conversion methods
- **`Maybe<T>`** — `ToEither(TLeft)`, `ToValidation(string)` conversion methods
- **`Either<TLeft, TRight>`** — `ToMaybe()`, `ToResult(Func<TLeft, string>)` conversion methods
- All functional types (`Result<T>`, `Maybe<T>`, `Either<L,R>`, `Validation<T>`) can now convert to each other

### Changed

- License changed from GPL-3.0 to MIT
- Extension method count corrected from 29 to 30 in package metadata and documentation
- README streamlined — removed duplicated content, added "Why Grondo?" section, linked to docs site
- Updated `docs/types.md` with type conversion documentation and examples

### Fixed

- License badge in README now shows MIT instead of GPL v3
- `PackageLicenseExpression` in `.csproj` updated from `GPL-3.0-or-later` to `MIT`
- `LICENSE.md` copyright holder updated
- Removed dead link to non-existent `.vscode/TASKS_GUIDE.md`

---

## [1.1.0] - 2026-03-11

### Added

#### New Types
- **`Either<TLeft, TRight>`** - Dual-value type for representing success/failure with typed errors
  - Methods: `FromLeft`, `FromRight`, `Match`, `Map`, `Bind`, `MapLeft`, `Tap`, `TapLeft`
  - Async variants: `MapAsync`, `BindAsync`
  - Implements `IEquatable<Either<L,R>>` with `==` and `!=` operators
  - Includes `[DebuggerDisplay]` attribute for better debugging
- **`Validation<T>`** - Accumulative validation type that collects all errors
  - Methods: `Valid`, `Invalid`, `Combine`, `Map`, `Bind`, `Match`, `ToResult`
  - Async variants: `MapAsync`, `BindAsync`
  - Implements `IEquatable<Validation<T>>` with `==` and `!=` operators
  - Includes `[DebuggerDisplay]` attribute

#### LINQ Query Syntax Support
- **`Result<T>`** now supports LINQ query syntax via `Select` and `SelectMany`
- **`Maybe<T>`** now supports LINQ query syntax via `Select` and `SelectMany`
- Enables readable functional composition with `from`/`select` syntax

#### New Extension Classes
- **`EnumerableExAsync`** - Asynchronous LINQ extensions
  - `SelectAsync<TResult>` - Sequential async projection
  - `SelectAsyncParallel<TResult>` - Parallel async projection with concurrency control
  - `WhereAsync` - Async filtering
  - `ForEachAsync` - Async iteration
  - `AggregateAsync` - Async aggregation
  - `AnyAsync` - Async any predicate
  - `AllAsync` - Async all predicate
- **`FuncEx`** - Function utilities
  - `Memoize<T, TResult>()` - Cache function results (thread-safe)
  - `Debounce(TimeSpan)` - Delay execution until calls stop
  - `Throttle(TimeSpan)` - Limit execution frequency
  - `ToLazy<T>()` - Convert function to lazy initializer

#### Result<T> Enhancements
- `Ok(T value)` - Alias for `Success` (common convention)
- `Recover(Func<string, T>)` - Convert failure to success
- `RecoverAsync(Func<string, Task<T>>)` - Async recovery
- `Flatten(Result<Result<T>>)` - Flatten nested results
- `Zip<T1, T2>(Result<T1>, Result<T2>)` - Combine two results into tuple
- `Zip<T1, T2, T3>(...)` - Combine three results into tuple
- `OrElse(T defaultValue)` - Provide default value on failure
- `OrElse(Func<string, T>)` - Provide default via factory function

#### Maybe<T> Enhancements
- `OrElse(Maybe<T>)` - Provide alternative Maybe if None
- `OrElse(Func<Maybe<T>>)` - Provide alternative via factory
- `Filter(Func<T, bool>)` - Alias for `Where` (common in FP)
- `Flatten(Maybe<Maybe<T>>)` - Flatten nested Maybes
- `Zip<T1, T2>(Maybe<T1>, Maybe<T2>)` - Combine two Maybes into tuple
- `OrValue(T defaultValue)` - Convert None to Some with default
- `OrValue(Func<T>)` - Convert None to Some via factory

#### GuardEx Enhancements
- **String guards:**
  - `ThrowIfNullOrEmpty(string? paramName)`
  - `ThrowIfTooLong(int maxLength, string? paramName)`
  - `ThrowIfTooShort(int minLength, string? paramName)`
- **Collection guards:**
  - `ThrowIfContains(T item, string? paramName)`
  - `ThrowIfDoesNotContain(T item, string? paramName)`
- **Enum guards:**
  - `ThrowIfNotDefined(string? paramName)` for `TEnum where TEnum : struct, Enum`
- **DateTime guards:**
  - `ThrowIfInPast(string? paramName)`
  - `ThrowIfInFuture(string? paramName)`
  - `ThrowIfNotInRange(DateTime min, DateTime max, string? paramName)`
- **Guid guards:**
  - `ThrowIfEmpty(string? paramName)`

#### ConfigurationEx Enhancements
- `GetValue<T>(string key, T defaultValue)` - Get typed configuration value with default
- `GetRequiredValue<T>(string key)` - Get required typed value (throws if missing)
- `GetSection<T>(string sectionName)` - Bind configuration section to strongly-typed object

#### EnvironmentEx Enhancements
- `IsProduction()` - Check if environment is Production or Prod
- `IsDevelopment()` - Check if environment is Development
- `IsStaging()` - Check if environment is Staging
- `IsEnvironment(params string[])` - Check if environment matches any specified names

#### HttpEx Enhancements
- `GetRawBodyAsBytesAsync(CancellationToken)` - Get raw body as bytes with buffering
- `GetQueryParams()` - Get query parameters as read-only dictionary
- `GetFormDataAsync(CancellationToken)` - Get form data as read-only dictionary
- `IsAjaxRequest()` - Check if request is AJAX (X-Requested-With header)
- `GetClientIpAddress()` - Get client IP address (checks X-Forwarded-For)

#### New Exception Types
- **`ValidationException`** (400 Bad Request)
  - Includes `Errors` dictionary for field-level validation errors
  - Supports single field or multiple field errors
- **`TooManyRequestsException`** (429 Too Many Requests)
  - Includes optional `RetryAfter` property
  - For rate limiting scenarios
- **`ServiceUnavailableException`** (503 Service Unavailable)
  - Includes optional `RetryAfter` property
  - For temporary service outages
- **`UnprocessableEntityException`** (422 Unprocessable Entity)
  - For semantically invalid requests

#### StringFactory Enhancements
- `CreateRandomString(int length, bool includeSpecialChars)` - Thread-safe random string generation
- `CreateGuid()` - Helper method for GUID generation

#### Debugging Improvements
- Added `[DebuggerDisplay]` attributes to `Result<T>`, `Result`, `Maybe<T>`, `Either<L,R>`, `Validation<T>`
- Shows meaningful information in debugger (e.g., "Success(42)" or "Failure(error)")

### Fixed

#### Critical Bug Fixes
- **Thread-safety:** Fixed potential race condition in `StringFactory.CreateRandomString`
  - Now uses `Random.Shared` instead of static `Random` instance
  - Uses `string.Create` for zero-allocation string building
- **Cultural invariance:** Fixed CA1305 violations in string formatting
  - `NumericEx.ToOrdinal()` now uses `CultureInfo.InvariantCulture`
  - `TimeSpanEx.Pluralize()` now uses `CultureInfo.InvariantCulture`
  - `TimeSpanEx.ToRelativeString()` now uses `CultureInfo.InvariantCulture`

### Changed

#### Performance Optimizations
- **`ByteArrayEx.ToHexString()`** - Optimized with `Span<char>` and `string.Create`
  - Eliminates `ToLowerInvariant()` allocation
  - ~2x faster for large byte arrays
- **`EnumerableEx.Batch()`** - Optimized to reuse `List<T>` instead of allocating new ones
  - Returns arrays instead of lists for better performance
  - Reduces allocations in batching scenarios

#### API Improvements
- **`EnumerableEx`** - Updated XML documentation to reference `EnumerableExAsync`
- **`EnumerableExAsync`** - Updated XML documentation to reference `EnumerableEx`
- Cross-references between sync and async extension classes for better discoverability

### Documentation

#### Comprehensive Updates
- Updated README.md with all new features and examples
- Updated docs/index.md (GitHub Pages landing page)
- Updated docs/types.md with Either<L,R>, Validation<T>, and LINQ syntax
- Updated docs/extensions.md with EnumerableExAsync and FuncEx
- Updated docs/exceptions.md with 4 new exception types
- Added 20+ new code examples across all documentation
- Added LINQ query syntax examples
- Added real-world usage scenarios

---

## [1.0.0] - 2024-XX-XX (Previous Release)

### Added
- Initial release with core functionality
- `Result<T>` and `Result` types for railway-oriented programming
- `Maybe<T>` type for optional values
- 27 extension method classes
- 9 custom HTTP-mapped exception types
- Utility classes: `JsonDefaults`, `StringFactory`, `Environments`
- ASP.NET Core extensions: `ServiceProviderEx`, `EnvironmentEx`, `ConfigurationEx`, `HttpEx`

---

## Summary of Changes

### Statistics
- **New Types:** 2 (Either<L,R>, Validation<T>)
- **New Extension Classes:** 2 (EnumerableExAsync, FuncEx)
- **New Methods:** 60+
- **New Exception Types:** 4
- **Bug Fixes:** 2 critical (thread-safety, cultural invariance)
- **Performance Improvements:** 2 (ByteArrayEx, EnumerableEx)
- **Breaking Changes:** 0

### Migration Guide

#### From Previous Version

No breaking changes! All existing code continues to work.

**New features you can adopt:**

```csharp
// 1. LINQ query syntax (optional, but recommended)
// Before:
var result = GetUser(id)
    .Bind(user => GetProfile(user.Id)
        .Map(profile => new UserViewModel(user, profile)));

// After:
var result = from user in GetUser(id)
             from profile in GetProfile(user.Id)
             select new UserViewModel(user, profile);

// 2. Accumulative validation (new capability)
var validation = ValidateUser(request);
if (validation.IsInvalid)
{
    // Returns ALL errors at once!
    return BadRequest(validation.Errors);
}

// 3. Async LINQ (new capability)
var users = await userIds.SelectAsync(id => GetUserAsync(id));

// 4. Memoization (new capability)
var memoized = expensiveFunction.Memoize();
```

---

[Unreleased]: https://github.com/gt-downunder/Grondo/compare/v2.0.1...HEAD
[2.0.1]: https://github.com/gt-downunder/Grondo/compare/v2.0.0...v2.0.1
[2.0.0]: https://github.com/gt-downunder/Grondo/compare/v1.2.1...v2.0.0
[1.2.0]: https://github.com/gt-downunder/Grondo/compare/v1.1.0...v1.2.0
[1.1.0]: https://github.com/gt-downunder/Grondo/compare/v1.0.42...v1.1.0
[1.0.0]: https://github.com/gt-downunder/Grondo/releases/tag/v1.0.0

