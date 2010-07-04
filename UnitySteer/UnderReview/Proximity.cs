// ----------------------------------------------------------------------------
//
// Ported to Unity by Ricardo J. Mendez http://www.arges-systems.com/
//
// OpenSteer - pure .net port
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


namespace UnitySteer
{
    public class AbstractTokenForProximityDatabase
    {
        public virtual void updateForNewPosition (Vector3 position) { }

        // find all neighbors within the given sphere (as center and radius)
        public virtual void findNeighbors(Vector3 center, float radius, ArrayList results) { }
    }

    public class AbstractProximityDatabase
    {
        // type for the "tokens" manipulated by this spatial database
        //typedef AbstractTokenForProximityDatabase<ContentType> tokenType;
        // allocate a token to represent a given client object in this database
        public virtual AbstractTokenForProximityDatabase allocateToken(SteeringVehicle parentObject) { return new AbstractTokenForProximityDatabase(); }

        // returns the number of tokens in the proximity database
        public virtual int getPopulation() { return 0; }

        public virtual SteeringVehicle getNearestVehicle(Vector3 position, float radius) { return null; }

        public virtual Vector3 getMostPopulatedBinCenter() { return Vector3.zero; } 
        
        public virtual void RemoveToken(AbstractTokenForProximityDatabase token) {}
    };


    // O(n^2) ftw!
    public class BruteForceProximityDatabase : AbstractProximityDatabase
    {
   
        // STL vector containing all tokens in database
        public ArrayList group;

        // constructor
        public BruteForceProximityDatabase ()
        {
            group = new ArrayList();
        }

        // allocate a token to represent a given client object in this database
        //public override tokenType allocateToken (Object parentObject)
        public override AbstractTokenForProximityDatabase allocateToken(SteeringVehicle parentObject)
        {
            tokenType tToken=new tokenType (parentObject, this);
            return (AbstractTokenForProximityDatabase)tToken;
        }
        
        public override void RemoveToken(AbstractTokenForProximityDatabase token) 
        {
            group.Remove(token);
        }

        // return the number of tokens currently in the database
        public override int getPopulation()
        {
            return group.Count;
        }
    };

    // "token" to represent objects stored in the database
    public class tokenType : AbstractTokenForProximityDatabase
    {
        BruteForceProximityDatabase bfpd;
        SteeringVehicle tParentObject;
        Vector3 position;

        // constructor
        public tokenType(SteeringVehicle parentObject, BruteForceProximityDatabase pd)
        {
            // store pointer to our associated database and the object this
            // token represents, and store this token on the database's vector
            bfpd = pd;
            tParentObject = parentObject;
            bfpd.group.Add(this);
        }

        // destructor
        ~tokenType()
        {
            // remove this token from the database's ArrayList
            bfpd.group.Remove(this);
        }

        // the client object calls this each time its position changes
        public override void updateForNewPosition(Vector3 newPosition)
        {
            position = newPosition;
        }

        // find all neighbors within the given sphere (as center and radius)
        public override void findNeighbors(Vector3 center, float radius, ArrayList results)
        {
            // loop over all tokens
            float r2 = radius * radius;

            //bfpd.group.ForEach(delegate(tokenType tToken)
            for (int i=0;i<bfpd.group.Count;i++)
            {
                tokenType tToken=(tokenType) bfpd.group[i]; 

                //for (tokenIterator i = bfpd->group.begin(); i != bfpd->group.end(); i++)
                //{
                Vector3 offset = center - tToken.position;
                float d2 = offset.sqrMagnitude;
                
                // push onto result vector when within given radius
                if (d2 < r2) results.Add(tToken.tParentObject);//.push_back ((**i).object);
            };
        }
    };



}
