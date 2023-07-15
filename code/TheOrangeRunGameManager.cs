
using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using TheOrangeRun.Oranges;
using TheOrangeRun.UI;

namespace TheOrangeRun;

/// <summary>
/// This is your game class. This is an entity that is created serverside when
/// the game starts, and is replicated to the client. 
/// 
/// You can use this to create things like HUDs and declare which player class
/// to use for spawned players.
/// </summary>
public partial class TheOrangeRunGameManager : GameManager
{
    public static new TheOrangeRunGameManager Current
    {
        get => (TheOrangeRunGameManager)GameManager.Current;
        protected set => GameManager.Current = value;
    }

    public static Hud RootPanel
        => (Hud)Game.RootPanel;

    public IEnumerable<OrangeSpawner> OrangeSpawners { get; private set; }

    /// <summary>
    /// Called when the game is created (on both the server and client)
    /// </summary>
    public TheOrangeRunGameManager()
    {
        if ( Game.IsClient )
            Game.RootPanel = new Hud();
    }

    public const float DefaultSpawnDelay = 1;

    public SoundscapeRadiusEntity AmbianceSoundscape { get; private set; }

    [GameEvent.Entity.PostSpawn]
    internal void MakeWorkd()
    {
        Log.Info( nameof( TheOrangeRunGameManager ) + "." + nameof( MakeWorkd ) );

        InitializeSoundScapes();
        InitializeOrangeSpawners();
        StartGame();
    }

    private void InitializeSoundScapes()
    {
        AmbianceSoundscape = new SoundscapeRadiusEntity
        {
            Soundscape = "assets/sounds/mixkit/ambiance.sndscape",
            Position = new Vector3( 985f, 2220f, 0f ),
            Radius = 230
        };
    }

    private void InitializeOrangeSpawners()
    {
        OrangeSpawners = new[]
        {
            // Spawn building front
            new OrangeSpawner
            {
                Position = new Vector3( 542.1441f, 1827.9827f, 0.0052121878f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( 766.1073f, 1828.478f, 0.0052f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( 990.0706f, 1828.974f, 0.0052f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( 1214.034f, 1829.47f, 0.0052f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( 1437.997f, 1829.9656f, 0.0052121878f ),
                DelayInSeconds = DefaultSpawnDelay
            },

            // Spawn building side
            new OrangeSpawner
            {
                Position = new Vector3( 1533.5865f, 2206.0703f, 0.00390625f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( 1529.157f, 2428.796f, 0.0039f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( 1524.7279f, 2651.5222f, 0.00390625f ),
                DelayInSeconds = DefaultSpawnDelay
            },

            // Stairs back
            new OrangeSpawner
            {
                Position = new Vector3( -952.05084f, 2200.0525f, 32.003906f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( -1107.999f , 2200.052f ,32.0046f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( -1263.9476f, 2200.0508f, 32.00521f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( -1263.948f, 2394.016f, 32.0057f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( -1263.949f, 2587.982f, 32.0061f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( -1263.9493f, 2781.9475f, 32.00652f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( -1049.975f, 2784.948f, 32.0062f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( -836.0004f, 2787.947f, 32.0059f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( -622.0259f, 2790.947f, 32.0055f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( -408.05142f, 2793.9473f, 32.00521f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( -408.5912f, 2610.1138f, 32.00652f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( -771.5082f, 2617.863f, 32.00652f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( -953.5383f, 2436.1763f, 32.00652f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( -319.8182f, 2771.487f, 0.00390625f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( -190.21806f, 2773.5432f, 0.00390625f ),
                DelayInSeconds = DefaultSpawnDelay
            },

            // Fence
            new OrangeSpawner
            {
                Position = new Vector3( -954.5934f, 1940.3035f, 0.00390625f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( -824.1599f, 1940.651f, 0.0039f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( -693.7263f, 1940.998f, 0.0039f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( -563.2928f, 1941.345f, 0.0039f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( -432.8593f, 1941.692f, 0.0039f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( -302.4258f, 1942.039f, 0.0039f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( -171.9922f, 1942.386f, 0.0039f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( -41.5587f, 1942.733f, 0.0039f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( 88.8749f, 1943.08f, 0.0039f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( 219.3083f, 1943.427f, 0.0039f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( 349.7418f, 1943.775f, 0.0039f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( 480.17535f, 1944.1217f, 0.00390625f ),
                DelayInSeconds = DefaultSpawnDelay
            },

            // Roof line 1
            new OrangeSpawner
            {
                Position = new Vector3( 1098.9215f, 2367.8647f, 411.0065f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( 875.4347f, 2440.205f, 410.4062f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( 651.9479f, 2512.544f, 409.806f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( 428.4611f, 2584.884f, 409.2057f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( 204.9742f, 2657.224f, 408.6055f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( -18.51253f, 2729.564f, 408.00522f ),
                DelayInSeconds = DefaultSpawnDelay
            },

            // Roof line 2
            new OrangeSpawner
            {
                Position = new Vector3( 1099.668f, 2550.491f, 411.0065f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( 876.0988f, 2549.479f, 410.4062f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( 652.5296f, 2548.466f, 409.806f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( 428.9604f, 2547.454f, 409.2057f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( 205.3912f, 2546.441f, 408.6054f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( -18.178f, 2545.429f, 408.0052f ),
                DelayInSeconds = DefaultSpawnDelay
            },

            // Roof line 3
            new OrangeSpawner
            {
                Position = new Vector3( 1100.4154f, 2733.117f, 411.00653f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( 876.7637f, 2658.752f, 410.4063f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( 653.1119f, 2584.387f, 409.806f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( 429.4601f, 2510.023f, 409.2057f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( 205.8083f, 2435.658f, 408.6055f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( -17.843397f, 2361.2935f, 408.00522f ),
                DelayInSeconds = DefaultSpawnDelay
            },

            // Roof hard
            new OrangeSpawner
            {
                Position = new Vector3( 2922.2854f, 1678.3168f, 512.0039f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( 3530.787f, 1679.786f, 512.0039f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( 3327.953f, 1679.297f, 512.0039f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( 3125.119f, 1678.807f, 512.0039f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( 3733.6204f, 1680.2764f, 512.0039f ),
                DelayInSeconds = DefaultSpawnDelay
            },

            new OrangeSpawner
            {
                Position = new Vector3( 3732.9678f, 1811.2424f, 512.0039f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( 3528.889f, 1814.741f, 512.0042f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( 3324.811f, 1818.24f, 512.0045f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( 3120.732f, 1821.74f, 512.0049f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( 2916.6533f, 1825.2386f, 512.0052f ),
                DelayInSeconds = DefaultSpawnDelay
            },

            // Scaffolding
            new OrangeSpawner
            {
                Position = new Vector3( 2703.1274f, 1426.0828f, 516.64655f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( 2700.09f, 1424.366f, 388.6465f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( 2697.053f, 1422.65f, 260.6465f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( 2694.0154f, 1420.9337f, 132.64653f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( 2435.097f, 1421.3767f, 516.64655f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( 2424.096f, 1420.356f, 388.6465f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( 2413.095f, 1419.335f, 260.6465f ),
                DelayInSeconds = DefaultSpawnDelay
            },
            new OrangeSpawner
            {
                Position = new Vector3( 2402.0945f, 1418.314f, 132.64651f ),
                DelayInSeconds = DefaultSpawnDelay
            },
        };
    }

    internal void StartGame()
    {
        foreach ( var orangeSpawner in OrangeSpawners )
            orangeSpawner.IsActive = true;
    }

    [Net]
    public IList<PawnStats> PawnsStats { get; private set; }

    public void CollectOrange( Orange orange, Pawn pawn )
    {
        if ( Game.IsServer && orange.IsValid && pawn.IsValid )
        {
            var pawnStats = PawnsStats.FirstOrDefault( pawnStats => pawnStats.Id == pawn.Client.Id );
            if ( pawnStats is not null )
            {
                pawnStats.TotalOranges++;
                orange.Delete();
            }
        }
    }

    /// <summary>
    /// A client has joined the server. Make them a pawn to play with
    /// </summary>
    public override void ClientJoined( IClient client )
    {
        base.ClientJoined( client );
        PawnsStats.Add( new PawnStats { Id = client.Id, Name = client.Name, TotalOranges = 0 } );

        ChatBox.Say( client.Name + " has joined the game..." );

        // Create a pawn for this client to play with
        var pawn = new Pawn();
        client.Pawn = pawn;
        pawn.Respawn();
        pawn.DressFromClient( client );

        // Get all of the spawnpoints
        var spawnpoints = All.OfType<SpawnPoint>();

        // chose a random one
        var randomSpawnPoint = spawnpoints.OrderBy( x => Guid.NewGuid() ).FirstOrDefault();

        // if it exists, place the pawn there
        if ( randomSpawnPoint != null )
        {
            var tx = randomSpawnPoint.Transform;
            tx.Position = tx.Position + Vector3.Up * 50.0f; // raise it up
            pawn.Transform = tx;
        }
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
}
