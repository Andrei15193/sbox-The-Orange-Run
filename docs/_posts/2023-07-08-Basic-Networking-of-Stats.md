Just getting started, what else would be easier to do than add a small UI that shows the score screen?

The game template works, it loads the game nicely and I can move around. But when I press the tab key
I want to see a score screen with each player and how many oranges they have collected thus far.
This will make me familiar with how [Razor](learn.microsoft.com/aspnet/core/mvc/views/razor) is used
in [sbox](https://sbox.facepunch.com) as well as getting some [basic networking](https://wiki.facepunch.com/sbox/Network_Basics)
going.

What I want is to have a list of items that contain the ID, name and total orange count for each player.
Once a player joins, I add a new entry and when they leave, I remove the related entry. When an orange
is collected by a player, the relevant item is updated.

Obviously, I don't have any of this mechanics implemented so mocking the data will have to do, as we
usually do when making an UI. For collecting oranges we'll call a method that does just that.

```csharp
public partial class PawnStats : BaseNetworkable
{
    [Net]
    public int Id { get; set; }

    [Net]
    public string Name { get; set; }

    [Net]
    public int TotalOranges { get; set; }
}
```

From what I gather, the [BaseNetworkable](https://asset.party/api/Sandbox.BaseNetworkable) class makes
the subclass somewhat observable. Any time we change a `[Net]` decorated property it will get
propagated to all the clients, additionally we can use the `[Change]` attribute for `On..Changed`
methods to be called, which are very similar to event handlers. The idea is the same, it's just
implemented a bit different.

The [GameManager](https://asset.party/api/Sandbox.GameManager) is a subclass of
[BaseNetworkable](https://asset.party/api/Sandbox.BaseNetworkable) thus we get all the basic networking
that we need to share this information among the clients.

```csharp
[Net]
public IList<PawnStats> PawnsStats { get; private set; }
```

A bit of pluralizations going on there, collection names should always be plural as they can contain
several items and we generally treat them as such even when they are empty or contain one element.

The issue here comes from "stats" which already is a plural, but an item contains the stats for a single
player, making a collection of such items a collection of players stats (multiple players). When iterating
we wouldn't get the same stat for all players, but rather the stats for a single player.

I can rename this later on to something like `PlayerData` or `PlayerInfo` so there wouldn't be two plurals
in the collection name. This is information about the player after all, not their pawn.

This is something interesting that [sbox](https://sbox.facepunch.com) does for you. `PawnsStats` is an
[`IList<>`](https://learn.microsoft.com/dotnet/api/system.collections.generic.ilist-1) which gets
set for us. Behind the scenes, this is actually an observable collection, so whenever we make changes
to the collection, the system will know and it will propagate these changes to the clients.

This works hand in hand wih the individual items which propagate as well, so there's no need to remove
and add back the item to generate an update, we can update the item directly.

Alright, let's add stats when a player joins and remove them when they leave.

```csharp
public override void ClientJoined( IClient client )
{
    base.ClientJoined( client );
    PawnsStats.Add( new PawnStats { Id = client.Id, Name = client.Name, TotalOranges = 0 } );

    // ...
}

public override void ClientDisconnect( IClient client, NetworkDisconnectionReason reason )
{
    base.ClientDisconnect( client, reason );

    var statsIndex = 0;
    using ( var pawnStats = PawnsStats.GetEnumerator() )
        while ( pawnStats.MoveNext() && pawnStats.Current.Id != client.Id )
            statsIndex++;

    if ( statsIndex < PawnsStats.Count )
        PawnsStats.RemoveAt( statsIndex );
}
```

A classic search algorithm, typed enumerators implement [`IDisposable`](learn.microsoft.com/dotnet/api/system.idisposable),
it is unknown whether it actually does something in this case, but when we are dealing with a disposable
object it is best use it with the [`using` statement](https://learn.microsoft.com/dotnet/csharp/language-reference/statements/using)
to ensure the [`Dispose()`](https://learn.microsoft.com/dotnet/api/system.idisposable.dispose) method gets called.

This can be wrapped in an [extension method](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/extension-methods)
since the [`IList<>`](https://learn.microsoft.com/dotnet/api/system.collections.generic.ilist-1) interface does not provide
us with a method to find the index of an element using a predicate.

Now, to add oranges.

```csharp
public partial class TheOrangeRunGameManager : GameManager
{
    public void AddOrangeTo( IClient client )
    {
        var pawnStats = PawnsStats.FirstOrDefault( pawnStats => pawnStats.Id == client.Id );
        if (pawnStats is not null)
            pawnStats.TotalOranges++;
    }

    // ...
}
```

Just some defensive programming there, we don't know if the client for which we want to add oranges to is actually in our list.

Next, binding the input, we already got the `attack1` input defined so we'll just tap into that to add oranges for the respective player.

```csharp
public void Simulate( IClient client )
{
    if ( Input.Pressed( "attack1" ) )
        TheOrangeRunGameManager.Current.AddOrangeTo( client );

    // ...
}
```

It's probably obvious, nothing is visible thus it cannot be tested. We can add a few logs to address this.

```csharp
public partial class TheOrangeRunGameManager : GameManager
{
    public void AddOrangeTo( IClient client )
    {
        Log.Info( "AddOrangeTo" );
        var pawnStats = PawnsStats.FirstOrDefault( pawnStats => pawnStats.Id == client.Id );
        if ( pawnStats is not null )
        {
            pawnStats.TotalOranges++;
            Log.Info( $"name = {pawnStats.Name}, oranges = {pawnStats.TotalOranges}" );
        }
    }

    // ...
}
```

Now, whenever clicking the left mouse button, the logs should be showing up on the screen. This is good progress,
next up is getting this information shown on the UI, in a table or something like that.

In case you are wondering how `TheOrangeRunGameManager.Current` works, I've simply defined this property similarly
to the one in the base class.

Since it is a member with the same name the compiler will issue a warning about hiding members from the base class.
Static members do need the declaring type name in order to access them, but this is not the case for class member
declarations or when we import a class so we can access its members without the full qualifier (e.g.:
`using static System.Console;` no longer requires the `Console` qualifier to call the `WriteLine` method, we can do
it directly by just writing it).

To remove the warning we can use the [new modifier](https://learn.microsoft.com/dotnet/csharp/language-reference/keywords/new-modifier).

```csharp
public partial class TheOrangeRunGameManager : GameManager
{
    public static new TheOrangeRunGameManager Current
    {
        get => (TheOrangeRunGameManager)GameManager.Current;
        protected set => GameManager.Current = value;
    }

    // ...
}
```

This property is based on the one from the base class so at all times, both will point to the exact same instance.
The effect is that when using a more specific class name, in this case `TheOrangeRunGameManager`, we get the
instance of that specific type rather than base class type. This should save some explicit casting whenever we
need to access the game manager instance.
