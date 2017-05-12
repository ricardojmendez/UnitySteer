using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// base class shared by the Tween and TweenChain classes to allow a seemless API when controlling
/// either of them
/// </summary>
public abstract class AbstractGoTween
{
	public int id = 0; // optional id used for identifying this tween
	public GoTweenState state { get; protected set; } // current state of the tween
	public float duration { get; protected set; } // duration for a single loop
	public float totalDuration { get; protected set; } // duration for all loops of this tween
	public float timeScale { get; set; } // time scale to be used by this tween
	
	public GoUpdateType updateType { get; protected set; }
	public GoLoopType loopType { get; protected set; }
	public int iterations { get; protected set; } // set to -1 for infinite
	
	public bool autoRemoveOnComplete { get; set; } // should we automatically remove ourself from the Go's list of tweens when done?
	public bool isReversed { get; protected set; } // have we been reversed? this is different than a PingPong loop's backwards section
	protected bool _didStart; // flag to ensure onStart only gets fired once and TweenProperties are initialized once
	protected bool _didComplete; // flag to ensure onComplete only gets fired once
	
	// internal state for update logic
	protected float _elapsedTime; // elapsed time for the current loop iteration
	protected float _totalElapsedTime; // total elapsed time of the entire tween
	public float totalElapsedTime { get { return _totalElapsedTime; } }
	
	protected bool _isLoopingBackOnPingPong;
	public bool isLoopoingBackOnPingPong { get { return _isLoopingBackOnPingPong; } }
	
	protected int _completedIterations;
	public int completedIterations { get { return _completedIterations; } }
	
	
	// action event handlers
	protected Action<AbstractGoTween> _onStart; // executes only once at initial startup
	protected Action<AbstractGoTween> _onComplete; // exectures whenever a Tween completes
	
	
	
	public void setOnStartHandler( Action<AbstractGoTween> onStart )
	{
		_onStart = onStart;
	}

	
	public void setOnCompleteHandler( Action<AbstractGoTween> onComplete )
	{
		_onComplete = onComplete;
	}
	
	
	/// <summary>
	/// called once per tween when it is first started
	/// </summary>
	protected virtual void onStart()
	{
		_didStart = true;
		
		if( _onStart != null )
			_onStart( this );
	}
	
	
	/// <summary>
	/// called once per tween when it completes
	/// </summary>
	protected virtual void onComplete()
	{
		_didComplete = true;
		
		if( _onComplete != null )
			_onComplete( this );
	}
	
	
	/// <summary>
	/// tick method. if it returns true it indicates the tween is complete
	/// </summary>
	public virtual bool update( float deltaTime )
	{
		// handle startup
		if( !_didStart )
			onStart();
		
		// increment or decrement the total elapsed time then clamp from 0 to totalDuration
        if( isReversed )
			_totalElapsedTime -= deltaTime;
        else
			_totalElapsedTime += deltaTime;
		
		_totalElapsedTime = Mathf.Clamp( _totalElapsedTime, 0, totalDuration );
		
		
		// using our fresh totalElapsedTime, figure out what iteration we are on
		_completedIterations = (int)Mathf.Floor( _totalElapsedTime / duration );
		
		// we can only be loopiong back on a PingPong if our loopType is PingPong and we are on an odd numbered iteration
		_isLoopingBackOnPingPong = false;
		if( loopType == GoLoopType.PingPong )
		{
			// infinite loops and we are on an odd numbered iteration
			if( iterations < 0 && _completedIterations % 2 != 0 )
			{
				_isLoopingBackOnPingPong = true;
			}
			else if( iterations > 0 )
			{
				// we have finished all iterations and we went one over to a non looping back iteration
				// so we still count as looping back so that we finish in the proper location
				if( completedIterations >= iterations && _completedIterations % 2 == 0 )
					_isLoopingBackOnPingPong = true;
				else if( completedIterations < iterations && _completedIterations % 2 != 0 )
					_isLoopingBackOnPingPong = true;
			}
		}
		
		
		// figure out the current elapsedTime
		if( iterations > 0 && _completedIterations >= iterations )
		{
			// we finished all iterations so clamp to the end of this tick
			_elapsedTime = duration;
			
			// if we arent reversed, we are done
			if( !isReversed )
				state = GoTweenState.Complete;
		}
		else if( _totalElapsedTime < duration )
		{
			_elapsedTime = _totalElapsedTime; // havent finished a single iteration yet
		}
		else
		{
			// TODO: when we increment a completed iteration (go from 0 to 1 for example) we should probably run through once setting
			// _elapsedTime = duration so that complete handlers in a chain or flow fire when expected
			_elapsedTime = _totalElapsedTime % duration; // have finished at least one iteration
		}
		
		
		// check for completion when going in reverse
		if( isReversed && _totalElapsedTime <= 0 )
			state = GoTweenState.Complete;
		
		return false;
	}
	
	
	/// <summary>
	/// subclasses should return true if they are a valid and ready to be added to the list of running tweens
	/// or false if no
	/// technically, this should be marked as internal
	/// </summary>
	public abstract bool isValid();
	
	
	/// <summary>
	/// attempts to remove the tween property returning true if successful
	/// technically, this should be marked as internal
	/// </summary>
	public abstract bool removeTweenProperty( AbstractTweenProperty property );
	
	
	/// <summary>
	/// returns true if the tween contains the same type (or propertyName) property in its
	/// technically, this should be marked as internal
	/// property list
	/// </summary>
	public abstract bool containsTweenProperty( AbstractTweenProperty property );
	
	
	/// <summary>
	/// returns a list of all the TweenProperties contained in the tween and all its children (if it is
	/// a TweenChain or a TweenFlow)
	/// technically, this should be marked as internal
	/// </summary>
	public abstract List<AbstractTweenProperty> allTweenProperties();
	
	
	/// <summary>
	/// removes the Tween from action and cleans up its state
	/// </summary>
	public virtual void destroy()
	{
		state = GoTweenState.Destroyed;
		//Go.removeTween( this ); //this done now in Go.handleUpdateOfType to avoid removing a tween while they are parsed in the main loop
	}

	
	/// <summary>
	/// pauses playback
	/// </summary>
	public void pause()
	{
		state = GoTweenState.Paused;
	}
	
	
	/// <summary>
	/// resumes playback
	/// </summary>
	public void play()
	{
		state = GoTweenState.Running;
	}
	
	
	/// <summary>
	/// plays the tween forward. if it is already playing forward has no effect
	/// </summary>
	public void playForward()
	{
		isReversed = false;
		state = GoTweenState.Running;
	}
	
	
	/// <summary>
	/// plays the tween backwards. if it is already playing backwards has no effect
	/// </summary>
	public void playBackwards()
	{
		isReversed = true;
		state = GoTweenState.Running;
	}
	
	
	/// <summary>
	/// rewinds the tween to the beginning and pauses playback
	/// </summary>
	public abstract void rewind();
	
	
	/// <summary>
	/// rewinds the tween to the beginning and starts playback optionally skipping delay (only relevant for Tweens). Note that onComplete
	/// will again fire after calling restart
	/// </summary>
	public void restart( bool skipDelay = true )
	{
		// reset state when we restart
		_didComplete = false;
		rewind();
		state = GoTweenState.Running;
	}
	
	
	/// <summary>
	/// reverses playback. if going forward it will be going backward after this and vice versa.
	/// </summary>
	public void reverse()
	{
		isReversed = !isReversed;
	}
	
	
	/// <summary>
	/// completes the tween. sets the object to it's final position as if the tween completed normally. takes into effect
	/// if the tween was playing forward or reversed
	/// </summary>
	public virtual void complete()
	{
		if( iterations < 0 )
			return;
		
		// set full elapsed time and let the next iteration finish it off
		_elapsedTime = isReversed ? 0 : duration;
		_totalElapsedTime = isReversed ? 0 : totalDuration;
		_completedIterations = isReversed ? 0 : iterations;
		state = GoTweenState.Running;
	}
	
	
	/// <summary>
	/// goes to the specified time clamping it from 0 to the total duration of the tween. if the tween is
	/// not playing it can optionally be force updated to the time specified. delays are not taken into effect
	/// </summary>
	public virtual void goTo( float time )
	{
		// clamp time to valid numbers
        if ( loopType == GoLoopType.PingPong )
            time = Mathf.Clamp( time, 0, totalDuration * 2 );
        else
            time = Mathf.Clamp( time, 0, totalDuration );
		
		// set time and force an update so we move to the desired location if we are not running
		_totalElapsedTime = time;
		_elapsedTime = _totalElapsedTime;
		
		// manually set the loop count and if we are on the reverse end of a PingPong loop
		if( iterations > 0 || iterations < 0 )
		{
			_completedIterations = (int)Mathf.Floor( _totalElapsedTime / duration );
			if( loopType == GoLoopType.PingPong )
				_isLoopingBackOnPingPong = _completedIterations % 2 != 0;
			
			// set elapsed time taking into account if we are set to loop.
			// if we have loops left we need to get a proper elapsed by modding the totalElapsed
			if( iterations < 0 || ( iterations > 0 && _completedIterations < iterations + 1 ) )
				_elapsedTime = _totalElapsedTime % duration;
		}

		update( 0 );
	}

	
	/// <summary>
	/// goes to the time and starts playback skipping any delays
	/// </summary>
	public void goToAndPlay( float time )
	{
		state = GoTweenState.Running;
		goTo( time );
	}
	
	
	/// <summary>
	/// waits for either completion or destruction. call in a Coroutine and yield on the return
	/// </summary>
    public IEnumerator waitForCompletion()
    {
        while( state != GoTweenState.Complete && state != GoTweenState.Destroyed )
            yield return null;

        yield break;
    }
	

}
