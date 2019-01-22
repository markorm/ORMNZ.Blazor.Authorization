using System;

namespace ORMNZ.Blazor.Authorization
{
    /// <summary>
    /// Define authorization requirements for a component.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AuthorizeAttribute: Attribute
    {

        /// <summary>
        /// The policy that should apply.
        /// </summary>
        public string Policy { get; set; }

        /// <summary>
        /// The array of roles.
        /// </summary>
        private string[] roles = new string[] { };

        /// <summary>
        /// The roles that are required to render the component.
        /// </summary>
        public string Roles {
            get
            {
                return string.Join(", ", roles);
            }
            set
            {
                roles = value.Replace(" ", "").Split(',');
            }
        }

        /// <summary>
        /// Define a policy and roles
        /// </summary>
        /// <param name="policy">The name of the authorization policy that will apply.</param>
        /// <param name="roles">The roles to be checked by the IAuthorizationService managing this policy.</param>
        public AuthorizeAttribute(string policy, string roles = null)
        {
            Policy = policy;
            if (roles != null) Roles = roles;
        }

        /// <summary>
        /// Get all roles in this instance.
        /// </summary>
        /// <returns>A list of roles.</returns>
        public string[] GetRolesArray()
        {
            return roles;
        }

    }

}
