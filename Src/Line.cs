using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossSectionalImageToSTL.Src
{
    //Represents Line in 3D space
    //Implements Vector3Df
    public struct Line
    {
        //Basic constructor
        public Line(Vector3Df _p0, Vector3Df _p1)
        {
            p0 = _p0;
            p1 = _p1;
        }

        //Two Points of Line
        public Vector3Df p0, p1;
    }
}
