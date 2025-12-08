# Code Review - RecipesAPI

**Review Date:** 2025-11-18
**Project:** RecipesAPI (.NET 9.0 Web API)
**Reviewer:** AI Code Review

---

## Executive Summary

The RecipesAPI is a well-structured .NET 9.0 Web API with clean architecture principles. The codebase demonstrates good use of modern .NET features including dependency injection, Entity Framework Core, and JWT authentication. However, there are several critical security issues, code quality improvements, and best practice violations that should be addressed.

**Severity Levels:**
- 🔴 **Critical** - Must fix immediately (security/data loss risks)
- 🟠 **High** - Should fix soon (significant issues)
- 🟡 **Medium** - Should address (quality/maintainability)
- 🟢 **Low** - Nice to have (minor improvements)

---

## 🔴 Critical Issues

### 1. Facebook Authentication Security Vulnerability
**Location:** `Controllers/AuthController.cs:87-107`, `Services/AuthService.cs:209-255`

**Issue:** The Facebook login endpoint accepts user data without any verification. Unlike Google OAuth which validates the token, Facebook login trusts the client-provided data completely.

```csharp
[HttpPost("facebook")]
public async Task<IActionResult> FacebookSignIn([FromBody] LoginFacebookDTO user)
{
    var authResponse = await _authService.LoginFacebookAsync(user);
    // No token verification!
}
```

**Risk:** An attacker can impersonate any Facebook user by sending fabricated data.

**Recommendation:**
```csharp
// Verify Facebook access token with Facebook Graph API
var client = new HttpClient();
var response = await client.GetAsync(
    $"https://graph.facebook.com/me?access_token={facebookToken}&fields=id,email,first_name,last_name"
);
```

### 2. Missing Authorization on Recipe Creation
**Location:** `Controllers/RecipeController.cs:20-32`

**Issue:** Recipe creation endpoints are not protected with `[Authorize]` attribute. Anyone can create recipes without authentication.

**Current:**
```csharp
[HttpPost]
public async Task<IActionResult> CreateRecipe([FromBody] CreateRecipeDTO dto)
```

**Should be:**
```csharp
[Authorize]
[HttpPost]
public async Task<IActionResult> CreateRecipe([FromBody] CreateRecipeDTO dto)
```

### 3. No File Size Limits on Image Uploads
**Location:** `Controllers/ImageController.cs:19-63`

**Issue:** No maximum file size validation, allowing potential DoS attacks through massive file uploads.

**Recommendation:**
```csharp
const long MAX_FILE_SIZE = 5 * 1024 * 1024; // 5MB

foreach (var file in files)
{
    if (file.Length > MAX_FILE_SIZE)
        return BadRequest($"File {file.FileName} exceeds maximum size of 5MB.");
}
```

### 4. Sensitive Error Information Disclosure
**Location:** Multiple controllers

**Issue:** Exception messages are returned directly to clients, potentially exposing internal implementation details.

**Example:**
```csharp
catch (Exception ex)
{
    return BadRequest(new { message = ex.Message }); // Exposes internal details
}
```

**Recommendation:** Log detailed errors server-side, return generic messages to clients.

---

## 🟠 High Priority Issues

### 5. Missing Logging Framework
**Location:** `Services/RecipeService.cs:130`, throughout application

**Issue:** Using `Console.WriteLine()` instead of proper logging framework.

```csharp
catch (Exception ex)
{
    Console.WriteLine(ex.Message); // Not production-ready
    throw new Exception("An error occurred while creating the recipe. " + ex.Message);
}
```

**Recommendation:** Implement ILogger
```csharp
private readonly ILogger<RecipeService> _logger;

_logger.LogError(ex, "Failed to create recipe for AuthorId {AuthorId}", dto.AuthorId);
```

### 6. No Pagination on List Endpoints
**Location:** `Controllers/RecipeController.cs:94-106`, `Services/RecipeService.cs:30-44`

**Issue:** `GetAllRecipes()` returns all recipes at once, which will cause performance issues as data grows.

**Recommendation:**
```csharp
public async Task<IActionResult> GetAllRecipes(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20)
{
    var recipes = await _recipeService.GetAllRecipesAsync(page, pageSize);
    return Ok(recipes);
}
```

### 7. Bulk Insert Without Transaction
**Location:** `Controllers/RecipeController.cs:34-49`

**Issue:** Bulk recipe creation doesn't use transactions - partial failures leave database in inconsistent state.

**Current:**
```csharp
foreach(var recipie in dto)
{
    var createdRecipe = await _recipeService.CreateRecipeAsync(recipie);
}
```

**Recommendation:**
```csharp
await using var transaction = await _context.Database.BeginTransactionAsync();
try
{
    foreach(var recipe in dto)
    {
        await _recipeService.CreateRecipeAsync(recipe);
    }
    await transaction.CommitAsync();
}
catch
{
    await transaction.RollbackAsync();
    throw;
}
```

### 8. Refresh Tokens Never Expire/Cleaned Up
**Location:** `Services/AuthService.cs:71-114`

**Issue:** Revoked refresh tokens accumulate indefinitely in database.

**Recommendation:** Add background job to clean up expired/revoked tokens:
```csharp
// Delete tokens older than expiry date + 30 days
var cutoffDate = DateTime.UtcNow.AddDays(-30);
await _context.Token
    .Where(t => t.ExpiryDate < cutoffDate || t.RevokedAt < cutoffDate)
    .ExecuteDeleteAsync();
```

### 9. Configuration Parsing Without Error Handling
**Location:** `Services/AuthService.cs:43, 48, 89, 94`, `Program.cs`

**Issue:** `int.Parse()` will throw if configuration is missing or invalid.

**Current:**
```csharp
int.Parse(_configuration["JwtSettings:TokenExpirationMinutes"])
```

**Recommendation:**
```csharp
if (!int.TryParse(_configuration["JwtSettings:TokenExpirationMinutes"], out var expirationMinutes))
{
    throw new InvalidOperationException("Invalid TokenExpirationMinutes configuration");
}
```

### 10. Missing Input Validation
**Location:** `Controllers/RecipeController.cs:20-32`

**Issue:** No ModelState validation on recipe creation.

**Add:**
```csharp
if (!ModelState.IsValid)
{
    return BadRequest(ModelState);
}
```

---

## 🟡 Medium Priority Issues

### 11. Naming Inconsistencies and Typos

**Issues found:**
- File/folder: `RecipiesAPI` should be `RecipesAPI`
- DTO: `AuthResponceDTO` should be `AuthResponseDTO`
- Variable: `recipie` should be `recipe` (line 39 in RecipeController.cs)
- Namespace: `Models.DTO.Responce` should be `Models.DTO.Response`

**Impact:** Confusing for developers, reduces code professionalism

### 12. Duplicate Code in OAuth Methods
**Location:** `Services/AuthService.cs:156-207, 209-255`

**Issue:** Google and Facebook login have nearly identical token generation logic.

**Recommendation:** Extract to shared method:
```csharp
private async Task<AuthResponceDTO> CreateAuthResponseForUser(User user)
{
    var accessToken = GenerateJwtToken(user);
    var accessTokenExpiry = DateTime.UtcNow.AddMinutes(
        int.Parse(_configuration["JwtSettings:TokenExpirationMinutes"]));

    var refreshToken = GenerateRefreshToken();
    var refreshTokenExpiry = DateTime.UtcNow.AddDays(
        int.Parse(_configuration["JwtSettings:RefreshTokenExpirationDays"]));

    var newRefreshToken = new Token {
        RefreshToken = refreshToken,
        ExpiryDate = refreshTokenExpiry,
        UserId = user.Id
    };

    _context.Token.Add(newRefreshToken);
    await _context.SaveChangesAsync();

    return new AuthResponceDTO {
        AccessToken = accessToken,
        RefreshToken = refreshToken,
        AccessTokenExpiry = accessTokenExpiry,
        UserId = user.Id,
        Email = user.Email
    };
}
```

### 13. Magic Values
**Location:** `Services/ImageService.cs:17, 19, 21, 39`

**Issue:** Magic value `-1` used to determine parameter usage.

**Recommendation:**
```csharp
public async Task<List<Image>> CreateImageAsync(
    List<CreateImageDTO> images,
    int? recipeId = null) // Use nullable instead of -1
{
    if (!recipeId.HasValue)
    {
        var recipe = images.First().RecipeId;
        // ...
    }
}
```

### 14. Missing Email Uniqueness Constraint in Database
**Location:** `Models/User.cs`, `Data/AppDbContext.cs`

**Issue:** Email uniqueness only enforced in application code, not database level.

**Recommendation:** Add to `AppDbContext.OnModelCreating()`:
```csharp
modelBuilder.Entity<User>()
    .HasIndex(u => u.Email)
    .IsUnique();
```

### 15. Inconsistent Error Messages
**Location:** `Controllers/AuthController.cs:77, 105`

**Issue:** Google and Facebook sign-in return "Invalid email or password" which doesn't make sense for OAuth.

**Should be:**
```csharp
return Unauthorized(new { message = "Authentication failed." });
```

### 16. No CORS Configuration
**Location:** `Program.cs`

**Issue:** No CORS policy defined - will cause issues for frontend applications.

**Recommendation:**
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", builder =>
    {
        builder.WithOrigins(Configuration["AllowedOrigins"])
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

// In middleware pipeline:
app.UseCors("AllowFrontend");
```

### 17. Missing API Versioning
**Location:** All controllers

**Issue:** No versioning strategy makes future API changes breaking.

**Recommendation:**
```csharp
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

// Controllers:
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
```

### 18. No Health Check Endpoints
**Location:** `Program.cs`

**Recommendation:**
```csharp
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection"));

app.MapHealthChecks("/health");
```

---

## 🟢 Low Priority Issues

### 19. Missing XML Documentation
**Location:** Throughout codebase

**Issue:** Many public methods lack XML documentation comments.

**Recommendation:**
```csharp
/// <summary>
/// Creates a new recipe with associated ingredients, categories, and images.
/// </summary>
/// <param name="dto">Recipe creation data</param>
/// <returns>The created recipe entity</returns>
/// <exception cref="Exception">Thrown when author doesn't exist</exception>
public async Task<Recipe> CreateRecipeAsync(CreateRecipeDTO dto)
```

### 20. No Custom Exception Types
**Location:** Throughout services

**Issue:** Using generic `Exception` and `InvalidOperationException`.

**Recommendation:** Create domain-specific exceptions:
```csharp
public class RecipeNotFoundException : Exception
public class UserAlreadyExistsException : Exception
public class InvalidRefreshTokenException : Exception
```

### 21. Potential N+1 Query Issues
**Location:** `Services/RecipeService.cs:82-90`

**Issue:** `GetRecipesByCategoryId` doesn't eager load related entities like other methods do.

**Current:**
```csharp
var recipes = await _context.Recipes
    .Where(r => r.RecipeCategories.Any(rc => rc.CategoryId == categoryId))
    .ToListAsync();
```

**Should include:**
```csharp
.Include(r => r.Author)
.Include(r => r.RecipeCategories).ThenInclude(rc => rc.Category)
.Include(r => r.RecipeIngredients)
.Include(r => r.Images)
```

### 22. Password Validation Only in Model
**Location:** `Models/User.cs:23-25`

**Issue:** `[MinLength(8)]` on password won't validate hashed passwords. Should be in DTO.

**Recommendation:** Move validation to `CreateUserDTO` and add complexity requirements:
```csharp
[Required]
[MinLength(8)]
[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$",
    ErrorMessage = "Password must contain uppercase, lowercase, and number")]
public string Password { get; set; }
```

### 23. Static File Serving Not Configured
**Location:** `Controllers/ImageController.cs:57`

**Issue:** Returns URL to uploaded images but no static file middleware configured.

**Recommendation:** Add to `Program.cs`:
```csharp
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "uploads")),
    RequestPath = "/uploads"
});
```

### 24. No Rate Limiting
**Location:** Authentication endpoints

**Recommendation:**
```csharp
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("auth", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 5;
    });
});

// On auth endpoints:
[EnableRateLimiting("auth")]
```

### 25. Missing UpdatedAt Tracking
**Location:** `Models/Recipe.cs:32`

**Issue:** `UpdatedAt` field defined but never set.

**Recommendation:** Override SaveChanges in AppDbContext:
```csharp
public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
{
    var entries = ChangeTracker.Entries()
        .Where(e => e.Entity is Recipe && e.State == EntityState.Modified);

    foreach (var entry in entries)
    {
        ((Recipe)entry.Entity).UpdatedAt = DateTime.UtcNow;
    }

    return await base.SaveChangesAsync(cancellationToken);
}
```

---

## Testing Recommendations

### Missing Test Coverage
No unit tests or integration tests found in the repository.

**Recommendations:**
1. **Unit Tests** - Test services in isolation with mocked dependencies
2. **Integration Tests** - Test API endpoints with test database
3. **Security Tests** - Test authentication/authorization flows
4. **Performance Tests** - Load testing for bulk operations

**Suggested test structure:**
```
RecipesAPI.Tests/
├── Unit/
│   ├── Services/
│   │   ├── AuthServiceTests.cs
│   │   ├── RecipeServiceTests.cs
│   │   └── UserServiceTests.cs
│   └── Controllers/
│       ├── AuthControllerTests.cs
│       └── RecipeControllerTests.cs
└── Integration/
    ├── AuthenticationFlowTests.cs
    └── RecipeManagementTests.cs
```

---

## Performance Recommendations

### 1. Add Database Indexes
**Location:** `Data/AppDbContext.cs`

```csharp
// Add to OnModelCreating
modelBuilder.Entity<Recipe>()
    .HasIndex(r => r.AuthorId);

modelBuilder.Entity<Recipe>()
    .HasIndex(r => r.CreatedAt);

modelBuilder.Entity<RecipeCategory>()
    .HasIndex(rc => rc.CategoryId);

modelBuilder.Entity<Token>()
    .HasIndex(t => t.RefreshToken);
```

### 2. Implement Caching
**Recommendation:** Cache frequently accessed, rarely changed data (categories, units)

```csharp
builder.Services.AddMemoryCache();

// In CategoryService:
public async Task<List<Category>> GetAllCategoriesAsync()
{
    return await _cache.GetOrCreateAsync("all_categories", async entry =>
    {
        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
        return await _context.Categories.ToListAsync();
    });
}
```

### 3. Use Projection for List Queries
Instead of loading full entities with all navigation properties, project to DTOs:

```csharp
var recipes = await _context.Recipes
    .Select(r => new RecipeResponse
    {
        Id = r.Id,
        Name = r.Name,
        // ... only needed fields
    })
    .ToListAsync();
```

---

## Security Recommendations

### 1. Environment Variables for Secrets
**Current:** Configuration in appsettings.json (gitignored but risky)

**Recommendation:** Use environment variables or Azure Key Vault for production:
```csharp
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddAzureKeyVault(...); // For production
```

### 2. Add Request Validation Middleware
Protect against common attacks:
```csharp
builder.Services.AddAntiforgery();
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10MB
});
```

### 3. Implement Security Headers
```csharp
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Referrer-Policy", "no-referrer");
    await next();
});
```

---

## Code Quality Improvements

### 1. Use Result Pattern Instead of Null Returns
**Location:** `Services/AuthService.cs`

**Current:**
```csharp
if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
{
    return null;
}
```

**Better:**
```csharp
public class AuthResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public AuthResponceDTO? Data { get; set; }
}
```

### 2. Use Constants for Configuration Keys
```csharp
public static class ConfigurationKeys
{
    public const string JwtSecret = "JwtSettings:Secret";
    public const string TokenExpiration = "JwtSettings:TokenExpirationMinutes";
    // ...
}

// Usage:
_configuration[ConfigurationKeys.JwtSecret]
```

### 3. Separate DTOs from Models Namespace
**Current:** DTOs are nested under `Models.DTO`

**Better:** Keep DTOs separate
```
DTOs/
├── Requests/
└── Responses/
```

---

## Docker & Deployment Improvements

### 1. Add Health Checks to docker-compose
```yaml
healthcheck:
  test: ["CMD", "curl", "-f", "http://localhost:5000/health"]
  interval: 30s
  timeout: 3s
  retries: 3
```

### 2. Use Multi-Stage Build Optimization
Your Dockerfile is already using multi-stage builds ✓

### 3. Add Environment-Specific Configuration
Support for Development, Staging, Production environments

---

## Summary Statistics

| Category | Critical | High | Medium | Low | Total |
|----------|----------|------|--------|-----|-------|
| Security | 4 | 0 | 2 | 3 | 9 |
| Code Quality | 0 | 3 | 5 | 5 | 13 |
| Performance | 0 | 1 | 0 | 2 | 3 |
| Best Practices | 0 | 3 | 6 | 5 | 14 |
| **Total** | **4** | **7** | **13** | **15** | **39** |

---

## Recommended Action Plan

### Phase 1: Immediate (This Week)
1. ✅ Fix Facebook authentication vulnerability
2. ✅ Add `[Authorize]` to recipe creation endpoints
3. ✅ Add file size limits to image upload
4. ✅ Implement proper logging with ILogger
5. ✅ Add ModelState validation to all POST endpoints

### Phase 2: Short Term (Next 2 Weeks)
1. Add pagination to list endpoints
2. Implement error handling middleware
3. Add configuration validation on startup
4. Fix naming inconsistencies (typos)
5. Add database indexes
6. Implement CORS policy

### Phase 3: Medium Term (Next Month)
1. Add comprehensive unit tests
2. Implement API versioning
3. Add rate limiting
4. Create custom exception types
5. Add health check endpoints
6. Implement caching strategy

### Phase 4: Long Term (Next Quarter)
1. Add integration tests
2. Implement security headers middleware
3. Add performance monitoring
4. Create admin endpoints for token cleanup
5. Implement audit logging
6. Add API documentation (separate from Swagger)

---

## Positive Aspects

The codebase demonstrates several strengths:

✅ **Good Architecture** - Clean separation of concerns with Controllers → Services → Data layers
✅ **Modern .NET** - Uses .NET 9.0 with latest features
✅ **Dependency Injection** - Proper use of DI container
✅ **Entity Framework** - Good use of EF Core with migrations
✅ **JWT Implementation** - Solid JWT + refresh token pattern
✅ **Docker Support** - Containerization with docker-compose
✅ **Password Security** - BCrypt for password hashing
✅ **Transaction Support** - Uses transactions for complex operations
✅ **DTOs** - Proper separation between entities and API contracts
✅ **OAuth Integration** - Google OAuth implemented correctly

---

## Conclusion

The RecipesAPI is a solid foundation with good architectural choices. The critical security issues (especially Facebook authentication) should be addressed immediately. Most other issues are related to production-readiness, code quality, and best practices that can be addressed incrementally.

The codebase would benefit from:
1. Comprehensive testing suite
2. Better error handling and logging
3. Security hardening
4. Performance optimizations
5. Production-ready configuration management

With these improvements, the API will be production-ready and maintainable long-term.

---

**End of Code Review**
