using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ORMNZ.Blazor.Authorization
{
    /// <summary>
    /// The authorization service manager.
    /// </summary>
    public interface IAuthorizationManager
    {

        /// <summary>
        /// Get an authorization service by name.
        /// </summary>
        /// <param name="policyName">The name of the policy that will apply.</param>
        /// <returns>The matching IAuthorizationService instance.</returns>
        IAuthorizationService GetAuthorizationService(string policyName);

        /// <summary>
        /// Get an authorization service by type.
        /// </summary>
        /// <typeparam name="T">The type that will be used.</typeparam>
        /// <returns>The matching IAuthorizationService instance.</returns>
        IAuthorizationService GetAuthorizationService<T>() where T : IAuthorizationService;

        /// <summary>
        /// Get all services of a given type.
        /// </summary>
        /// <typeparam name="T">The type of service.</typeparam>
        /// <returns></returns>
        IEnumerable<IAuthorizationService> GetAuthorizationServices<T>() where T : IAuthorizationService;

        /// <summary>
        /// Get all registered authorization services.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IAuthorizationService> GetAuthorizationServices();

        /// <summary>
        /// Register anew service.
        /// </summary>
        /// <param name="authorizationService"></param>
        void AddAuthorizationService(IAuthorizationService authorizationService);

        /// <summary>
        /// Add a collection of authorization services.
        /// </summary>
        /// <param name="authorizationServices">The services to add.</param>
        void AddAuthorizationServices(params IAuthorizationService[] authorizationServices);

        /// <summary>
        /// Authorize this attribute.
        /// </summary>
        /// <param name="sender">The class invoking this method.</param>
        /// <param name="attribute">The attribute being evaluated.</param>
        /// <returns>A bool indicating authorization success</returns>
        Task<bool> AuthorizeAsync(object sender, params AuthorizeAttribute[] attribute);

    }
}
