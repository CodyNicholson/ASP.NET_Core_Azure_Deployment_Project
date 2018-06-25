# Installing Dependencies

First we install Visual Studio 2017, then NodeJS by going to their respective websites and clicking all the download links. Then we go to our terminal and make a new directory called Angular_Demo using: *mkdir Angular_Demo*

We go into our folder using: *cd Angular_Demo*

Next, we need to install the templates we are going to use to make our project by using: dotnet new --install Microsoft.AspNetCore.SpaTemplates::*

Now that we have our templates we can create our Angular project by running: dotnet new angular

Our project won't work until we install the node and dotnet dependencies using the following commands:

*dotnet restore*
*npm install*

## Running the Application

Now we can run the applicaton by using: *dotnet run*

We then go to the localhost provided in the output in the terminal
