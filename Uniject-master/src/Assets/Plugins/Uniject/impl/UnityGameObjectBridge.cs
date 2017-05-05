using System;
using Uniject.Impl;
using UnityEngine;

public class UnityGameObjectBridge : MonoBehaviour {
    public void OnDestroy() {
        wrapping.Destroy();
    }

    public void Update() {
        wrapping.Update();
    }

    public void OnCollisionEnter(Collision c) {
        UnityGameObjectBridge other = c.gameObject.GetComponent<UnityGameObjectBridge>();
        if (null != other) {
                Uniject.Collision testableCollision =
                new Uniject.Collision(c.relativeVelocity,
                                      other.wrapping.transform,
                                      other.wrapping,
                                      c.contacts);
            wrapping.OnCollisionEnter(testableCollision);
        }
    }

    public UnityGameObject wrapping;
}

