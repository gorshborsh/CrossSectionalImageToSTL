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
        //Struct used for sorting norm. pix. coords
        struct NormPixCoordSortStruct
        {
            public Vector2Df coord { get; set; }
            public double angle { get; set; }
            //unused field 'mag', kept to not break anything ATM
            //will be cleaned up later
            public double mag { get; set; }
        }

        //Enum used in the CollapseNormCoordsWithinAngleTolerance(...)
        //Determines the method to 'collapse' the NormCoords
        public enum NormCoordCollapseOperation {UseClosestToOrigin, UseFurthestFromOrigin, UseAverage };

        //Plain English: The models generated must represent closed volumes - if there are NormCoords that form overlapping volumes, the generated model will not be useful
        //By collapsing all the NormCoords within a sector whose angle is given by 'sectorAngleDivision'
        //The image space has been converted to polar coordinates at this point, and the 360 degree image space is chopped into sectors with angles 'sectorAngleDivision'
        //Therefore, 360 / 'sectorAngleDivision' will return the number of subsections created
        //If there are multiple NormCoords within a subsection, they are collapsed into a single NormCoord
        //The operation to determine this new NormCoord is specified by the 3rd Parameter
        private static List<NormPixCoordSortStruct> CollapseNormCoordsWithinAngleTolerance( List<NormPixCoordSortStruct> normPixCoordSortStructs, double sectorAngleDivision)
        {

        }

        //Read image, obtain normalized pixel coordinates of BLACK pixels ONLY
        // NEED FOR SPEED
        //This does not sort the normal pixel coordinates in a meaningful way. this should be done 
        //elsewhere
        //WARNING:  !!! ORDER MATTERS !!!
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
                        //       Normalizing coefficient               Positional coefficient
                        float pX = (1.0f / (float)(bData.Width - 1)) * (float)ii;
                        float pY = (1.0f / (float)(bData.Height - 1)) * (float)jj;

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

        //Create a vertex list
        public static List< Vector3Df > PixelsToVertices(Bitmap image, Plane plane,  bool connectFirstAndLast)
        {
            //This is where we will store the geometry
            List< Vector3Df > geometry = new List< Vector3Df >();

            //Null check
            if(image is null)
            {
                return geometry;
            }

            //Obtain normalized pixel coordinates
            List< Vector2Df >  normCoords = BitmapToNormPixCoords(image);

            //normalized pixel coordinates not found
            if (normCoords.Count < 2)
            {
                return geometry;
            }

            //Used to sort 'normCoords'
            List< NormPixCoordSortStruct > sortedNormCoords = new List< NormPixCoordSortStruct >();
            //Sort the normalized coordinates
            //Transform the Cartesian coordinate space of 'normCoords' into polar coordinates
            //This allows us to sort the normCoords by their angle wrt. the origin
            //This is done so that when normCoords are converted to vertices and stitched,
            //they follow a clockwise direction. Otherwise the stitching would occur based on
            //the traversal of the BitmapToNormCoords() function, which is not meaningful
            //By sorting the normCoords according to their angle wrt. the origin, we can sort
            //in a clockwise / counterclockwise manner, which ensures that when the stitching occurs,
            //it will provide a meanginful model / vertex list

            foreach(Vector2Df normCoord in normCoords)
            {
                //Populate sorting List<>
                sortedNormCoords.Add(new NormPixCoordSortStruct
                {
                    //preserve the normal coordinates
                    coord = normCoord,
                    //compute the angle wrt. horizontal thru origin
                    angle = Math.Atan2(normCoord.y - 0.5, normCoord.x - 0.5),
                    //compute Euclidean Distance (not squared)
                    //may be used later
                    mag = Math.Pow(normCoord.y - 0.5, 2.0) + Math.Pow(normCoord.x - 0.5, 2.0)
                });
            }

            //Sort, then replace normCoords with it's sorted counterpart
            sortedNormCoords.Sort(delegate(NormPixCoordSortStruct a, NormPixCoordSortStruct b)
            {
                return a.angle.CompareTo(b.angle);
            });

            //Replace
            for(int i = 0;i < sortedNormCoords.Count;i++)
            {
                normCoords[i] = sortedNormCoords[i].coord;
            }

            //Add to geometry
            foreach(Vector2Df currentPixCoord in normCoords)
            {
                //Map prev pixel coordinates (2D) to vertices (3D)
                Vector3Df v0 = MapPixCoordToVertex(currentPixCoord, plane);
                //Add 1.0f to the component perpindicular to 'plane'
                //This creates a line with one point represented by the norm. pixel coord
                //and the other 1.0f 'into' the image space. 
                //Same idea as having a piece of paper that represents the XY plane. One point of the line is visible,
                //the other is into the page (z direction) behind the one we can see
                Vector3Df v1 = MapPixCoordToVertex(currentPixCoord, plane).AddPerpToPlane(plane);

                //Add them to geometry
                geometry.Add(v0);
                geometry.Add(v1);
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
