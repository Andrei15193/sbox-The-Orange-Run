using Sandbox;
using System;

namespace TheOrangeRun.Oranges;

public class Orange : ModelEntity
{
    private Vector3 _basePosition;

    public event EventHandler Collected;

    public override void Spawn()
    {
        base.Spawn();
        Model = Cloud.Model( "rust.orange" );
        Scale = 2;
        SetupPhysicsFromCylinder( PhysicsMotionType.Keyframed, new Capsule( Vector3.Zero, Vector3.Up * 5, 5 ) );
        Tags.Add( "trigger" );

        EnableTouch = true;
    }

    public Vector3 BasePosition
    {
        get => _basePosition;
        set => Position = _basePosition = value;
    }

    [GameEvent.Tick.Server]
    public void Tick()
        => Position = BasePosition + Vector3.Up * 10 * ((float)Math.Sin( Time.Now * 2 ) + 1);

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
