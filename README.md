Ratings App

A simple application which provides platform for ratings - be it of music and composers, books and their authors or movies and their directors.

The basis is a user account system that has the following characteristics:
- unlogged user is able to select an author and see his photo, bio, list of works, their ratings and reviews (if there are any)
- unlogged user may create his own account and then rate works and optionally provide some reviews; before user rating is saved in the database a new average rating is computed and stored in the database - that allows not to compute an average each time someone visits some author's page
- if a particular user is active and provides many reviews and ratings, the administrator may change his role to "moderator" - such user still not only can rate works and write reviews, but also add new artists to the database, provide their photo and bio and add their works to be rated by all users
- user with an admin role not only has aforementioned moderator's privileges, but can also manage users - edit their credentials or change their role.

Technologies used: C#, ASP.NET Core, ASP.NET Core Identity, Entity Framework Core

Requirements:

- .NET 6.0 SDK

Optional:
- Microsoft Entity Framework Core tool package
- Some Database Server, i.e. MS SQL Server

In order to run an app please do the following:

- go to main direcory (containing "Ratings.csproj" file), open "appsettings.json" file and provide credentials for admin account (default are Admin/Admin123!)
- as the app works using https, you'll also need dev certificates which can be created using the following command: dotnet dev-certs https --trust
- in the same directory run the command: dotnet run --isMemoryDb=true

It's also possible to run app with SQL Server:

- go to main directory (containing "Ratings.csproj" file), open "appsettings.json" file and substitute connection string provided for "IdentityConnection" and "RatingsConnection" to match your database (example for MS SQL: "Server=SERVER_NAME\DB_INSTANCE;Database=DB_NAME;Trusted_Connection=True;MultipleActiveResultSets=True")
- in the same directory run the following command: dotnet ef database update --context IdentityContext
- in the same directory run the following command: dotnet ef database update --context RatingsContext
- being still in the same run the command: dotnet run
