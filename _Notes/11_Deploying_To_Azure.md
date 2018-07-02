# Deploying To Azure

Our goal is to setup a continuous deployment pipeline to deploy our app from GitHub to Azure - or any other cloud platform. We will setup a continuous integration environment and deploy code on each check in directly to GitHub using three simple steps:

1. Create GitHub Repo and upload our code
2. Setup continuous integration using Visual Studio team services (Or Jenkins, TeamCity, Etc.)
3. Use Visual Studio Team Services to deploy our application once we have a green build from the continuous integration pipeline to an Azure website

***

## Create Azure Website & Add Some Code To It

Now we can go to portal.azure.com and log in, search for "Web App", create a new Web App with whatever name we want, add the default subscription, create a new resource group, and click "Create". Now our app will deploy and have no code.

Next, we can go to GitHub.com and create a new repo with any name. Then add our code to it.

