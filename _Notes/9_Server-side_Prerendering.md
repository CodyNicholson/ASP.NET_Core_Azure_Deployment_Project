# Server-side Prerendering

Server-side prerendering makes your application's initial UI appear much more quickly because users don't have to wait for their browser to download, evaluate, and execute large JavaScript libraries before the application appears. Server-side prerendering are often associated with universal - also known as isomorphic - applications. This means that the code can run both on the server and the client, meaning that our Angular or React components are first rendered on the server and then transferred to the client where execution continues. This makes your app faster even if your code base is very large. 

Since SAP services package isn't tied to any particular client-side framework, it doesn't force you to set up your client-side application in any particular style, so SPA services doesn't contain hardcoded logic for rendering Angular or React components. We will setup server-side prerendering on a brand new application.

To create a new empty ASPNET Core project we create a new directory called Prerendering and then run: *dotnet new web*.

In our new project we need to add our dotnet dependencies so we go to the *csproj* file and add these to the ItemGroup:

```cs
  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.All" Version="2.0.8" />
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
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvcWithDefaultRoute();
        }
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

## Adding Server-side Prerendering

Now that we have our Hello World ASPNETCORE App we will add *prerender tag helpers*. To enable our tag helpers in our Razor files we need to create a new file to our Views/ folder called **_Viewimpots.cshtml** and we're going to import the tag helpers into that file:

```html
@addTagHelper "*, Microsoft.AspNetCore.SpaServices"
```

Then we need to install npm packages so we run:

```
npm init (Leave all cli options blank)
npm install --save aspnet-prerendering
```

Now we can go to our Home/Index.cshtml file and add prerendering to our home view:

```html
<div id="my-spa" asp-prerender-module="ClientApp/boot-server"></div>>
```

Now if we run our application we will get an error that we cannot find the boot-server in the ClientApp.

We can create a folder in our application root called ClientApp/ and put a JavaScript file in that folder called: **boot-server.js**:

```js
const prerendering = require('aspnet-prerendering');

module.exports = prerendering.createServerRenderer(function(params) {
    return new Promise(function (resolve, reject) {
        var result = "<h1>Hello world, from JS!</h1>"
            + "<p>Current time in node: " + new Date() + "</p>"
            + "<p>URL: " + params.location.path + "</p>";
        resolve({ html: result });
    })
});
```

We first use the *aspnet-prerendering* library we installed from npm. Then we export the function **createServerRenderer()** that returns a promise (Since it's asynchronous) with our HTML code inside of it. We can then return the time and date and URL path as well. This is the same way we would return React or Angular components. 

If we want we can also supply additional data to the JavaScript function that performs our prerendering. We can use the ASPNET prerendered data attribute. Let's go back to our Index.cshtml file for our Home page:

```html
<div id="my-spa" 
    asp-prerender-module="ClientApp/boot-server"
    asp-prerender-data="new {
        IsAdministrator = true,
        Cookies = ViewContext.HttpContext.Request.Cookies
    }"></div>>
```

The above attribute we added is pure C# code. We can now go back to the boot-server.js and access the data:

```html
const prerendering = require('aspnet-prerendering');

module.exports = prerendering.createServerRenderer(function(params) {
    return new Promise(function (resolve, reject) {
        var result = "<h1>Hello world, from JS!</h1>"
            + "<p>Current time in node: " + new Date() + "</p>"
            + "<p>URL: " + params.location.path + "</p>"
            + "<p>data (IsAdministrator): " + params.data.isAdministrator + "</p>"
            + "<p>Number of cookies: " + params.data.cookies.length + "</p>";
        resolve({ html: result });
    })
});
```

Now when we run this code we see the data in the output on our home page. However, we are not even using Webpack yet! Instead of writing JavaScript, we we change to TypeScript which SPA templates leverage.

***

To enable writing client-side TypeScript code, we need to setup a **build system**. Make sure you have Webpack installed! (*npm -g webpack*). Then we can add a *wepack.config.js* file to our project's root folder:

```js
const path = require('path');

module.exports = {
    entry: { 'main-server': './ClientApp/boot-server.ts' },
    resolve: {
        extensions: [ '.js', '.ts' ]
    },
    module: {
        rules: [
            { test: /\.ts$/, loader: 'ts-loader' }
        ]
    },
    target: 'node',
    devtool: 'inline-source-map',
    output: {
        path: path.join(__dirname, './clientApp/dist'),
        filename: '[name].js',
        libraryTarget: 'commonjs'
    }
};
```

The *module* and *resolve* objects tell our application how to handle js and ts files. We don't have a configuration file for TypeScript yet so we add a *tsconfig.json* file to our root folder to tell our application how to transpile TS files into JS files:

```js
{
    "compilerOptions": {
        "moduleResolution": "node",
        "target": "es5",
        "sourceMap": true,
        "lib": ["es6", "dom"]
    },
    "exclude": [
        "bin", "node_modules"
    ]
}
```

Now we can change our boot-server.js file to boot-server.ts and modify the code to be TS:

```js
import { createServerRenderer } from 'aspnet-prerendering';

export default createServerRenderer(params => {
    return new Promise(function (resolve, reject) {
        const html = `<h1>Hello world, from JS!</h1>
            <p>Current time in node: ${ new Date() }</p>
            <p>URL: ${params.location.path}</p>
            <p>data (IsAdministrator): ${params.data.isAdministrator}</p>
            <p>Number of cookies: ${params.data.cookies.length}</p>
            `;
        resolve({ html });
    })
});
```

Now that we created a new main-server.js file in the ClientApp/dist folder we need to update our path in Index.cshtml:

```html
<div id="my-spa" 
    asp-prerender-module="ClientApp/dist/main-server"
    asp-prerender-data="new {
        IsAdministrator = true,
        Cookies = ViewContext.HttpContext.Request.Cookies
    }"></div>>
```

Now if we dotnet run our application we will get our same result from before but now we are tranpiling TypeScript files using webpack just as the SPA templates are. Now we will take a look at the prerendering of our angular components.

***

## Angular/React+Redux Prerendering

If you're using the Angular or React+Redux templates then you have server-side prerendering set up. This means that if we disable JavaScript in our browser, we can still view and navigate through our Angular-Sample Application. However, we cannot use all functions, like incrementing the counter and toggling the hamburger menu.

This is handled very similarly in our boot-server.ts file, but a little different for Angular. They use the same attribute in their Index.cshtml file.
