using Sandbox;

namespace TheOrangeRun.Oranges
{
    [Category("Spawners")]
    public class OrangeSpawner : Entity
    {
        private bool _isActive = false;

        public float LastSpawnTimeInSeconds { get; private set; }

        public float LastCollectTimeInSeconds { get; private set; }

        public float DelayInSeconds { get; set; }

        public Orange Orange { get; private set; }

        public void SpawnOrange()
        {
            if ( Game.IsServer && Orange is not null && Orange.IsValid )
                Orange.Delete();

            Orange = new Orange
            {
                BasePosition = Position + Vector3.Up * 20
            };
            Orange.Collected += delegate { LastCollectTimeInSeconds = Time.Now; };
            LastSpawnTimeInSeconds = Time.Now;
        }

        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                if ( value != _isActive )
                {
                    _isActive = value;
                    if ( _isActive )
                        Event.Register( this );
                    else
                        Event.Unregister( this );
                }
            }
        }

        protected override void OnDestroy()
        {
            IsActive = false;
            base.OnDestroy();
        }

        [GameEvent.Tick.Server]
        internal void OnServerTick()
        {
            if ( Orange is null || (!Orange.IsValid && LastCollectTimeInSeconds + DelayInSeconds < Time.Now ))
                SpawnOrange();
        }
    }
}
