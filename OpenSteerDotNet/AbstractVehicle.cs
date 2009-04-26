// ----------------------------------------------------------------------------
//
// OpenSteerDotNet - pure .net port
// Port by Simon Oliver - http://www.handcircus.com
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

using System;
using System.Collections;
using System.Text;
using UnityEngine;

namespace OpenSteerDotNet
{
    public class AbstractVehicle : LocalSpace
    {
        // mass (defaults to unity so acceleration=force)
        public virtual float mass() { return 0; }
        public virtual float setMass(float mass) { return 0; }

        // size of bounding sphere, for obstacle avoidance, etc.
        public virtual float radius() { return 0; }
        public virtual float setRadius(float radius) { return 0; }

        // velocity of vehicle
        public virtual Vector3 velocity() { return Vector3.zero; }

        // speed of vehicle  (may be faster than taking magnitude of velocity)
        public virtual float speed() { return 0; }
        public virtual float setSpeed(float speed) { return 0; }

        // groups of (pointers to) abstract vehicles, and iterators over them
        //typedef std::vector<AbstractVehicle*> group;
        //typedef group::const_iterator iterator;    

        // predict position of this vehicle at some time in the future
        // (assumes velocity remains constant)
        public virtual Vector3 predictFuturePosition(float predictionTime) { return Vector3.zero; }

        // ----------------------------------------------------------------------
        // XXX this vehicle-model-specific functionality functionality seems out
        // XXX of place on the abstract base class, but for now it is expedient

        // the maximum steering force this vehicle can apply
        public virtual float maxForce() { return 0; }
        public virtual float setMaxForce(float max) { return 0; }

        // the maximum speed this vehicle is allowed to move
        public virtual float maxSpeed() { return 0; }
        public virtual float setMaxSpeed(float max) { return 0; }
        //public virtual void newPD(AbstractProximityDatabase pd) { }
    }
}
