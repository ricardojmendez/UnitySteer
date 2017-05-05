using System;
using UnityEngine;

namespace Uniject.Impl {
    public class UnityLight : ILight {

        private Light light;
        public UnityLight(GameObject obj) {
            light = obj.GetComponent<Light>();
            if (null == light) {
                light = obj.AddComponent<Light>();
            }
        }

        public bool enabled {
            get { return light.enabled; }
            set { light.enabled = value; }
        }

        public LightType type {
            get { return light.type; }
            set { light.type = value; }
        }

        public Color color {
            get { return light.color; }
            set { light.color = value; }
        }

        public float intensity {
            get { return light.intensity; }
            set { light.intensity = value; }
        }

        public LightShadows shadows {
            get { return light.shadows; }
            set { light.shadows = value; }
        }

        public float shadowStrength {
            get { return light.shadowStrength; }
            set { light.shadowStrength = value; }
        }

        public float shadowBias {
            get { return light.shadowBias; }
            set { light.shadowBias = value; }
        }

        public float shadowSoftness {
            get { return light.shadowSoftness; }
            set { light.shadowSoftness = value; }
        }

        public float shadowSoftnessFade {
            get { return light.shadowSoftnessFade; }
            set { light.shadowSoftnessFade = value; }
        }

        public float range {
            get { return light.range; }
            set { light.range = value; }
        }

        public float spotAngle {
            get { return light.spotAngle; }
            set { light.spotAngle = value; }
        }

        public LightRenderMode renderMode {
            get { return light.renderMode; }
            set { light.renderMode = value; }
        }

        public int cullingMask {
            get { return light.cullingMask; }
            set { light.cullingMask = value; }
        }
    }
}

