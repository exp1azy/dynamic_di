# DynamicDI

Attribute-based automatic service registration for `Microsoft.Extensions.DependencyInjection`.
## Overview

DynamicDI scans assemblies for classes decorated with `[Service]` and registers them in an `IServiceCollection`. Service lifetime and service types can be configured through the attribute. If service types are not specified, the library uses the interfaces implemented by the class. If the class implements no interfaces, the concrete class type is registered.
## Installation

```bash
dotnet add package DynamicDI
```
The library targets `Microsoft.Extensions.DependencyInjection` and uses `Microsoft.Extensions.DependencyModel` for project assembly discovery.
## Usage
### Basic registration

Mark a class with `[Service]`:
```csharp
using DynamicDI;

[Service]
public class EmailSender : IEmailSender
{
}
```

Register services during application startup:
```csharp
using DynamicDI;

services.RegisterServices();
```

The parameterless overload scans project assemblies discovered through `DependencyContext.Default`. To scan specific assemblies, use the overload that accepts assemblies:
```csharp
services.RegisterServices(typeof(Program).Assembly);
```
### Specifying lifetime

The default lifetime is `Transient`. Use the `ServiceLifetime` constructor to override it:
```csharp
[Service(ServiceLifetime.Singleton)]
public class CacheService : ICacheService
{
}
```
Supported lifetimes: `Transient`, `Scoped`, `Singleton`.
### Specifying service types

By default, all interfaces implemented by the class are used as service types. To register the class for specific service types, pass them to the attribute:
```csharp
[Service(typeof(IEmailSender), typeof(INotificationSender))]
public class EmailSender : IEmailSender, INotificationSender
{
}
```

Lifetime and service types can be combined:
```csharp
[Service(ServiceLifetime.Scoped, typeof(IOrderService))]
public class OrderService : IOrderService
{
}
```
## Important
`ImplementedInterfaces` includes interfaces inherited from base classes. If the decorated class derives from a base class that implements interfaces you do not want to expose as services for the decorated class, you must pass the decorated class type explicitly to the `ServiceAttribute` constructor. Otherwise, the library will register the class for every inherited interface.

This applies, for example, to classes derived from `DbContext`, which implements several infrastructure interfaces (such as `IInfrastructure<IServiceProvider>`, `IDbContextDependencies`, `IDbContextPoolable`, and others) that should not normally be registered in DI.

To register such a class as the concrete type only, specify its own type:
```csharp
[Service(ServiceLifetime.Scoped, typeof(DataContext))]
public class DataContext : DbContext
{
}
```
Without `typeof(DataContext)`, the library would infer all interfaces implemented by `DbContext` and register `DataContext` for each of them.

If you need to register the class for a subset of interfaces, list only those interfaces:
```csharp
[Service(ServiceLifetime.Scoped, typeof(IMyRepository), typeof(IUnitOfWork))]
public class MyRepository : BaseRepository, IMyRepository, IUnitOfWork
{
}
```