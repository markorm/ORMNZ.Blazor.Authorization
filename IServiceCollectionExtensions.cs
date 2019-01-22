using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ORMNZ.Blazor.Authorization
{
    public static class IServiceCollectionExtensions
    {

        /// <summary>
        /// The AuthorizationServiceManager instance.
        /// </summary>
        private static IAuthorizationManager _authorizationManager = null;

        /// <summary>
        /// Configure the authorization manager.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <param name="authorizationManager"></param>
        /// <returns></returns>
        public static IServiceCollection UseAuthorizationManager(this IServiceCollection services, IAuthorizationManager authorizationManager = null)
        {
            // This will cause problems if an authorization manager is already configured.
            // To avoid this UseAuthorizationManager should be called before adding authorization services.
            if (_authorizationManager != null)
            {
                throw new AuthorizationException(@"
                    An authorization manager is already registered.
                    Ensure you call UseAuthorizationManager only once and that it is called before calling AddAuthorization");
            }
            _authorizationManager = authorizationManager;
            return services;
        }

        /// <summary>
        /// Configure the authorizaion manager.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection UseAuthorizationManager<T>(this IServiceCollection services)
            where T: class, IAuthorizationManager
        {
            IAuthorizationManager authorizationManager = Activator.CreateInstance<T>();
            return UseAuthorizationManager(services, authorizationManager);
        }

        /// <summary>
        /// Add an authorization service instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <param name="authorizationService"></param>
        /// <returns></returns>
        public static IServiceCollection AddAuthorization<T>(
            this IServiceCollection services,
            AuthorizationServiceOptionsBuilder builder)
            where T: class, IAuthorizationService
        {

            // Add the authorization manager instance.
            if (_authorizationManager == null)
            {
                _authorizationManager = new AuthorizationManager();
                services.AddSingleton<IAuthorizationManager>(_authorizationManager);
            }

            // Register the service by interface.
            services.AddSingleton<IAuthorizationService, T>();

            // Get the options from the builder function.
            AuthorizaationServiceOptions options = new AuthorizaationServiceOptions();
            builder.Invoke(options);

            // Create an instance of the service and configure.
            IAuthorizationService instance = Activator.CreateInstance<T>();
            instance.Configure(options);

            // Add the instance to the AuthorizationServiceManager.
            _authorizationManager.AddAuthorizationService(instance);

            // Register the configured instance.
            var serviceDescriptor = services.LastOrDefault();
            services.Add(new ServiceDescriptor(serviceDescriptor.ImplementationType, instance));

            // Return the IServiceCollection for chainability.
            return services;
        }

    }
}
