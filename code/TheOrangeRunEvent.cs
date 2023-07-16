using Sandbox;

namespace TheOrangeRun;

public static class TheOrangeRunEvent
{
    private const string Prefix = "TheOrangeRun";

    public static class GameState
    {
        public const string Changed = nameof( Prefix ) + "." + nameof( GameState ) + "." + nameof( Changed );

        [MethodArguments( typeof( TheOrangeRun.GameState ) )]
        public class ChangedAttribute : EventAttribute
        {
            public ChangedAttribute()
                : base( Changed )
            {
            }
        }

        public static class Starting
        {
            public const string Entry = nameof( Prefix ) + "." + nameof( GameState ) + "." + nameof( Starting ) + "." + nameof( Entry );
            public const string Leave = nameof( Prefix ) + "." + nameof( GameState ) + "." + nameof( Starting ) + "." + nameof( Leave );

            public class EntryAttribute : EventAttribute
            {
                public EntryAttribute()
                    : base( Entry )
                {
                }
            }

            public class LeaveAttribute : EventAttribute
            {
                public LeaveAttribute()
                    : base( Leave )
                {
                }
            }
        }

        public static class Lobby
        {
            public const string Entry = nameof( Prefix ) + "." + nameof( GameState ) + "." + nameof( Lobby ) + "." + nameof( Entry );
            public const string Leave = nameof( Prefix ) + "." + nameof( GameState ) + "." + nameof( Lobby ) + "." + nameof( Leave );

            public class EntryAttribute : EventAttribute
            {
                public EntryAttribute()
                    : base( Entry )
                {
                }
            }

            public class LeaveAttribute : EventAttribute
            {
                public LeaveAttribute()
                    : base( Leave )
                {
                }
            }
        }

        public static class OrangeRun
        {
            public const string Entry = nameof( Prefix ) + "." + nameof( GameState ) + "." + nameof( OrangeRun ) + "." + nameof( Entry );
            public const string Leave = nameof( Prefix ) + "." + nameof( GameState ) + "." + nameof( OrangeRun ) + "." + nameof( Leave );

            public class EntryAttribute : EventAttribute
            {
                public EntryAttribute()
                    : base( Entry )
                {
                }
            }

            public class LeaveAttribute : EventAttribute
            {
                public LeaveAttribute()
                    : base( Leave )
                {
                }
            }
        }

        public static class Leaderboards
        {
            public const string Entry = nameof( Prefix ) + "." + nameof( GameState ) + "." + nameof( Leaderboards ) + "." + nameof( Entry );
            public const string Leave = nameof( Prefix ) + "." + nameof( GameState ) + "." + nameof( Leaderboards ) + "." + nameof( Leave );

            public class EntryAttribute : EventAttribute
            {
                public EntryAttribute()
                    : base( Entry )
                {
                }
            }

            public class LeaveAttribute : EventAttribute
            {
                public LeaveAttribute()
                    : base( Leave )
                {
                }
            }
        }
    }
}
