using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class GoTween : AbstractGoTween
{
	// Tween specific properties
	public object target { get; private set; } // the target of the tweens
	public float delay { get; private set; } // delay before starting the actual animations
	private float _elapsedDelay; // total time delayed
	private bool _delayComplete; // once we complete the delay this gets set so we can reverse and play properly for the future
	public bool isFrom { get; private set; } // a value of true will make this a "from" tween

	private List<AbstractTweenProperty> _tweenPropertyList = new List<AbstractTweenProperty>();
	private string targetTypeString;
	
    /// <summary>
    /// sets the ease type for all Tweens. this will overwrite the ease types for each Tween!
    /// </summary>
    private GoEaseType _easeType;
    public GoEaseType easeType
    {
        get
        {
            return _easeType;
        }
        set
        {
            _easeType = value;
			
            // change ease type of all existing tweens.
            foreach( var tween in _tweenPropertyList )
            	tween.setEaseType( value );
        }
    }
	
	

	/// <summary>
	/// initializes a new instance and sets up the details according to the config parameter
	/// </summary>
	public GoTween( object target, float duration, GoTweenConfig config, Action<AbstractGoTween> onComplete = null )
	{
		// default to removing on complete
		autoRemoveOnComplete = true;
		
		this.target = target;
		this.targetTypeString = target.GetType().ToString();
		this.duration = duration;
		
		// copy the TweenConfig info over
		id = config.id;
		delay = config.delay;
		loopType = config.loopType;
		iterations = config.iterations;
		_easeType = config.easeType;
		updateType = config.propertyUpdateType;
		isFrom = config.isFrom;
		timeScale = config.timeScale;
		_onComplete = config.onCompleteHandler;
		_onStart = config.onStartHandler;
		
		if( config.isPaused )
			state = GoTweenState.Paused;
		
		// if onComplete is passed to the constructor it wins. it is left as the final param to allow an inline Action to be
		// set and maintain clean code (Actions always try to be the last param of a method)
		if( onComplete != null )
			_onComplete = onComplete;
		
		// add all our properties
		for( var i = 0; i < config.tweenProperties.Count; i++ )
		{
			var tweenProp = config.tweenProperties[i];
			
			// if the tween property is initialized already it means it is being reused so we need to clone it
			if( tweenProp.isInitialized )
				tweenProp = tweenProp.clone();
			
			addTweenProperty( tweenProp );
		}
		
		// calculate total duration
		if( iterations < 0 )
			totalDuration = float.PositiveInfinity;
		else
			totalDuration = iterations * duration;
	}

	
	/// <summary>
	/// tick method. if it returns true it indicates the tween is complete
	/// </summary>
	public override bool update( float deltaTime )
	{
		// should we validate the target?
		if( Go.validateTargetObjectsEachTick )
		{
			// This might seem to be overkill, but on the case of Transforms that
			// have been destroyed, target == null will return false, whereas 
			// target.Equals(null) will return true.  Otherwise we don't really
			// get the benefits of the nanny.
			if( target == null || target.Equals(null) )
			{
				// if the target doesn't pass validation
				Debug.LogWarning( "target validation failed. destroying the tween to avoid errors. Target type: " + this.targetTypeString );
				autoRemoveOnComplete = true;
				return true;
			}
		}
		
		// handle delay and return if we are still delaying
		if( !_delayComplete && _elapsedDelay < delay )
		{
			// if we have a timeScale set we need to remove its influence so that delays are always in seconds
			if( timeScale != 0 )
				_elapsedDelay += deltaTime / timeScale;
			
			// are we done delaying?
			if( _elapsedDelay >= delay )
				_delayComplete = true;

			return false;
		}
		
		// base will calculate the proper elapsedTime for us
		base.update( deltaTime );
		
		// if we are looping back on a PingPong loop
		var convertedElapsedTime = _isLoopingBackOnPingPong ? duration - _elapsedTime : _elapsedTime;
		
		// update all properties
		for( var i = 0; i < _tweenPropertyList.Count; i++ )
			_tweenPropertyList[i].tick( convertedElapsedTime );
		
		if( state == GoTweenState.Complete )
		{
			if( !_didComplete )
				onComplete();
			
			return true; //true if complete
		}
		
		return false; //false if not complete
	}
	
	
	/// <summary>
	/// we are valid if we have a target and at least one TweenProperty
	/// </summary>
	public override bool isValid()
	{
		return target != null;
	}
	
	
	/// <summary>
	/// adds the tween property if it passes validation and initializes the property
	/// </summary>
	public void addTweenProperty( AbstractTweenProperty tweenProp )
	{
		// make sure the target is valid for this tween before adding
		if( tweenProp.validateTarget( target ) )
		{
			// ensure we dont add two tweens of the same property so they dont fight
			if( _tweenPropertyList.Contains( tweenProp ) )
			{
				Debug.Log( "not adding tween property because one already exists: " + tweenProp );
				return;
			}
			
			_tweenPropertyList.Add( tweenProp );
			tweenProp.init( this );
		}
		else
		{
			Debug.Log( "tween failed to validate target: " + tweenProp );
		}
	}
	
	
	public override bool removeTweenProperty( AbstractTweenProperty property )
	{
		if( _tweenPropertyList.Contains( property ) )
		{
			_tweenPropertyList.Remove( property );
			return true;
		}
		
		return false;
	}
	
	
	public override bool containsTweenProperty( AbstractTweenProperty property )
	{
		return _tweenPropertyList.Contains( property );
	}
	
	
	public void clearTweenProperties()
	{
		_tweenPropertyList.Clear();
	}

	
	public override List<AbstractTweenProperty> allTweenProperties()
	{
		return _tweenPropertyList;
	}
	
	
	// override to prepare all the TweenProperies for use
	protected override void onStart()
	{
		base.onStart();
		
		// prepare our TweenProperties
		for( var i = 0; i < _tweenPropertyList.Count; i++ )
			_tweenPropertyList[i].prepareForUse();
	}
	
	
	#region AbstractTween overrides
	
	/// <summary>
	/// removes the tween and cleans up its state
	/// </summary>
	public override void destroy()
	{
		base.destroy();
		
		_tweenPropertyList.Clear();
		target = null;
	}
	
	
	/// <summary>
	/// goes to the specified time clamping it from 0 to the total duration of the tween. if the tween is
	/// not playing it will be force updated to the time specified.
	/// </summary>
	public override void goTo( float time )
	{
		// handle delay, which is specific to Tweens
		_delayComplete = true;
		_elapsedDelay = delay;
		
		base.goTo( time );
	}
	

	/// <summary>
	/// rewinds the tween to the beginning and pauses playback
	/// </summary>
	public override void rewind()
	{
		rewind( true );
	}
	
	
	public void rewind( bool skipDelay )
	{
		state = GoTweenState.Paused;
		
		// reset all state here
		_elapsedTime = _totalElapsedTime = 0;
		_elapsedDelay = skipDelay ? duration : 0;
		_delayComplete = skipDelay;
		_isLoopingBackOnPingPong = false;
		_completedIterations = 0;
	}
	
	
	/// <summary>
	/// completes the tween. sets the object to it's final position as if the tween completed normally
	/// </summary>
	public override void complete()
	{
		if( iterations < 0 )
			return;
		
		base.complete();
		
		// set delayComplete so we get one last update in before we die (base will set the elapsed time for us)
		_delayComplete = true;
	}

	#endregion

}
