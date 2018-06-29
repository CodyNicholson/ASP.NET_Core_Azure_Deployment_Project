# Efficient Production Builds

Before being able to deploy our application, we need to make an Efficient Production Build. We have seen that the project templates are setup to build our client-side assets: typeScript, bundled HTML, CSS, etc. in two different modes: Development (Which includes source maps for easy debugging) and Production (Which tightly minifies your code and does not include source maps). This is achieved using Webpack - we can easily edit the Webpack config files to set up whatever combination of build options we require. Before we are ready to publish for production, we need to enable support for compiling LESS, SASS, or other popular front-end file types and languages.

We will build our LESS files with Webpack, and this could easily be adapted to SASS too. There are three main approaches to doing this. If you're using Angular you can use it's **Angular native style loader** to attach the styles to components. Since this only applies to Angular components, we sometimes need different techniques. You could also use **Webpack's style loader** to attach the styles at runtime. The CSS markup will be included in your JavaScript bundles and will be attached to the document dynamically - which is good for development but not for production. Lastly, we could have each build write a **Standalone CSS file** to the disk and load it at runtime using a regular Link tag. This is likely the approach you'll want for production use, at least for non-Angular applications.

## Scoping Styles In Angular

Scoping styles is the easiest way to perform styling if you're using Angular. It works both with Server and the client rendering and supports hot module replacement.

If we go into our Angular_Sample/ClientApp/app/components/app/ you will see our *app.component.css* our *app.component.html* and our *app.component.ts* files. In our *app.component.ts* file you can see the **styleUrls** attribe of the @Component annotation. That's how we link in the CSS. This is being built with Webpack. If we open *webpack.config.js* and then go to the *rules* in *module* in *sharedConfig* we can see that the **test** for *css* files is using *to-string-loader* and the *css-loader*. These CSS files will be injected as a string literal into the build file. We are not using LESS here. To be able to handle LESS files we need to install some NPM packages:

```
npm install --save less-loader less
```

Now that we have those npm packages we can add a line to our **webpack.config.js** file in *sharedConfig*, *module*, *rules*:

```
{ test: /\.less/, include: /ClientApp/, loader: 'raw-loader!less-loader' }
```

This line added to our loaders array transforms LESS syntax into plain CSS syntax. Then the raw-loader turns the result into a string literal. With this in place you can reference .less files from your Angular components. Lets go back to our *app.component.ts* file to do this: 

```ts
import { Component } from '@angular/core';

@Component({
    selector: 'app',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.less']
})
export class AppComponent {
}
```

Instead of referencing the CSS file we will reference a LESS file and our styles will now be applied both on the server-side and on the client-side rendering. Before we actually see this in action, we need to rename *app.component.css* to be *app.component.less*. 

```css
@media (max-width: 767px) {
    /* On small screens, the nav menu spans the full width of the screen. Leave a space for it. */
    .body-content {
        padding-top: 50px;
    }
}
```

Since we are using LESS we can create variables to hold data like this:

```less
@paddingTop = 50px;

@media (max-width: 767px) {
    /* On small screens, the nav menu spans the full width of the screen. Leave a space for it. */
    .body-content {
        padding-top: @paddingTop;
    }
}
```

Now that we have our styling rendering on the server, we can change the styling from 50px to 150px and get immediate changes on our localhost without restarting the server! This only works with Angular components, so we will learn different ways to do this for other program stacks.

## Loading Styles With Webpack & JavaScript

To start, we will copy over the Angular project and undo the Angular Scoping Styles modifications we made to the ClientApp/app/components/app/ files. This gives us a fresh start to learn how to load styles with Webpack & JavaScript.

This technique will work for any client-side framework (Not just Angular) and it can apply styles to an entire document, rather than just individual components. It also works with Hot Module Replacement. The downside is that it's really only good for development time because in production you don't want the users to wait until JS is loaded before the styles are applied to the page because that would mean that they'd see a flash of unstyled content while the page is being loaded. So in VS Code let's create a new folder in ClientApp/ called styles/. In this folder we will add **mystyles.less** which will have some basic styling for h1 elements:

```
@base: #f938ab;

h1 {
    color: @base;
}
```

We still need the *less-loader* and *less* which we installed in the Angular Style Loader tutorial before this. We need to reference our *mystyles.less* file from an import or require statement in one of our JavaScript of TypeScript files. So we can go to our ClientApp/boot.browser.ts file and add the import "import './styles/mystyles.less'":

```ts
import 'reflect-metadata';
import 'zone.js';
import 'bootstrap';
import './styles/mystyles.less';
import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { AppModule } from './app/app.browser.module';

if (module.hot) {
    module.hot.accept();
    module.hot.dispose(() => {
        // Before restarting the app, we create a new root element and dispose the old one
        const oldRootElem = document.querySelector('app');
        const newRootElem = document.createElement('app');
        oldRootElem!.parentNode!.insertBefore(newRootElem, oldRootElem);
        modulePromise.then(appModule => appModule.destroy());
    });
} else {
    enableProdMode();
}

// Note: @ng-tools/webpack looks for the following expression when performing production
// builds. Don't change how this line looks, otherwise you may break tree-shaking.
const modulePromise = platformBrowserDynamic().bootstrapModule(AppModule);
```

We will make one more change to our *webpack.config.js* file as well. We add this line:

```js
{ test: /\.less/, loader: 'style-loader!css-loader!less-loader' }
```

This line means when we import or require a LESS file it should pass it first to the LESS compiler to produce CSS, and then the output does through a CSS and style loaders that know how to attach it dynamically to the page at runtime. This Results in:

```js
const path = require('path');
const webpack = require('webpack');
const merge = require('webpack-merge');
const AotPlugin = require('@ngtools/webpack').AotPlugin;
const CheckerPlugin = require('awesome-typescript-loader').CheckerPlugin;

module.exports = (env) => {
    // Configuration in common to both client-side and server-side bundles
    const isDevBuild = !(env && env.prod);
    const sharedConfig = {
        stats: { modules: false },
        context: __dirname,
        resolve: { extensions: [ '.js', '.ts' ] },
        output: {
            filename: '[name].js',
            publicPath: 'dist/' // Webpack dev middleware, if enabled, handles requests for this URL prefix
        },
        module: {
            rules: [
                { test: /\.ts$/, use: isDevBuild ? ['awesome-typescript-loader?silent=true', 'angular2-template-loader', 'angular2-router-loader'] : '@ngtools/webpack' },
                { test: /\.html$/, use: 'html-loader?minimize=false' },
                { test: /\.css$/, use: [ 'to-string-loader', isDevBuild ? 'css-loader' : 'css-loader?minimize' ] },
                { test: /\.less/, loader: 'style-loader!css-loader!less-loader' },
                { test: /\.(png|jpg|jpeg|gif|svg)$/, use: 'url-loader?limit=25000' }
            ]
        },
        plugins: [new CheckerPlugin()]
    };

    // Configuration for client-side bundle suitable for running in browsers
    const clientBundleOutputDir = './wwwroot/dist';
    const clientBundleConfig = merge(sharedConfig, {
        entry: { 'main-client': './ClientApp/boot.browser.ts' },
        output: { path: path.join(__dirname, clientBundleOutputDir) },
        plugins: [
            new webpack.DllReferencePlugin({
                context: __dirname,
                manifest: require('./wwwroot/dist/vendor-manifest.json')
            })
        ].concat(isDevBuild ? [
            // Plugins that apply in development builds only
            new webpack.SourceMapDevToolPlugin({
                filename: '[file].map', // Remove this line if you prefer inline source maps
                moduleFilenameTemplate: path.relative(clientBundleOutputDir, '[resourcePath]') // Point sourcemap entries to the original file locations on disk
            })
        ] : [
            // Plugins that apply in production builds only
            new webpack.optimize.UglifyJsPlugin(),
            new AotPlugin({
                tsConfigPath: './tsconfig.json',
                entryModule: path.join(__dirname, 'ClientApp/app/app.browser.module#AppModule'),
                exclude: ['./**/*.server.ts']
            })
        ])
    });

    // Configuration for server-side (prerendering) bundle suitable for running in Node
    const serverBundleConfig = merge(sharedConfig, {
        resolve: { mainFields: ['main'] },
        entry: { 'main-server': './ClientApp/boot.server.ts' },
        plugins: [
            new webpack.DllReferencePlugin({
                context: __dirname,
                manifest: require('./ClientApp/dist/vendor-manifest.json'),
                sourceType: 'commonjs2',
                name: './vendor'
            })
        ].concat(isDevBuild ? [] : [
            // Plugins that apply in production builds only
            new AotPlugin({
                tsConfigPath: './tsconfig.json',
                entryModule: path.join(__dirname, 'ClientApp/app/app.server.module#AppModule'),
                exclude: ['./**/*.browser.ts']
            })
        ]),
        output: {
            libraryTarget: 'commonjs',
            path: path.join(__dirname, './ClientApp/dist')
        },
        target: 'node',
        devtool: 'inline-source-map'
    });

    return [clientBundleConfig, serverBundleConfig];
};
```

Now if we run the application and change our *mystyle.less* file color and save the file, our hot module replacement will kick in and change the color immediately without needed to restart the server. This means that this technique is compatible with both source maps and hot module replacement. That's why we can edit our LESS files and see the changes appear live in the browser immediately. This is not optimal in production because we don't want our users to wait for our JavaScript to load before seeing styles applied.

## Building LESS to CSS Files on Disk

The third approach is to build LESS to write CSS files on the disk. This technique takes a little more work to set up than the others and it lacks the compatibility with hot module replacement, but it's much better for production use if styles are applied to the whole page because it loads the CSS independently of the JavaScript. So we will keep the *mystyles.less* file and also the *boot-client.ts* file. We will keep the import of the *mystyles.less* file in the *boot-client.ts* file. We will need a third npm module we don't have yet:

```
npm install --save extract-text-webpack-plugin
```

Now that we have this we need to go to our *webpack.config.js* file and add a new line at the top:

```js
const extractStyles = new (require('extract-text-webpack-plugin'))('mystyles.css');
```

This creates a plugin instance that will output a text file called *mystyle.css*. We can now compile LESS files and emit resulting CSS text into that file, and to do so, we're going to need to add the following to our loaders array in our Webpack configuration:

```js
{ test: /\.less$/, loader: extractStyles.extract('css-loader!less-loader') },
```

This tells Webpack that whenever it finds a LESS file, it should use the less-loader to produce CSS and then feed that CSS into the extractStyles object which we're already configured to write a file on the disk called *mystyle.css*.

For this to work, we need to include the *extractStyles* in the list of active plugins. We add it to the *plugins* array in our **webpack.config.js**:

```js
        plugins: [
            new CheckerPlugin(),
            extractStyles
        ]
```

If we switch back to command prompt and manually run webpack - this will output a new file at dist/mystyles.css as we can see in the output. Now we can just make sure the browser loads this file by adding a regular Link tag to our *Views/Shared/_Layout.cshtml* file:

```html
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"] - Angular_Sample</title>
    <base href="~/" />
    <link rel="stylesheet" href="~/dist/vendor.css" asp-append-version="true" />
    <link rel="stylesheet" href="~/dist/mystyles.css" asp-append-version="true" />
</head>
<body>
    @RenderBody()

    @RenderSection("scripts", required: false)
</body>
</html>
```

Now when we start the application again we can see our styles header. This technique is ideal for a production environment but not at development time because it does not support hot module replacement and we can see that by changing the style and saving the file. However, if we refresh the page then we can see the updated file - so we don't need to restart the server.

## Publishing For Development

Now we will create our first production build. First we will take a look at the payload that we are pulling down into our browser currently. We can do this by running the application with dotnet run, then opening dev tools, then clicking "Network", then refresh and hard reload the page. You should see that we get about 5 MB of data - which is a lot considering how simple our application is. If we take a look at our vendor file, we can see it is not minified. Also, the same is true for our own application code.

If we wanted, we could run Webpack manually to create the builds but we don't normally do that. To do this we would run:

```
webpack --config webpack.config.vendor.js
```

This would minify all of our third-party libraries like Angular and all of its dependencies. We only need to run this if we modify our third-party dependencies, such as if we update to a newer version of our chosen SPA framework. We don't pass any params for the config file, we will use the webpack.config.js file by default, which corresponds to our own application code and this is equivalent to typing webpack and these commands produce development mode builds"

```
webpack
```

If that doesn't work, try this:

```
.\node_modules\.bin\webpack
```

If we want to produce production mode builds, we can also pass the flag "--env.prod" and this goes both for the vendor files and their own application code:

```
webpack --env.prod
```

But instead of invoking webpack manually, we can use the publish feature, which is built into dotnet comand line tooling, so we can simply write:

```
dotnet publish -c Release
```

This will produce our ready-to-deploy production build of our application. This will include the dotnet code compiled in Release mode and it will invoke the webpack commands that we just saw with a "--environment.prod" flag to produce production a build of our front end assets.

Now we can run the application, open dev tools, go to "Network", hard reload refresh the page, and we will see much smaller sized files.

If we stop the application and go to the \bin\Release
netcoreapp1.1\publish\ folder we can find all the files we need to deploy to the platform of our choosing.
