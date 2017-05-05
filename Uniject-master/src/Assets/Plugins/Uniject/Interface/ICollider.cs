using System;

namespace Uniject {
    public interface ICollider {
        bool enabled { get; set; }
        IPhysicMaterial material { get; set; }
    }
}

