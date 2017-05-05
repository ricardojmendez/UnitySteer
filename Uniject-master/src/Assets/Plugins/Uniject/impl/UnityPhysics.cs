using System;
using Uniject;
using UnityEngine;

namespace Uniject.Impl {

    /// <summary>
    /// Bridges UnityEngine.Physics to Uniject.Physics.
    /// </summary>
    public class UnityPhysics : IPhysics {

        public bool Raycast(Vector3 origin, Vector3 direction, float distance, int layerMask) {
            return UnityEngine.Physics.Raycast(origin, direction, distance, layerMask);
        }

        public bool Raycast(Vector3 origin, Vector3 direction, out Uniject.RaycastHit hitinfo, float distance, int layerMask) {
            UnityEngine.RaycastHit unityHit = new UnityEngine.RaycastHit ();
            bool result = UnityEngine.Physics.Raycast(origin, direction, out unityHit, distance, layerMask);

            if (result) {

                TestableGameObject testable = null;
                UnityGameObjectBridge bridge = unityHit.collider.gameObject.GetComponent<UnityGameObjectBridge>();
                if (null != bridge) {
                    testable = bridge.wrapping;
                }

                hitinfo = new RaycastHit (unityHit.point,
                                         unityHit.normal,
                                         unityHit.barycentricCoordinate,
                                         unityHit.distance,
                                         unityHit.triangleIndex,
                                         unityHit.textureCoord,
                                         unityHit.textureCoord2,
                                         unityHit.lightmapCoord,
                                         testable,
                                         unityHit.collider);
            } else {
                hitinfo = new RaycastHit();
            }

            return result;
        }
    }
}
