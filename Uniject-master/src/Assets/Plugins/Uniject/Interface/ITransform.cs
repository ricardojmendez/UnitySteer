using System;
using UnityEngine;

namespace Uniject {
    public interface ITransform {
        Vector3 Position {
            get;
            set;
        }

        Vector3 localScale {
            get;
            set;
        }

        Quaternion Rotation {
            get;
            set;
        }

        Vector3 Forward {
            get;
            set;
        }

        Vector3 Up {
            get;
            set;
        }

        ITransform Parent { get; set; }

        void Translate(Vector3 byVector);
        void LookAt(Vector3 point);
        Vector3  TransformDirection(Vector3 dir);
    }
}

