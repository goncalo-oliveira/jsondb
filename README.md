# JsonDb

A small JSON database for small projects.

## Getting Started

Add the appropriate NuGet package to your project. See [alternatives to local storage](#alternatives-to-local-storage) for more options.

```bash
$ dotnet add package JsonDb
```

To use the JSON database, you first need to configure your dependency injection container with the desired implementation. The bellow is an example using `LocalStorage`.

```csharp
IServiceCollection services = ...

services.AddLocalJsonDb( options =>
{
    options.DbPath = "db";
} );
```

This gives us access to an `IJsonDbFactory`, injected wherever we need it. Here's an example to retrieve users from a `users.json` file.

```csharp
public class MyService
{
    private readonly IJsonDb db;

    public MyService( IJsonDbFactory jsonDbFactory )
    {
        db = jsonDbFactory.GetJsonDb();
    }

    public Task<IEnumerable<User>> GetUsersAsync()
    {
        var users = await db.GetCollectionAsync<User>( "users" );

        return ( users );
    }
}
```

The collections implement an `IEnumerable<T>`, which means we can iterate directly or use LINQ extensions to query our collection.

```csharp
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
}

...

public class MyService
{
...
    public Task<User> GetUserAsync( int id )
    {
        var users = await db.GetCollectionAsync<User>( "users" );

        var user = users.Where( x => x.Id == id )
            .SingleOrDefault();

        return ( user );
    }
}
```

Similarly, removing items from our collection also uses a predicate.

```csharp
users.Remove( x => x.Id == 2 );
```

When we add or remove items to/from our collection, these changes aren't automatically written to the source. The reason why is that this would be much slower when adding multiple items. Therefore, after adding or removing items we need to write the changes.

```cshar
await users.WriteAsync();
```

## Ad-hoc Usage

If you just need a "readonly" collection (no physical write support), you can also use the `JsonDbCollection` helpers to directly read JSON content into a collection.

```csharp
// reading directly from a JSON file
var usersFile = await JsonDbCollection.ReadFileAsync<User>( "path/users.json" );

// reading directly from an URI
var stuffUrl = await JsonDbCollection.ReadUrlAsync<User>( "https://gist.githubusercontent.com/goncalo-oliveira/2e7386336652721c06943e9099e4c622/raw/ee21a456573cfff79ebc01cb1b8daa1f9294b364/jsondb-users.json" );
```

## Encryption

Currently, there's no encryption over the json data. There are plans to implement this.

## Alternatives to local storage

There is also support for AWS S3 compatible storage. Install the package `JsonDb.S3` instead and add the following to the container setup

```csharp
IServiceCollection services = ...

// example with DigitalOcean's S3 compatible Spaces
services.AddS3JsonDb( options =>
{
    options.ServiceUrl = "https://ams3.digitaloceanspaces.com/";
    options.BucketName = "digitalocean-space-name";
    options.AccessKey = "digitalocean-access-key";
    options.SecretKey = "digitalocean-secret-key";
    options.DbPath = "jsondb";
} );
```

If you'd like to write your own, install the package `JsonDb.Core` and implement the following interfaces:

- IJsonDbFactory
- IJsonDb
- IJsonCollection<T>

You can derive your collection from `InMemoryJsonCollection<T>` and just implement the `WriteAsync` method.
