using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrossSectionalImageToSTL.Src;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace CrossSectionalImageToSTL.Src
{
    //This class contains a method that iterates through a Vector array
    //and stitches the Vertices they represent into faces
    static class FaceStitcher
    {
        //Read image, obtain normalized pixel coordinates of BLACK pixels ONLYs
        // NEED FOR SPEED
        private static List< Vector2Df > BitmapToNormPixCoords(Bitmap image)
        {
            //Norm. Pixel Coord List
            List< Vector2Df > normPixCoords = new List< Vector2Df >();

            //Return Empty list if not at least 2x2 image
            if (image.Width < 2 || image.Height < 2) return normPixCoords;

            //Lock the bitmap so we may operate on it
            //Faster to directly index the bitmap, rather than using GetPixel which is slow as molasses
            BitmapData bData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);

            //Compute space required for image
            int imageSize = bData.Stride * bData.Height;

            //Allocate space for the bytes
            byte[] pixels = new byte[ imageSize ];

            //Copy
            Marshal.Copy(bData.Scan0, pixels, 0, imageSize);

            //double letter LCVs, very nice
            for (int ii = 0; ii < bData.Height; ii++)
            {
                for (int jj = 0; jj < bData.Width; jj++)
                {
                    //Grab the pixel at this particular location
                    byte pixel = pixels[ii * bData.Stride + jj];

                    //Check if it is the correct color
                    //If so, normalize it's coordinates
                    if(pixel == 0)
                    {
                        //Compute
                        float pX = 1.0f / (float)(bData.Width - 1);
                        float pY = 1.0f / (float)(bData.Height - 1);

                        //Add 'em
                        normPixCoords.Add( new Vector2Df(pX, pY) );
                    }
                }
            }

            //Return control to system
            image.UnlockBits(bData);

            //Return
            return normPixCoords;
        }

        //Stitch faces together
        public static List< Face > PixelsToFaces(Bitmap image, Plane plane,  bool connectFirstAndLast)
        {
            //This is where we will store the geometry
            List< Face > geometry = new List< Face >();

            //Null check
            if(image is null)
            {
                return geometry;
            }

            //Obtain normalized pixel coordinates
            List< Vector2Df >  normCoords = BitmapToNormPixCoords(image);

            //Not normalized pixel coordinates found
            if(normCoords.Count < 2)
            {
                return geometry;
            }

            //Used to traverse the list of Faces
            //A face is created from 4 points
            //Current and Previous represent two points of a face
            //Another point is created with an offset of 1.0 in the perpindicular direction
            //of the current plane
            //This gives us 4 points, enough for a face
            Vector2Df current = normCoords.ElementAt(1);
            Vector2Df previous = normCoords.ElementAt(0);

            //Start at Index 1 (current)
            using(IEnumerator<Vector2Df> pixEnumerator = normCoords.Skip(1).GetEnumerator())
            {
                //Assign Current
                current = pixEnumerator.Current;

                //Process
                //Convert Vector2Df pixcoords -> Vector3Df
                //Duplicate 'current', 'previous' with same coordinates + an offset in perp. direction to plane
                Vector3Df v0 = MapPixCoordToVertex(current, plane);
                Vector3Df v1 = v0.PerpToPlane(plane);


                //Assign previous
                previous = current;
            }

            //Give them back to me!
            return geometry;
        }


        //Map 2D Pixel Coordinates into 3D Vertices
        private static Vector3Df MapPixCoordToVertex(Vector2Df pix, Plane plane)
        {
            //Vertex to be returned 
            Vector3Df vertex = new Vector3Df(0.0f, 0.0f, 0.0f);
            switch( plane )
            {
                //Zero out Z for XY Plane
                case Plane.XY:
                    {
                        vertex.x = pix.x;
                        vertex.y = pix.y;
                        vertex.z = 0.0f;
                        break;
                    }

                //Zero out X for YZ Plane
                //Notice that pix.x, pix.y are assigned in order
                //AKA pix.y is always assigned last
                // pix. x is always assigned first
                case Plane.YZ:
                    {
                        vertex.x = 0.0f;
                        vertex.y = pix.x;
                        vertex.z = pix.y;
                        break;
                    }

                //Zero out Y for XZ Plane
                case Plane.XZ:
                    {
                        vertex.x = pix.x;
                        vertex.y = 0.0f;
                        vertex.z = pix.y;
                        break;
                    }
            }

            //Just return it
            return vertex;
        }
    }
}
