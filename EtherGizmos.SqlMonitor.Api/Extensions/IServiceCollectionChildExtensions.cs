using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Collections.Concurrent;

namespace EtherGizmos.SqlMonitor.Api.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> for adding child containers.
/// </summary>
public static class IServiceCollectionChildExtensions
{
    /// <summary>
    /// Gets the next child container id.
    /// </summary>
    /// <returns>The next child container id.</returns>
    private static Guid GetChildContainerId()
    {
        var childContainerId = Guid.NewGuid();
        return childContainerId;
    }

    /// <summary>
    /// Adds a child container to the current service collection. Child containers exist on their own, with their own
    /// services. Services from the parent container can be imported into the child container, and services added to the
    /// child container can be forwarded back to the parent container.
    /// </summary>
    /// <param name="this">Itself.</param>
    /// <param name="configureChild">The child configuration.</param>
    /// <returns>Itself.</returns>
    public static IChildContainerBuilder AddChildContainer(this IServiceCollection @this, Action<IServiceCollection, IServiceProvider> configureChild)
    {
        //Add the factory that can produce child containers
        @this.TryAddSingleton<ChildServiceProviderFactory>();

        //Produce the next child container id
        var childContainerId = GetChildContainerId();

        //Construct a builder for the child container
        var childServices = new ServiceCollection();
        var builder = new ChildContainerBuilder(childContainerId, @this, childServices, configureChild);

        return builder;
    }

    /// <summary>
    /// Provides methods to pass services back and forth between a parent and a child service container. Does not initialize any scopes.
    /// </summary>
    public interface IChildContainerBuilder
    {
        /// <summary>
        /// Forwards a scoped service from the child container back to the parent.
        /// </summary>
        /// <typeparam name="TService">The type of service being forwarded.</typeparam>
        /// <returns>The builder.</returns>
        IChildContainerBuilder ForwardScoped<TService>() where TService : class;

        /// <summary>
        /// Forwards a singleton service from the child container back to the parent.
        /// </summary>
        /// <typeparam name="TService">The type of service being forwarded.</typeparam>
        /// <returns>The builder.</returns>
        IChildContainerBuilder ForwardSingleton<TService>() where TService : class;

        /// <summary>
        /// Forwards a transient service from the child container back to the parent.
        /// </summary>
        /// <typeparam name="TService">The type of service being forwarded.</typeparam>
        /// <returns>The builder.</returns>
        IChildContainerBuilder ForwardTransient<TService>() where TService : class;

        /// <summary>
        /// Imports logging from the parent container into the child.
        /// </summary>
        /// <returns>The builder.</returns>
        IChildContainerBuilder ImportLogging();

        /// <summary>
        /// Imports a scoped service from the parent container into the child.
        /// </summary>
        /// <typeparam name="TService">The type of service being imported.</typeparam>
        /// <returns>The builder.</returns>
        IChildContainerBuilder ImportScoped<TService>() where TService : class;

        /// <summary>
        /// Imports a singleton service from the parent container into the child.
        /// </summary>
        /// <typeparam name="TService">The type of service being imported.</typeparam>
        /// <returns>The builder.</returns>
        IChildContainerBuilder ImportSingleton<TService>() where TService : class;

        /// <summary>
        /// Imports a transient service from the parent container into the child.
        /// </summary>
        /// <typeparam name="TService">The type of service being imported.</typeparam>
        /// <returns>The builder.</returns>
        IChildContainerBuilder ImportTransient<TService>() where TService : class;
    }

    private class ChildContainerBuilder : IChildContainerBuilder
    {
        private readonly Guid _childContainerId;

        private readonly IServiceCollection _parentServices;
        private readonly IServiceCollection _childServices;
        private readonly Action<IServiceCollection, IServiceProvider> _configureChild;
        private readonly List<(Type ServiceType, ServiceLifetime Lifetime)> _childImports = new();

        public Action<IServiceCollection, IServiceProvider> ConfigureChild => _configureChild;
        public List<(Type ServiceType, ServiceLifetime Lifetime)> Imports => _childImports;

        public ChildContainerBuilder(
            Guid childContainerId,
            IServiceCollection parentServices,
            IServiceCollection childServices,
            Action<IServiceCollection, IServiceProvider> configureChild)
        {
            _childContainerId = childContainerId;
            _parentServices = parentServices;
            _childServices = childServices;
            _configureChild = configureChild;
        }

        IChildContainerBuilder IChildContainerBuilder.ForwardScoped<TService>()
        {
            _parentServices.AddScoped<TService>(services =>
            {
                var factory = services.GetRequiredService<ChildServiceProviderFactory>();

                factory.TryAddServiceCollection(_childContainerId, _childServices, _configureChild, _childImports);
                var thisServiceProvider = factory.GetScopedServiceProvider(_childContainerId, services);

                var service = thisServiceProvider.GetRequiredService<TService>();

                return service;
            });

            return this;
        }

        IChildContainerBuilder IChildContainerBuilder.ForwardSingleton<TService>()
        {
            _parentServices.AddSingleton<TService>(services =>
            {
                var factory = services.GetRequiredService<ChildServiceProviderFactory>();

                factory.TryAddServiceCollection(_childContainerId, _childServices, _configureChild, _childImports);
                var thisServiceProvider = factory.GetSingletonServiceProvider(_childContainerId, services);

                var service = thisServiceProvider.GetRequiredService<TService>();

                return service;
            });

            return this;
        }

        IChildContainerBuilder IChildContainerBuilder.ForwardTransient<TService>()
        {
            _parentServices.AddTransient<TService>(services =>
            {
                var factory = services.GetRequiredService<ChildServiceProviderFactory>();

                factory.TryAddServiceCollection(_childContainerId, _childServices, _configureChild, _childImports);
                var thisServiceProvider = factory.GetSingletonServiceProvider(_childContainerId, services);

                var service = thisServiceProvider.GetRequiredService<TService>();

                return service;
            });

            return this;
        }

        IChildContainerBuilder IChildContainerBuilder.ImportLogging()
        {
            _childImports.Add((typeof(ILoggerFactory), ServiceLifetime.Singleton));
            _childServices.AddSingleton(typeof(ILogger<>), typeof(LoggerForward<>));

            return this;
        }

        IChildContainerBuilder IChildContainerBuilder.ImportScoped<TService>()
        {
            _childImports.Add((typeof(TService), ServiceLifetime.Scoped));

            return this;
        }

        IChildContainerBuilder IChildContainerBuilder.ImportSingleton<TService>()
        {
            _childImports.Add((typeof(TService), ServiceLifetime.Singleton));

            return this;
        }

        IChildContainerBuilder IChildContainerBuilder.ImportTransient<TService>()
        {
            _childImports.Add((typeof(TService), ServiceLifetime.Transient));

            return this;
        }
    }

    /// <summary>
    /// Produces child service providers.
    /// </summary>
    private class ChildServiceProviderFactory
    {
        private readonly IServiceProvider _parentRootProvider;
        private readonly ConcurrentDictionary<Guid, IServiceProvider> _childProviders = new();

        public ChildServiceProviderFactory(
            IServiceProvider parentRootProvider)
        {
            _parentRootProvider = parentRootProvider;
        }

        public void TryAddServiceCollection(
            Guid id,
            IServiceCollection childServices,
            Action<IServiceCollection, IServiceProvider> configureChild,
            List<(Type ServiceType, ServiceLifetime Lifetime)> imports)
        {
            //Attempt to build and add the provider
            _childProviders.AddOrUpdate(
                id,
                id =>
                {
                    childServices.AddSingleton<ParentServiceProviderSingletonSource>();
                    childServices.AddScoped<ParentServiceProviderScopedSource>();

                    configureChild(childServices, _parentRootProvider);

                    foreach (var import in imports)
                    {
                        ServiceDescriptor descriptor;
                        if (import.Lifetime == ServiceLifetime.Scoped)
                        {
                            descriptor = ServiceDescriptor.Describe(import.ServiceType, childProvider =>
                                childProvider.GetRequiredService<ParentServiceProviderScopedSource>()
                                    .ParentProvider
                                    .GetRequiredService(import.ServiceType),
                                import.Lifetime);
                        }
                        else
                        {
                            descriptor = ServiceDescriptor.Describe(import.ServiceType, childProvider =>
                                childProvider.GetRequiredService<ParentServiceProviderSingletonSource>()
                                    .ParentProvider
                                    .GetRequiredService(import.ServiceType),
                                import.Lifetime);
                        }
                        childServices.Add(descriptor);
                    }

                    return childServices.BuildServiceProvider();
                },
                (_, old) => old);
        }

        /// <summary>
        /// Produces a scoped service provider.
        /// </summary>
        /// <returns></returns>
        public IServiceProvider GetScopedServiceProvider(
            Guid id,
            IServiceProvider parentProvider)
        {
            var scope = _childProviders[id]
                .CreateScope()
                .ServiceProvider;

            var parentProviderSingletonSource = scope.GetRequiredService<ParentServiceProviderSingletonSource>();
            parentProviderSingletonSource.SetProvider(parentProvider);

            var parentProviderScopedSource = scope.GetRequiredService<ParentServiceProviderScopedSource>();
            parentProviderScopedSource.SetProvider(parentProvider);

            return scope;
        }

        /// <summary>
        /// Produces a scoped service provider.
        /// </summary>
        /// <returns></returns>
        public IServiceProvider GetSingletonServiceProvider(
            Guid id,
            IServiceProvider parentProvider)
        {
            var scope = _childProviders[id];

            var parentProviderSingletonSource = scope.GetRequiredService<ParentServiceProviderSingletonSource>();
            parentProviderSingletonSource.SetProvider(parentProvider);

            return scope;
        }
    }

    private class ParentServiceProviderSingletonSource
    {
        private IServiceProvider? _parentProvider;

        public IServiceProvider ParentProvider => _parentProvider
            ?? throw new InvalidOperationException("No parent provider was specified");

        public void SetProvider(IServiceProvider parentProvider)
        {
            _parentProvider = parentProvider;
        }
    }

    private class ParentServiceProviderScopedSource
    {
        private IServiceProvider? _parentProvider;

        public IServiceProvider ParentProvider => _parentProvider
            ?? throw new InvalidOperationException("No parent provider was specified");

        public void SetProvider(IServiceProvider parentProvider)
        {
            _parentProvider = parentProvider;
        }
    }

    private class LoggerForward<T> : ILogger<T>
    {
        private readonly ILogger _logger;

        public LoggerForward(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<T>();
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return _logger.BeginScope(state);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _logger.IsEnabled(logLevel);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            _logger.Log(logLevel, eventId, state, exception, formatter);
        }
    }
}
