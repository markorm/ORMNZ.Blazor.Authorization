using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ORMNZ.Blazor.Authorization
{

    /// <summary>
    /// Blazor component authorization service.
    /// </summary>
    public interface IAuthorizationService
    {

        /// <summary>
        /// The policy that this service applies to.
        /// </summary>
        string Policy { get;  }

        /// <summary>
        /// Configure the IAuthorizationService.
        /// </summary>
        /// <param name="options">The service configuration options</param>
        void Configure(AuthorizaationServiceOptions options);

        /// <summary>
        /// The method called to begin authorization
        /// </summary>
        /// <returns></returns>
        Task<bool> AuthorizeAsync(AuthorizationContext context);

        /// <summary>
        /// Method called when authorization is required.
        /// </summary>
        /// <param name="roles">The roles on the annotated component.</param>
        /// <returns></returns>
        Task<bool> GetAuthorizationResult(object sender, AuthorizeAttribute authorizeAttribute);

    }

    /// <summary>
    /// The options used to configure an authorization service.
    /// </summary>
    public class AuthorizaationServiceOptions
    {

        /// <summary>
        /// The policy that the configured instance will apply to.
        /// </summary>
        public string Policy { get; set; }

    }

    /// <summary>
    /// Describes the context in which authorization is taking place.
    /// </summary>
    public class AuthorizationContext
    {
        /// <summary>
        /// The policy this context triggered on.
        /// </summary>
        public string Policy { get; set; }

        /// <summary>
        /// The roles that were required.
        /// </summary>
        public string[] Roles { get; set; } = null;

        /// <summary>
        /// The type of the class that initiated the operation.
        /// </summary>
        public Type Caller { get; set; } = null;

        /// <summary>
        /// The date on which the context was created.
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// A authorization options builder delegate signature.
    /// </summary>
    /// <param name="options"></param>
    public delegate void AuthorizationServiceOptionsBuilder(AuthorizaationServiceOptions options);

    /// <summary>
    /// Create an Authorization exception.
    /// </summary>
    public class AuthorizationException : Exception
    {
        public AuthorizationException(string message, Exception innerException = null)
            : base(message, innerException) { }
    }

}
