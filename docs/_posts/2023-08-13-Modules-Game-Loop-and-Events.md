---
title: Modules, Game Loop, and Events
---

With pickups working, delivery points implemented in a similar fashion and sounds playing in the background and on specific events, it is time to get the game loop working.

In this case, the entire game can be modeled as a state machine where the player is able to do one thing or another depending on the state the game is in. On entry and exit of a particular state each entity may perform different actions or stop doing others.

For instance, the pawn entity will no longer take player input into consideration when the game is not in the "Orange Run" state. Basically, on entry to this state the input is taken into account, on exit it is ignored.

This applies to pickups as well, while the game state is "Orange Run" then oranges can be collected and dropped, otherwise the touching part of the collision is ignored. You can still stand on the box where you drop the oranges, it's just that you will not be dropping any past that point.

Orange Spawners behave a bit different. On entry in the "Orange Run" state, all oranges are respawned so they are available for pickup. That's all they do.

### Modules

The idea sounnds good and it probably works great for small or simple games where you have a handful of states that you can easily place in a signle diagram. There are few entry and exit actions that are performed. Mostly just flags being set or some entities being (re)created.

We could have an entire orchestrator that keeps track of the game state and makes all the changes for each object upon entry and exit for each state. We would have a grand state-machine modeled in our code and any time we need the state-machine to do something extra during transitions we would update this orchestrator.

A lot of code would be written for this and it would be concentrated in one place, or a number of callbacks which they themselves get centralized so we can perform these actions.

In addition to this, we would need decapsulate our entities as we would need to set different flags on them. Not necessarily a bad approach, we can see everything that changes for each transition, however this can be a lot and it may be difficult to follow.

Let's think about how we would go about debugging state transitions as that is what we will be doing when trying to fix an issue, or add new features.

Would we want to see _everything_ that changes in a transition? Would it be easier to follow each entity that changes and maybe even see how throughout state transitions an entity changes? I would assume in most cases it is a an entity that misbehaves, or a number of them, but even in that case we try to fix each one in particular rather than every single one of them.

Being able to follow state transitions per entity type, as well as following when they are spawned, would give us the entry points for each, the initial entity state, as well as follow how this initial entity state changes as the game state changes.

Two key concepts to separate here, game state which is just a label or a number, we can achieve both with enums, that is transitioned by the game manager, and entity state which is managed by each entity instance.

A **change** in **game state** is a **transition**, this is following the state-machine terminology, while a **change** in **entity state** is a **mutation**. This should clarify which type of change I am referring to going forward.

We can have other objects that react to state transitions as well, although it would mainly be entities as they are objects we interact with in the game.

Instead of the centralized state transitioner, we have each entity do something when the state changes. This can be implemented as easily as having an interface for transition reactive entities.

```csharp
public enum GameState
{
    Lobby,
    OrangeRun,
    Leaderboards
}

public interface IStateTransitioner
{
    void OnEntry(GameState state);

    void OnExit(GameState state);
}

public partial class Pawn : AnimatedEntity, IStateTransitioner
{
    public void OnEntry(GameState state)
    {
        switch (state)
        {
            case GameState.OrangeRun:
                // Enable input
                break;

            default:
                // Ignore input
                break;
        }
    }

    public void OnExit(GameState state)
    {
        // Should I be doing something?
    }

    // ...
}
```

The game manager which controls the game state would need to call these methods on all entities that implement our interface.

```csharp
public partial class TheOrangeRunGameManager : GameManager
{
    public GameState State { get; private set; }

    private void _ChangeState(GameState state)
    {
        // Make a copy just in case entities are spawned or destroyed
        foreach ( var stateTransitioner in All.OfType<IStateTransitioner>().ToList() )
            stateTransitioner.OnExit(State);

        State = state;

        // Same as before,
        // Make sure only existing entities at this point get their methods called.
        // Entities spawned during the exit of the previous state will be considered,
        // while destroyed entities will no longer be picked.
        foreach ( var stateTransitioner in All.OfType<IStateTransitioner>().ToList() )
            stateTransitioner.OnEntry(State);
    }

    // ...
}
```

This is rudimentary, but does the trick. There are better alternatives such as the [event system](https://wiki.facepunch.com/sbox/EventSystem) in [sbox](https://sbox.facepunch.com). More on this later.

With this approach, the code becomes modular. Each entity is a module and is responsible for its state and behaviour. Adding new entities, or modules, is easy as nothing extra needs to be done other than plug it to the state-machine.

Debugging should be easy as we can follow each mutation an entity goes through as the game progresses, the entire logic for how state transitions occur is contained inside the game manager.

This is an excellent example of [Dependency Inversion](https://en.wikipedia.org/wiki/Dependency_inversion_principle). We no longer have references to concrete entities (and mutate them), we reference objects implementing an interface and call their methods. We do not know the concrete type of the implementors, we only know they can perform these actions.

### The Game Loop

I've made references to this a number of times. The Orange Run is a looped game meaning that you start in the lobby, then you get a chance to run for the oranges and finally, we find out who has collected the most oranges.

Once this is done, we get back to the lobby. If you join after the game has started, no longer in the lobby, you have to wait until the current loop ends.

Below is the game loop described.

1. **Starting** (initial Game Manager state), orange spanwers are... spawned, pawn spawners are placed along side the soundscape entity. This is a one-time state as the game is being initialized.
2. **Lobby**, a 7 + [3, 5] second wait period is added. The [3, 5] bit is a random number between the two.
3. **The Orange Run**, 120 seconds, or 2 minutes, of running around
4. **Leaderboards**, 20 seconds of admiring the winner

Once the Leaderboards time elapses, the game manager moves back to Lobby where a new run will be started. All collected oranges are cleared and all pawns are placed back at spawn.

### Events

As stated previously, the interface approach works, the drawback is that we need to go through all entities and check if they implement a particular interface and if this is the case call some methods.

It works, but it is rudimentary, we have to check everything and it does not account for other types of objects that should react to state transitions.

[Sbox](https://sbox.facepunch.com) comes with an [event system](https://wiki.facepunch.com/sbox/EventSystem) that is quite powerful and it would make for a much better alternative.

We already have a number of [`GameEvent`](https://asset.party/api/Sandbox.GameEvent)s that allow us to plug into different events and react to what the game is doing. We can extend these events with events of our own and use the exact same mechanism for state transitions as well.

We can define constants that descibe the event names and reuse these, similar to how we did for assets. Or, in addition to this, we can define attributes that allow us to easily select the events we are interested in.

```csharp
// A container class, nothing more. We can use partial classes as well.
public static class TheOrangeRunEvent
{
    // Make sure we don't interfere with sbox events,
    // we will prefix all of our custom events with this value.
    private const string Prefix = "TheOrangeRun";

    public static class GameState
    {
        // Beautiful constants, this value is determined at compile-time,
        // no concatenation of string at run-time
        public const string Changed = nameof( Prefix ) + "." + nameof( GameState ) + "." + nameof( Changed );

        // A general event attribute that notifies us when the state has changed.
        // Unsure where this would be used, but still, it's nice to have.
        [MethodArguments( typeof( TheOrangeRun.GameState ) )]
        public class ChangedAttribute : EventAttribute
        {
            public ChangedAttribute()
                : base( Changed )
            {
            }
        }

        // This is one of the game states, still a container class.
        public static class Starting
        {
            public const string Entry = nameof( Prefix ) + "." + nameof( GameState ) + "." + nameof( Starting ) + "." + nameof( Entry );
            public const string Leave = nameof( Prefix ) + "." + nameof( GameState ) + "." + nameof( Starting ) + "." + nameof( Leave );

            // Finally, the attribute delcarations.
            public class EntryAttribute : EventAttribute
            {
                public EntryAttribute()
                    : base( Entry )
                {
                }
            }

            public class LeaveAttribute : EventAttribute
            {
                public LeaveAttribute()
                    : base( Leave )
                {
                }
            }
        }
    }
}
```

This approach can seem a bit convoluted, there probably are better ways of writing this. The intent is to get a container class for all game events for The Orange Run game that are separate from the [sbox](https://sbox.facepunch.com) [`GameEvent`](https://asset.party/api/Sandbox.GameEvent)s while following a similar patterns.

The above allows us to subscribe to events using the two following approaches.

```csharp
public partial class Pawn : AnimatedEntity
{
    [TheOrangeRunEvent.GameState.Changed]
    // or [Event(TheOrangeRunEvent.GameState.Changed)]
    protected void OnGameStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.OrangeRun:
                // Enable input
                break;

            default:
                // Ignore input
                break;
        }
    }

    // ...
}
```

Or be more specific and pick the transition triggers making code more clear.

```csharp
public partial class Pawn : AnimatedEntity
{
    [TheOrangeRunEvent.GameState.OrangeRun.Entry]
    protected void OnOrangeRunEntry()
    {
        // Enable input
    }

    [TheOrangeRunEvent.GameState.OrangeRun.Leave]
    protected void OnOrangeRunLeave()
    {
        // Ignore input
    }

    // ...
}
```

The reason the two approaches are interchangeable is because we have declared both constants and attribute types and because of something that the C# compiler does for us.

Althout the attribute type name ends in "Attribute", we do not need to specify this for C# to know we are referencing an attribute. When it compiles `[TheOrangeRunEvent.GameState.OrangeRun.Entry]` it actually looks up `[TheOrangeRunEvent.GameState.OrangeRun.EntryAttribute]`. We can specify the full name and it will still work.

It's just clear that decorating members like this can only be done only through attributes thus is no longer necessary to say that an attribute is an "Attribute". For more info check [Attributes](https://learn.microsoft.com/dotnet/csharp/advanced-topics/reflection-and-attributes).

### Conclusions

With this approach I can extend the standard events with events of my own and have everything work through the same infrastructure. My entities are pluggable and are contained inside one module.

Each module is responsible for its state and how it reacts to game state changes. This allows them to encapsulate their state and not have it set from anywhere else, it is more safe as only specific objects can mutate this state.

Last but not least, this can scale well as we can keep adding entities and just have the plug into the game events including game state transition events.

This does not mean there can be no performance impact, it depends on how much code runs during each transition, but we generally optimise when it is needed and when we 1st implement something we look at the alternatives that are available and pick the one that works best.
