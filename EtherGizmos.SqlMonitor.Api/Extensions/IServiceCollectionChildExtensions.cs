namespace EtherGizmos.SqlMonitor.Api.Extensions;

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> for adding child containers.
/// </summary>
public static class IServiceCollectionChildExtensions
{
    /// <summary>
    /// The previous child container id.
    /// </summary>
    private static int _childContainerId = -1;

    /// <summary>
    /// Gets the next child container id.
    /// </summary>
    /// <returns>The next child container id.</returns>
    private static int GetChildContainerId()
    {
        var childContainerId = Interlocked.Increment(ref _childContainerId);
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
        //Produce the next child container id
        var childContainerId = GetChildContainerId();

        //Construct a builder for the child container
        var childServices = new ServiceCollection();
        var builder = new ChildContainerBuilder(false, @this, childServices, childContainerId);

        //Add a singleton factory producing child service containers
        @this.AddSingleton(parentProvider =>
        {
            configureChild(childServices, parentProvider);
            return builder.CreateFactory(parentProvider);
        });

        //Create a scoped service container
        @this.AddSingleton<ChildServiceProviderSingletonSource>(parentProvider =>
        {
            var factory = parentProvider.GetRequiredService<ChildServiceProviderFactory>();
            var childProvider = factory.GetSingletonServiceProvider(parentProvider);

            var source = new ChildServiceProviderSingletonSource();
            source.SetProvider(childProvider);

            return source;
        });

        @this.AddScoped<ChildServiceProviderScopedSource>(parentProvider =>
        {
            var factory = parentProvider.GetRequiredService<ChildServiceProviderFactory>();
            var childProvider = factory.GetScopedServiceProvider(parentProvider);

            var source = new ChildServiceProviderScopedSource();
            source.SetProvider(childProvider);

            return source;
        });

        return builder;
    }

    ///// <summary>
    ///// Adds a child container to the current service collection. Child containers exist on their own, with their own
    ///// services. Services from the parent container can be imported into the child container, and services added to the
    ///// child container can be forwarded back to the parent container.
    ///// </summary>
    ///// <param name="this">Itself.</param>
    ///// <param name="configureChild">The child configuration.</param>
    ///// <returns>Itself.</returns>
    //public static IScopedChildContainerBuilder AddScopedChildContainer(this IServiceCollection @this, Action<IServiceCollection, IServiceProvider> configureChild)
    //{
    //    //Produce the next child container id
    //    var childContainerId = GetChildContainerId();

    //    //Construct a builder for the child container
    //    var childServices = new ServiceCollection();
    //    var builder = new ChildContainerBuilder(true, @this, childServices, childContainerId);

    //    //Add a singleton factory producing child service containers
    //    @this.AddScoped(parentProvider =>
    //    {
    //        configureChild(childServices, parentProvider);
    //        return builder.CreateFactory(parentProvider);
    //    });

    //    //Create a scoped service container
    //    @this.AddScoped(parentProvider =>
    //    {
    //        var factory = parentProvider.GetRequiredService<ChildServiceProviderFactory>();
    //        var childProvider = factory.GetServiceProvider(parentProvider);

    //        var source = new ChildServiceProviderSource();
    //        source.SetProvider(childProvider);

    //        return source;
    //    });

    //    return builder;
    //}

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
        private readonly bool _isScoped;

        private readonly IServiceCollection _parentServices;
        private readonly IServiceCollection _childServices;
        private readonly int _childContainerId;
        private readonly List<(Type ServiceType, ServiceLifetime Lifetime)> _childImports = new List<(Type ServiceType, ServiceLifetime Lifetime)>();

        public ChildContainerBuilder(
            bool isScoped,
            IServiceCollection parentServices,
            IServiceCollection childServices,
            int childContainerId)
        {
            _isScoped = isScoped;
            _parentServices = parentServices;
            _childServices = childServices;
            _childContainerId = childContainerId;
        }

        /// <summary>
        /// Creates a factory for producing child service providers.
        /// </summary>
        /// <param name="parentProvider">The parent service provider.</param>
        /// <returns>The child service provider factory.</returns>
        public ChildServiceProviderFactory CreateFactory(IServiceProvider parentProvider)
        {
            _childServices.AddSingleton<ParentServiceProviderSingletonSource>();
            _childServices.AddScoped<ParentServiceProviderScopedSource>();

            foreach (var import in _childImports)
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
                _childServices.Add(descriptor);
            }

            return new ChildServiceProviderFactory(_childServices);
        }

        IChildContainerBuilder IChildContainerBuilder.ForwardScoped<TService>()
        {
            _parentServices.AddScoped<TService>(services =>
            {
                var allSources = services.GetRequiredService<IEnumerable<ChildServiceProviderScopedSource>>();
                var thisSource = allSources.ElementAt(_childContainerId);

                var thisServiceProvider = thisSource.ChildProvider;
                var service = thisServiceProvider.GetRequiredService<TService>();

                return service;
            });

            return this;
        }

        IChildContainerBuilder IChildContainerBuilder.ForwardSingleton<TService>()
        {
            _parentServices.AddSingleton<TService>(services =>
            {
                var allSources = services.GetRequiredService<IEnumerable<ChildServiceProviderSingletonSource>>();
                var thisSource = allSources.ElementAt(_childContainerId);

                var thisServiceProvider = thisSource.ChildProvider;
                var service = thisServiceProvider.GetRequiredService<TService>();

                return service;
            });

            return this;
        }

        IChildContainerBuilder IChildContainerBuilder.ForwardTransient<TService>()
        {
            _parentServices.AddTransient<TService>(services =>
            {
                var allSources = services.GetRequiredService<IEnumerable<ChildServiceProviderSingletonSource>>();
                var thisSource = allSources.ElementAt(_childContainerId);

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
        private readonly IServiceCollection _childServices;

        private IServiceProvider? _childProvider;

        public ChildServiceProviderFactory(
            IServiceCollection childServices)
        {
            _childServices = childServices;
        }

        /// <summary>
        /// Produces a scoped service provider.
        /// </summary>
        /// <returns></returns>
        public IServiceProvider GetScopedServiceProvider(IServiceProvider parentProvider)
        {
            _childProvider ??= _childServices.BuildServiceProvider();

            var scope = _childProvider!
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
        public IServiceProvider GetSingletonServiceProvider(IServiceProvider parentProvider)
        {
            _childProvider ??= _childServices.BuildServiceProvider();

            var scope = _childProvider;

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
        private IServiceProvider? _childProvider;

        public IServiceProvider ChildProvider => _childProvider
            ?? throw new InvalidOperationException("No child provider was specified");

        public void SetProvider(IServiceProvider childProvider)
        {
            _childProvider = childProvider;
        }
    }

    private class ChildServiceProviderScopedSource
    {
        private IServiceProvider? _childProvider;

        public IServiceProvider ChildProvider => _childProvider
            ?? throw new InvalidOperationException("No child provider was specified");

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
