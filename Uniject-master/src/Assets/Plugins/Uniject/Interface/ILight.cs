using System;
using UnityEngine;

namespace Uniject {
    public interface ILight {
        bool enabled { get; set; }
        LightType type { get; set; }
        Color color { get; set; }
        float intensity { get; set; }
        LightShadows shadows { get; set; }
        float shadowStrength { get; set; }
        float shadowBias { get; set; }
        float shadowSoftness { get; set; }
        float shadowSoftnessFade { get; set; }
        float range { get; set; }
        float spotAngle { get; set; }
        LightRenderMode renderMode { get; set; }
        int cullingMask { get; set; }
    }
}

