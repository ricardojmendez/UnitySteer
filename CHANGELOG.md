# UnitySteer changelog

## v3.0 (RC3)

Breaking changes from UnitySteer 2.x: 

* Namespace reorganization.
* Radar no longer has an API to ignore specific objects. See [commit afb7e14](https://github.com/ricardojmendez/UnitySteer/commit/afb7e1459f0f63f559652c1fdc6fab22272f7e5d) for the rationale.
* Removed scaled values from DetectableObject. *You will need to modify your vehicle and obstacle prefabs if you're scaling them*. See [commit 969d817](https://github.com/ricardojmendez/UnitySteer/commit/969d817dbb2d651664a7a9bc815675d151929b47) for the rationale.
* New Min/Max distance parameters for SteerForNeighbors. Removed redundant parameters from subclasses. *You will need to modify your prefabs that use SteerForSeparation/Cohesion/Alignment*. See [commit 7eb5b3b](https://github.com/ricardojmendez/UnitySteer/commit/7eb5b3b090c01ba5b8074e589b73f3955b8eae61) for details.
* GetSeekVector will not consider the velocity by default, since that can produce inconsistent behavior on an agent that for performance reasons does not update its forces every frame.
* Removed TickedVehicle._accelerationSmoothRate.  This affects both Bipeds and AutonomousVehicles.
* New properties on AutonomousVehicle to control the acceleration and deceleration rates.
* Using the AutonomousVehicle's TargetSpeed to indicate the speed we're aiming for, and Speed the one we're actually moving at.  Speed will gradually aim for TargetSpeed at its vehicle’s acceleration/deceleration rates.
* SteerForWander.SmoothRate is now an amount per second. This helps make it frame-rate independent.
* [You can read more about the acceleration smoothing changes here](http://arges-systems.com/blog/2014/01/30/unitysteer-acceleration-smoothing-changes/).
* Replaced IsPlanar with AllowedMovementAxes. We can now limit movement on any arbitrary axis, not only the Y.
* Removed obsolete SteerForSphericalObstacleAvoidance.  SteerForSphericalObstacleRepulsion is now simply called SteerForSphericalObstacles.
* Pruned cyclic Vector3Pathways. It was never properly implemented, and they're just as doable by having an event handler for the arrival event of the path steering behavior.
* Removed redundant Pathway. There were some vestigial methods there that are better done on Vector3Pathway.
* I have removed SteerForTarget. Use SteerForPoint instead.

Other changes and improvements:

* Improved spherical obstacle avoidance to consider the distance from the collision with each obstacle when deciding its avoidance vector.
* DetectableObjects now register themselves with Radar on enable. They no longer need to be on the root.
* New SplinePathway.  Takes a list of Vector3s and uses them to create a spline for a path. Chances are this is not what you want to use to create a pathway for bipeds dealing with spatial constraints (say, following a navmesh).  I'm using it to get smoother turning on a group of flying agents.
* Removed vestigial SphericalObstacleData. See DetectableObject.
* Added namespaces to the behaviors.  Decided against adding indentation to minimize the number of lines changed in case someone's doing a diff and forgets to exclude whitespace differences.
* Removed SteeringEvent. The class was unnecessary, we can just do everything with standard actions.
* New property Steering.ShouldRetryForce, so that behaviors handling the OnArrival even can indicate to the steering if they should retry the force calculation before giving up (in case any parameters changed).
* SteerForTether is now a post-processing behavior. It now will also average between the impulse pulling it back and the desired velocity.  I like that behavior better.
* SteerForEvasion is now a post-processing behavior.  This helps overlay it with other behaviors in a more natural fashion, since we can steer the vehicle while still keeping away from a target.
* SteerForPursuit has a new parameter to indicate if it should consider the vehicle's velocity on approach. It's false by default.
* Vehicle.Speed no longer has a setter to avoid confusion. In most cases what users mean to do is adjust the maximum speed, and the only vehicle which currently lets you set the speed is AutonomousVehicle, where it would be promptly overwritten by the velocity calculations.
* The setter for Vehicle.Velocity is now protected. See the case for Vehicle.Speed.
* Updated TickedPriorityQueue.dll with bugfixes and new features.
* Sealing SteerForNeighbors.CalculateForce to make it absolutely clear subclasses should not override it.
* New SteerForMatchingVelocity behavior.
* Renamed DesiredSpeed to TargetSpeed, so that it's clear that it's not a function of DesiredVelocity.
* Improvements and new properties on SteerForCohesion and SteerForSeparation.
* New Vehicle.DeltaTime property.
* Removed old, unsupported sample path steering behaviors to avoid confusion. You can find them as gists here: [SteerForPathTrivial](https://gist.github.com/ricardojmendez/88488a8550ea62bfa119), [SteerForPathPredictiveTrivial](https://gist.github.com/ricardojmendez/f4fff18b34faa0ce17bd).
* (Potentially breaking change). Removed redundant BlendIntoAccumulator methods from OpenSteerUtility - they were doing nothing but wrap Lerp variants. No need to have an extra call with parameters in a non-standard order.
* Removed VehicleLookAtOverride behavior. It was unused ever since I separated vehicles into Biped and AutonomousVehicle.

