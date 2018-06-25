# Setting Up ASPNET Core Dev Mode

Open Windows command prompt and run the command:

```
setx ASPNETCORE_ENVIRONMENT "Development"
```

You should get a success message upon doing this

If you are on Linux or Mac run this in the terminal:

```
export ASPNETCORE_ENVIRONMENT=Development
```

To check in Windows if it worked, restart command prompt and run:

```
echo %ASPNETCORE_ENVIRONMENT%
```
