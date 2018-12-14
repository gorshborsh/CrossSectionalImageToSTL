using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossSectionalImageToSTL.Src
{
    public struct Vector2Df
    {
        //Basic constructor
        public Vector2Df(float _x, float _y)
        {
            this.x = _x;
            this.y = _y;
        }

        //Components
        public float x, y;
    }
}
