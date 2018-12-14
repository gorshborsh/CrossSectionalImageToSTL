using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossSectionalImageToSTL.Src
{
    //Enumerated type that describes which plane the pixelLocations lie on
    public enum Plane { XY, YZ, XZ };

    //Various vector operations
    static class Vector
    {
        public static Vector3Df PerpVectorToPlane(Plane plane)
        {
            //Vector to return
            Vector3Df perpVector = new Vector3Df(0.0f, 0.0f, 0.0f);

            switch( plane )
            { 
                //Perpindicular Vector in Z direction
                case Plane.XY:
                    {
                        perpVector.Set(0.0f, 0.0f, 1.0f);
                        break;
                    }
                //Perp. Vector in Y dir.
                case Plane.XZ:
                    {
                        perpVector.Set(0.0f, 1.0f, 0.0f);
                        break;
                    }
                //Perp. Vector in X dir.
                case Plane.YZ:
                    {
                        perpVector.Set(1.0f, 0.0f, 0.0f);
                        break;
                    }
            }

            //Get it 
            return perpVector;
        }
    }
 
}
