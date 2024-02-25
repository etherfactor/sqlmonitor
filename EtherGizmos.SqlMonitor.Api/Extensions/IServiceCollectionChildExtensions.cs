﻿using Microsoft.Extensions.DependencyInjection.Extensions;

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
        var builder = new ChildContainerBuilder(childContainerId, @this, childServices);

        //Create a scoped service container
        @this.AddSingleton<ChildServiceProviderSingletonSource>(parentProvider =>
        {
            var factory = parentProvider.GetRequiredService<ChildServiceProviderFactory>();

            //Add the child container to the factory
            configureChild(childServices, parentProvider);
            factory.AddServiceProvider(childContainerId, childServices, builder.Imports);

            //Pull the generated singleton container out of the factory
            var childProvider = factory.GetSingletonServiceProvider(childContainerId, parentProvider);

            //Create a reference to the singleton child container and add it to the parent
            var source = new ChildServiceProviderSingletonSource(childContainerId);
            source.SetProvider(childProvider);

            return source;
        });

        @this.AddScoped<ChildServiceProviderScopedSource>(parentProvider =>
        {
            //Generating the singleton source configures the factory
            parentProvider.GetRequiredService<ChildServiceProviderSingletonSource>();

            var factory = parentProvider.GetRequiredService<ChildServiceProviderFactory>();

            //Pull the generated scoped container out of the factory
            var childProvider = factory.GetScopedServiceProvider(childContainerId, parentProvider);

            //Create a reference to the scoped child container and add it to the parent
            var source = new ChildServiceProviderScopedSource(childContainerId);
            source.SetProvider(childProvider);

            return source;
        });

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
        private readonly List<(Type ServiceType, ServiceLifetime Lifetime)> _childImports = new();

        public List<(Type ServiceType, ServiceLifetime Lifetime)> Imports => _childImports;

        public ChildContainerBuilder(
            Guid childContainerId,
            IServiceCollection parentServices,
            IServiceCollection childServices)
        {
            _childContainerId = childContainerId;
            _parentServices = parentServices;
            _childServices = childServices;
        }

        IChildContainerBuilder IChildContainerBuilder.ForwardScoped<TService>()
        {
            _parentServices.AddScoped((Func<IServiceProvider, TService>)(services =>
            {
                var source = services.GetServices<ChildServiceProviderScopedSource>()
                    .Single(e => e.Id == _childContainerId);

                var thisServiceProvider = source.ChildProvider;
                var service = ServiceProviderServiceExtensions.GetRequiredService<TService>(thisServiceProvider);

                return service;
            }));

            return this;
        }

        IChildContainerBuilder IChildContainerBuilder.ForwardSingleton<TService>()
        {
            _parentServices.AddSingleton<TService>(services =>
            {
                var source = services.GetServices<ChildServiceProviderSingletonSource>()
                    .Single(e => e.Id == _childContainerId);

                var thisServiceProvider = source.ChildProvider;
                var service = thisServiceProvider.GetRequiredService<TService>();

                return service;
            });

            return this;
        }

        IChildContainerBuilder IChildContainerBuilder.ForwardTransient<TService>()
        {
            _parentServices.AddTransient<TService>(services =>
            {
                var allSources = services.GetServices<ChildServiceProviderSingletonSource>();
                var thisSource = allSources.Single(e => e.Id == _childContainerId);

                var thisServiceProvider = thisSource.ChildProvider;
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
        private readonly Dictionary<Guid, IServiceProvider> _childProviders = new();

        public void AddServiceProvider(
            Guid id,
            IServiceCollection services,
            List<(Type ServiceType, ServiceLifetime Lifetime)> imports)
        {
            services.AddSingleton<ParentServiceProviderSingletonSource>();
            services.AddScoped<ParentServiceProviderScopedSource>();

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
                services.Add(descriptor);
            }

            _childProviders.Add(id, services.BuildServiceProvider());
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

    private class ChildServiceProviderSingletonSource
    {
        public Guid Id { get; }

        private IServiceProvider? _childProvider;

        public IServiceProvider ChildProvider => _childProvider
            ?? throw new InvalidOperationException("No child provider was specified");

        public ChildServiceProviderSingletonSource(Guid id)
        {
            Id = id;
        }

        public void SetProvider(IServiceProvider childProvider)
        {
            _childProvider = childProvider;
        }
    }

    private class ChildServiceProviderScopedSource
    {
        public Guid Id { get; }

        private IServiceProvider? _childProvider;

        public IServiceProvider ChildProvider => _childProvider
            ?? throw new InvalidOperationException("No child provider was specified");

        public ChildServiceProviderScopedSource(Guid id)
        {
            Id = id;
        }

        public void SetProvider(IServiceProvider childProvider)
        {
            _childProvider = childProvider;
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
