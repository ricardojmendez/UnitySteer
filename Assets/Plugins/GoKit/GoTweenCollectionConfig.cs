using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class GoTweenCollectionConfig
{
	public int id; // id for finding the Tween at a later time. multiple Tweens can have the same id
	public int iterations = 1; // number of times to iterate. -1 will loop indefinitely
	public GoLoopType loopType = Go.defaultLoopType;
	public GoUpdateType propertyUpdateType = Go.defaultUpdateType;
	public Action<AbstractGoTween> onCompleteHandler;
	public Action<AbstractGoTween> onStartHandler;

	
	/// <summary>
	/// sets the number of iterations. setting to -1 will loop infinitely
	/// </summary>
	public GoTweenCollectionConfig setIterations( int iterations )
	{
		this.iterations = iterations;
		return this;
	}
	
	
	/// <summary>
	/// sets the number of iterations and the loop type. setting to -1 will loop infinitely
	/// </summary>
	public GoTweenCollectionConfig setIterations( int iterations, GoLoopType loopType )
	{
		this.iterations = iterations;
		this.loopType = loopType;
		return this;
	}
	
	
	/// <summary>
	/// sets the update type for the Tween
	/// </summary>
	public GoTweenCollectionConfig setUpdateType( GoUpdateType setUpdateType )
	{
		this.propertyUpdateType = setUpdateType;
		return this;
	}
	
	
	/// <summary>
	/// sets the onComplete handler for the Tween
	/// </summary>
	public GoTweenCollectionConfig onComplete( Action<AbstractGoTween> onComplete )
	{
		onCompleteHandler = onComplete;
		return this;
	}
	
	
	/// <summary>
	/// sets the onStart handler for the Tween
	/// </summary>
	public GoTweenCollectionConfig onStart( Action<AbstractGoTween> onStart )
	{
		onStartHandler = onStart;
		return this;
	}
	
	
	/// <summary>
	/// sets the id for the Tween. Multiple Tweens can have the same id and you can retrieve them with the Go class
	/// </summary>
	public GoTweenCollectionConfig setId( int id )
	{
		this.id = id;
		return this;
		
	}

}