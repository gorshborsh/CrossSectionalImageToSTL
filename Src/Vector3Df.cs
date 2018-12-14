using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossSectionalImageToSTL.Src
{
    //Generic Vector class, with specialized functions for our purposes
    public struct Vector3Df
    {
        //Constructor
        public Vector3Df(float _x, float _y, float _z)
        {
            this.x = _x;
            this.y = _y;
            this.z = _z;
        }

        //Component-wise Multplication
        public Vector3Df Mult(Vector3Df b)
        {
            this.x *= b.x;
            this.y *= b.y;
            this.z *= b.z;
            return this;
        }

        //Scale this by 'c'
        public Vector3Df Mult(float c)
        {
            this.x *= c;
            this.y *= c;
            this.z *= c;
            return this;
        }

        //Add 2 vectors
        public Vector3Df Add(Vector3Df b)
        {
            this.x += b.x;
            this.y += b.y;
            this.z += b.z;
            return this;
        }

        //Copy b to this
        public Vector3Df Copy(Vector3Df b)
        {
            this.x = b.x;
            this.y = b.y;
            this.z = b.z;
            return this;
        }

        //Set
        public Vector3Df Set(float a, float b,float c)
        {
            this.x = a;
            this.y = b;
            this.z = c;
            return this;
        }

        //Add 1.0f to the component that represents the magnitude of the normal vector of 'plane'
        public Vector3Df PerpToPlane(Plane plane)
        {
            //Which plane are we in?
            switch( plane )
            {
                //Add 1.0f to Z
                case Plane.XY:
                    {
                        this.z += 1.0f;
                        break;
                    }

                //Add 1.0f to X
                case Plane.YZ:
                    {
                        this.x += 1.0f;
                        break;
                    }

                //Add 1.0f to Y
                case Plane.XZ:
                    {
                        this.y += 1.0f;
                        break;
                    }
            }

            //Return what we have
            return this;
        }

        //Components
        public float x, y, z;
    }
}