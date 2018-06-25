# ASPNET Core With Configuration File

If you don't want to change your environment to Development with the ASPNETCORE_ENVIRONMENT variable we used, you can do it with a hosting.json file instead that will tell the ASPNETCORE environment to run in dev mode. Let's go to VS Code for our project.

In ASPNETCORE, the client-side code lived in wwwroot/ and the server-side code is everything else. We need to tell our project to read a file we will create called *hosting.json*:

```js
{
    "server.urls": "http://localhost:5000",
    "environment": "Development"
}
```

We now need to add this file to our *Program.cs* file. Our *Program.cs* file looks like this:

```cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Angular_Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("hosting.json", optional: true)
                .Build();

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseConfiguration(config)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();
            
            //BuildWebHost(args).Run();

            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
```

Above we created a new **config** variable and passed it through the function we chained onto our **host** variable: *UseConfiguration(config)*.
