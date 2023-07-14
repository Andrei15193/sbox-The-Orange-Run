---
title: Touching, No Touching
---

A central element to this game is the Orange, which is a pickup item. For this we will use
the collision system that we have in [sbox](https://sbox.facepunch.com).

This took me a bit to figure out and I could not have done it as quickly if it wasn't for
the community help. They are quick to answer questions and can get you going in the right
direction, that has been my experience so far and it's quite awesome.

Before we continue, a quick word about tags. All entities in [sbox](https://sbox.facepunch.com)
can be tagged and this is later on used to determine what should happen when two entities
come into contact with one another. Tags are used for traces and probably a bunch of other
things as well.

Tagging is essential in [sbox](https://sbox.facepunch.com), whenever two entities come into
contract the following rules are applied to determine what should happen, This information
is available on the collision settings in the editor.

* If there's a matching pair then apply the configred collision
* If there are multiple matches then the least colliding option is used
* If there are no matches, then the tag default is used
* If there are multiple default values, the the least colliding option is used
* If there are no defaults and one of the entities **does not have any tags**, then the default is to **collide**

The last rule can be a bit tricky as the template does not tag anything implicitly, meaning
that the player pawn will be colliding with any other object that has collisions enabled.

The first step is to configure collisions for the player pawn.

```csharp
public partial class Pawn : AnimatedEntity
{
    public override void Spawn()
    {
        SetModel( "models/citizen/citizen.vmdl" );
        SetupPhysicsFromModel( PhysicsMotionType.Keyframed );
        // Or anything else.
        // If an entity has no tags then it will default to collide.
        Tags.Add( "player" );
    }
}
```

The next step is to configure our orange so that it detects when it gets touched. Whenever
a player pawn touches an orange they pick it up.

Unfortunatelly, the model we are using does not have collisions configured and it doesn't
work with the same settings, but we can use the crate to test these settings.

```csharp
public class Orange : ModelEntity
{
    public override void Spawn()
    {
        base.Spawn();
        // Not all models have physics defined.
        // In that case cylinder or capsule physics can work
        SetModel( "models/citizen_props/crate01.vmdl" );
        SetupPhysicsFromModel( PhysicsMotionType.Keyframed );
        Tags.Add( "trigger" );

        EnableTouch = true;
    }

    public override void StartTouch( Entity other )
    {
        base.StartTouch( other );
        Log.Info( "StartTouch" );
    }

    public override void Touch( Entity other )
    {
        base.Touch( other );
        Log.Info( "Touch" );
    }

    public override void EndTouch( Entity other )
    {
        base.EndTouch( other );
        Log.Info( "EndTouch" );
    }
}
```

These are the minimum settings that I know of to enable collisions, the orange allows us to
pass through it. This makes sense as we are going to pickup items and we don't really need
to _hit_ these items, but rather detect when we are in bounds so we can pick them up.

Now to change our model to the orange and setup the collisions using a cylinder. This should
give us the range from where we can pickup the item.

```csharp
public override void Spawn()
{
    base.Spawn();
    Model = Cloud.Model( "rust.orange" );
    Scale = 2;
    SetupPhysicsFromCylinder( PhysicsMotionType.Keyframed, new Capsule( Vector3.Zero, Vector3.Up * 5, 5 ) );
    Tags.Add( "trigger" );

    EnableTouch = true;
}
```

Next step is to actually collect the orange once we come in contact with it. Similar to how we
add oranges when we click the mouse button, we can call a method on the game manager that will
collect the orange for the player.

```csharp
public override void StartTouch( Entity other )
{
    base.StartTouch( other );

    if (other is Pawn pawn )
        TheOrangeRunGameManager.Current.CollectOrange( this, pawn);
}
```

Once we collect an item, we should spawn another one to we can keep running around and test
how this is working for us.

```csharp
public partial class TheOrangeRunGameManager : GameManager
{
    public void CollectOrange( Orange orange, Pawn pawn)
    {
        if ( Game.IsServer && orange.IsValid && pawn.IsValid)
        {
            var pawnStats = PawnsStats.FirstOrDefault( pawnStats => pawnStats.Id == pawn.Client.Id );
            if ( pawnStats is not null )
            {
                pawnStats.TotalOranges++;
                orange.Delete();

                var orangeSpawnPoint = All.OfType<SpawnPoint>().OrderBy( _ => Guid.NewGuid() ).First();
                var newOrange = new Orange
                {
                    Position = orangeSpawnPoint.Position + Vector3.Up * 20
                };
            }
        }
    }
}
```

Networked entities need to be managed on the server, from what I gather these are entities
that everyone can see, they are not just on the client. This makes sense as we need to remove
the entity for everyone, not just for the player that collects it.

There are probably multiple ways of doing this, one of which is using [BaseTrigger](https://asset.party/api/Sandbox.BaseTrigger).
This got recommended to me by someone in the community and at a first glace it looks like
it's doing the same configuraitons that I made and it adds a bunch of features to select who
can interact with an entity (who can pick up the orange) and get notified when it happens.

This is also done by using tags, as mentioned before, they are a critical component of
[sbox](https://sbox.facepunch.com) and a lot seems to be built on them. This is really cool
because with a clear configuration of tags they become an extremely powerful tool and we
won't need to check for different types of entities, we only check the tags and continue
from there.
