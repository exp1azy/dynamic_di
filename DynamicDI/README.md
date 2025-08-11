# Dynamic DI Registration Library

## Overview
This library provides **automatic service registration** in the **ASP.NET Core Dependency Injection container** by scanning assemblies and detecting classes marked with a `RegisterService` and `RegisterDbContext` attributes. 
It simplifies and streamlines the process of service registration, reducing manual configurations and improving maintainability.

## Features
✔ **Automatic Registration** – Detects and registers services dynamically based on attributes.

✔ **Flexible Interface Binding** – Supports registering either the first implemented interface or all interfaces.

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
### 1. Mark Services with the Attribute
Apply the `RegisterService` or `RegisterDbContext` attributes to classes:
```csharp
[RegisterService] // transient by default
public class TestService(ITestRepository repository) : ITestService, ITestable
{
    private readonly ITestRepository _repository = repository;

    public string GetHelloMessage() => "Hello World!";
    public List<string> GetMessages() => _repository.GetMessages();
    public bool IsThisATest() => true;
}

[RegisterService(ServiceLifeCycle.Singleton)]
public class TestRepository : ITestRepository
{
    public List<string> GetMessages()
    {
        return [ "Hello", "World", "!" ];
    }
}

[RegisterDbContext]
public class DataContext : DbContext
{
}
```
### 2. Register Services
Modify the `Program.cs` file to call the `RegisterServices()` and `RegisterDbContexts()` extension methods:
```csharp
using DynamicDI;

var builder = WebApplication.CreateBuilder(args);
builder.Services.RegisterServices(); // services registration
builder.Services.RegisterDbContexts(); // db contexts registration
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