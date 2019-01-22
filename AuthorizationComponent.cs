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
        public IAuthorizationManager AuthorizationManager { get; set; }

        /// <summary>
        /// Seal the OnInit method.
        /// This ensures the base OnInit is never called.
        /// </summary>
        protected override void OnInit()
        {
            OnInitAsync().Wait();
        }

        /// <summary>
        /// Use the authorization service to verify that access to the AuthorizationComponent
        /// instance is permitted.
        /// </summary>
        protected override async Task OnInitAsync()
        {
            // Get all authorize attributes on this instance.
            // We use the BaseType here as the instance in which this method is executing is actually the derived razor view.
            AuthorizeAttribute[] authorizeAttributes = GetType().BaseType.GetCustomAttributes(typeof(AuthorizeAttribute), false) as AuthorizeAttribute[];
            if (await AuthorizationManager.AuthorizeAsync(authorizeAttributes))
            {
                await base.OnInitAsync();
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

}
