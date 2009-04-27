using System;
using System.Text;
using UnityEngine;

namespace OpenSteer
{
    public class LocalSpace
    {
// ----------------------------------------------------------------------------
//
// Ported to Unity by Ricardo J. Méndez http://www.arges-systems.com/
//
//
// OpenSteer -- Steering Behaviors for Autonomous Characters
//
// Copyright (c) 2002-2003, Sony Computer Entertainment America
// Original author: Craig Reynolds <craig_reynolds@playstation.sony.com>
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.  IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
//
//
// ----------------------------------------------------------------------------
//
//
// LocalSpace: a local coordinate system for 3d space
//
// Provide functionality such as transforming from local space to global
// space and vice versa.  Also regenerates a valid space from a perturbed
// "forward vector" which is the basis of abnstract vehicle turning.
//
// These are comparable to a 4x4 homogeneous transformation matrix where the
// 3x3 (R) portion is constrained to be a pure rotation (no shear or scale).
// The rows of the 3x3 R matrix are the basis vectors of the space.  They are
// all constrained to be mutually perpendicular and of unit length.  The top
// ("x") row is called "side", the middle ("y") row is called "up" and the
// bottom ("z") row is called forward.  The translation vector is called
// "position".  Finally the "homogeneous column" is always [0 0 0 1].
//
//     [ R R R  0 ]      [ Sx Sy Sz  0 ]
//     [ R R R  0 ]      [ Ux Uy Uz  0 ]
//     [ R R R  0 ]  ->  [ Fx Fy Fz  0 ]
//     [          ]      [             ]
//     [ T T T  1 ]      [ Tx Ty Tz  1 ]
//
// This file defines three classes:
//   AbstractLocalSpace:  pure virtual interface
//   LocalSpaceMixin:     mixin to layer LocalSpace functionality on any base
//   LocalSpace:          a concrete object (can be instantiated)
//
// 10-04-04 bk:  put everything into the OpenSteer namespace
// 06-05-02 cwr: created 
//
//
// ----------------------------------------------------------------------------


// ----------------------------------------------------------------------------


        Vector3 _side;     //    side-pointing unit basis vector
        Vector3 _up;       //  upward-pointing unit basis vector
        Vector3 _forward;  // forward-pointing unit basis vector
        Vector3 _position; // origin of local space


        public Vector3 side     ()  {return _side;}
        public Vector3 up() { return _up; }
        public Vector3 forward() { return _forward; }
        public Vector3 Position {
            get
            {
                return _position; 
            }
        }
        public Vector3 setSide(Vector3 s) { return _side = s; }
        public Vector3 setUp(Vector3 u) { return _up = u; }
        public Vector3 setForward(Vector3 f) { return _forward = f; }
        public Vector3 setPosition(Vector3 p) { return _position = p; }
        public Vector3 setSide(float x, float y, float z) { return _side = new Vector3(x, y, z); }
        public Vector3 setUp(float x, float y, float z) { return _up = new Vector3(x, y, z); }
        public Vector3 setForward(float x, float y, float z) { return _forward = new Vector3(x, y, z); }
        public Vector3 setPosition(float x, float y, float z) { return _position = new Vector3(x, y, z); }




        // use right-(or left-)handed coordinate space
        public bool rightHanded() { return true; }


        // ------------------------------------------------------------------------
        // reset transform: set local space to its identity state, equivalent to a
        // 4x4 homogeneous transform like this:
        //
        //     [ X 0 0 0 ]
        //     [ 0 1 0 0 ]
        //     [ 0 0 1 0 ]
        //     [ 0 0 0 1 ]
        //
        // where X is 1 for a left-handed system and -1 for a right-handed system.

        public void resetLocalSpace()
        {
            _forward = new Vector3(0, 0, 1);
            _side = localRotateForwardToSide (_forward);
            _up = new Vector3(0, 1, 0);
            _position = new Vector3(0, 0, 0);
        }


        // ------------------------------------------------------------------------
        // transform a direction in global space to its equivalent in local space


        public Vector3 localizeDirection(Vector3 globalDirection)
        {
            // dot offset with local basis vectors to obtain local coordiantes
            return new Vector3 (Vector3.Dot(globalDirection, _side),
                         Vector3.Dot(globalDirection, _up),
                         Vector3.Dot(globalDirection, _forward));
        }


        // ------------------------------------------------------------------------
        // transform a point in global space to its equivalent in local space


        public Vector3 localizePosition(Vector3 globalPosition)
        {
            // global offset from local origin
            Vector3 globalOffset = globalPosition - _position;

            // dot offset with local basis vectors to obtain local coordiantes
            return localizeDirection (globalOffset);
        }


        // ------------------------------------------------------------------------
        // transform a point in local space to its equivalent in global space


        public Vector3 globalizePosition(Vector3 localPosition)
        {
            return _position + globalizeDirection (localPosition);
        }


        // ------------------------------------------------------------------------
        // transform a direction in local space to its equivalent in global space


        public Vector3 globalizeDirection(Vector3 localDirection)
        {
            return ((_side    * localDirection.x) +
                    (_up      * localDirection.y) +
                    (_forward * localDirection.z));
        }


        // ------------------------------------------------------------------------
        // set "side" basis vector to normalized cross product of forward and up


        public void setUnitSideFromForwardAndUp()
        {
            // derive new unit side basis vector from forward and up
            if (rightHanded())
                //_side.cross (_forward, _up);
                _side = Vector3.Cross(_forward, _up);
            else
                //_side.cross(_up, _forward);
                _side = Vector3.Cross(_up, _forward);

            _side.Normalize ();
        }


        // ------------------------------------------------------------------------
        // regenerate the orthonormal basis vectors given a new forward
        // (which is expected to have unit length)


        public void regenerateOrthonormalBasisUF(Vector3 newUnitForward)
        {
            _forward = newUnitForward;

            // derive new side basis vector from NEW forward and OLD up
            setUnitSideFromForwardAndUp ();

            // derive new Up basis vector from new Side and new Forward
            // (should have unit length since Side and Forward are
            // perpendicular and unit length)
            if (rightHanded())
                //_up.cross (_side, _forward);
                _up = Vector3.Cross(_side, _forward);
            else
                //_up.cross (_forward, _side);
                _up = Vector3.Cross(_forward, _side);

        }


        // for when the new forward is NOT know to have unit length

        public void regenerateOrthonormalBasis(Vector3 newForward)
        {
            newForward.Normalize();
            regenerateOrthonormalBasisUF (newForward);
        }


        // for supplying both a new forward and and new up

       public void regenerateOrthonormalBasis (Vector3 newForward,
                                          Vector3 newUp)
        {
            _up = newUp;
            newForward.Normalize();
            regenerateOrthonormalBasis(newForward);
        }


        // ------------------------------------------------------------------------
        // rotate, in the canonical direction, a vector pointing in the
        // "forward" (+Z) direction to the "side" (+/-X) direction


        public Vector3 localRotateForwardToSide (Vector3 v)
        {
            return new Vector3 (rightHanded () ? -v.z : +v.z,
                         v.y,
                         v.x);
        }

        // not currently used, just added for completeness

        public Vector3 globalRotateForwardToSide(Vector3 globalForward)
        {
            Vector3 localForward = localizeDirection (globalForward);
            Vector3 localSide = localRotateForwardToSide (localForward);
            return globalizeDirection (localSide);
        }
    }
} 
