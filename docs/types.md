---
title: Types
layout: default
nav_order: 5
---

# Types

---

## Result\<T\>

A `readonly struct` for railway-oriented programming. An operation either succeeds with a value
or fails with an error message — no exceptions needed for expected failures.

### Creating Results

```csharp
// Explicit factory methods
Result<int> ok  = Result<int>.Success(42);
Result<int> err = Result<int>.Failure("Something went wrong");

// Implicit conversion from value
Result<int> implicit = 42;

// Wrap any value
Result<string> wrapped = "hello".ToResult();

// Safe execution (catches exceptions)
Result<int> safe = ResultEx.TryExecute(() => int.Parse("abc"));
// Failure("Input string was not in a correct format.")

// Async safe execution
Result<string> data = await ResultEx.TryExecuteAsync(
    () => httpClient.GetStringAsync("/api/data"));
```

### Inspecting Results

```csharp
Result<int> result = GetResult();

if (result.IsSuccess)
    Console.WriteLine(result.Value);   // access the value

if (result.IsFailure)
    Console.WriteLine(result.Error);   // access the error message

int fallback = result.GetValueOrDefault(-1); // value or fallback

string display = result.ToString(); // "Success(42)" or "Failure(error)"
```

### Railway-Oriented Pipeline (Synchronous)

```csharp
Result<Order> result = GetUserId()              // Result<int>
    .Ensure(id => id > 0, "Invalid user ID")    // validate
    .Map(id => new OrderRequest(id))             // transform value
    .Bind(req => PlaceOrder(req))                // chain to another Result
    .Tap(order => logger.Log($"Order {order.Id} placed"))  // side-effect on success
    .TapError(err => logger.LogWarning(err))     // side-effect on failure
    .MapError(err => $"Order failed: {err}");    // transform error message

// Pattern match to extract the final value
string message = result.Match(
    onSuccess: order => $"Order #{order.Id} confirmed",
    onFailure: error => $"Error: {error}");
```

### Railway-Oriented Pipeline (Async)

Instance async methods (when you already have a `Result<T>`):

```csharp
Result<int> userId = GetUserId();

Result<Order> order = await userId
    .MapAsync(id => FetchUserAsync(id))          // async transform
    .BindAsync(user => CreateOrderAsync(user))   // async chain
    .TapAsync(o => SendConfirmationAsync(o))     // async side-effect
    .TapErrorAsync(e => LogErrorAsync(e));        // async error side-effect
```

Fluent extensions on `Task<Result<T>>` (full async pipeline):

```csharp
Result<OrderConfirmation> confirmation = await GetUserIdAsync()    // Task<Result<int>>
    .EnsureAsync(id => id > 0, "Invalid ID")
    .MapAsync(id => LookupUserAsync(id))
    .BindAsync(user => PlaceOrderAsync(user))
    .TapAsync(order => NotifyAsync(order))
    .TapErrorAsync(err => AlertOpsAsync(err))
    .MapErrorAsync(err => $"Pipeline failed: {err}");

// Async pattern match
string msg = await GetUserIdAsync()
    .MatchAsync(
        onSuccess: id => $"Found user {id}",
        onFailure: err => $"Error: {err}");
```

### Type Conversions

```csharp
Result<int> result = Result<int>.Success(42);

// Result → Maybe (drops the error, keeps presence/absence)
Maybe<int> maybe = result.ToMaybe();         // Some(42)
Result<int>.Failure("err").ToMaybe();        // None

// Result → Either (Success → Right, Failure → Left)
Either<string, int> either = result.ToEither(); // Right(42)
Result<int>.Failure("err").ToEither();          // Left("err")

// Result → Validation (Success → Valid, Failure → Invalid)
Validation<int> validation = result.ToValidation(); // Valid(42)
Result<int>.Failure("err").ToValidation();          // Invalid(["err"])
```

### Combining Results

```csharp
Result<string> name  = Result<string>.Success("Alice");
Result<int>    age   = Result<int>.Success(30);

// Combine two results into a tuple
Result<(string, int)> combined = ResultEx.Combine(name, age);
// Success(("Alice", 30))

// Combine a collection
var results = new[] { Result<int>.Success(1), Result<int>.Success(2) };
Result<IReadOnlyList<int>> all = ResultEx.Combine(results);
// Success([1, 2])

// If any fails, the first error propagates
var mixed = new[] { Result<int>.Success(1), Result<int>.Failure("bad") };
Result<IReadOnlyList<int>> fail = ResultEx.Combine(mixed);
// Failure("bad")
```

---

## Result (non-generic)

For void operations that succeed or fail but don't return a value.

```csharp
// Create
Result ok  = Result.Success();
Result err = Result.Failure("Operation failed");

// Safe execution
Result safe = ResultEx.TryExecute(() => File.Delete("temp.txt"));
Result asyncSafe = await ResultEx.TryExecuteAsync(() => SendEmailAsync());

// Inspect
if (ok.IsSuccess) Console.WriteLine("Done");
if (err.IsFailure) Console.WriteLine(err.Error);
Console.WriteLine(ok.ToString());  // "Success"
Console.WriteLine(err.ToString()); // "Failure(Operation failed)"

// ROP pipeline
Result result = Result.Success()
    .Tap(() => logger.Log("Starting"))
    .Ensure(() => CanProceed(), "Cannot proceed")
    .TapError(err => logger.LogWarning(err))
    .MapError(err => $"Wrapped: {err}");

// Bind to produce a Result<T>
Result<Order> order = Result.Success()
    .Bind(() => CreateOrder());

// Pattern match
string msg = result.Match(
    onSuccess: () => "All good",
    onFailure: error => $"Failed: {error}");

// Async pipeline on Task<Result>
Result final = await DoWorkAsync()       // Task<Result>
    .TapAsync(() => NotifyAsync())
    .TapErrorAsync(err => LogAsync(err))
    .EnsureAsync(() => IsValid(), "Invalid state")
    .MapErrorAsync(err => $"Pipeline: {err}");

string asyncMsg = await DoWorkAsync()
    .MatchAsync(
        onSuccess: () => "OK",
        onFailure: err => $"Error: {err}");

// Async bind to Result<T>
Result<int> value = await DoWorkAsync()
    .BindAsync(() => ComputeAsync());
```

---

## Maybe\<T\>

A `readonly struct` representing an optional value — similar to `Option` in functional languages.
Use `Maybe<T>` when a value might legitimately be absent (instead of returning `null`).

### Creating

```csharp
Maybe<string> some = Maybe<string>.Some("hello");
Maybe<string> none = Maybe<string>.None;

// Implicit conversion (null → None, non-null → Some)
Maybe<string> fromValue = "hello";        // Some("hello")
Maybe<string> fromNull  = (string?)null;  // None
```

### Inspecting

```csharp
Maybe<int> maybe = Maybe<int>.Some(42);

if (maybe.HasValue)
    Console.WriteLine(maybe.Value);  // 42

if (maybe.HasNoValue)
    Console.WriteLine("No value");

int safe = maybe.GetValueOrDefault(-1); // 42 (or -1 if None)
Console.WriteLine(maybe.ToString());    // "Some(42)" or "None"
```

### Transformations

```csharp
Maybe<string> name = Maybe<string>.Some("  Alice  ");

// Map — transform the inner value
Maybe<string> trimmed = name.Map(n => n.Trim()); // Some("Alice")

// Bind — chain to another Maybe
Maybe<User> user = name.Bind(n => FindUserByName(n)); // Some(User) or None

// Where — filter
Maybe<string> long = name.Where(n => n.Length > 10); // None (too short)

// Execute — side-effect (does nothing if None)
name.Execute(n => Console.WriteLine($"Hello, {n}!"));
```

### Pattern Matching

```csharp
Maybe<int> age = Maybe<int>.Some(25);

string message = age.Match(
    some: a => $"Age is {a}",
    none: () => "Age unknown");
// "Age is 25"
```

### Type Conversions

```csharp
Maybe<User> maybeUser = FindUser(42);

// Maybe → Result (None becomes Failure with your error message)
Result<User> result = maybeUser.ToResult("User not found");

// Maybe → Either (None becomes Left with your value)
Either<string, User> either = maybeUser.ToEither("User not found");

// Maybe → Validation (None becomes Invalid with your error message)
Validation<User> validation = maybeUser.ToValidation("User is required");

// Then continue with the full Result pipeline
string output = result
    .Map(u => u.Name)
    .Match(
        onSuccess: name => $"Found: {name}",
        onFailure: err => err);
```

### Async Transformations

```csharp
Maybe<int> userId = Maybe<int>.Some(123);

// Async map
Maybe<User> user = await userId.MapAsync(id => GetUserAsync(id));

// Async bind
Maybe<Profile> profile = await userId.BindAsync(id => GetProfileAsync(id));

// Async tap (side-effect)
await userId.TapAsync(id => LogAccessAsync(id));
```

### New Methods (Enhanced API)

```csharp
// OrElse - provide alternative Maybe
Maybe<string> name = GetName().OrElse(Maybe<string>.Some("Anonymous"));

// OrValue - convert None to Some with default
Maybe<int> count = GetCount().OrValue(0);

// Filter - alias for Where
Maybe<int> positive = GetNumber().Filter(n => n > 0);

// Flatten - unwrap nested Maybe
Maybe<Maybe<int>> nested = Maybe<Maybe<int>>.Some(Maybe<int>.Some(42));
Maybe<int> flat = Maybe<int>.Flatten(nested); // Some(42)

// Zip - combine two Maybes
Maybe<(string, int)> combined = Maybe<(string, int)>.Zip(
    Maybe<string>.Some("Alice"),
    Maybe<int>.Some(30));
// Some(("Alice", 30))
```


---

## Either\<TLeft, TRight\>

A `readonly struct` representing a value that can be one of two types. By convention, `Right` represents success and `Left` represents failure/error. Unlike `Result<T>` which uses strings for errors, `Either<L,R>` allows you to use **any type** for the error.

### Creating

```csharp
// Create from left (error)
Either<ValidationError, User> left = Either<ValidationError, User>.FromLeft(
    new ValidationError("Invalid email"));

// Create from right (success)
Either<ValidationError, User> right = Either<ValidationError, User>.FromRight(
    new User { Name = "Alice" });
```

### Pattern Matching

```csharp
Either<ValidationError, User> result = CreateUser(request);

string message = result.Match(
    onLeft: error => $"Validation failed: {error.Message}",
    onRight: user => $"Created user: {user.Name}");
```

### Transformations

```csharp
Either<string, int> either = Either<string, int>.FromRight(42);

// Map - transform the Right value
Either<string, string> mapped = either.Map(n => n.ToString());
// Right("42")

// Bind - chain operations
Either<string, int> doubled = either.Bind(n =>
    Either<string, int>.FromRight(n * 2));
// Right(84)

// MapLeft - transform the Left value
Either<int, string> leftMapped = either.MapLeft(err => err.Length);

// Tap - side-effect on Right
either.Tap(n => Console.WriteLine($"Value: {n}"));

// TapLeft - side-effect on Left
either.TapLeft(err => Console.WriteLine($"Error: {err}"));
```

### Async Operations

```csharp
Either<string, int> either = Either<string, int>.FromRight(42);

// Async map
Either<string, User> user = await either.MapAsync(id => GetUserAsync(id));

// Async bind
Either<string, Profile> profile = await either.BindAsync(id =>
    GetProfileAsync(id));
```

### Type Conversions

```csharp
Either<string, int> either = Either<string, int>.FromRight(42);

// Either → Maybe (Right becomes Some, Left is discarded)
Maybe<int> maybe = either.ToMaybe();          // Some(42)

// Either → Result (Left mapped to error string)
Result<int> result = either.ToResult(err => $"Failed: {err}");
// Success(42)

Either<int, string>.FromLeft(404).ToResult(code => $"Error {code}");
// Failure("Error 404")
```

### Use Cases

```csharp
// Typed errors instead of strings
public Either<ValidationError, Order> CreateOrder(OrderRequest request)
{
    var validation = ValidateOrder(request);
    if (!validation.IsValid)
        return Either<ValidationError, Order>.FromLeft(
            new ValidationError(validation.Errors));

    var order = new Order { /* ... */ };
    return Either<ValidationError, Order>.FromRight(order);
}

// In controller
var result = CreateOrder(request);
return result.Match(
    onLeft: error => BadRequest(new { errors = error.Fields }),
    onRight: order => Ok(order));
```

---

## Validation\<T\>

A `readonly struct` for **accumulative validation** — collects **all** validation errors instead of stopping at the first one. Perfect for form validation where you want to show all errors to the user.

### Creating

```csharp
// Valid result
Validation<User> valid = Validation<User>.Valid(new User { Name = "Alice" });

// Invalid result with errors
Validation<User> invalid = Validation<User>.Invalid(
    "Name is required",
    "Email is invalid",
    "Age must be 18+");
```

### Accumulating Errors

```csharp
public Validation<User> ValidateUser(UserRequest request)
{
    var errors = new List<string>();

    // Validate name
    if (string.IsNullOrWhiteSpace(request.Name))
        errors.Add("Name is required");
    if (request.Name?.Length > 100)
        errors.Add("Name must be 100 characters or less");

    // Validate email
    if (string.IsNullOrWhiteSpace(request.Email))
        errors.Add("Email is required");
    if (!IsValidEmail(request.Email))
        errors.Add("Email must be valid");

    // Validate age
    if (request.Age < 18)
        errors.Add("Age must be 18 or older");

    // Return all errors at once!
    if (errors.Count > 0)
        return Validation<User>.Invalid(errors);

    return Validation<User>.Valid(new User
    {
        Name = request.Name,
        Email = request.Email,
        Age = request.Age
    });
}

// Usage
var validation = ValidateUser(request);
if (validation.IsInvalid)
{
    // Returns ALL errors:
    // ["Name is required", "Email must be valid", "Age must be 18 or older"]
    return BadRequest(new { errors = validation.Errors });
}

var user = validation.Value;
```

### Transformations

```csharp
Validation<User> validation = ValidateUser(request);

// Map - transform the value if valid
Validation<UserDto> dto = validation.Map(user => new UserDto(user));

// Bind - chain validations
Validation<Order> order = validation.Bind(user => CreateOrder(user));

// Pattern match
string message = validation.Match(
    onValid: user => $"Created: {user.Name}",
    onInvalid: errors => $"Errors: {string.Join(", ", errors)}");
```

### Combining Validations

```csharp
var nameValidation = ValidateName(request.Name);
var emailValidation = ValidateEmail(request.Email);
var ageValidation = ValidateAge(request.Age);

// Combine - accumulates all errors from all validations
var combined = Validation<User>.Combine(
    nameValidation,
    emailValidation,
    ageValidation);

if (combined.IsInvalid)
{
    // Returns errors from ALL three validations!
    return BadRequest(combined.Errors);
}
```

### Convert to Result

```csharp
Validation<User> validation = ValidateUser(request);

// Convert to Result (combines all errors into single message)
Result<User> result = validation.ToResult();
// If invalid: Failure("Name is required; Email must be valid; Age must be 18+")
```

### Async Operations

```csharp
Validation<User> validation = Validation<User>.Valid(user);

// Async map
Validation<Profile> profile = await validation.MapAsync(u =>
    EnrichProfileAsync(u));

// Async bind
Validation<Order> order = await validation.BindAsync(u =>
    CreateOrderAsync(u));
```

---

## LINQ Query Syntax Support

All functional types (`Result<T>`, `Maybe<T>`) now support **LINQ query syntax** for more readable functional composition!

### Result\<T\> with LINQ

```csharp
// Before: Nested Bind calls
var result = GetUser(id)
    .Bind(user => GetProfile(user.Id)
        .Bind(profile => GetSettings(profile.Id)
            .Map(settings => new UserViewModel(user, profile, settings))));

// After: LINQ query syntax
var result = from user in GetUser(id)
             from profile in GetProfile(user.Id)
             from settings in GetSettings(profile.Id)
             select new UserViewModel(user, profile, settings);

// More complex example
var orderResult =
    from userId in ValidateUserId(request.UserId)
    from user in GetUser(userId)
    from product in GetProduct(request.ProductId)
    from inventory in CheckInventory(product.Id)
    select new Order
    {
        UserId = user.Id,
        ProductId = product.Id,
        Quantity = request.Quantity
    };
```

### Maybe\<T\> with LINQ

```csharp
// Before: Nested Bind calls
var name = GetUser(id)
    .Bind(user => user.GetProfile()
        .Map(profile => profile.DisplayName));

// After: LINQ query syntax
var name = from user in GetUser(id)
           from profile in user.GetProfile()
           select profile.DisplayName;

// With filtering
var activeName =
    from user in GetUser(id)
    where user.IsActive
    from profile in user.GetProfile()
    where !string.IsNullOrEmpty(profile.DisplayName)
    select profile.DisplayName;
```

### Mixing Result and Maybe

```csharp
// Convert Maybe to Result, then use LINQ
var result =
    from user in GetUser(id).ToResult("User not found")
    from profile in GetProfile(user.Id)
    from settings in GetSettings(profile.Id)
    select new UserData(user, profile, settings);
```

### Benefits

- **More readable** - Looks like regular LINQ queries
- **Familiar syntax** - Developers already know LINQ
- **Composable** - Easy to add more steps
- **Type-safe** - Full IntelliSense support
- **Short-circuiting** - Stops at first failure (Result) or None (Maybe)
