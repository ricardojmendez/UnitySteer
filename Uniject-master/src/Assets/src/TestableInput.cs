using System;
using Uniject;
using UnityEngine;

/// <summary>
/// A demonstration of testable input using IInput.
/// The scene contains a sphere on a plane that is controlled 
/// using the horizontal and vertical input axes.
/// Inputs are translated to forces applied to the sphere's rigid body.
/// </summary>
public class TestableInput : TestableComponent {
    public Sphere sphere { get; private set; }
    private IInput input;
    public TestableInput(TestableGameObject obj, IInput input,
                         Sphere sphere) : base(obj) {
        this.sphere = sphere;
        this.input = input;
    }

    public override void Update() {
        float deltaX = input.GetAxis("Horizontal");
        float deltaY = input.GetAxis("Vertical");
        Vector3 forceVec = (Vector3.right * deltaX + Vector3.forward * deltaY);
        if (forceVec.sqrMagnitude > 0) {
            sphere.body.AddForce(forceVec);
        }
    }
}
