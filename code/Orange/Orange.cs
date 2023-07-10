using Sandbox;
using System;

namespace TheOrangeRun;

public class Orange : ModelEntity
{
    private Vector3 _basePosition;

    public override void Spawn()
    {
        base.Spawn();
        // Model = Cloud.Model( "rust.orange" );
        SetModel( "models/citizen_props/crate01.vmdl" );
        Scale = 2;
        SetupPhysicsFromModel( PhysicsMotionType.Keyframed );
        Tags.Add( "trigger" );

        // PhysicsEnabled = true;
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
