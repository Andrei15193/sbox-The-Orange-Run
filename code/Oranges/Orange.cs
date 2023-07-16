using Sandbox;
using System;

namespace TheOrangeRun.Oranges;

[Category("Pickups")]
public class Orange : ModelEntity
{
    private readonly float _positionOffset = Random.Shared.Float( 0, (float)(2 * Math.PI) );
    private readonly float _rotationOffset = Random.Shared.Float( 0, 365 );

    public event EventHandler Collected;

    public override void Spawn()
    {
        base.Spawn();

        Model = Cloud.Model( "rust.orange" );
        Scale = 3;
        SetupPhysicsFromCylinder( PhysicsMotionType.Keyframed, new Capsule( Vector3.Zero, Vector3.Up * 5, 5 ) );
        Tags.Add( "trigger" );

        EnableTouch = true;
        Rotation = Rotation.RotateAroundAxis( Vector3.Up, _rotationOffset );
    }

    public Vector3 BasePosition { get; set; }

    [GameEvent.Tick.Server]
    protected void OnServerTick()
    {
        Position = BasePosition + Vector3.Up * 10 * ((float)Math.Sin( _positionOffset + Time.Now * 2 ) + 1);
        Rotation = Rotation.RotateAroundAxis( Vector3.Up, Time.Delta * 70 );
    }

    public override void StartTouch( Entity other )
    {
        base.StartTouch( other );

        if ( other is Pawn pawn )
        {
            Collected?.Invoke( this, EventArgs.Empty );
            TheOrangeRunGameManager.Current.CollectOrange( this, pawn );
        }
    }
}
