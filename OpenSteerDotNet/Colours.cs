using System;
using System.Collections;
using System.Text;
using UnityEngine;

namespace OpenSteerDotNet
{
    class OpenSteerColours
    {
        public static Vector3 gBlack = new Vector3(0, 0, 0);
        public static Vector3 gWhite = new Vector3(1, 1, 1);

        public static Vector3 gRed = new Vector3(1, 0, 0);
        public static Vector3 gYellow = new Vector3(1, 1, 0);
        public static Vector3 gGreen = new Vector3(0, 1, 0);
        public static Vector3 gCyan = new Vector3(0, 1, 1);
        public static Vector3 gBlue = new Vector3(0, 0, 1);
        public static Vector3 gMagenta = new Vector3(1, 0, 1);

        public static Vector3 gOrange = new Vector3(1, 0.5f, 0);

        public static Vector3 grayColor(float g) { return new Vector3(g, g, g); }

        public static Vector3 gGray10 = grayColor(0.1f);
        public static Vector3 gGray20 = grayColor(0.2f);
        public static  Vector3 gGray30 = grayColor(0.3f);
        public static Vector3 gGray40 = grayColor(0.4f);
        public static Vector3 gGray50 = grayColor(0.5f);
        public static Vector3 gGray60 = grayColor(0.6f);
        public static Vector3 gGray70 = grayColor(0.7f);
        public static Vector3 gGray80 = grayColor(0.8f);
        public static Vector3 gGray90 = grayColor(0.9f);
    }
}
