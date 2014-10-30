# UnitySteer 3.0 (RC2)


## General notes

UnitySteer is a toolkit to help build steering behaviors for autonomous agents in Unity.  Initially based on OpenSteer, UnitySteer has been significantly reworked since it was first translated - the concepts and some of the code remain the same, but it follows a more Unity-like component-oriented philosophy. 

Please read License.txt before using in your projects.  It's short, sweet and to the point.


## Main repository

If you have obtained this library from a third party, [you can always find the latest version on Github](https://github.com/ricardojmendez/UnitySteer).

## Tutorials and examples

Looking for a UnitySteer tutorial?  The [UnitySteer Examples](https://github.com/ricardojmendez/UnitySteerExamples) repository contains a series of examples and experimentation notes for you to teach yourself the library basics and how to compose your own agents.

## Dependencies

UnitySteer uses [TickedPriorityQueue](https://github.com/Garufortho/TickedPriorityQueue). The latest library is now included on this repository.

## Stable and beta versions

The current stable release is UnitySteer 3.0 RC2.  It contains a significant number of improvements and fixes over 2.7, but it also introduced several breaking changes, so make sure you catch up with [the latest UnitySteer blogposts](http://arges-systems.com/blog/category/unitysteer/), as well as reading the [changelog](CHANGELOG.md).

I develop UnitySteer following git-flow, so if you're looking for a specific version, you can look at the project tags:

* [UnitySteer 3.0 RC 2](https://github.com/ricardojmendez/UnitySteer/tree/v3.0.0-RC2)
* [UnitySteer 3.0 RC 1](https://github.com/ricardojmendez/UnitySteer/tree/v3.0.0-RC1)
* [UnitySteer 2.7](https://github.com/ricardojmendez/UnitySteer/tree/v2.7)


## UnitySteer and iOS

If you are using UnitySteer on iOS, bear in mind that you may need to search for and change any LINQ calls, since Unity has a penchant for not AOT'ing them properly.
