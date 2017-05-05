using System;
using Uniject;

/// <summary>
/// A rotating box that casts a ray into the world,
/// placing a sphere at the point of intersection.
/// </summary>
public class ScanningLaser : TestableComponent {

    private IPhysics physics;
    private Sphere sphere;
    private int mask;
    public ScanningLaser(TestableGameObject obj,
                         Box box,
                         [GameObjectBoundary] Sphere sphere,
                         IPhysics physics,
                         IRigidBody body,
                         ILayerMask layerMask) : base(obj) {
        this.physics = physics;
        this.sphere = sphere;
        body.useGravity = false;
        sphere.collider.enabled = false;
        box.Obj.transform.localScale = new UnityEngine.Vector3(10, 1, 1);
        sphere.body.isKinematic = true;
        body.AddTorque(new UnityEngine.Vector3(0, 5, 0), UnityEngine.ForceMode.Impulse);
        mask = 1 << layerMask.NameToLayer("Default");
    }

    public override void Update() {
        RaycastHit hit = new RaycastHit();
        if (physics.Raycast(Obj.transform.Position, Obj.transform.Forward, out hit, float.MaxValue, mask)) {
            sphere.obj.transform.Position = Obj.transform.Position + Obj.transform.Forward * hit.distance;
        }
    }
}
