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

## Randomly seeded users
The backend now implements random seeding of users, creating 300 unique users which can be used for testing. The users are generated with four different passwords and unique hash codes:
- "$2a$11$mbfii1SzR9B7ZtKbYydLOuAPBSA2ziAP0CrsdU8QgubGo2afw7Wuy", // Timianerkul1!
- "$2a$11$5ttNr5DmMLFlyVVv7PFkQOhIstdGTBmSdhMHaQcUOZ8zAgsCqFT6e", // SuperHash!4
- "$2a$11$KBLC6riEn/P78lLCwyi0MO9DrlxapLoGhCfdgXwLU2s44P.StKO/6", // Neidintulling!l33t
- "$2a$11$DFMtyLv243uk2liVbzCxXeshisouexhitDg5OUuBU.4LVn//QG5O."  // lettPassord123!
To log in with any of the users, make sure your backend is up to date with the server repo. Add a migration and update your database. The users will then be displayed in a users table in your database. To know which password is correct for a user, check the matching hash in the passwordhash column with the four hash codes above, then use the password for the matching hash, and the email for the user, and log in.
