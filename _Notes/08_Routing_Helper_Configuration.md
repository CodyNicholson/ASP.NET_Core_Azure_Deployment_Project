# Routing Helper Configuration

Most of the time our Client-side routing and Server-side routing do not interfere with each other. However, in the case of a 404 error they do. If a request arrives for a route our client-side will hopefully understand it. However, if a request arrives for a static file we should also be able to handle that. How do we handle when our server and client-side don't recognize the route?

In our project we handle this using the *Routing Helper: MapSpaFallbackRoute*.

In our *Startup.cs*:

```cs
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
```

You can see in the above code that our fallback "if we give the application a bad route" is to go to the index page of the home controller. However, if we enter an incorrect static file in as a route: "home/home.sadf", we will get a 404 page. This is because the MapSpaFallbackRoute does not handle incorrect file extension routes. For this, we would be better off having a catch-all error route and not using MapSpaFallbackRoute.

## Dotnet Watch Run

When we use the dotnet watch tool, when we change our server-side code the watch tool will detect the changes and recompile our code so we don't have to kill and start the server again.

To install the watch tool we go to our *csproj* file and add:

```cs
  <ItemGroup>
    <DotNetCliToolReference Include="Microsoft.DotNet.Watcher.Tools" Version="2.0.0" />
  </ItemGroup>
```

Then we run *dotnet retore* and the tool will install. Now we can run:

```
dotnet watch run
```

And the tool will tell us that it is running and waiting to detect changes

