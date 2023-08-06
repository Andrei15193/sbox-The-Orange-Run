using Sandbox;
using Sandbox.UI;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using TheOrangeRun.Oranges;

namespace TheOrangeRun.Pawns;

public partial class Pawn : AnimatedEntity
{
    private static readonly IReadOnlyDictionary<int, string> _dropMessageFragments = new Dictionary<int, string>
    {
        { 1, "an orange" },
        { 2, "two oranges" },
        { 3, "three oranges" },
        { 4, "four oranges" },
        { 5, "five oranges" },
        { 6, "half a dozen oranges" },
        { 7, "seven oranges" },
        { 8, "eight oranges" },
        { 9, "nine oranges" },
        { 10, "a full bag of oranges" }
    };

    private bool _fallsEndlessly = false;

    public bool IsScoreScreenVisible { get; set; }

    [Net]
    public int OrangeCarryCount { get; set; }

    public int MaximumOrangeCarryCount
        => 10;

    public int RespawnOrangeCount
        => 3;

    [Net]
    public int CollectedOrangesCount { get; set; }

    [ClientInput]
    public Vector3 InputDirection { get; set; }

    [ClientInput]
    public Angles ViewAngles { get; set; }

    /// <summary>
    /// Position a player should be looking from in world space.
    /// </summary>
    [Browsable( false )]
    public Vector3 EyePosition
    {
        get => Transform.PointToWorld( EyeLocalPosition );
        set => EyeLocalPosition = Transform.PointToLocal( value );
    }

    /// <summary>
    /// Position a player should be looking from in local to the entity coordinates.
    /// </summary>
    [Net, Predicted, Browsable( false )]
    public Vector3 EyeLocalPosition { get; set; }

    /// <summary>
    /// Rotation of the entity's "eyes", i.e. rotation for the camera when this entity is used as the view entity.
    /// </summary>
    [Browsable( false )]
    public Rotation EyeRotation
    {
        get => Transform.RotationToWorld( EyeLocalRotation );
        set => EyeLocalRotation = Transform.RotationToLocal( value );
    }

    /// <summary>
    /// Rotation of the entity's "eyes", i.e. rotation for the camera when this entity is used as the view entity. In local to the entity coordinates.
    /// </summary>
    [Net, Predicted, Browsable( false )]
    public Rotation EyeLocalRotation { get; set; }

    public BBox Hull
    {
        get => new
        (
            new Vector3( -16, -16, 0 ),
            new Vector3( 16, 16, 64 )
        );
    }

    [BindComponent]
    public PawnController Controller { get; }

    [BindComponent]
    public PawnAnimator Animator { get; }

    [BindComponent]
    public PawnCamera Camera { get; }

    public override Ray AimRay
        => new( EyePosition, EyeRotation.Forward );

    public PawnSpawnPoint SpawnPoint { get; set; }

    /// <summary>
    /// Called when the entity is first created 
    /// </summary>
    public override void Spawn()
    {
        base.Spawn();
        Tags.Add( "player" );

        SetModel( "models/citizen/citizen.vmdl" );
        SetupPhysicsFromModel( PhysicsMotionType.Keyframed );

        EnableTouch = true;
        EnableDrawing = true;
        EnableHideInFirstPerson = true;
        EnableShadowInFirstPerson = true;
    }

    public void Respawn()
    {
        Components.Create<PawnController>();
        Components.Create<PawnAnimator>();
        Components.Create<PawnCamera>();
    }

    public void DressFromClient( IClient cl )
    {
        var c = new ClothingContainer();
        c.LoadFromClient( cl );
        c.DressEntity( this );
    }

    public override void Simulate( IClient cl )
    {
        if ( Position.z < -7000f )
        {
            if ( Game.IsServer && !_fallsEndlessly )
                if ( CollectedOrangesCount >= RespawnOrangeCount )
                {
                    if ( OrangeCarryCount > 0 )
                        ChatBox.Say( $"{cl.Name} dropped the ball and lost all the oranges they were carrying." );

                    OrangeCarryCount = 0;
                    CollectedOrangesCount -= RespawnOrangeCount;
                    Position = SpawnPoint.Position;
                }
                else
                {
                    _fallsEndlessly = true;
                    ChatBox.Say( $"{cl.Name} didn't drop any oranges before doing something daring and now falls for eternity." );
                }
        }
        else
        {
            SimulateRotation();
            if ( !_fallsEndlessly )
                Controller?.Simulate( cl );
            Animator?.Simulate();

#if DEBUG
            if ( Game.IsServer && Input.Pressed( "attack1" ) )
            {
                Log.Info( "Add spawner" );
                new OrangeSpawner
                {
                    Position = Position,
                    DelayInSeconds = 1,
                    IsActive = true
                };
            }
#endif
        }
    }

    public override void Touch( Entity other )
    {
        base.Touch( other );

        if ( TheOrangeRunGameManager.Current.State == GameState.OrangeRun )
            switch ( other )
            {
                case Orange _ when Game.IsServer && OrangeCarryCount < MaximumOrangeCarryCount:
                    OrangeCarryCount++;
                    other.Delete();
                    Sound.FromWorld( Sounds.Events.OrangeCollected, Position );
                    break;

                case OrangeCollector _ when Game.IsServer && OrangeCarryCount > 0:
                    CollectedOrangesCount += OrangeCarryCount;
                    ChatBox.Say( $"{Client.Name} has dropped {_dropMessageFragments[OrangeCarryCount]}!" );
                    Sound.FromScreen( Sounds.Events.MessageSent );
                    OrangeCarryCount = 0;
                    break;
            }
    }

#if DEBUG
    public static ICollection<Vector3> SpawnerPositions = new List<Vector3>();
    private Vector3? _lastSpawnerPosition = null;

    [Event.Hotload]
    public void OnHotReload()
    {
        SpawnerPositions.Clear();
        _lastSpawnerPosition = null;
    }
#endif

    public override void BuildInput()
    {
        Camera?.BuildInput();

        if ( Input.Pressed( "score" ) )
            IsScoreScreenVisible = true;
        if ( Input.Released( "score" ) )
            IsScoreScreenVisible = false;

#if DEBUG
        if ( Input.Pressed( "attack1" ) )
        {
            Log.Info( Position );
            _lastSpawnerPosition = Position;
        }
        if ( Input.Pressed( "attack2" ) && _lastSpawnerPosition.HasValue )
        {
            SpawnerPositions.Add( _lastSpawnerPosition.Value );
            _lastSpawnerPosition = null;
        }

        if ( Input.Pressed( "use" ) )
        {
            Log.Info( FileSystem.Data.GetFullPath( "spawners.txt" ) );

            FileSystem.Data.WriteAllText(
                "spawners.txt",
                SpawnerPositions
                    .Aggregate(
                        new StringBuilder(),
                        ( result, spawnerPosition ) => result
                            .AppendLine( "new OrangeSpawner" )
                            .AppendLine( "{" )
                            .AppendLine( $"    Position = new Vector3( {spawnerPosition.x}f, {spawnerPosition.y}f, {spawnerPosition.z}f )," )
                            .AppendLine( "    DelayInSeconds = DefaultSpawnDelay" )
                            .AppendLine( "}," )
                    )
                    .ToString()
            );
        }

        if ( Input.Pressed( "reload" ) )
        {
            var start = new Vector3( 542.1441f, 1827.9827f, 0.0052121878f );
            var end = new Vector3( 1437.997f, 1829.9656f, 0.0052121878f );
            var diff = end - start;
            var segmentsCount = 4;
            for ( var segment = 1; segment < segmentsCount; segment++ )
                Log.Info( start + segment * diff / segmentsCount );
        }
#endif
    }

    public override void FrameSimulate( IClient cl )
    {
        SimulateRotation();
        Camera?.Update();
    }

    [TheOrangeRunEvent.GameState.Lobby.Entry]
    protected void OnEnterLobby()
    {
        _fallsEndlessly = false;
        Velocity = 0;
        OrangeCarryCount = 0;
        CollectedOrangesCount = 0;
        Position = SpawnPoint.Position;
    }

    public TraceResult TraceBBox( Vector3 start, Vector3 end, float liftFeet = 0.0f )
    {
        return TraceBBox( start, end, Hull.Mins, Hull.Maxs, liftFeet );
    }

    public TraceResult TraceBBox( Vector3 start, Vector3 end, Vector3 mins, Vector3 maxs, float liftFeet = 0.0f )
    {
        if ( liftFeet > 0 )
        {
            start += Vector3.Up * liftFeet;
            maxs = maxs.WithZ( maxs.z - liftFeet );
        }

        var tr = Trace
            .Ray( start, end )
            .Size( mins, maxs )
            .WithAnyTags( "solid", "playerclip", "passbullets" )
            .Ignore( this )
            .Run();

        return tr;
    }

    protected void SimulateRotation()
    {
        var idealRotation = ViewAngles.ToRotation();
        EyeRotation = Rotation.Slerp( Rotation, idealRotation, Time.Delta * 10f );
        Rotation = EyeRotation;
    }
}
