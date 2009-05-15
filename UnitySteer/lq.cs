using System;
using System.Collections;
using System.Text;
using UnityEngine;

namespace UnitySteer
{
    public class lqClientProxy
    {   
      
        // Active bin 
        public lqBin bin=null;

        // Pointer to client object
        public System.Object clientObject;

        //* The object's location ("key point") used for spatial sorting
        public float x;
        public float y;
        public float z;

       

        public lqClientProxy(System.Object tClientObject)
        {
            
            clientObject = tClientObject;
            x = 0f;
            y = 0f;
            z = 0f;
        }
    } 

    // Class used to store each list of clients
    public class lqBin
    {
        public ArrayList clientList;
        public Vector3 center;

        public lqBin(Vector3 binCenter)
        {
            clientList=new ArrayList();//<lqClientProxy>();
            center = binCenter;
        }
    }

    class locationQueryDatabase
    {

        /* the origin is the super-brick corner minimum coordinates */
        public float originx, originy, originz;

        // length of the edges of the super-brick
        public float sizex, sizey, sizez;

        // number of sub-brick divisions in each direction 
        public int divx, divy, divz;

        // pointer to an array of pointers, one for each bin 
        public lqBin[] bins;

        //extra bin for "everything else" (points outside super-brick) 
        public lqBin other;

        int bincount;

        public locationQueryDatabase (float _originx, float _originy, float _originz,
				        float _sizex, float _sizey, float _sizez,
				        int _divx, int _divy, int _divz)
        {
            originx = _originx;
            originy = _originy;
            originz = _originz;
            sizex = _sizex;
            sizey = _sizey;
            sizez = _sizez;
            divx = _divx;
            divy = _divy;
            divz = _divz;
           
	        int i;
	        bincount = divx * divy * divz;

            bins = new lqBin[bincount];// List<lqBin>(); 

            for (int x = 0; x < divx; x++)
            {
                for (int y = 0; y < divy; y++)
                {
                    for (int z = 0; z < divz; z++)
                    {
                        i = (int) ((x * divy * divz) + (y * divz) + z);
                        float tx = originx + ((float)x) * ((float)sizex / (float)divx);// -(sizex / 2f);

                        float ty = originy + ((float)y) * ((float)sizey / (float)divy); // -(sizey / 2f);
                        float tz = originz + ((float)z) * ((float)sizez / (float)divz);// -(sizez / 2f);

                        Vector3 binCenter = new Vector3(tx, ty, tz); 
                        bins[i] = new lqBin(binCenter);
                        //Console.WriteLine("Bin is " + i);
                    }
                }
            }
            /*
	        for (i=0; i<bincount; i++) {

                ix = (int)(((x - originx) / sizex) * divx);
                iy = (int)(((y - originy) / sizey) * divy);
                iz = (int)(((z - originz) / sizez) * divz);
                //return (int) ((ix * divy * divz) + (iy * divz) + iz);
                bins[i]=new lqBin();
            }
            */
            other = new lqBin(Vector3.zero);
        }



        // ------------------------------------------------------------------ 
        // Determine index into linear bin array given 3D bin indices 

        public int lqBinCoordsToBinIndex(float ix, float iy, float iz)
        {
            return (int) ((ix * divy * divz) + (iy * divz) + iz);
        }


        /* ------------------------------------------------------------------ */
        /* Find the bin ID for a location in space.  The location is given in
           terms of its XYZ coordinates.  The bin ID is a pointer to a pointer
           to the bin contents list.  */


        public lqBin lqBinForLocation ( float x, float y, float z)
        {
            int i, ix, iy, iz;

            /* if point outside super-brick, return the "other" bin */
            if (x < originx)              return (other);
            if (y < originy)              return (other);
            if (z < originz)              return (other);
            if (x >= originx + sizex) return (other);
            if (y >= originy + sizey) return (other);
            if (z >= originz + sizez) return (other);

            /* if point inside super-brick, compute the bin coordinates */
            ix = (int) (((x - originx) / sizex) * divx);
            iy = (int) (((y - originy) / sizey) * divy);
            iz = (int) (((z - originz) / sizez) * divz);

            /* convert to linear bin number */
            i = lqBinCoordsToBinIndex ( ix, iy, iz);

            if (i < 0 || i >= bincount) return other;

            /* return pointer to that bin */
            return bins[i];
        }


        // Adds a given client object to a given bin, linking it into the bin
        // contents list.

        public void lqAddToBin (lqClientProxy clientObject, lqBin bin)
        {

            bin.clientList.Add(clientObject);
            clientObject.bin=bin;
        }


        // Removes a given client object from its current bin, unlinking it
        //   from the bin contents list. 

        public void lqRemoveFromBin (lqClientProxy clientObject)
        {
            if (clientObject.bin!=null) {
                clientObject.bin.clientList.Remove(clientObject);
            }   
        }


        // Call for each client object every time its location changes.  For
        //   example, in an animation application, this would be called each
        //   frame for every moving object.  


        public void lqUpdateForNewLocation  (lqClientProxy clientObject, float x, float y, float z)
        {
            // find bin for new location 
            lqBin newBin = lqBinForLocation ( x, y, z);

            // store location in client object, for future reference
            clientObject.x = x;
            clientObject.y = y;
            clientObject.z = z;

            /* has object moved into a new bin? */
            if (newBin != clientObject.bin)
            {
	            lqRemoveFromBin (clientObject);
 	            lqAddToBin (clientObject, newBin);
            }
        }

        // Given a bin's list of client proxies, traverse the list and invoke
        //   the given lqCallBackFunction on each object that falls within the
        //   search radius.  

        public ArrayList getBinClientObjectList(lqBin bin, float x, float y, float z, float radiusSquared) 
        {
            //List<lqClientProxy> tList = new List<lqClientProxy>();
            ArrayList tList = new ArrayList();
            for (int i = 0; i < bin.clientList.Count; i++)
            {
                //bin.clientList..clientList.ForEach(delegate(lqClientProxy tClientObject) {
                lqClientProxy tClientObject = (lqClientProxy) bin.clientList[i];


                /* compute distance (squared) from this client   */
                /* object to given locality sphere's centerpoint */
                float dx = x - tClientObject.x;
                float dy = y - tClientObject.y;
                float dz = z - tClientObject.z;
                float distanceSquared = (dx * dx) + (dy * dy) + (dz * dz);

                /* apply function if client object within sphere */
                if (distanceSquared < radiusSquared) tList.Add(tClientObject);
                //(*func) (co->object, distanceSquared, state);             
            }                                   
            //});
            return tList;
        }



        /* ------------------------------------------------------------------ */
        /* This subroutine of lqMapOverAllObjectsInLocality efficiently
           traverses of subset of bins specified by max and min bin
           coordinates. */


        public ArrayList getAllClientObjectsInLocalityClipped (  float x, float y, float z,
					           float radius,
					           int minBinX, int minBinY, int minBinZ,
					           int maxBinX, int maxBinY, int maxBinZ)
        {
            int i, j, k;
            int iindex, jindex, kindex;
            int slab = divy * divz;
            int row = divz;
            int istart = minBinX * slab;
            int jstart = minBinY * row;
            int kstart = minBinZ;
            //lqClientProxy* co;
            lqBin bin;
            float radiusSquared = radius * radius;

            ArrayList returnList = new ArrayList();

            // loop for x bins across diameter of sphere 
            iindex = istart;
            for (i = minBinX; i <= maxBinX; i++)
            {
	            // loop for y bins across diameter of sphere
	            jindex = jstart;
	            for (j = minBinY; j <= maxBinY; j++)
	            {
	                // loop for z bins across diameter of sphere
	                kindex = kstart;
	                for (k = minBinZ; k <= maxBinZ; k++)
	                {
		                // get current bin's client object list
		                bin = bins[iindex + jindex + kindex];

		                // traverse current bin's client object list
                        ArrayList tSubList = getBinClientObjectList(bin,x,y,z, radiusSquared);
                        returnList.AddRange(tSubList);
					                   //func,
					                   //clientQueryState);
		                kindex += 1;
	                }
	                jindex += row;
	            }
	            iindex += slab;
            }
            return returnList;
        }


        // ------------------------------------------------------------------
        // If the query region (sphere) extends outside of the "super-brick"
        //   we need to check for objects in the catch-all "other" bin which
        //   holds any object which are not inside the regular sub-bricks  


        public ArrayList getAllOutsideObjects ( float x, float y, float z,float radius)
        {
            float radiusSquared = radius * radius;
            return(getBinClientObjectList(other,x,y,z,radiusSquared));
        }


        // ------------------------------------------------------------------
        // Apply an application-specific function to all objects in a certain
        //   locality.  The locality is specified as a sphere with a given
        //   center and radius.  All objects whose location (key-point) is
        //   within this sphere are identified and the function is applied to
        //   them.  The application-supplied function takes three arguments:
        //
        //     (1) a void* pointer to an lqClientProxy's "object".
        //     (2) the square of the distance from the center of the search
        //         locality sphere (x,y,z) to object's key-point.
        //     (3) a void* pointer to the caller-supplied "client query state"
        //         object -- typically NULL, but can be used to store state
        //         between calls to the lqCallBackFunction.
        //
        //   This routine uses the LQ database to quickly reject any objects in
        //   bins which do not overlap with the sphere of interest.  Incremental
        //   calculation of index values is used to efficiently traverse the
        //   bins of interest.


        public ArrayList getAllObjectsInLocality (float x, float y, float z, float radius)
        {
            bool partlyOut = false;
            bool completelyOutside = 
	        (((x + radius) < originx) ||
	         ((y + radius) < originy) ||
	         ((z + radius) < originz) ||
	         ((x - radius) >= originx + sizex) ||
	         ((y - radius) >= originy + sizey) ||
	         ((z - radius) >= originz + sizez));

            int minBinX, minBinY, minBinZ, maxBinX, maxBinY, maxBinZ;

            

            // is the sphere completely outside the "super brick"?
            if (completelyOutside)
            {
                return getAllOutsideObjects(x, y, z, radius); 
            }

            ArrayList returnList=new ArrayList();

            /* compute min and max bin coordinates for each dimension */
            minBinX = (int) ((((x - radius) - originx) / sizex) * divx);
            minBinY = (int) ((((y - radius) - originy) / sizey) * divy);
            minBinZ = (int) ((((z - radius) - originz) / sizez) * divz);
            maxBinX = (int) ((((x + radius) - originx) / sizex) * divx);
            maxBinY = (int) ((((y + radius) - originy) / sizey) * divy);
            maxBinZ = (int) ((((z + radius) - originz) / sizez) * divz);

            /* clip bin coordinates */
            if (minBinX < 0)         {partlyOut = true; minBinX = 0;}
            if (minBinY < 0)         {partlyOut = true; minBinY = 0;}
            if (minBinZ < 0)         {partlyOut = true; minBinZ = 0;}
            if (maxBinX >= divx) {partlyOut = true; maxBinX = divx - 1;}
            if (maxBinY >= divy) {partlyOut = true; maxBinY = divy - 1;}
            if (maxBinZ >= divz) {partlyOut = true; maxBinZ = divz - 1;}

            // map function over outside objects if necessary (if clipped)
            if (partlyOut) returnList.AddRange(getAllOutsideObjects(x, y, z, radius));
				                        
            // map function over objects in bins 
            returnList.AddRange(getAllClientObjectsInLocalityClipped(
					          x, y,z,radius,
					          minBinX, minBinY, minBinZ,
					          maxBinX, maxBinY, maxBinZ));
            return returnList;
        }




        /* ------------------------------------------------------------------ */
        /* Search the database to find the object whose key-point is nearest
           to a given location yet within a given radius.  That is, it finds
           the object (if any) within a given search sphere which is nearest
           to the sphere's center.  The ignoreObject argument can be used to
           exclude an object from consideration (or it can be NULL).  This is
           useful when looking for the nearest neighbor of an object in the
           database, since otherwise it would be its own nearest neighbor.
           The function returns a void* pointer to the nearest object, or
           NULL if none is found.  */


        
        public lqClientProxy lqFindNearestNeighborWithinRadius ( 
					         float x, float y, float z,
					         float radius,
					         System.Object ignoreObject)
        {



            float minDistanceSquared = float.MaxValue;

            // map search helper function over all objects within radius 
            ArrayList foundList=getAllObjectsInLocality( x, y, z, radius);

            lqClientProxy nearestObject=null;

            for (int i = 0; i < foundList.Count; i++)
            {
                lqClientProxy tProxyObject = (lqClientProxy)foundList[i];
                //foundList.ForEach(delegate(lqClientProxy tProxyObject)
                //{
                if (tProxyObject != ignoreObject)
                {
                    float dx = tProxyObject.x - x;
                    float dy = tProxyObject.y - y;
                    float dz = tProxyObject.z - z;

                    float distanceSquared = dx * dx + dy * dy + dz * dz;
                    if (distanceSquared < minDistanceSquared)
                    {
                        nearestObject = tProxyObject;
                        minDistanceSquared = distanceSquared;
                    }
                }
            }
            //});
            return nearestObject;
        }
        

 

        /* ------------------------------------------------------------------ */
        /* Apply a user-supplied function to all objects in the database,
           regardless of locality (cf lqMapOverAllObjectsInLocality) */

        public ArrayList getAllObjects ()
        {
            int i;
            int bincount = divx * divy * divz;

            ArrayList returnList = new ArrayList();
            for (i=0; i<bincount; i++)
            {
                returnList.AddRange(bins[i].clientList);//, func, clientQueryState));
            }
            returnList.AddRange(other.clientList);//, func, clientQueryState));
            return returnList;
        }


        /* ------------------------------------------------------------------ */
        /* internal helper function */


        public void lqRemoveAllObjectsInBin(lqBin bin)
        {
            bin.clientList.Clear();
        }

        /* ------------------------------------------------------------------ */
        /* Removes (all proxies for) all objects from all bins */


        public void lqRemoveAllObjects ()
        {
            int i;
            int bincount = divx * divy * divz;

            for (i=0; i<bincount; i++)
            {
	            lqRemoveAllObjectsInBin (bins[i]);
            }
            lqRemoveAllObjectsInBin (other);
        }

        public Vector3 getMostPopulatedBinCenter()
        {

            lqBin mostPopulatedBin = getMostPopulatedBin();
            if (mostPopulatedBin != null)
            {
                return mostPopulatedBin.center;
            }
            else return Vector3.zero;
        }

        public lqBin getMostPopulatedBin()
        {
            int i;
            int bincount = divx * divy * divz;
            int largestPopulation=0;
            lqBin mostPopulatedBin = null;

            for (i = 0; i < bincount; i++)
            {
                if (bins[i].clientList.Count > largestPopulation)
                {
                    largestPopulation = bins[i].clientList.Count;
                    mostPopulatedBin = bins[i];
                }
            }
            // We will ignore other for now. Hope that works out ok
            return mostPopulatedBin;
        }
    }
}
