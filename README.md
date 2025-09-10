# Team Simulation - Server (C# Version)

## Setting Up and Getting Started

- You'll need a Postgres Database set up either on Neon, in Docker or some other location. 
	- Configure your database (either Neon or Docker using the provided compose) 
	- Make sure the `Program.cs` is passing the string to the `DataContext.cs`
	- A first migration has been provided. With the db connection string setup, simply then update-database to run the migration.
	- The register and login endpoints should work from Swagger, Insomnia, .Http file provided or from the React app.

- JWT has been setup for you
- The Endpoints folder contains some code to get you started.  

## Working with the Project

The code in the application is designed to recreate basic functionality that is found in the Node version of the Server which was supplied as an initial proof of concept by the client.

The Client code should allow you to login and display the initial dashboard in almost identical ways, whether the C# or the Node versions of the Server are in use. The functionality is severely limited beyond that, however.

Once you are fully up to speed and working on the project it is perfectly acceptable to change the structure of this code in any way that you see fit, as long as you adhere to whatever process your team has in place and you meet the requirements in the backlog.

## Enjoy!

Once you have your teams set up, enjoy working on the code.

We look forward to seeing what you manage to produce from it!


