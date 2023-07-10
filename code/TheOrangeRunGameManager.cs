
using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using TheOrangeRun.UI;

//
// You don't need to put things in a namespace, but it doesn't hurt.
//
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

    /// <summary>
    /// Called when the game is created (on both the server and client)
    /// </summary>
    public TheOrangeRunGameManager()
    {
        if ( Game.IsClient )
            Game.RootPanel = new Hud();
    }

    [Net]
    public IList<PawnStats> PawnsStats { get; private set; }

    public void AddOrangeTo( IClient client )
    {
        using var pawnStats = PawnsStats.GetEnumerator();
        var hasValue = pawnStats.MoveNext();
        while ( hasValue && pawnStats.Current.Id != client.Id )
            hasValue = pawnStats.MoveNext();

        if ( hasValue )
            pawnStats.Current.TotalOranges++;
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

    /// <summary>
    /// A client has joined the server. Make them a pawn to play with
    /// </summary>
    public override void ClientJoined( IClient client )
    {
        base.ClientJoined( client );
        PawnsStats.Add( new PawnStats { Id = client.Id, Name = client.Name, TotalOranges = 0 } );

        var orangeSpawnPoint = All.OfType<SpawnPoint>().First();
        var orange = new Orange
        {
            BasePosition = orangeSpawnPoint.Position + Vector3.Up * 20
        };

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
}
