# UnitySteer 2.x changelog 

## v2.7

Yet another release with major changes.  Make sure you read the notes below before updating!

* MAJOR CHANGE: Reworked classes that descend from SteerForNeighbors, you'll now need a SteerForNeighborGroup.  See the description provided here: https://github.com/ricardojmendez/UnitySteer/issues/18
* Exposing AcceptableDistance on SteerForPursuit.
* Exposing minimum speed on SteerForMinimumSpeed
* SteerForMinimumSpeed optionally moves forward by default
* SteerForNeighborGroup optimization: Neighbors are cached on detection. This means that IsInNeighborhood is evaluated when detected, not every time that the behavior is going to calculate its forces.
* SteerForSeparation optimization: v.sqrMagnitude is faster than Dot(v,v)
* Improvement: SteerForAlignment wasn't using the cached Transform value
* Unified layers checked on SteerForNeighborGroup and Radar.
* MAJOR CHANGE: Vehicles will always use their own mass for calculations.  We will no longer consider the Rigidbody mass in calculations, just whatever value is set on the Vehicle. The property name changed from _internalMass to _mass, so you should make sure you note your previous values to avoid data loss.
* Radar collider caching is now by id, so we can use a sorted dictionary to improve performance.
* Force is now affected by mass before being clamped to MaxForce
* New TickedVehicle.Stop method. See the description here: https://github.com/ricardojmendez/UnitySteer/commit/e6eca152f107c4c848e412bad73d853599f6b5f1
* SteerForPathSimplified now expose properties for the distance along the path, as well as the total path percentage traversed.

I've also removed some redundant and unnecessary methods, as well as the outdated RadarPing, which I deprecated a long while ago.


## v2.6

This release contains a major change altering the way that acceleration is smoothed. 
MAKE SURE YOU REVIEW THE NOTES BELOW BEFORE IMPORTING INTO YOUR PROJECT.

While the vehicles still applied acceleration smoothing, they applied it to the result of the force calculation. However, force calculations don't necessarily take place every frame, whereas force application does. 

From this release on, UnitySteer will smooth the displacement delta being applied to the object every frame, instead of smoothing the periodic speed calculation.



## v2.5.1

This will likely be the final minor release on 2.5, and I'm planning some relatively major changes which will end up on the development branch soon.  

MAKE SURE YOU READ THE COMMENTS BEFORE UPGRADING TO THE DEVELOPMENT BRANCH AFTER THIS RELEASE.

* Fixed a bug on Vector3Pathway when configuring cyclical paths
* Exposed a radar method so that the owner vehicle (or other classes) can force it to update outside of its cycle
* Updated Priority Queue to allow pausing
* Improved documentation
* Other minor improvements and fixes




## v2.5 

UnitySteer 2.5 significantly changes the way that vehicles and radars behave internally, in order to provide both a more coherent structure and increased performance.  Read the change notes below before upgrading.

Warning:

Because of how the class/meta file references work and the way classes were renamed during development, you may have your previous AutonomousVehicle behaviors show up as Bipeds.  Make sure you verify.


General changes:

* UnitySteer now uses Mike Garforth's TickedPriorityQueue.
* Removed dependency on the C5 class libraries
* Removed outdated classes which I'd kept under review for ages.
* Implemented workarounds for Unity's deficiencies on AOT compilation. It should now work on iOS.

Steering and vehicle changes:

* Vehicles now update their desired velocity via a TickedPriorityQueue.
* New trivial SteerToFollow, SteerForForward implementations
* Re-did SteerForSphericalObstacleAvoidance, then decided to mark it as Obsolete.
* New SteerForSphericalObstacleRepulsion, which gives better results on areas with multiple spherical obstacles close together.
* New Biped vehicle class. See http://www.arges-systems.com/articles/573/unitysteer-autonomous-vehicle-biped/
* Vehicles now gradually turn towards their desired velocity, something that wasn't properly handled before
* Separated the application of the velocity from the vehicle's turning. This is a subtle change, but will likely make it easier to make tank-like vehicles.
* SteerForPursuit now has an acceptable distance on which the pursuer is considered "caught"
* New OnStartMoving steering event
* Added a Speedometer behavior, which is not UnitySteer-exclusive and can be attached to any object, and expanded Vehicle. If a Vehicle detects that its gameObject has a Speedometer, when its speed is requested it will return the value measured by the Speedometer instead of just using the Velocity's magnitude.
* We can now have post-processing steering behaviors, which are evaluated after the regular behaviors and can return adjustments based on the vehicle's DesiredVelocity.
* SteerForWander no longer returns a merely lateral vector.  
* Added new PassiveVehicle, which can be attached to non-UnitySteer vehicles (say, a player-controller character) so that other UnitySteer behaviors can track it.
* Removed legacy property HasInertia.
* Optimizations.

Radar changes: 

* Radars are now updated via a TickedPriorityQueue, since we will almost never need to update them every frame. This means you can independently set some vehicles to update their radars more often than others.
* Removed Radar.ObstacleLayer.  Any DetectableObject that is not a vehicle will be considered an obstacle.
* Removed Radar.ObstacleFactory.  DetectableObjects should know how to create their own obstacles if they're not spherical.
* Updated other behaviors to work with DetectableObjects.
* Added methods to Radar to ignore a DetectableObject.
* RadarPing is now there only for backwards compatibility reasons, and its functionality has been rolled into the base Radar class.  I'll disappear in the future, so I recommend you change your component references for
Radar ones.
* Optimizations for both performance and to significantly reduce allocations.



## v2.2

* Added DetectableObject as a Vehicle base class.  This will allow us to make some static targets in scenes approachable with the same routines as we would approach a vehicle, without the need to have the Vehicle overhead.
* Removed outdated AngryAntPathway.  This was meant to be used with Path 1, which does not work with Unity 3.  For a draft Path 2 seeker, see https://gist.github.com/841909
* Allowing 'speed' and 'forward' override on to Vehicle.ComputeNearestApproachPositions.  This override, as well as the previous one, will allow us to use this method to estimate the likelihood of crashing into a vehicle at a future orientation and speed, which are not necessarily the same as the ones that our current vehicle has. This is something I am using on my RVO implementation.
* SphericalObstacleData now expressed in terms of DetectableObject.
* VehicleEditor now relies on DetectableObjectEditor.
* SteerForPathSimplified bugfix. There was an issue when the character's path began on its position (i.e., the character was already on the path) but the character's    speed was 0. In this case, the future path position estimated was the same as the current position, which caused the steering behavior to believe it had already arrived.
* SteerForPathSimplified bugfix.  On very slow agents with a twisting path and a close prediction time, it's possible that the future predicted point will still be within the vehicle's radius, which caused it to believe it had arrived.
* Setting cached transform on Position getter if necessary.
* Radar will now disregard disabled vehicles by default. You can change this behavior by setting its DetectDisabledVehicles property to true.
* Added property SteerForPathSimplified.MinSpeedToConsider for configuring the minimum estimation speed.
* Increased default steering weight. The previous default weight of 1 could be too low in some cases.
* Increased default prediction time on SteerForPathSimplified. The default value might have been too low for the default vehicle speed of 1, so I've increased it to allow people who are just starting to build up their own examples to get a better feel for the reaction.
* Steering.ReportedArrival defaults to true on Start() to avoid unnecessary arrival notifications.
* SteerForNeighbors is now an abstract class, to avoid confusion for users. Some people were trying to use it directly, instead of using the classes that implement it.


## v2.1

When introducing a new configurable value, I've aimed to keep the same behavior as before by defaulting it to the previous constant.

* Allowing you to disable the acceleration blending. Simply set AutonomousVehicle's AccelerationSmoothRate to 0.
* Added VehicleLookAtOverride.
* Zeroing the Y force for planar vehicles before the force and speed even get clamped.   This may have an effect on your previously created vehicles, but it shouldn't alter their behavior significantly.  I'd rather not do these sort of changes after release, but the alternative was forking AutonomousVehicle.
* AutonomousVehicle now takes into account its CanMove value.  If CanMove is FALSE, the forces are still calculated, but the vehicle does not update its position. If you want to avoid calculating the forces altogether simply disable the vehicle.
* Vehicle.GetSeekVector now has a parameter so you can indicate that its own speed should be disregarded for arrival purposes.
* Documented the actual state of SteerToFollowPath.
* Removed PolylinePathway.  Use Vector3Pathway instead.
* AutonomousVehicle no longer evaluating HasInertia. All the steering behaviors I've created assume "Star Wars physics" for their calculations, where if you stop applying a force to the object it should stop.  This means that HasInertia should be false.  No longer evaluating the value on AutonomousVehicle to avoid the confusion caused by the vehicle not acting as expected, and added a warning on vehicle start.
* Added ArrivalRadius to Vehicle, to separate the concept of the vehicle's volume/area from when it's close enough to its target (if said target is a point).
* Added OnArrival to Steering
* BUGFIX: Setting the Tick's TickLapse will reset the next tick time
* See this for more details on the path following classes:
  http://www.arges-systems.com/articles/213/unitysteer-upcoming-path-following-changes/
* Any receiver of an OnArrival call can now tell the vehicle to retry once by setting the SteeringEvent's Action property to 'retry'.   This is useful if you don't want the steering behavior to skip a beat when it has arrived at the designated spot and needs to await instructions from the receiver (as otherwise the value returned might get cached for a few frames).
* Expanded comments

BREAKING CHANGE:

* Arrival is now calculated not by the Radius, but by the ArrivalRadius.  Please adjust accordingly.