Ratings App

Simple Ratings application made mostly by using ASP.NET Core and a little bit of JS. It uses Identity Core framework to enable using and creating users accounts.
There are 3 types of accounts:
- "admin" has all the control over user accounts (edit/delete) and has access to functionalities that the moderator account has (please see below)
- "moderator" can add/delete/edit an artist and his/her works; can also write reviews and rate works
- "user" can only write reviews and rate works
Unlogged users can only see the works, their average ratings and reviews. Each time a logged in user rates a work there's an average rate computed and saved in the daabase.
Unlogged users can create their own accounts. If an admin decides a user may be admitted "moderator" priviledges, he can change user's role from "user" to "moderator".

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
