# ORMNZ.Blazor.Authorization

#### This is currently a pre-release WORK IN PROGRESS. No guarantees of reliability are given.

Service and Component classes for Blazor component authorization.

This class is heavily inspired by the ASPNetCore authorization system, though it does not leverage the identity system.
Instead a method on an IAuthorizationService instance matching on "policy" will be invoked, allowing you to use whatever
methods for determining authorization that you like.

## Getting Started

* Add a project or DLL reference to your Blazor application (Nuget package TBA).
* Add `@using ORMNZ.Blazor.Authorization`
* Create your IAuthorizationService instances for each authorization policy you need, or simply derive
from the AuthorizationServiceBase class.
* Create a custom IAuthorizationManager if required (the default will be sufficient for most cases) and
register it as a service:
`services.UseAuthorizationManager<MyAuthorizationManager>();`
* Add your IAuthorizationService instances:
`services.AddAuthorization<MyAuthorizationService>(options => {});`
* Inject the IAuthorizationManager service into your Blazor component.
* Inherit from the AuthorizedComponent class
* Add AuthorizeAttribute Attributes to your component:
`[Authorize(Policy = "Users", Roles = "CanDoTheThing")]`
* Put your post-authorization logic in an OnAuthorizeSuccess method:
`public override async Task OnAuthorizeSuccess() {}`

When an AuthorizeComponent is initialized the IAuthorizationService.AuthorizeAsync method is invoked configured for each policy.
This method returns a boolean to indicate if authorization is successful. The component initialization will not continue until this
result is returned.

***

### Example IAuthorizationService implementation.

An example of basic user authorization service.

```csharp
public class UserAuthorizationService: AuthorizationServiceBase
{

    /// <summary>
    /// The uri helper we will use to navigate on policy fail.
    /// </summary>
    private readonly BrowserUriHelper _browserUriHelper;

    /// <summary>
    /// An example of a user that we are going to evaluate.
    /// </summary>
    public User CurrentUser = new User
    {
        FirstName = "Bob",
        LastName = "Dole",
        Email = "bob.dole@illuminati.biz",
        Active = true,
        Type = UserType.User,
        Roles = new string[] { "GetWeatherForecasts" }
    };

    /// <summary>
    /// Construct an instance of the UserAuthorizationService with dependencies.
    /// </summary>
    public UserAuthorizationService(BrowserUriHelper browserUriHelper)
    {
        _browserUriHelper = browserUriHelper;
    }

    /// <summary>
    /// Authorize access against the current user.
    /// </summary>
    public override async Task<bool> AuthorizeAsync(AuthorizationContext context)
    {
        // If the current user is null redirect to login.
        if (CurrentUser == null)
        {
            _browserUriHelper.NavigateTo("/login");
            return false;
        }

        // If the user is not active they should not authorized,
        // redirect the use to the locked page.
        if (!CurrentUser.Active)
        {
            _browserUriHelper.NavigateTo("/locked");
            return false;
        }

        // If the current user is an admin pass them irrespective of the required roles,
        // otherwise determine if the user has all required roles.
        if (CurrentUser.Type == UserType.Admin || context.Roles.Intersect(CurrentUser.Roles).Count() == context.Roles.Count())
        {
            return true;
        }

        // The default state is failing authorization,
        // redirect the user to the forbidden page.
        _browserUriHelper.NavigateTo("/forbidden");
        return false;
    }
}
```

***

### Example Registering services

```csharp
public void ConfigureServices(IServiceCollection services)
{

    // Add the authorization services
    services
        // EXMAPLE: Specify the IAuthorizationManager instance to use.
        // In this case adding the default AuthorizationManager is redundant,
        // this default will be used if no custom IAuthorizationManager is
        // used here.
        .UseAuthorizationManager<AuthorizationManager>()
        // Add an authorization service to handle the "Users" policy.
        // This class' Authorize method will be invoked any time there is
        // an Authorize attribute with this policy set
        .AddAuthorization<UserAuthorizationService>(options =>
        {
            options.Policy = "Users";
        })
        // Add an authorization service to handle the "Admins" policy.
        .AddAuthorization<AdminAuthorizationService>(options =>
        {
            options.Policy = "Admins";
        });

    // Add your other services...

}
```

***

### Example AuthorizedComponent

An example of the FetchData compnnent using authorization.
The base AuthorizedComponent class receives the injected IAuthorizationManager as AuthorizationManager. 

```csharp
[Authorize(Policy = "Users", Roles = "GetWeatherForecasts")]
public class FetchDataViewModel : AuthorizedComponent
{

	public WeatherForecast[] forecasts;

	[Inject]
	public WeatherForecastService ForecastService { get; set; }
	
	/// <summary>
	/// This method is invoked after authorization.
	/// You should move your OnInit/OnInitAsync logic into
	/// this method to ensure that it only executes after authorization is completed.
	/// </summary>
	public override async Task OnAuthorizationComplete()
	{
		forecasts = await ForecastService.GetForecastAsync(DateTime.Now);
	}

}
```

And the modified FetchData page now using authorization.

```html
@using ORMNZ.Blazor.DevEnv.App.Services
@using ORMNZ.Blazor.Authorization
@page "/fetchdata"
@inject IAuthorizationManager AuthorizationManager
@inject WeatherForecastService ForecastService
@inherits FetchDataViewModel

<h1>Weather forecast</h1>

<p>This component demonstrates fetching data from the server.</p>

@if (forecasts == null)
{
    <p><em>Loading...</em></p>
}
else
{
    <table class="table">
        <thead>
            <tr>
                <th>Date</th>
                <th>Temp. (C)</th>
                <th>Temp. (F)</th>
                <th>Summary</th>
            </tr>
        </thead>
        <tbody>
            @foreach (var forecast in forecasts)
            {
                <tr>
                    <td>@forecast.Date.ToShortDateString()</td>
                    <td>@forecast.TemperatureC</td>
                    <td>@forecast.TemperatureF</td>
                    <td>@forecast.Summary</td>
                </tr>
            }
        </tbody>
    </table>
}
```

***

### Limitations

* The only authorization mechanisms implemented so far are roles and policies. The other features you might be used to from
ASPNetCore such as AuthenticationScheme are not supported (perhaps in the future?).

* This project is deliberately designed not to care about performing authorization evaluations it's self, you should perform
the required work on your IAuthorizationService.AuthorizeAsync methods.

### TODOS

* Implement unit tests for the project to validate the effectiveness and reliability of the current implementation.

* Extend the AuthorizationServiceOptionsBuilder and AuthorizationServiceOptions types to allow for greater configurability.

* Use this assembly from other Blazor projects to ensure this approach is sensible for a variety of existing applications.

* Create a separate development branch to keep new features out of the main branch until they are considered and tested.

* Publish a package on nuget.org
