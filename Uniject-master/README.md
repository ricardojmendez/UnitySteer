Uniject
=======

The <a href="http://en.wikipedia.org/wiki/Software_testability">testability</a> framework for <a href="http://www.unity3d.com">Unity3D</a>, offering:

* Plain Old C Sharp, testable MonoBehaviour equivalents
* Unit/integration test your code outside of Unity in your IDE of choice
* A robust and flexible way of creating GameObjects automatically, by inference of the code that drives them
* Constructors!
* An extremely flexible code base – in short, the benefits of <a href="http://en.wikipedia.org/wiki/Dependency_injection">Dependency injection</a> + <a href="http://en.wikipedia.org/wiki/Inversion_of_control">Inversion of control</a>

<dl>
  <dt>Prerequisites</dt>
</dl>

* MonoDevelop 3.0 - NOT the Unity bundled version

The Unity bundled version of Monodevelop is borked; the unit test runner is broken. Get the latest version from <a href="http://monodevelop.com/Download">monodevelop.com</a>

<dl>
  <dt>To get started</dt>
</dl>

* Open the project in Unity
* Choose Assets/Sync Monodevelop project to rebuild the Unity managed csproj files
* In MonoDevelop 3.0, open the test project at /src/test/Test.sln
* Run the unit tests
* Load and run the example scenes within Unity

<dl>
  <dt>To build and run the Unit tests from the terminal (OSX)</dt>
</dl>

Run BUILD.sh

<dl>
  <dt>To run the example scene</dt>
</dl>

Open the project in Unity and load the 'example' scene.

<a href="http://outlinegames.com/2012/08/29/on-testability/">Read more about Uniject</a>