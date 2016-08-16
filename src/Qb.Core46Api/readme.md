# qb.core46api

Api server using ASP.Net Core targettigns full .Net 4.6 framework.
Core framework is only useful if targetting a non windows platform, full net46 has far more libraries available (e.g. Twilio API).


## Running Locally For Dev

### Create and Add Local Db

* Open `SQL Server Object Explorer` in Visual Studio.
* Under the `MSSQLLocalDB/Databases` section create a new db called `qbdb`.
* The connection string in `appsettings.development.json` should match the connection string from `qbdb`, but check if you have problems.

The app uses `ConnectionStrings/DefaultConnection` from the `appsettings.json` when it runs (the `.development.` version is applied over the top).
Only the local testing database should be set directly via `appsettings.*.json`;
Azure databases should not be saved here otherwise it'll make a mess of deploying to mutliple server and or deployment slots.
There is a value in the application settings on the Azure server that replaces this value with the connection string for the Azure SQL database.


### Migrate and Update the DB

Full tools documentation : https://docs.efproject.net/en/latest/miscellaneous/cli/dotnet.html

To use the `dotnet` commandline tools first **cd into the project folder**  (not the solution but `src/{projectname}`).

The ef tools are detected from the tools section of `project.json`, also require the design dependency for some reason:

```json
  "dependencies": {
    "Microsoft.EntityFrameworkCore.Design": "1.0.0-preview2-final",
  },

  "tools": {
    "Microsoft.EntityFrameworkCore.Tools": "1.0.0-preview2-final",
  },
```

The `ef` commands will want to detect the connection string that has been set up, the commands default to looking for settings in the `development` environment.

```bash
dotnet ef migrations add {migration name}
```

To apply the migration:

```bash
dotnet ef database update
```

Have a look at the database in `SQL Server Object Explorer` in Visual Studio and make sure that there are tables in it.


## Publishing Production to Azure

* Must tick the migrate option on publishing to apply the migration to the server, needs to be doen on every new migration.

> It is possible to use `dotnet ef database update --environment production` instead, but the Azure database connection string will need to the `appsettings.json` (bad because it is inflexible).

* Open the `Application Settings` for the server in Azure portal.
* Go to the `Connection Strings` section and make sure that there is an entry named `DefaultConneciton` with a value of the connection string for the Azure database.
* This value will override the value read from `appsettings.json`.

Finally press publish.


## Start-Up Tests

The MVC `TestController` class exists to test that the basic parts of the server are working.

* `api/test` Should always show a simple hello message.
* `api/test/exception` throws a server crashing exception.
  * In development the developer exception page should appear.
  * In production a generic fatal error page should appear (`error.html`).
* The site also has a homepage at `/` (`index.html`).


## Authentication

### Admin user

When server is started a default admin user is created if it does not exists. This uses has the `admin` role, this give it access to apis that can modify and verify users, which are marked with the `[Authorize(Roles = "admin")]` attribute.

```
username = admin
password = xxxxxxxx
```

Normal users do not have a specified role. This can be changed later when data sharing tokens are introduced.

Tokens will only grant role based access if `Roles` is one of the entries in the value for the `scope` attribute in the request.

### Token Vending

The tokens are generated using the [OpenIddict](https://github.com/openiddict/openiddict-core) library's implentation of [OAuth2 password flow](https://tools.ietf.org/html/rfc6749#section-4.3).

```
Attribute  |  Value
-----------------------
grant_type | password
username   | {username}
password   | {password}
scope      | Roles
```