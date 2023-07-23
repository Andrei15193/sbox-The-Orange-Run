using Sandbox;

namespace TheOrangeRun.Oranges
{
    [Category("Spawners")]
    public class OrangeSpawner : Entity
    {
        private bool _hasValidOrange = false;

        public float LastSpawnTimeInSeconds { get; private set; }

        public float LastCollectTimeInSeconds { get; private set; }

        public float DelayInSeconds { get; set; } = 5 * 60;

        public Orange Orange { get; private set; }

        public void SpawnOrange()
        {
            if ( Game.IsServer && Orange is not null && Orange.IsValid )
                Orange.Delete();

            Orange = new Orange
            {
                BasePosition = Position + Vector3.Up * 20,
            };
            _hasValidOrange = true;
            LastSpawnTimeInSeconds = Time.Now;
        }

        public bool IsActive { get; set; }

        protected override void OnDestroy()
        {
            IsActive = false;
            base.OnDestroy();
        }

        [GameEvent.Tick.Server]
        internal void OnServerTick()
        {
            if ( (Orange is null || !Orange.IsValid) && _hasValidOrange )
            {
                _hasValidOrange = false;
                LastCollectTimeInSeconds = Time.Now;
            }
            if ( IsValid && (Orange is null || (!Orange.IsValid && LastCollectTimeInSeconds + DelayInSeconds < Time.Now )))
                SpawnOrange();
        }
    }
}
