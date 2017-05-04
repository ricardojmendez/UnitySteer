using System;
using UnityEngine;

namespace Uniject {

    /// <summary>
    /// Extracted from UnityEngine.Input.
    /// </summary>
    public interface IInput {
        Vector3 mousePosition { get; }
        bool anyKey { get; }
        bool anyKeyDown { get; }
        string inputString { get; }
        Vector3 acceleration { get; }
        AccelerationEvent[] accelerationEvents { get; }
        int accelerationEventCount { get; }
        Touch[] touches { get; }
        int touchCount { get; }
        bool multiTouchEnabled { get; }

        float GetAxis(string name);
        float GetAxisRaw(string name);
        bool GetButton(string name);
        bool GetButtonDown(string name);
        bool GetButtonUp(string name);
        bool GetKey(string name);
        bool GetKeyDown(string name);
        bool GetKeyUp(string name);
        string[] GetJoystickNames();
        bool GetMouseButton(int button);
        bool GetMouseButtonDown(int button);
        bool GetMouseButtonUp(int button);
        void ResetInputAxes();
        AccelerationEvent GetAccelerationEvent(int index);
        Touch GetTouch(int index);
    }
}

