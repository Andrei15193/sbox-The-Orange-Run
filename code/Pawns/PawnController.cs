﻿using Sandbox;
using System;
using System.Collections.Generic;

namespace TheOrangeRun.Pawns;

public class PawnController : EntityComponent<Pawn>
{
    private bool _canMove = false;

    public int StepSize => 24;
    public int GroundAngle => 45;
    public int JumpSpeed => 410;
    public float Gravity => 800f;

    HashSet<string> ControllerEvents = new( StringComparer.OrdinalIgnoreCase );

    bool Grounded => Entity.GroundEntity.IsValid();

    [TheOrangeRunEvent.GameState.OrangeRun.Entry]
    protected void OnEnterOrangeRunState()
    {
        _canMove = true;
    }

    [TheOrangeRunEvent.GameState.OrangeRun.Leave]
    protected void OnLeaveOrangeRunState()
    {
        _canMove = false;
    }

    public void Simulate( IClient client )
    {
        ControllerEvents.Clear();

        var movement = Entity.InputDirection.Normal;

        var angles = Entity.ViewAngles.WithPitch( 0 );
        var moveVector = Rotation.From( angles ) * movement * 320f;
        var groundEntity = CheckForGround();

        if ( _canMove )
        {
            if ( groundEntity.IsValid() )
            {
                if ( !Grounded )
                {
                    Entity.Velocity = Entity.Velocity.WithZ( 0 );
                    AddEvent( "grounded" );
                }

                var carryModifier = 1f - 0.5f / Entity.MaximumOrangeCarryCount * Entity.OrangeCarryCount;
                Entity.Velocity = Accelerate( Entity.Velocity, moveVector.Normal, moveVector.Length, carryModifier * 400.0f, 7.5f );
                Entity.Velocity = ApplyFriction( Entity.Velocity, 4.0f );
            }
            else
            {
                Entity.Velocity = Accelerate( Entity.Velocity, moveVector.Normal, moveVector.Length, 15, 20f );
                Entity.Velocity += Vector3.Down * Gravity * Time.Delta;
            }

            if ( Input.Pressed( "jump" ) )
            {
                DoJump();
            }
        }

        var mh = new MoveHelper( Entity.Position, Entity.Velocity );
        mh.Trace = mh.Trace.Size( Entity.Hull ).Ignore( Entity );

        if ( mh.TryMoveWithStep( Time.Delta, StepSize ) > 0 )
        {
            Entity.Position = Grounded ? StayOnGround( mh.Position ) : mh.Position;
            Entity.Velocity = mh.Velocity;
        }
        else if (Entity.Velocity != Vector3.Zero && mh.TryUnstuck() )
        {
            Entity.Position = mh.Position;
            Entity.Velocity = mh.Velocity;
        }

        Entity.GroundEntity = groundEntity;
    }

    void DoJump()
    {
        if ( Grounded )
            Entity.Velocity = ApplyJump( Entity.Velocity, "jump" );
    }

    Entity CheckForGround()
    {
        if ( Entity.Velocity.z > 300f )
            return null;

        var trace = Entity.TraceBBox( Entity.Position, Entity.Position + Vector3.Down, 2f );

        if ( !trace.Hit )
            return null;

        if ( trace.Normal.Angle( Vector3.Up ) > GroundAngle )
            return null;

        return trace.Entity;
    }

    Vector3 ApplyFriction( Vector3 input, float frictionAmount )
    {
        float StopSpeed = 100.0f;

        var speed = input.Length;
        if ( speed < 0.1f ) return input;

        // Bleed off some speed, but if we have less than the bleed
        // threshold, bleed the threshold amount.
        float control = speed < StopSpeed ? StopSpeed : speed;

        // Add the amount to the drop amount.
        var drop = control * Time.Delta * frictionAmount;

        // scale the velocity
        float newspeed = speed - drop;
        if ( newspeed < 0 )
            newspeed = 0;
        if ( newspeed == speed )
            return input;

        newspeed /= speed;
        input *= newspeed;

        return input;
    }

    Vector3 Accelerate( Vector3 input, Vector3 wishdir, float wishspeed, float speedLimit, float acceleration )
    {
        if ( speedLimit > 0 && wishspeed > speedLimit )
            wishspeed = speedLimit;

        var currentspeed = input.Dot( wishdir );
        var addspeed = wishspeed - currentspeed;

        if ( addspeed <= 0 )
            return input;

        var accelspeed = acceleration * Time.Delta * wishspeed;

        if ( accelspeed > addspeed )
            accelspeed = addspeed;

        input += wishdir * accelspeed;

        return input;
    }

    Vector3 ApplyJump( Vector3 input, string jumpType )
    {
        AddEvent( jumpType );

        var carryModifier = 1f - 0.16f / Entity.MaximumOrangeCarryCount * Entity.OrangeCarryCount;
        return input + Vector3.Up * JumpSpeed * carryModifier;
    }

    Vector3 StayOnGround( Vector3 position )
    {
        var carryModifier = 1f - 0.5f / Entity.MaximumOrangeCarryCount * Entity.OrangeCarryCount;
        var start = position + Vector3.Up * 2 * carryModifier;
        var end = position + Vector3.Down * StepSize;

        // See how far up we can go without getting stuck
        var trace = Entity.TraceBBox( position, start );
        start = trace.EndPosition;

        // Now trace down from a known safe position
        trace = Entity.TraceBBox( start, end );

        if ( trace.Fraction <= 0 )
            return position;
        if ( trace.Fraction >= 1 )
            return position;
        if ( trace.StartedSolid )
            return position;
        if ( Vector3.GetAngle( Vector3.Up, trace.Normal ) > GroundAngle )
            return position;

        return trace.EndPosition;
    }

    public bool HasEvent( string eventName )
    {
        return ControllerEvents.Contains( eventName );
    }

    void AddEvent( string eventName )
    {
        if ( HasEvent( eventName ) )
            return;

        ControllerEvents.Add( eventName );
    }
}
