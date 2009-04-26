using System;
using System.Collections;
using System.Text;

namespace OpenSteerDotNet
{
    public class LQProximityDatabase : AbstractProximityDatabase
    {

        locationQueryDatabase lq;

        // constructor
        public LQProximityDatabase(Vector3 center, Vector3 dimensions, Vector3 divisions)
        {
            Vector3 halfsize = dimensions * 0.5f;
            Vector3 origin = center - halfsize;
            

            lq = new locationQueryDatabase(origin.x, origin.y, origin.z,
                                   dimensions.x, dimensions.y, dimensions.z,
                                   (int)System.Math.Round(divisions.x),
                                   (int)System.Math.Round(divisions.y),
                                   (int)System.Math.Round(divisions.z));
        }

        // destructor
        ~LQProximityDatabase()
        {
            //lqDeleteDatabase(lq);
            //lq = NULL;
        }

        // "token" to represent objects stored in the database
        public class tokenType : AbstractTokenForProximityDatabase
        {
            lqClientProxy proxy;
            locationQueryDatabase lq;

            // constructor
            public tokenType(Object parentObject, LQProximityDatabase lqsd)
            {
                proxy = new lqClientProxy(parentObject);// lqInitClientProxy(proxy, parentObject);
                lq = lqsd.lq;
            }

            // destructor
            ~tokenType()
            {
                lq.lqRemoveFromBin(proxy);
            }

            // the client object calls this each time its position changes
            public override void updateForNewPosition(Vector3 p)
            {
                lq.lqUpdateForNewLocation(proxy, p.x, p.y, p.z);
            }

            // find all neighbors within the given sphere (as center and radius)
            public override void findNeighbors(Vector3 center, float radius, ArrayList results)
            {
                //lqMapOverAllObjectsInLocality(lq,

                ArrayList tList = lq.getAllObjectsInLocality(center.x, center.y, center.z, radius);
                for (int i = 0; i < tList.Count; i++)
                {
                    lqClientProxy tProxy = (lqClientProxy)tList[i];
                    //tList.ForEach(delegate(lqClientProxy tProxy)
                    //{
                    results.Add((AbstractVehicle)tProxy.clientObject);
                    //});
                }
            }

            // called by LQ for each clientObject in the specified neighborhood:
            // push that clientObject onto the ContentType vector in void*
            // clientQueryState
            // (parameter names commented out to prevent compiler warning from "-W")


            /*
            static void perNeighborCallBackFunction(void* clientObject,
                                                      float distanceSquared,
                                                      void* clientQueryState)
            {
                typedef std::vector<ContentType> ctv;
                ctv& results = *((ctv*) clientQueryState);
                results.push_back ((ContentType) clientObject);
            }
            */


        };


        // allocate a token to represent a given client object in this database
        public override AbstractTokenForProximityDatabase allocateToken(AbstractVehicle parentObject)
        {
            return new tokenType(parentObject, this);
        }

        // count the number of tokens currently in the database
        public override int getPopulation()
        {
            //int count = 0;
            //lqMapOverAllObjects(lq, counterCallBackFunction, &count);

            int count = lq.getAllObjects().Count;

            return count;
        }

        public override AbstractVehicle getNearestVehicle(Vector3 position,float radius) {
            lqClientProxy tProxy= lq.lqFindNearestNeighborWithinRadius(position.x, position.y, position.z, radius, null);
             AbstractVehicle tVehicle=null;
             if (tProxy != null)
             {
                 tVehicle = (AbstractVehicle) tProxy.clientObject;
             }
            return tVehicle;
        }

        public override Vector3 getMostPopulatedBinCenter() {
            return lq.getMostPopulatedBinCenter();
        } 


        /*
        // (parameter names commented out to prevent compiler warning from "-W")
        static void counterCallBackFunction(void* clientObject,
                                              float distanceSquared,
                                              void* clientQueryState)
        {
            int& counter = *(int*)clientQueryState;
            counter++;
        }
        */

    }
}
