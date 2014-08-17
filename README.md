# UnitySteer 3.0 beta


## General notes

UnitySteer is a toolkit to help build steering behaviors for autonomous agents in Unity.  Initially based on OpenSteer, UnitySteer has been significantly reworked since it was first translated - the concepts and some of the code remain the same, but it follows a more Unity-like component-oriented philosophy. 

Please read License.txt before using in your projects.  It's short, sweet and up to the point.

You are currently looking at UnitySteer 3.0 *beta*. Make sure you catch up with [the latest UnitySteer blogposts](http://arges-systems.com/blog/category/unitysteer/), as well as reading the [changelog](CHANGELOG.md), to be aware of the latest changes.


## Main repository

If you have obtained this library from somewhere a third party, [you can always find the latest version on Github](https://github.com/ricardojmendez/UnitySteer).

## Tutorials and examples

Looking for a UnitySteer tutorial?  The [UnitySteer Examples](https://github.com/ricardojmendez/UnitySteerExamples) repository contains a series of examples and experimentation notes for you to teach yourself the library basics and how to compose your own agents.

## Dependencies

Starting on 2.5, UnitySteer depends on TickedPriorityQueue. The latest library is now included on this repository.

## Stable and beta versions

The latest stable 2.x release is [UnitySteer 2.7](https://github.com/ricardojmendez/UnitySteer/tree/v2.7)

If you're looking for a specific version, you can look at the project tags:

* [UnitySteer 2.7](https://github.com/ricardojmendez/UnitySteer/tree/v2.7)
* [UnitySteer 2.5.1](https://github.com/ricardojmendez/UnitySteer/tree/v2.5.1)


## UnitySteer and iOS

If you are using UnitySteer on iOS, bear in mind that you may need to search for and change any LINQ calls, since Unity has a penchant for not AOT'ing them properly.
