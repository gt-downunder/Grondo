---
title: Extension Methods
layout: default
nav_order: 2
---

# Extension Methods

---

## BooleanEx

Extensions for `bool` and `bool?`.

```csharp
bool enabled = true;

// Execute an action only when true
enabled.RunIfTrue(() => Console.WriteLine("Feature is ON"));

// Execute an action only when false
enabled.RunIfFalse(() => Console.WriteLine("Feature is OFF"));

// Treat null as false
bool? nullable = null;
bool safe = nullable.ToFalseIfNull(); // false
```

---

## ByteArrayEx

Extensions for `byte[]`.

```csharp
byte[] data = [0xCA, 0xFE, 0xBA, 0xBE];

string hex    = data.ToHexString();     // "CAFEBABE"
string base64 = data.ToBase64();        // Base64-encoded string
byte[] sha256 = data.ComputeSha256();   // SHA-256 hash

// Reverse operations (static-like, called on string)
byte[] fromHex    = "CAFEBABE".FromHexString();
byte[] fromBase64 = base64.FromBase64ToBytes();
```

---

## ClaimsPrincipalEx

Extensions for `ClaimsPrincipal` (ASP.NET Core authentication).

```csharp
ClaimsPrincipal user = httpContext.User;

string? role    = user.GetClaimValue("role");
bool   isAdmin  = user.HasClaim("admin");
string? userId  = user.GetUserId();       // "sub" or "nameidentifier"
string? email   = user.GetEmail();        // "emailaddress" claim
string? display = user.GetDisplayName();  // "name" claim
```

---

## ConfigurationEx

Extensions for `IConfiguration` (ASP.NET Core configuration).

```csharp
IConfiguration config = builder.Configuration;

// Throws InvalidOperationException if missing
string connStr = config.GetRequiredValue("ConnectionStrings:Default");

// Returns fallback if missing
string env = config.GetValueOrDefault("Env", "development");

// Check existence
bool hasKey = config.HasKey("FeatureFlags:NewUI");
```

---

## DateTimeEx

Extensions for `DateTime`, `DateTime?`, and date string parsing.

```csharp
DateTime now = DateTime.UtcNow;

string date     = now.ToFormattedDate();       // "dd/MM/yyyy"
string dateTime = now.ToFormattedDateTime();    // "dd/MM/yyyy HH:mm:ss"
DateTime next   = now.AddWeeks(2);
DateTime clean  = now.TruncateMilliseconds();

bool weekday = now.IsWeekday();
bool weekend = now.IsWeekend();

DateTime start = now.StartOfDay();   // 00:00:00
DateTime end   = now.EndOfDay();     // 23:59:59.9999999
DateTime som   = now.StartOfMonth(); // 1st of the month
DateTime eom   = now.EndOfMonth();   // last moment of the month

bool between = now.IsBetween(start, end);
string rel   = now.AddHours(-3).ToRelativeTime(); // "3 hours ago"

// Nullable overloads
DateTime? nullable = null;
string safe = nullable.ToFormattedDate();     // ""
string rel2 = nullable.ToRelativeTime();      // ""
```

Date string parsing:

```csharp
DateTime parsed  = "25/12/2025".FromFormattedDate();        // dd/MM/yyyy
bool     ok      = "25/12/2025".TryFromFormattedDate(out DateTime result);
DateTime parsed2 = "25/12/2025 14:30:00".FromFormattedDateTime();
bool     ok2     = "nope".TryFromFormattedDateTime(out _);  // false
```

Week/year/age/relative helpers:

```csharp
DateTime now = DateTime.UtcNow;

DateTime mon  = now.StartOfWeek();              // Monday 00:00 of current week
DateTime sun  = now.EndOfWeek();                // last tick of Sunday
DateTime jan1 = now.StartOfYear();              // first tick of Jan 1
DateTime dec31 = now.EndOfYear();               // last tick of Dec 31

int age      = birthDate.Age();                 // whole years to now (UTC)
int ageAt    = birthDate.AgeAt(asOf);           // whole years to a reference date

bool today   = someDate.IsToday();
bool past    = someDate.IsInPast();
bool future  = someDate.IsInFuture();
int daysLeft = expiresAt.DaysUntil();           // negative if past
bool leap    = someDate.IsLeapYear();

long epochS  = now.ToUnixTimeSeconds();
long epochMs = now.ToUnixTimeMilliseconds();
```

---

## DateTimeOffsetEx

Mirrors every method in [DateTimeEx](#datetimeex) for `DateTimeOffset` and `DateTimeOffset?`:
`ToFormattedDate`, `ToFormattedDateTime`, `AddWeeks`, `TruncateMilliseconds`, `IsWeekday`, `IsWeekend`,
`StartOfDay`, `EndOfDay`, `StartOfMonth`, `EndOfMonth`, `IsBetween`, `ToRelativeTime`, `StartOfWeek`,
`EndOfWeek`, `StartOfYear`, `EndOfYear`, `Age`, `AgeAt`, `IsToday`, `IsInPast`, `IsInFuture`,
`DaysUntil`, `IsLeapYear`, and the nullable overloads. Usage is identical — just substitute
`DateTimeOffset` for `DateTime`.

---

## DictionaryEx

Extensions for `IDictionary<TKey, TValue>`.

```csharp
var dict = new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 };

string json = dict.ToJson();                        // JSON string
bool has    = dict.HasKeyAndValue("a", 1);           // true
bool any    = dict.HasAnyKey("x", "a");              // true (has "a")
string qs   = dict.ToQueryString();                  // "a=1&b=2"
string dbg  = dict.ToDebugString();                  // "[a, 1] [b, 2]"

var other = new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 };
bool equal = dict.IsDeepEqualTo(other);              // true

// Merge (second wins on conflicts)
var merged = dict.Merge(new Dictionary<string, int> { ["b"] = 99, ["c"] = 3 });
// { a=1, b=99, c=3 }

// Get or lazily add
int val = dict.GetOrAdd("z", () => 42);             // adds "z"=42, returns 42

// Add or update in a single call
int result = dict.AddOrUpdate("a", addValue: 1, (_, existing) => existing + 10);
// If "a" existed with value 1, result is 11

// Remove matching entries (returns count removed)
int removed = dict.RemoveWhere(kv => kv.Value > 50);

// Get with fallback (no mutation)
int safe = dict.GetValueOrDefault("missing", -1);    // -1

// Nested dictionary helpers
var nested = new Dictionary<string, Dictionary<string, int>>();
nested.AddEntryToNestedDictionary("outer", "inner", 10);
nested.RemoveEntryFromNestedDictionary("outer", "inner");
```

---

## EnumEx

Extensions for `Enum` values.

```csharp
public enum Status
{
    [Description("In Progress")]
    InProgress,

    [EnumMember(Value = "not_started")]
    NotStarted
}

string desc = Status.InProgress.GetDescription();      // "In Progress"
string member = Status.NotStarted.GetEnumMemberValue(); // "not_started"

// Get any custom attribute
var attr = Status.InProgress.GetCustomAttribute<DescriptionAttribute>();
```

---

## EnumerableEx

Extensions for `IEnumerable<T>`.

```csharp
int[] numbers = [1, 2, 3, 4, 5];

// Null/empty checks
bool empty = numbers.IsNullOrEmpty();      // false
bool hasItems = numbers.IsNotNullOrEmpty(); // true

// Indexed ForEach
numbers.ForEach((item, index) => Console.WriteLine($"{index}: {item}"));

// ForEach with early exit (return false to stop)
numbers.ForEach((item, _) => { Console.WriteLine(item); return item < 3; });

// Batch into groups of N
IEnumerable<IEnumerable<int>> batches = numbers.Batch(2);
// [[1,2], [3,4], [5]]

// Partition by predicate
var (evens, odds) = numbers.Partition(n => n % 2 == 0);
// evens: [2,4], odds: [1,3,5]

// Interleave two sequences
int[] a = [1, 3, 5], b = [2, 4, 6];
var interleaved = a.Interleave(b); // [1,2,3,4,5,6]

// Running accumulator
var running = numbers.Scan(0, (acc, x) => acc + x); // [1,3,6,10,15]

// Sliding window
var windows = numbers.Window(3); // [[1,2,3],[2,3,4],[3,4,5]]

// Shuffle (optional Random parameter)
var shuffled = numbers.Shuffle();

// Fallback if empty
int[] result = Array.Empty<int>().FallbackIfEmpty(99).ToArray(); // [99]

// Consecutive pairs
var pairs = numbers.Pairwise((a, b) => $"{a}->{b}");
// ["1->2","2->3","3->4","4->5"]

// Tag first/last
var tagged = numbers.TagFirstLast((item, isFirst, isLast) =>
    $"{item} first={isFirst} last={isLast}");

// Remove nulls
string?[] mixed = ["hello", null, "world"];
IEnumerable<string> clean = mixed.WhereNotNull(); // ["hello","world"]

// Duplicate and predicate checks
numbers.HasDuplicates();                        // false
numbers.None(n => n > 100);                     // true

// Infinite cycling (caller must limit)
int[] taken = numbers.Cycle().Take(8).ToArray();    // [1,2,3,4,5,1,2,3]

// Flatten nested sequences
IEnumerable<int[]> nested = [[1, 2], [3], [4, 5]];
IEnumerable<int> flat = nested.Flatten();       // [1,2,3,4,5]
```

---

## EnvironmentEx

Extensions for `IHostEnvironment` (ASP.NET Core).

```csharp
IHostEnvironment env = app.Environment;

bool local = env.IsLocal(); // env.EnvironmentName == "local"
bool test  = env.IsTest();  // "test"
bool uat   = env.IsUat();   // "uat"
```

---

## ExceptionEx

Extensions for `Exception` and `ExceptionBase`.

```csharp
try { /* ... */ }
catch (Exception ex)
{
    // Full detail string including inner exceptions and stack traces
    string detail = ex.ToDetailString();

    // Flatten aggregate/inner exceptions into a single list
    IEnumerable<Exception> all = ex.Flatten();
}

// Convert ExceptionBase to ErrorResponse
try { throw new BadRequestException("Invalid input"); }
catch (ExceptionBase ex)
{
    ErrorResponse response = ex.ToErrorResponse();
    // response.Message       = "Invalid input"
    // response.MessageHeader = "Bad Request"
}
```

---

## GuardEx

Argument guard extensions for any value.

```csharp
public void Process(string name, int age, Guid id, IList<string> tags)
{
    name.ThrowIfNull();                          // ArgumentNullException
    name.ThrowIfNullOrWhiteSpace();              // ArgumentException
    name.ThrowIfInvalidFormat(@"^[A-Z]");        // FormatException

    age.ThrowIfNegative();                       // ArgumentOutOfRangeException
    age.ThrowIfNegativeOrZero();
    age.ThrowIfZero();
    age.ThrowIfOutOfRange(1, 120);               // must be in [1, 120]
    age.ThrowIfLessThan(18);                     // ArgumentOutOfRangeException if < 18
    age.ThrowIfGreaterThan(65);                  // ArgumentOutOfRangeException if > 65

    id.ThrowIfDefault();                         // ArgumentException (Guid.Empty)
    tags.ThrowIfEmpty();                         // ArgumentException (empty collection)

    // Custom predicate guard
    age.ThrowIf(a => a > 200, "Age cannot exceed 200");
}
```

---

## GuidEx

Extensions for `Guid`.

```csharp
Guid id = Guid.NewGuid();

bool isEmpty = Guid.Empty.IsEmpty();   // true
string short = id.ToShortString();     // first 8 hex chars
```

---

## HttpEx

Extensions for `HttpResponseMessage` and `HttpRequest` (ASP.NET Core).

```csharp
// Deserialize an HTTP response body
HttpResponseMessage response = await httpClient.GetAsync("/api/users/1");
User user = await response.EnsureObjectAsync<User>();

// Case-insensitive deserialization
User user2 = await response.EnsureObjectAsync<User>(caseInsensitive: true);

// Read raw request body (ASP.NET Core middleware)
app.Use(async (context, next) =>
{
    string body = await context.Request.GetRawBodyAsync();
    await next();
});
```

---

## JsonEx

Extensions for JSON serialization on any object or string.

```csharp
var order = new { Id = 1, Total = 42.50 };

// Serialize
string json = order.ToJson();                       // compact
string pretty = order.ToJson(indented: true);       // pretty-printed

// Deserialize
Order parsed = json.FromJson<Order>();

// Safe try-parse
if (json.TryFromJson<Order>(out Order? result))
    Console.WriteLine(result.Id);
```

---

## ListEx

Extensions for `IList<T>`, `List<T>`, and `List<string>`.

```csharp
var list = new List<string>();

// Add only if not null
string? maybe = GetValueOrNull();
list.AddIfNotNull(maybe);

// Batch add
list.AddRange("a", "b", "c");

// Batch add, skipping nulls
list.AddRangeIfNotNull("x", null, "y"); // adds "x" and "y"

// Fluent Add (returns the list)
var built = new List<int>().Add(1, 2, 3).Add(4);

// String-specific: case-insensitive contains, no-duplicate add
var tags = new List<string> { "CSharp" };
bool has = tags.ContainsIgnoreCase("csharp");        // true
tags.AddIfNotExists("CSharp");                       // no-op (already exists)
tags.AddRangeNoDuplicates(["dotnet", "CSharp"]);     // adds only "dotnet"
```

---

## NumericEx

Extensions for numbers (`INumber<T>`), `int`, `long`, and `double`.

```csharp
int n = 42;

bool zero     = 0.IsZero();        // true
bool positive = n.IsPositive();     // true
bool negative = (-1).IsNegative();  // true

// Human-readable byte sizes
long bytes = 1_073_741_824L;
string size = bytes.ToHumanByteSize();                 // "1.00 GB"
string si   = bytes.ToHumanByteSize(useSiUnits: true); // "1.07 GB"

// Ordinal suffix
string ord = 3.ToOrdinal();  // "3rd"

// Fluent TimeSpan builders
TimeSpan ts = 5.Seconds();       // TimeSpan.FromSeconds(5)
TimeSpan d  = 2.5.Hours();       // TimeSpan.FromHours(2.5)
// Also: .Days(), .Minutes(), .Milliseconds()

// Clamp, range test (INumber<T>)
15.ClampTo(min: 5, max: 10);  // 10
7.IsBetween(5, 10);           // true (inclusive)

// Rounding and percentages (double / decimal)
3.14159.RoundTo(2);           // 3.14
25.0.PercentageOf(200.0);     // 12.5
```

---

## ObjectEx

Extensions for `object?` and generic `T`.

```csharp
object value = "42";

bool isNum   = value.IsNumeric();                     // true
double? dbl  = value.ToNullableDouble();              // 42.0
int?    i    = value.ToNullableInteger();              // 42
bool?   b    = "true".ToNullableBoolean();            // true
DateTime? dt = "2025-01-01".ToNullableDateTime("yyyy-MM-dd");

// Create StringContent for HTTP POST
StringContent content = new { Name = "test" }.ToStringContent();

// Pipe (execute side-effect, returns the original value)
var user = GetUser().Pipe(u => Console.WriteLine(u.Name));
```

---

## SemaphoreSlimEx

Extensions for `SemaphoreSlim`.

```csharp
var semaphore = new SemaphoreSlim(1, 1);

// Async lock with RAII-style disposal
await using (await semaphore.LockAsync())
{
    // exclusive access here
}

// With cancellation token
await using (await semaphore.LockAsync(cancellationToken))
{
    // ...
}
```

---

## ServiceProviderEx

Extensions for `IServiceProvider` (scoped service access).

```csharp
IServiceProvider sp = app.Services;

// Async scoped access with return value
int count = await sp.UseScopedAsync<IUserRepository, int>(
    async repo => await repo.CountAsync());

// Async scoped access without return value
await sp.UseScopedAsync<IEmailService>(
    async svc => await svc.SendWelcomeAsync(userId));

// Synchronous scoped access
sp.UseScoped<ICacheService>(cache => cache.Clear());
```

---

## SetEx

Extensions for `ISet<T>`.

```csharp
ISet<int> set = new HashSet<int> { 1, 2, 3 };

// Compare a set against a JSON-serialized representation
bool match = set.EqualsSerializedSet("[1,2,3]"); // true (order-independent)
```

---

## SpanEx

Extensions for `ReadOnlySpan<char>`.

```csharp
ReadOnlySpan<char> span = "  Hello, World!  ".AsSpan();

bool contains = span.ContainsIgnoreCase("hello");  // true
bool numeric  = "12345".AsSpan().IsNumeric();       // true
ReadOnlySpan<char> trimmed = span.SafeTrim();       // "Hello, World!"
```

---

## StreamEx

Extensions for `Stream`.

```csharp
Stream stream = GetFileStream();

byte[]       bytes  = await stream.ToByteArrayAsync();
string       text   = await stream.ToStringAsync();                // UTF-8
string       latin  = await stream.ToStringAsync(Encoding.Latin1);
MemoryStream copy   = await stream.ToMemoryStreamAsync();

// With cancellation
byte[] data = await stream.ToByteArrayAsync(cancellationToken);
```

---

## StringEx

Extensions for `string`, `string?`, and `byte[]?`.

```csharp
string text = "Hello, World!";

// Encoding
byte[] bytes   = text.ToByteArray();        // UTF-8
string base64  = text.ToBase64();
string decoded = base64.FromBase64();

// Cleanup
string clean = "h3ll0!@#w0rld".RemoveSpecialCharacters(); // "h3ll0w0rld"
string trimmed = ((string?)null).SafeTrim();                // ""

// Splitting
string[] parts = "one::two::three".RegexSplit("::");

// Comparisons (case-insensitive)
bool eq  = "ABC".EqualsIgnoreCase("abc");            // true
bool sw  = "Hello".StartsWithIgnoreCase("hel");      // true
bool ew  = "Hello".EndsWithIgnoreCase("LLO");        // true
bool ct  = "Hello".ContainsIgnoreCase("ell");        // true

// With auto-trim
bool eqt = "  ABC  ".EqualsIgnoreCaseWithTrim("abc"); // true

// Enum parsing
DayOfWeek day = "Monday".ToEnum<DayOfWeek>();

// Validation
bool isEmail = "user@example.com".IsWellFormedEmailAddress(); // true
bool isNum   = "42".IsNumeric();                              // true
bool isGuid  = Guid.NewGuid().ToString().IsGuid();            // true
bool blank   = ((string?)null).IsNullOrWhiteSpace();          // true

// Truncation & masking
string trunc  = "Hello, World!".Truncate(5);           // "Hello..."
string masked = "4111111111111111".Mask(4);             // "************1111"

// Case conversion & humanization
string snake  = "HelloWorld".ToSnakeCase();     // "hello_world"
string kebab  = "HelloWorld".ToKebabCase();     // "hello-world"
string camel  = "hello_world".ToCamelCase();    // "helloWorld"
string human  = "order_line_item".Humanize();   // "Order line item"

// Slug & reverse
string slug = "Hello, World! 2025".ToSlug();    // "hello-world-2025"
string rev  = "abc".Reverse();                  // "cba"

// byte[]? -> string
byte[]? raw = GetBytes();
string str = raw.FromByteArray(); // UTF-8 decode, or "" if null
```

Transformations and formatting:

```csharp
"my_property_name".ToPascalCase();          // "MyPropertyName" (also handles camelCase, kebab-case)
"hello world".ToTitleCase();                // "Hello World"
"<p>hi <b>there</b></p>".StripHtml();       // "hi there"
"café naïve".RemoveDiacritics();            // "cafe naive"
"hello world".WordCount();                  // 2
"ab".Repeat(3);                             // "ababab"

"world".EnsureStartsWith("hello-");         // "hello-world" (or as-is if already present)
"file".EnsureEndsWith(".txt");              // "file.txt"

// Multiple replacements in one pass
"foo and baz".ReplaceMultiple(new Dictionary<string, string>
{
    ["foo"] = "bar",
    ["baz"] = "qux",
}); // "bar and qux"
```

---

## TaskEx

Extensions for `Task`, `Task<T>`, and static async helpers.

```csharp
// Sequential execution (respects order)
await TaskEx.WhenAllSequentialAsync(
    () => StepOneAsync(),
    () => StepTwoAsync(),
    () => StepThreeAsync());

// Sequential with results
IEnumerable<int> results = await TaskEx.WhenAllSequentialAsync(
    () => ComputeAAsync(),
    () => ComputeBAsync());

// Retry with exponential backoff
string html = await TaskEx.RetryAsync(
    () => httpClient.GetStringAsync("https://example.com"),
    maxRetries: 3,
    delay: TimeSpan.FromSeconds(1),
    exponentialBackoff: true);

// Retry with exception filter
await TaskEx.RetryAsync(
    () => SaveAsync(),
    maxRetries: 5,
    retryWhen: ex => ex is TimeoutException);

// Timeout
string data = await LongRunningAsync()
    .WithTimeoutAsync(TimeSpan.FromSeconds(10));

// Error callback
string safe = await RiskyAsync()
    .OnFailureAsync(ex => logger.LogError(ex, "Failed"));

// Fire and forget (swallows exceptions, optional handler)
SendAnalyticsAsync().FireAndForget(ex => logger.LogWarning(ex, "Analytics failed"));

// Attach a cancellation token to a task that does not natively accept one.
// Throws OperationCanceledException if the token fires before the task completes.
int value = await someTask.WithCancellationAsync(cancellationToken);
```

---

## TimeSpanEx

Extensions for `TimeSpan`.

```csharp
TimeSpan elapsed = TimeSpan.FromHours(2) + TimeSpan.FromMinutes(30);

string human = elapsed.ToHumanReadable(); // "2 hours 30 minutes"

// Control decimal precision
TimeSpan precise = TimeSpan.FromSeconds(61.456);
string p = precise.ToHumanReadable(decimalPlaces: 2); // "1 minute 1.46 seconds"

// Clock formatting (HH:MM:SS) — hours are not capped at 24
new TimeSpan(1, 3, 10, 0).ToClockString();  // "27:10:00"
TimeSpan.FromSeconds(-65).ToClockString();  // "-00:01:05"

TimeSpan.FromSeconds(1).IsPositive();       // true
TimeSpan.FromSeconds(-1).IsNegative();      // true
```

---

## UriEx

Extensions for `Uri`.

```csharp
var uri = new Uri("https://user:pass@example.com:8080/api/v1?q=test#section");

string dump = uri.DumpProperties();
// Multi-line string with Scheme, Host, Port, Path, Query, Fragment, etc.

// Path and query manipulation
Uri api = new Uri("https://example.com/api/").AppendPath("users");
// https://example.com/api/users

IReadOnlyDictionary<string, string> query = uri.GetQueryParameters();
// { "q": "test" }

Uri withQuery = new Uri("https://example.com/api")
    .WithQueryParameter("page", "2")
    .WithQueryParameter("size", "50");
// https://example.com/api?page=2&size=50
```

---

## XmlSerializerEx

Extensions for XML serialization.

```csharp
// Serialize any object to XML
var item = new Product { Id = 1, Name = "Widget" };
string xml = item.Serialize();

// Deserialize from XML string
Product restored = xml.Deserialize<Product>();
```


---

## EnumerableExAsync

Asynchronous extension methods for `IEnumerable<T>`. For synchronous extensions, see [EnumerableEx](#enumerableex).

```csharp
var userIds = new[] { 1, 2, 3, 4, 5 };

// Sequential async projection
var users = await userIds.SelectAsync(id => GetUserAsync(id));

// Parallel async projection with concurrency limit
var enrichedUsers = await users.SelectAsyncParallel(
    user => EnrichUserDataAsync(user),
    maxConcurrency: 10);

// Async filtering
var activeUsers = await users.WhereAsync(user => IsActiveAsync(user));

// Async iteration
await users.ForEachAsync(user => ProcessUserAsync(user));

// Async aggregation
var totalScore = await scores.AggregateAsync(
    0,
    async (sum, score) => sum + await CalculateAsync(score));

// Async predicates
bool hasAdmin = await users.AnyAsync(user => IsAdminAsync(user));
bool allActive = await users.AllAsync(user => IsActiveAsync(user));
```

---

## FuncEx

Extensions for `Func<T>` and `Action` delegates providing memoization, debouncing, and throttling.

```csharp
// Memoization - cache expensive function results
Func<int, int> fibonacci = null!;
fibonacci = n => n <= 1 ? n : fibonacci(n - 1) + fibonacci(n - 2);
var memoized = fibonacci.Memoize();

var result1 = memoized(40); // Calculated
var result2 = memoized(40); // Retrieved from cache (instant!)

// Debounce - delay execution until calls stop
Action search = () => PerformSearch();
var debouncedSearch = search.Debounce(TimeSpan.FromMilliseconds(300));

// In event handler (e.g., search-as-you-type)
textBox.TextChanged += (s, e) => debouncedSearch();
// Only executes after user stops typing for 300ms

// Throttle - limit execution frequency
Action updateMetrics = () => SendMetricsToServer();
var throttled = updateMetrics.Throttle(TimeSpan.FromSeconds(5));

// Called many times, but only executes once per 5 seconds
for (int i = 0; i < 100; i++)
    throttled();

// Lazy initialization
Func<ExpensiveResource> factory = () => new ExpensiveResource();
Lazy<ExpensiveResource> lazy = factory.ToLazy();

// Resource only created on first access
var resource = lazy.Value;
```

---

## RegexEx

Regex extension methods on `string` that avoid constructing a `Regex` manually.

```csharp
"abc123".RegexIsMatch(@"[a-z]+\d+");                          // true
"abc123def456".RegexMatch(@"\d+");                            // "123"
"abc123def456".RegexMatches(@"\d+");                          // ["123", "456"]
"a1 b2 c3".RegexReplace(@"\d", m => (int.Parse(m.Value) * 10).ToString());
                                                              // "a10 b20 c30"
"abc123".RegexReplace(@"\d+", "X");                           // "abcX"
```

---

## CancellationTokenEx

```csharp
// Convert a token to a Task that completes (cancelled) when the token fires
Task task = cancellationToken.AsTask();

// Create a linked token that also cancels after a timeout (caller disposes)
using CancellationTokenSource linked = outerToken.WithTimeout(TimeSpan.FromSeconds(30));
```
