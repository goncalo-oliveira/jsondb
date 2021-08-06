# JsonDb

A small JSON database for small projects.

> WARNING: This is an unfinished project. Use at your own risk.

## Getting Started

Add the NuGet package to your project

```bash
dotnet add package JsonDb
```

To use the JSON database, you first need to configure your dependency injection container with the desired implementation. The bellow is an example using `LocalStorage`.

```csharp
IServiceCollection services = ...

services.AddLocalJsonDb( options =>
{
    options.DbPath = "db";
} );
```

This gives us access to a `IJsonDbFactory`, injected wherever we need it. Here's an example to retrieve users from a `users.json` file.

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

The collections implement an `IEnumerable<T>`, which means we can also use LINQ extensions to query our collection.

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

## Encryption

Currently, there's no encryption over the json data. There are plans to implement.

## Alternatives to local storage

There are plans to implement an S3 compatible `IJsonDb`.

If you'd like to write your own, you'll need to implement the following

- IJsonDbFactory
- IJsonDb
- IJsonCollection<T>
