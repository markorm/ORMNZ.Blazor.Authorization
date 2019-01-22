using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ORMNZ.Blazor.Authorization
{
    /// <summary>
    /// The default Authorization service manager.
    /// </summary>
    public class AuthorizationManager : IAuthorizationManager
    {

        /// <summary>
        /// A list of registered IAuthorizationService instances.
        /// </summary>
        private List<IAuthorizationService> services = new List<IAuthorizationService>();

        /// <summary>
        /// Create a new instance of the AuthorizationServiceManager
        /// </summary>
        /// <param name="authorizationServices"></param>
        public AuthorizationManager(params IAuthorizationService[] authorizationServices)
        {
            AddAuthorizationServices(authorizationServices);
        }

        /// <summary>
        /// Add a single authorization service.
        /// </summary>
        /// <param name="authorizationService">The IAuthorizationService to add.</param>
        public void AddAuthorizationService(IAuthorizationService authorizationService)
        {
            AddAuthorizationServices(authorizationService);
        }

        /// <summary>
        /// Add a collection of authorization services.
        /// </summary>
        /// <param name="authorizationServices">The services to add.</param>
        public void AddAuthorizationServices(params IAuthorizationService[] authorizationServices)
        {
            // Find services that conflict with an existing policy.
            IEnumerable<IAuthorizationService> conflictingServices = authorizationServices.Where(service =>
                services.FindAll(existingService => existingService.Policy == service.Policy) == null);

            // If there are conflicting services throw an exception
            if (conflictingServices.Count() > 0)
            {
                string conflictingPolicyNames = string.Join(", ", conflictingServices.SelectMany(service => service.Policy));
                throw new AuthorizationException($"One or more conflicting policies registered: {conflictingPolicyNames}");
            }

            // Add all unique services (there is not already a policy registered).
            services.AddRange(authorizationServices);
        }

        /// <summary>
        /// Get an authorization service by policy name.
        /// </summary>
        /// <param name="policyName">The name of the policy.</param>
        /// <returns></returns>
        public IAuthorizationService GetAuthorizationService(string policyName)
        {
            return services.Find(service => service.Policy == policyName);
        }
        
        /// <summary>
        /// Get an authorization service by type.
        /// </summary>
        /// <remarks>
        /// If multiple authorizaton services of the same type are registered use GetAuthorizationServices<T>,
        /// this method will only return the first match.
        /// </remarks>
        /// <typeparam name="T">The type of service.</typeparam>
        /// <returns>The first authorization service of that type.</returns>
        public IAuthorizationService GetAuthorizationService<T>() where T : IAuthorizationService
        {
            return services.Find(service => service.GetType().IsAssignableFrom(typeof(T)));
        }

        /// <summary>
        /// Get all authorization services of a given type.
        /// </summary>
        /// <typeparam name="T">The type of service.</typeparam>
        /// <returns>A collection of all services of a given type.</returns>
        public IEnumerable<IAuthorizationService> GetAuthorizationServices<T>() where T: IAuthorizationService
        {
            return services.FindAll(service => services.GetType() == typeof(T));
        }

        /// <summary>
        /// Get all registered authorization services.
        /// </summary>
        /// <returns>A collection of all services.</returns>
        public IEnumerable<IAuthorizationService> GetAuthorizationServices()
        {
            return services;
        }
    }
}
