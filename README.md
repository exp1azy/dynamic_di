# Dynamic DI Registration Library

## Overview
This library provides **automatic service registration** in the **ASP.NET Core Dependency Injection container** by scanning assemblies and detecting classes marked with a `[Service]` attribute. 
It simplifies and streamlines the process of service registration, reducing manual configurations and improving maintainability.

## Features
✔ **Automatic Registration** – Detects and registers services dynamically based on attributes.

✔ **Flexible Interface Binding** – Allows you to register the specified interfaces, or register all of them.

✔ **Configurable Lifetimes** – Allows specifying service lifetimes (Transient, Scoped, Singleton).

✔ **Assembly Scanning** – Automatically discovers and registers services from all project assemblies.

✔ **Zero Boilerplate Code** – Eliminates the need for manual AddTransient, AddScoped, or AddSingleton calls.

## Installation
Install via NuGet Package Manager:
```bash
Install-Package DynamicDI
```
Or using .NET CLI:
```bash
dotnet add package DynamicDI
```

## Usage
### 1. Mark Services with the `ServiceAttribute`
Apply the `[Service]` attribute to classes:
```csharp
[Service([typeof(ITestService)])]
public class TestService(ITestRepository repository) : ITestService, ITestable
{
    private readonly ITestRepository _repository = repository;

    public async Task<IEnumerable<CriticalSituationImage>> GetCsiAsync(CancellationToken cancellationToken = default)
    {
        return await _repository.GetCsiAsync(cancellationToken);
    }

    public string GetHelloMessage() => "Hello World!";
    public List<string> GetMessages() => _repository.GetMessages();
    public bool IsThisATest() => true;
}

[Service]
public class TestRepository : ITestRepository
{
    private readonly DataContext _dbContext;

    public TestRepository(DataContext dataContext)
    {
        _dbContext = dataContext;
    }

    public async Task<IEnumerable<CriticalSituationImage>> GetCsiAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.CriticalSituationImages.AsNoTracking().ToListAsync(cancellationToken);
    }

    public List<string> GetMessages()
    {
        return [ "Hello", "World", "!" ];
    }
}

[Service([typeof(DataContext)])]
public class DataContext : DbContext
{
}
```
### 2. Register Services
Modify the `Program.cs` file to call the `builder.Services.RegisterServices()` extension method:
```csharp
using DynamicDI;

var builder = WebApplication.CreateBuilder(args);
builder.Services.RegisterServices(); // services registration
```
You can also specify the assemblies from where you want to register services:
```csharp
using DynamicDI;

Assembly[] assemblies = GetAssemblies(); // your assemblies
builder.Services.RegisterServices(assemblies);
```
### 3. Inject Services Anywhere
Once registered, services can be injected as usual:
```csharp
public class MyController : ControllerBase
{
    private readonly IUserService _userService;

    public MyController(IUserService userService)
    {
        _userService = userService;
    }
}
```

## License
This library is open-source and licensed under the MIT License. Contributions and feedback are welcome! 🚀