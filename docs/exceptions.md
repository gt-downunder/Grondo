---
title: Custom Exceptions
layout: default
nav_order: 3
---

# Custom Exceptions

Grondo provides a hierarchy of HTTP-aware domain exceptions that extend `ExceptionBase`.
Each exception carries an HTTP status code and a short message header.

| Exception | HTTP Status | Default Header |
|---|---|---|
| `BadRequestException` | 400 Bad Request | "Bad Request" |
| `NotAuthorizedException` | 401 Unauthorized | "Not authorized" |
| `PaymentRequiredException` | 402 Payment Required | "Payment Required" |
| `ForbiddenException` | 403 Forbidden | "Forbidden" |
| `EntityNotFoundException` | 404 Not Found | "Not found" |
| `ConflictException` | 409 Conflict | "Conflict" |
| `BusinessException` | 409 Conflict | "Business rule violation" |
| `DuplicateFoundException` | 409 Conflict | "Duplicate found" |
| `UnprocessableEntityException` | 422 Unprocessable Entity | "Unprocessable Entity" |
| `TooManyRequestsException` | 429 Too Many Requests | "Too Many Requests" |
| `TechnicalException` | 500 Internal Server Error | "Internal server error" |
| `MethodNotAvailableException` | 501 Not Implemented | "Method not available" |
| `ServiceUnavailableException` | 503 Service Unavailable | "Service Unavailable" |
| `ValidationException` | 400 Bad Request | "Validation Failed" |

---

## Throwing and Catching

```csharp
// Throw with a custom message
throw new EntityNotFoundException("User with ID 42 was not found");

// Throw with custom message and header
throw new BadRequestException("Email is invalid", "Validation Error");

// Throw with inner exception
throw new TechnicalException("Database timeout", innerException: ex);

// TechnicalException has a parameterless constructor (uses defaults)
throw new TechnicalException();
// Message: "Please contact the system administrator"
// Header:  "Internal server error"
```

---

## Converting to ErrorResponse

```csharp
try
{
    throw new ForbiddenException("You do not have access to this resource");
}
catch (ExceptionBase ex)
{
    // ExceptionBase properties
    HttpStatusCode code = ex.StatusCode;       // 403
    string header       = ex.MessageHeader;    // "Forbidden"
    string message      = ex.Message;          // "You do not have access..."

    // Convert to a serializable DTO
    ErrorResponse response = ex.ToErrorResponse();
    // response.Message       → "You do not have access to this resource"
    // response.MessageHeader → "Forbidden"

    return Results.Json(response, statusCode: (int)code);
}
```

---

## ErrorResponse Record

```csharp
// ErrorResponse is a simple record with two required properties:
public record ErrorResponse
{
    public required string Message { get; init; }
    public required string MessageHeader { get; init; }
}

// You can also construct it manually:
var error = new ErrorResponse
{
    Message = "Something went wrong",
    MessageHeader = "Error"
};
```

---

## New Exception Types

### ValidationException

For field-level validation errors with detailed error messages per field.

`ValidationException.Errors` is exposed as an immutable
`IReadOnlyDictionary<string, IReadOnlyList<string>>` — both the dictionary and the inner lists are
read-only, and the supplied errors are defensively copied by the constructor.

```csharp
// Single field error
throw new ValidationException("Email", "Email is required");

// Multiple field errors (the dictionary is copied; callers may safely mutate their own copy)
throw new ValidationException(new Dictionary<string, string[]>
{
    ["Email"] = new[] { "Email is required", "Email must be valid" },
    ["Age"] = new[] { "Age must be 18 or older" },
    ["Name"] = new[] { "Name is required" }
});

// Read-only shape (preferred for new code)
IReadOnlyDictionary<string, IReadOnlyList<string>> errors = new Dictionary<string, IReadOnlyList<string>>
{
    ["Email"] = new[] { "Email is required" },
    ["Age"]   = new[] { "Age must be 18 or older" },
};
throw new ValidationException(errors);

// In controller
try
{
    var user = CreateUser(request);
    return Ok(user);
}
catch (ValidationException ex)
{
    // Returns structured validation errors
    return BadRequest(new
    {
        message = ex.Message,
        errors = ex.Errors
    });
}

// Response:
// {
//   "message": "One or more validation errors occurred.",
//   "errors": {
//     "Email": ["Email is required", "Email must be valid"],
//     "Age": ["Age must be 18 or older"],
//     "Name": ["Name is required"]
//   }
// }
```

### TooManyRequestsException

For rate limiting scenarios with optional retry-after duration.

```csharp
// Basic usage
throw new TooManyRequestsException("Rate limit exceeded");

// With retry-after
throw new TooManyRequestsException("Too many requests")
{
    RetryAfter = TimeSpan.FromMinutes(5)
};

// In middleware
if (rateLimiter.IsLimitExceeded(userId))
{
    var retryAfter = rateLimiter.GetRetryAfter(userId);
    throw new TooManyRequestsException("Rate limit exceeded")
    {
        RetryAfter = retryAfter
    };
}

// Client can check RetryAfter property
catch (TooManyRequestsException ex)
{
    if (ex.RetryAfter.HasValue)
    {
        // Wait before retrying
        await Task.Delay(ex.RetryAfter.Value);
    }
}
```

### ServiceUnavailableException

For temporary service outages with optional retry-after duration.

```csharp
// Basic usage
throw new ServiceUnavailableException("Database is temporarily unavailable");

// With retry-after
throw new ServiceUnavailableException("Service is down for maintenance")
{
    RetryAfter = TimeSpan.FromMinutes(30)
};

// In health check
if (!await healthCheck.IsHealthyAsync())
{
    throw new ServiceUnavailableException("Service is unhealthy")
    {
        RetryAfter = TimeSpan.FromSeconds(30)
    };
}
```

### UnprocessableEntityException

For requests that are syntactically correct but semantically invalid.

```csharp
// When business rules prevent processing
if (order.Total < minimumOrderAmount)
{
    throw new UnprocessableEntityException(
        $"Order total must be at least {minimumOrderAmount:C}");
}

// When data is valid but cannot be processed
if (product.Stock < order.Quantity)
{
    throw new UnprocessableEntityException(
        $"Insufficient stock. Available: {product.Stock}, Requested: {order.Quantity}");
}
```


---

## ProblemDetails and automatic middleware

Every `ExceptionBase` can be converted to an RFC 7807 `ProblemDetails` response, and the provided
ASP.NET Core middleware does this automatically.

### Manual conversion

The `ToProblemDetails` extension lives in `Grondo.Extensions` (alongside the other `*Ex`
classes), so make sure that namespace is imported:

```csharp
using Grondo.Extensions;

catch (EntityNotFoundException ex)
{
    ProblemDetails details = ex.ToProblemDetails(instance: HttpContext.Request.Path);
    return Results.Json(details, statusCode: details.Status);
}
```

`ValidationException` is converted to a `ValidationProblemDetails` so field-level errors are
preserved in the `errors` member.

### Middleware

Register the middleware once at application start-up:

```csharp
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseGrondoExceptionHandling();
// Optionally, customize:
app.UseGrondoExceptionHandling(new ExceptionHandlingOptions
{
    HandleUnexpectedExceptions = true,         // catch non-ExceptionBase too
    IncludeUnexpectedExceptionDetails = false, // redact messages from unknown exceptions
    // Override the RFC 7807 `type` URI (defaults to MDN documentation, e.g.
    // https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/404):
    ProblemTypeUriFormatter = code => $"https://docs.example.com/api/errors/{code}",
});
```

Any `ExceptionBase` thrown during request handling is converted to `application/problem+json`
with the correct status code, title, and detail. Unhandled exceptions (if enabled) surface as
a generic 500 response. The `type` URI defaults to the MDN documentation page for the status
code; override `ProblemTypeUriFormatter` to point at application-specific documentation.
