using System;
using UnityEngine;

namespace Uniject {
    public interface IRigidBody {
        void AddForce(UnityEngine.Vector3 force);
        void AddTorque(Vector3 torque, ForceMode mode);

        bool enabled { get; set; }

        Quaternion Rotation {
            get;
            set;
        }

        float drag { get; set; }
        float mass { get; set; }

        Vector3 Position {
            get;
        }

        Vector3 Forward {
            get;
        }

        RigidbodyConstraints constraints {
            get;
            set;
        }

        bool useGravity {
            get;
            set;
        }

        bool isKinematic { get; set; }
    }
}
