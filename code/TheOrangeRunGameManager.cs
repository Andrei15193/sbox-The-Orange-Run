
using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using TheOrangeRun.Oranges;
using TheOrangeRun.Pawns;
using TheOrangeRun.UI;

namespace TheOrangeRun;

public partial class TheOrangeRunGameManager : GameManager
{
    public static new TheOrangeRunGameManager Current
    {
        get => (TheOrangeRunGameManager)GameManager.Current;
        protected set => GameManager.Current = value;
    }

    public TheOrangeRunGameManager()
    {
        if ( Game.IsClient )
            Game.RootPanel = new Hud();
    }

    public IEnumerable<PawnSpawnPoint> SpawnPoints { get; private set; }

    [Net]
    public IList<Pawn> PlayerPawns { get; set; }

    public IEnumerable<OrangeSpawner> OrangeSpawners { get; private set; }

    public IEnumerable<OrangeCollector> OrangeCollectors { get; private set; }

    public const float DefaultSpawnDelayInSeconds = 90;

    public SoundscapeRadiusEntity AmbianceSoundscape { get; private set; }

    [GameEvent.Entity.PostSpawn]
    internal void MakeWorkd()
    {
        InitializeSoundScapes();
        InitializeSpawnPoints();
        InitializeOrangeSpawners();
        InitializeOrangeCollectors();

        Event.Run( _stateEventNames[_state].EntryEventName );
        Event.Run( TheOrangeRunEvent.GameState.Changed, _state );

        GameLoop();
    }

    private void InitializeSoundScapes()
    {
        AmbianceSoundscape = new SoundscapeRadiusEntity
        {
            Soundscape = Sounds.Soundscapes.Ambiance,
            Position = new Vector3( 985f, 2220f, 0f ),
            Radius = 230
        };
    }

    private void InitializeSpawnPoints()
    {
        SpawnPoints = new[]
        {
            new PawnSpawnPoint
            {
                Position = new Vector3( 1120.9475f, 2706.1333f, 0.1f )
            },
            new PawnSpawnPoint
            {
                Position = new Vector3( 1124.2898f, 2506.3325f, 0.1f )
            },
            new PawnSpawnPoint
            {
                Position = new Vector3( 971.0964f, 2494.413f, 0.1f )
            },
            new PawnSpawnPoint
            {
                Position = new Vector3( 1037.3136f, 2612.2134f, 0.1f )
            },
            new PawnSpawnPoint
            {
                Position = new Vector3( 951.96136f, 2688.0115f, 0.1f )
            },
            new PawnSpawnPoint
            {
                Position = new Vector3( 837.3842f, 2614.965f, 0.1f )
            },
            new PawnSpawnPoint
            {
                Position = new Vector3( 887.5841f, 2453.0503f, 0.1f )
            },
            new PawnSpawnPoint
            {
                Position = new Vector3( 745.8751f, 2431.1465f, 0.1f )
            },
            new PawnSpawnPoint
            {
                Position = new Vector3( 701.36774f, 2553.2644f, 0.1f )
            },
            new PawnSpawnPoint
            {
                Position = new Vector3( 791.76154f, 2686.722f, 0.1f )
            },
            new PawnSpawnPoint
            {
                Position = new Vector3( 669.2805f, 2689.2905f, 0.1f )
            },
            new PawnSpawnPoint
            {
                Position = new Vector3( 569.73706f, 2581.7915f, 0.1f )
            },
            new PawnSpawnPoint
            {
                Position = new Vector3( 598.16296f, 2471.4714f, 0.1f )
            },
            new PawnSpawnPoint
            {
                Position = new Vector3( 938.5091f, 2588.3008f, 0.1f )
            },
            new PawnSpawnPoint
            {
                Position = new Vector3( 825.6168f, 2514.893f, 0.1f )
            },
            new PawnSpawnPoint
            {
                Position = new Vector3( 566.36945f, 2686.0105f, 0.1f )
            }
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
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( 766.1073f, 1828.478f, 0.0052f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( 990.0706f, 1828.974f, 0.0052f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( 1214.034f, 1829.47f, 0.0052f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( 1437.997f, 1829.9656f, 0.0052121878f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },

            // Spawn building side
            new OrangeSpawner
            {
                Position = new Vector3( 1533.5865f, 2206.0703f, 0.00390625f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( 1529.157f, 2428.796f, 0.0039f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( 1524.7279f, 2651.5222f, 0.00390625f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },

            // Stairs back
            new OrangeSpawner
            {
                Position = new Vector3( -952.05084f, 2200.0525f, 32.003906f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( -1107.999f , 2200.052f ,32.0046f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( -1263.9476f, 2200.0508f, 32.00521f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( -1263.948f, 2394.016f, 32.0057f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( -1263.949f, 2587.982f, 32.0061f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( -1263.9493f, 2781.9475f, 32.00652f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( -1049.975f, 2784.948f, 32.0062f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( -836.0004f, 2787.947f, 32.0059f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( -622.0259f, 2790.947f, 32.0055f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( -408.05142f, 2793.9473f, 32.00521f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( -408.5912f, 2610.1138f, 32.00652f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( -771.5082f, 2617.863f, 32.00652f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( -953.5383f, 2436.1763f, 32.00652f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( -319.8182f, 2771.487f, 0.00390625f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( -190.21806f, 2773.5432f, 0.00390625f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },

            // Fence
            new OrangeSpawner
            {
                Position = new Vector3( -954.5934f, 1940.3035f, 0.00390625f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( -824.1599f, 1940.651f, 0.0039f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( -693.7263f, 1940.998f, 0.0039f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( -563.2928f, 1941.345f, 0.0039f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( -432.8593f, 1941.692f, 0.0039f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( -302.4258f, 1942.039f, 0.0039f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( -171.9922f, 1942.386f, 0.0039f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( -41.5587f, 1942.733f, 0.0039f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( 88.8749f, 1943.08f, 0.0039f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( 219.3083f, 1943.427f, 0.0039f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( 349.7418f, 1943.775f, 0.0039f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( 480.17535f, 1944.1217f, 0.00390625f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },

            // Roof line 1
            new OrangeSpawner
            {
                Position = new Vector3( 1098.9215f, 2367.8647f, 411.0065f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( 875.4347f, 2440.205f, 410.4062f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( 651.9479f, 2512.544f, 409.806f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( 428.4611f, 2584.884f, 409.2057f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( 204.9742f, 2657.224f, 408.6055f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( -18.51253f, 2729.564f, 408.00522f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },

            // Roof line 2
            new OrangeSpawner
            {
                Position = new Vector3( 1099.668f, 2550.491f, 411.0065f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( 876.0988f, 2549.479f, 410.4062f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( 652.5296f, 2548.466f, 409.806f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( 428.9604f, 2547.454f, 409.2057f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( 205.3912f, 2546.441f, 408.6054f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( -18.178f, 2545.429f, 408.0052f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },

            // Roof line 3
            new OrangeSpawner
            {
                Position = new Vector3( 1100.4154f, 2733.117f, 411.00653f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( 876.7637f, 2658.752f, 410.4063f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( 653.1119f, 2584.387f, 409.806f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( 429.4601f, 2510.023f, 409.2057f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( 205.8083f, 2435.658f, 408.6055f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( -17.843397f, 2361.2935f, 408.00522f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },

            // Roof hard
            new OrangeSpawner
            {
                Position = new Vector3( 2922.2854f, 1678.3168f, 512.0039f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( 3530.787f, 1679.786f, 512.0039f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( 3327.953f, 1679.297f, 512.0039f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( 3125.119f, 1678.807f, 512.0039f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( 3733.6204f, 1680.2764f, 512.0039f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },

            new OrangeSpawner
            {
                Position = new Vector3( 3732.9678f, 1811.2424f, 512.0039f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( 3528.889f, 1814.741f, 512.0042f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( 3324.811f, 1818.24f, 512.0045f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( 3120.732f, 1821.74f, 512.0049f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( 2916.6533f, 1825.2386f, 512.0052f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },

            // Scaffolding
            new OrangeSpawner
            {
                Position = new Vector3( 2703.1274f, 1426.0828f, 516.64655f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( 2700.09f, 1424.366f, 388.6465f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( 2697.053f, 1422.65f, 260.6465f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( 2694.0154f, 1420.9337f, 132.64653f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( 2435.097f, 1421.3767f, 516.64655f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( 2424.096f, 1420.356f, 388.6465f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( 2413.095f, 1419.335f, 260.6465f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
            new OrangeSpawner
            {
                Position = new Vector3( 2402.0945f, 1418.314f, 132.64651f ),
                DelayInSeconds = DefaultSpawnDelayInSeconds
            },
        };
    }

    private void InitializeOrangeCollectors()
    {
        OrangeCollectors = new[]
        {
            new OrangeCollector
            {
                Position = new Vector3( 622.17236f, 585.5672f, 0 ),
                Rotation = new Rotation(Vector3.Up, (float)Math.PI / 3)
            },
            new OrangeCollector
            {
                Position = new Vector3( 879.7922f, 735.0467f, 0 ),
                Rotation = new Rotation(Vector3.Up, (float)Math.PI / 7)
            },
            new OrangeCollector
            {
                Position = new Vector3( 759.40985f, 443.0306f, 0 ),
                Rotation = new Rotation(Vector3.Up, (float)Math.PI / 13)
            }
        };
    }

    [Net]
    private GameState _state { get; set; }
    private readonly IReadOnlyDictionary<GameState, (string EntryEventName, string LeaveEventName)> _stateEventNames = new Dictionary<GameState, (string EntryEventName, string LeaveEventName)>
    {
        { GameState.Starting, (TheOrangeRunEvent.GameState.Starting.Entry, TheOrangeRunEvent.GameState.Starting.Leave) },
        { GameState.Lobby, (TheOrangeRunEvent.GameState.Lobby.Entry, TheOrangeRunEvent.GameState.Lobby.Leave) },
        { GameState.OrangeRun, (TheOrangeRunEvent.GameState.OrangeRun.Entry, TheOrangeRunEvent.GameState.OrangeRun.Leave) },
        { GameState.Leaderboard, (TheOrangeRunEvent.GameState.Leaderboards.Entry, TheOrangeRunEvent.GameState.Leaderboards.Leave) }
    };

    public GameState State
    {
        get => _state;
        private set
        {
            if ( value != _state )
            {
                Event.Run( _stateEventNames[_state].LeaveEventName );
                _state = value;
                Event.Run( _stateEventNames[_state].EntryEventName );

                Event.Run( TheOrangeRunEvent.GameState.Changed, _state );
            }
        }
    }

    internal async void GameLoop()
    {
        foreach ( var orangeSpawner in OrangeSpawners )
            orangeSpawner.IsActive = true;

        do
        {
            State = GameState.Lobby;
            ChatBox.Say( "We are now in the lobby, waiting... for the game to start..." );
            await GameTask.DelayRealtimeSeconds( 7 );
            ChatBox.Say( "The game is going to start any time now..." );
            await GameTask.DelayRealtimeSeconds( Random.Shared.Int( 3, 5 ) );

            Sound.FromScreen( Sounds.Events.TheOrangeRunStarted );
            State = GameState.OrangeRun;
            ChatBox.Say( "Grab those oranges!" );
            await GameTask.DelayRealtimeSeconds( 60 );
            ChatBox.Say( "You're on the clock here! Only 60 seconds left!" );
            await GameTask.DelayRealtimeSeconds( 30 );
            ChatBox.Say( "30 seconds left..." );
            await GameTask.DelayRealtimeSeconds( 20 );
            for ( var secondsLeft = 10; secondsLeft > 1; secondsLeft-- )
            {
                ChatBox.Say( $"{secondsLeft} seconds left..." );
                await GameTask.DelayRealtimeSeconds( 1 );
            }
            ChatBox.Say( $"1 second left..." );
            await GameTask.DelayRealtimeSeconds( 1 );

            State = GameState.Leaderboard;
            Sound.FromScreen( Sounds.Events.TheOrangeRunEnded );
            var winner = PlayerPawns
                .OrderByDescending( pawn => pawn.CollectedOrangesCount )
                .ThenBy( _ => Guid.NewGuid() )
                .Select( pawn => $"{pawn.Client.Name}!" )
                .DefaultIfEmpty( "there's nobody here!" )
                .First();
            ChatBox.Say( $"We're in the endgame now... oh wait, it's over! And the winner is... {winner}" );
            await GameTask.DelayRealtimeSeconds( 20 );

        } while ( true );
    }

    public override void ClientJoined( IClient client )
    {
        base.ClientJoined( client );

        var spawnPoint = SpawnPoints
            .Where( spawnPoint => spawnPoint.Pawn is null )
            .OrderBy( _ => Guid.NewGuid() )
            .First();

        var pawn = new Pawn
        {
            SpawnPoint = spawnPoint
        };
        client.Pawn = pawn;
        spawnPoint.Pawn = pawn;
        pawn.Respawn();
        pawn.DressFromClient( client );

        PlayerPawns.Add( pawn );

        if ( State == GameState.Lobby )
            ChatBox.Say( client.Name + " has joined the game..." );
        else
            ChatBox.Say( client.Name + " has joined the game while it is in progress, it will take a bit to complete" );

        pawn.Position = pawn.SpawnPoint.Position;
    }

    public override void ClientDisconnect( IClient client, NetworkDisconnectionReason reason )
    {
        if ( client.Pawn is Pawn pawn )
        {
            PlayerPawns.Remove( pawn );
            foreach ( var spawnPoint in SpawnPoints )
                if ( spawnPoint.Pawn == pawn )
                {
                    spawnPoint.Pawn = null;
                    pawn.SpawnPoint = null;
                }
        }

        base.ClientDisconnect( client, reason );
    }

#if DEBUG
    [Event.Hotload]
    public void OnHotReload()
    {
        Log.Info( "Hot reload" );

        if ( Game.IsServer )
        {
            foreach ( var orangeSpawner in All.OfType<OrangeSpawner>().Except( OrangeSpawners ).ToList() )
                orangeSpawner.Delete();
            foreach ( var orange in All.OfType<Orange>().Except( OrangeSpawners.Select( orangeSpawner => orangeSpawner.Orange ) ).ToList() )
                orange.Delete();

            var moreOrangeSpawners = new OrangeSpawner[]
            {
            };

            foreach ( var orangeSpanwer in moreOrangeSpawners )
                orangeSpanwer.IsActive = true;
        }
    }
#endif
}
