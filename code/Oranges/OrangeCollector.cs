using Sandbox;

namespace TheOrangeRun.Oranges;

[Category("Collectors")]
public class OrangeCollector : ModelEntity
{
    public override void Spawn()
    {
        base.Spawn();
        Tags.Add( "trigger" );

        SetupPhysicsFromCylinder( PhysicsMotionType.Static, new Capsule( Vector3.Zero, Vector3.Up * 30, 50 ) );
        Scale = 2;

        _ = new OrangeCollectorCollisionEntity
        {
            Parent = this,
            Scale = Scale
        };
    }

    private class OrangeCollectorCollisionEntity : ModelEntity
    {
        public OrangeCollector OrangeCollector
        {
            get => (OrangeCollector)Parent;
        }

        public override void Spawn()
        {
            base.Spawn();
            Tags.Add( "solid" );

            Model = Cloud.Model( "facepunch.wooden_crate" );
            SetupPhysicsFromModel( PhysicsMotionType.Static );

            EnableAllCollisions = true;
        }
    }
}
