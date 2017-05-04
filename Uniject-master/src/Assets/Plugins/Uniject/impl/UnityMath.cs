using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Uniject.Impl
{
	public class UnityMath : Uniject.IMaths {

        private System.Random rand = new System.Random();

        public bool trueOneInN(int n) {
            return 0 == UnityEngine.Random.Range(0, n);
        }

        public float randomNormalisedNeg1to1() {
            return UnityEngine.Random.Range(-1.0f, 1.0f);
        }

        public float randomNormalised() {
            return UnityEngine.Random.Range(0.0f, 1.0f);
        }

        public int randZeroToNMinusOne(int n) {
            return rand.Next(n);
        }

        public Quaternion LookRotation(Vector3 direction, Vector3 up) {
            return Quaternion.LookRotation(direction, up);
        }
    }
}
