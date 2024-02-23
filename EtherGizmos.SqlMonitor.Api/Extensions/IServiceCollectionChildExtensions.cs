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
    public static ISingletonChildContainerBuilder AddChildContainer(this IServiceCollection @this, Action<IServiceCollection, IServiceProvider> configureChild)
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
        @this.AddSingleton(parentProvider =>
        {
            var factory = parentProvider.GetRequiredService<ChildServiceProviderFactory>();
            var childProvider = factory.GetServiceProvider(parentProvider);

            var source = new ChildServiceProviderSource();
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
    public interface ISingletonChildContainerBuilder
    {
        /// <summary>
        /// Forwards a singleton service from the child container back to the parent.
        /// </summary>
        /// <typeparam name="TService">The type of service being forwarded.</typeparam>
        /// <returns>The builder.</returns>
        ISingletonChildContainerBuilder ForwardSingleton<TService>() where TService : class;

        /// <summary>
        /// Forwards a transient service from the child container back to the parent.
        /// </summary>
        /// <typeparam name="TService">The type of service being forwarded.</typeparam>
        /// <returns>The builder.</returns>
        ISingletonChildContainerBuilder ForwardTransient<TService>() where TService : class;

        /// <summary>
        /// Imports a singleton service from the parent container into the child.
        /// </summary>
        /// <typeparam name="TService">The type of service being imported.</typeparam>
        /// <returns>The builder.</returns>
        ISingletonChildContainerBuilder ImportSingleton<TService>() where TService : class;

        /// <summary>
        /// Imports a transient service from the parent container into the child.
        /// </summary>
        /// <typeparam name="TService">The type of service being imported.</typeparam>
        /// <returns>The builder.</returns>
        ISingletonChildContainerBuilder ImportTransient<TService>() where TService : class;
    }

    /// <summary>
    /// Provides methods to pass services back and forth between a parent and a child service container. Initializes a new scope for each parent scope.
    /// </summary>
    public interface IScopedChildContainerBuilder
    {
        /// <summary>
        /// Forwards a scoped service from the child container back to the parent.
        /// </summary>
        /// <typeparam name="TService">The type of service being forwarded.</typeparam>
        /// <returns>The builder.</returns>
        IScopedChildContainerBuilder ForwardScoped<TService>() where TService : class;

        /// <summary>
        /// Imports a scoped service from the parent container into the child.
        /// </summary>
        /// <typeparam name="TService">The type of service being imported.</typeparam>
        /// <returns>The builder.</returns>
        IScopedChildContainerBuilder ImportScoped<TService>() where TService : class;

        /// <summary>
        /// Imports a singleton service from the parent container into the child.
        /// </summary>
        /// <typeparam name="TService">The type of service being imported.</typeparam>
        /// <returns>The builder.</returns>
        ISingletonChildContainerBuilder ImportSingleton<TService>() where TService : class;

        /// <summary>
        /// Imports a transient service from the parent container into the child.
        /// </summary>
        /// <typeparam name="TService">The type of service being imported.</typeparam>
        /// <returns>The builder.</returns>
        ISingletonChildContainerBuilder ImportTransient<TService>() where TService : class;
    }

    private class ChildContainerBuilder : ISingletonChildContainerBuilder, IScopedChildContainerBuilder
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
            _childServices.AddScoped<ParentServiceProviderSource>();

            foreach (var import in _childImports)
            {
                var descriptor = ServiceDescriptor.Describe(import.ServiceType, childProvider =>
                    childProvider.GetRequiredService<ParentServiceProviderSource>()
                        .ParentProvider
                        .GetRequiredService(import.ServiceType),
                    import.Lifetime);

                _childServices.Add(descriptor);
            }

            return new ChildServiceProviderFactory(_childServices);
        }

        IScopedChildContainerBuilder IScopedChildContainerBuilder.ForwardScoped<TService>()
        {
            _parentServices.AddScoped<TService>(services =>
            {
                var allSources = services.GetRequiredService<IEnumerable<ChildServiceProviderSource>>();
                var thisSource = allSources.ElementAt(_childContainerId);

                var thisServiceProvider = thisSource.ChildProvider;
                var service = thisServiceProvider.GetRequiredService<TService>();

                return service;
            });

            return this;
        }

        ISingletonChildContainerBuilder ISingletonChildContainerBuilder.ForwardSingleton<TService>()
        {
            _parentServices.AddSingleton<TService>(services =>
            {
                var allSources = services.GetRequiredService<IEnumerable<ChildServiceProviderSource>>();
                var thisSource = allSources.ElementAt(_childContainerId);

                var thisServiceProvider = thisSource.ChildProvider;
                var service = thisServiceProvider.GetRequiredService<TService>();

                return service;
            });

            return this;
        }

        ISingletonChildContainerBuilder ISingletonChildContainerBuilder.ForwardTransient<TService>()
        {
            _parentServices.AddTransient<TService>(services =>
            {
                var allSources = services.GetRequiredService<IEnumerable<ChildServiceProviderSource>>();
                var thisSource = allSources.ElementAt(_childContainerId);

                var thisServiceProvider = thisSource.ChildProvider;
                var service = thisServiceProvider.GetRequiredService<TService>();

                return service;
            });

            return this;
        }

        IScopedChildContainerBuilder IScopedChildContainerBuilder.ImportScoped<TService>()
        {
            _childImports.Add((typeof(TService), ServiceLifetime.Scoped));

            return this;
        }

        ISingletonChildContainerBuilder IScopedChildContainerBuilder.ImportSingleton<TService>()
        {
            _childImports.Add((typeof(TService), ServiceLifetime.Singleton));

            return this;
        }

        ISingletonChildContainerBuilder ISingletonChildContainerBuilder.ImportSingleton<TService>()
        {
            _childImports.Add((typeof(TService), ServiceLifetime.Singleton));

            return this;
        }

        ISingletonChildContainerBuilder IScopedChildContainerBuilder.ImportTransient<TService>()
        {
            _childImports.Add((typeof(TService), ServiceLifetime.Transient));

            return this;
        }

        ISingletonChildContainerBuilder ISingletonChildContainerBuilder.ImportTransient<TService>()
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
        public IServiceProvider GetServiceProvider(IServiceProvider parentProvider)
        {
            _childProvider ??= _childServices.BuildServiceProvider();

            var scope = _childProvider!
                .CreateScope()
                .ServiceProvider;

            var parentProviderSource = scope.GetRequiredService<ParentServiceProviderSource>();
            parentProviderSource.SetProvider(parentProvider);

            return scope;
        }
    }

    private class ParentServiceProviderSource
    {
        private IServiceProvider? _parentProvider;

        public IServiceProvider ParentProvider => _parentProvider
            ?? throw new InvalidOperationException("No parent provider was specified");

        public void SetProvider(IServiceProvider parentProvider)
        {
            _parentProvider = parentProvider;
        }
    }

    private class ChildServiceProviderSource
    {
        private IServiceProvider? _childProvider;

        public IServiceProvider ChildProvider => _childProvider
            ?? throw new InvalidOperationException("No child provider was specified");

        public void SetProvider(IServiceProvider childProvider)
        {
            _childProvider = childProvider;
        }
    }
}
