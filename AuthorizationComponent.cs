using Microsoft.AspNetCore.Blazor.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ORMNZ.Blazor.Authorization
{

    /// <summary>
    /// The BlazorComponent derived class adding authorization features
    /// </summary>
    public abstract class AuthorizationComponent: BlazorComponent
    {

        /// <summary>
        /// The IAuthorizationService to be used by the component.
        /// </summary>
        /// <remarks>
        /// You will need to inject this dependency in your Razor component file.
        /// </remarks>
        [Inject]
        public IAuthorizationManager AuthorizationServiceManager { get; set; }

        /// <summary>
        /// Seal the OnInit method.
        /// This ensures the base OnInit is never called.
        /// </summary>
        protected sealed override void OnInit()
        {
            OnInitAsync().Wait();
        }

        /// <summary>
        /// Use the authorization service to verify that access to the AuthorizationComponent
        /// instance is permitted.
        /// </summary>
        protected sealed override async Task OnInitAsync()
        {
            // Get all authorize attributes on this instance.
            AuthorizeAttribute[] authorizeAttributes = GetType().GetCustomAttributes(typeof(AuthorizeAttribute), false) as AuthorizeAttribute[];

            // If there are no authorization requirements continue component initialization.
            if (authorizeAttributes.Count() == 0)
            {
                await base.OnInitAsync();
                await OnAuthorizationSuccess();
            }

            // Iterate over AuthorizeAttributes on this instance.
            foreach (AuthorizeAttribute attribute in authorizeAttributes)
            {
                // Get the authorization service that matches the policy name.
                // Only one policy
                IAuthorizationService authorizationService = AuthorizationServiceManager.GetAuthorizationService(attribute.Policy);
                if (authorizationService != null)
                {
                    bool authorizeSuccess = await authorizationService.GetAuthorizationResult(this, attribute);
                    if (!authorizeSuccess)
                    {
                        await base.OnInitAsync();
                        await OnAuthorizationSuccess();
                        break;
                    }
                }
                // This hits if there is no service configured to handle the policy.
                else
                {
                    throw new AuthorizationException($"No IAuthorizationService found for policy: {attribute.Policy}");
                }
            }
            
        }

        /// <summary>
        /// Proceed after authorization success.
        /// </summary>
        /// <remarks>
        /// Derived classes can implement this method in place of the BlazorComponent
        /// OnInit and OnInitAsync methods, they are sealed by this class to prevent the programmer
        /// inadvertedly breaking authorization.
        /// </remarks>
        public virtual async Task OnAuthorizationSuccess() { }

    }

    public class AuthorizationException: Exception
    {
        public AuthorizationException(string message, Exception innerException = null)
            : base(message, innerException) { }
    }

}
