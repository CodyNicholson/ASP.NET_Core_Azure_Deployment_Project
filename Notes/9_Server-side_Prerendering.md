# Server-side Prerendering

Server-side prerendering makes your application's initial UI appear much more quickly because users don't have to wait for their browser to download, evaluate, and execute large JavaScript libraries before the application appears. Server-side prerendering are often associated with universal - also known as isomorphic - applications. This means that the code can run both on the server and the client, meaning that our Angular or React components are first rendered on the server and then transferred to the client where execution continues. This makes your app faster even if your code base is very large. 

Since SAP services package isn't tied to any particular client-side framework, it doesn't force you to set up your client-side application in any particular style, so SPA services doesn't contain hardcoded logic for rendering Angular or React components. We will setup server-side prerendering on a brand new application.

To create a new empty ASPNET Core project we create a new directory called Prerendering and then run: *dotnet new web*.

In our new project we need to add our dotnet dependencies so we go to the *csproj* file and add these to the ItemGroup:

```cs
  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore" Version="2.0.8" />
    <PackageReference Include="Microsoft.AspNetCore.Mvc" Version="1.1.1" />
    <PackageReference Include="Microsoft.AspNetCore.SpaServices" Version="1.1.0" />
  </ItemGroup>
```

Then we can save this and run *dotnet restore*.

Now we can go back to our *Startup.cs* and in the *ConfigureServices()* method we can add MVC:

```cs
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }
```

Then below that code we can replace *app.Run()* with *app.UseMvcWithDefaultRoute()*:

```cs

```

Then we can add the *Controllers* and *Views* folders to our project root directory and create the *HomeController.cs* file in our Controllers directory:

```cs
using Microsoft.AspNetCore.Mvc;

namespace prerendering.Controllers
{
    public class HomeController: Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
```

Now we can add our *Home* directory to our *Views* directory and put an *Index.cshtml* file into that directory:

```html
<h1>Hello world!</h1>
```

Now if we do *dotnet run* we will see our hello world! We are ready to add some server-side prerendering to our new project!
