using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossSectionalImageToSTL.Src
{
    //Plane / Face in 3D Space
    //Specified by two lines (quad), three vertices (tri), or four vertices (quad)
    //TBI
    public struct Face
    {
        //Basic Constructor
        public Face(Line _l0, Line _l1)
        {
            x0 = _l0.p0;
            x1 = _l0.p1;
            x2 = _l1.p1;
            x3 = _l1.p0;
        }

        //Representation
        // x1----x2
        // |    / |
        // |   /  |
        // |  /   |
        // x0----x3
        public Vector3Df x0, x1, x2, x3;
    }
}
