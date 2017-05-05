using System;
using Uniject;

public class Box {
    public TestableGameObject Obj { get; private set; }
    public Box(TestableGameObject obj, IBoxCollider collider,
               [Resource("mesh/cube")] TestableGameObject cubeMesh) {
        this.Obj = obj;
        cubeMesh.transform.Parent = obj.transform;
    }
}
