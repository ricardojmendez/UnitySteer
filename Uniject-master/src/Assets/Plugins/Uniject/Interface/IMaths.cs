using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Uniject
{
	public interface IMaths {
        bool trueOneInN(int n);
        float randomNormalised();
        float randomNormalisedNeg1to1();
        int randZeroToNMinusOne(int n);
        Quaternion LookRotation(Vector3 direction, Vector3 up);
	}
}
