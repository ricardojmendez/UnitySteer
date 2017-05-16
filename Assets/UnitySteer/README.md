# UnitySteer 3.1  

[![Build Status](https://travis-ci.org/GandaG/UnitySteer.svg?branch=feature%2Ftravis_integration)](https://travis-ci.org/GandaG/UnitySteer)

## General notes

UnitySteer is a toolkit to help build steering behaviors for autonomous agents in Unity.  Initially based on OpenSteer, UnitySteer has been significantly reworked since it was first translated - the concepts and some of the code remain the same, but it follows a more Unity-like component-oriented philosophy. 

Please read License.txt before using in your projects.  It's short, sweet and to the point.


## Main repository

If you have obtained this library from a third party, [you can always find the latest version on Github](https://github.com/ricardojmendez/UnitySteer).

## Tutorials and examples

Looking for a UnitySteer tutorial?  The [UnitySteer Examples](https://github.com/ricardojmendez/UnitySteerExamples) repository contains a series of examples and experimentation notes for you to teach yourself the library basics and how to compose your own agents.

## Dependencies

UnitySteer uses [TickedPriorityQueue](https://github.com/Garufortho/TickedPriorityQueue). The latest library is now included on this repository.

UnitySteer 3.1 requires Unity 5.x for 2D support. The last version to support Unity 4.x was [UnitySteer 3.0](https://github.com/ricardojmendez/UnitySteer/tree/v3.0.0).

## Stable and beta versions

The current stable release is UnitySteer 3.0 RC2.  It contains a significant number of improvements and fixes over 2.7, but it also introduced several breaking changes, so make sure you catch up with [the latest UnitySteer blogposts](http://numergent.com/tags/unitysteer/), as well as reading the [changelog](CHANGELOG.md).

I develop UnitySteer following git-flow, so if you're looking for a specific version, you can look at the project tags:

* [UnitySteer 3.0](https://github.com/ricardojmendez/UnitySteer/tree/v3.0.0)
* [UnitySteer 2.7](https://github.com/ricardojmendez/UnitySteer/tree/v2.7)


## UnitySteer and iOS

If you are using UnitySteer on iOS, bear in mind that you may need to search for and change any LINQ calls, since Unity has a penchant for not AOT'ing them properly.
