using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ORMNZ.Blazor.Authorization
{

    /// <summary>
    /// The base AuthorizationService class.
    /// </summary>
    public abstract class AuthorizationServiceBase: IAuthorizationService
    {
        /// <summary>
        /// The policy that this service will handle.
        /// </summary>
        public string Policy { get; private set; }

        /// <summary>
        /// Configure the IAuthorizationService implementation.
        /// </summary>
        /// <param name="options"></param>
        public void Configure(AuthorizaationServiceOptions options)
        {
            Policy = options.Policy;
        }

        /// <summary>
        /// Method invoked when authorization is required
        /// for the instance's policy.
        /// </summary>
        /// <param name="roles">The roles that are specified.</param>
        /// <returns>A bool indicating authorization success.</returns>
        public async Task<bool> GetAuthorizationResult(object sender, AuthorizeAttribute authorizeAttribute)
        {
            AuthorizationContext context = new AuthorizationContext
            {
                Policy = Policy,
                Roles = authorizeAttribute.GetRolesArray(),
                Caller = sender.GetType()
            };
            return await AuthorizeAsync(context);
        }

        /// <summary>
        /// Handle authorization.
        /// </summary>
        /// <remarks>This method must be overridden by the derived class.</remarks>
        /// <param name="context">The context in which authorization is taking place.</param>
        /// <returns>A bool indicating authorization success.</returns>
        public abstract Task<bool> AuthorizeAsync(AuthorizationContext context);
    }

}
