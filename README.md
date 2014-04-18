# UnitySteer

## General notes

Steering behaviors for Unity, initially based on OpenSteer. Please read License.txt before using in your projects.

Starting on 2.5, UnitySteer depends on TickedPriorityQueue. The latest  library is now included on this repository.

Perusing the latest changelog is strongly recommended.

If you're looking for a specific version, you can look at the project tags:

* [UnitySteer 2.7](https://github.com/ricardojmendez/UnitySteer/tree/v2.7)
* [UnitySteer 2.5.1](https://github.com/ricardojmendez/UnitySteer/tree/v2.5.1)

## Stable and alpha versions

The latest stable 2.x release is [UnitySteer 2.7](https://github.com/ricardojmendez/UnitySteer/tree/v2.7)

You are currently looking at UnitySteer 3.0 *alpha*. Make sure you catch up with [the latest UnitySteer blogposts](http://arges-systems.com/blog/category/unitysteer/), as well as reading the changelog, to be aware of the latest changes.


## UnitySteer and iOS

If you are using UnitySteer on iOS, bear in mind that you may need to change
LINQ calls, since Unity has a penchant for not AOT'ing them properly.
