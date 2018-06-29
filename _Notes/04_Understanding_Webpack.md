# Understanding Webpack

We can streamline the development experience by leveraging *Webpack Dev Middleware*. Webpack is included in the SPA templates by default. It intercepts requests that match files built by Webpack and dynamically build those files on demand. They don't need to be written to the disk, they're just held in memory and served directly through a browser.

Some benefits of using Webpack are that you don't need to run it manually or setup any file watchers. Browser receives up-to-date build output. The build artifacts are served instantly or at least extremely quickly because, internally, an instance of Webpack stays active and has partial compilation states pre-cached in memory. 
