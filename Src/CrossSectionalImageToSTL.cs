using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

using CrossSectionalImageToSTL.Src;

namespace CrossSectionalImageToSTL
{
    class CrossSectionalImageToSTL
    {
        //Return codes
        //Generic
        private const int EXIT_FAILURE = -1;
        private const int EXIT_SUCCESS = 0;

        //Other
        private const int EXIT_FILE_NOT_FOUND = -2;
        private const int EXIT_FACE_FROM_PIXEL_ERROR = -3;
        private const int EXIT_WRITE_GEOMETRY_ERROR = -4;

        //Arguments
        //arg 1: Name of Images
        //arg 2: Name of Output .OBJ File
        static int Main(string[] args)
        {
            //Make sure all arguments are present
            if (args.Length < 2)
            {
                Console.WriteLine("!!! Not Enough Arguments !!!");
                return EXIT_FAILURE;
            }

            //Variables / Storage
            //BITMAP
            Bitmap XYcrossSection = null, XZcrossSection = null, YZcrossSection = null;

            //~Variables / Storage

            //Attempt to open
            try
            {
                //Do it
                XYcrossSection = new Bitmap(args[0] + "1.bmp");
                XZcrossSection = new Bitmap(args[0] + "2.bmp");
                YZcrossSection = new Bitmap(args[0] + "3.bmp");
            }
            catch( Exception )
            {
                //Print and return
                Console.WriteLine("!!! File I/O Runtime Error !!!");
                return EXIT_FILE_NOT_FOUND;
            }

            //Obtain geometric representation of bitmaps
            List< Vector3Df > XYgeometry = FaceStitcher.PixelsToVertices(XYcrossSection, Plane.XY, false);
            List< Vector3Df > XZgeometry = FaceStitcher.PixelsToVertices(XZcrossSection, Plane.XZ, false);
            List< Vector3Df > YZgeometry = FaceStitcher.PixelsToVertices(YZcrossSection, Plane.YZ, false);

            //NULL Check
            if (XYgeometry is null || XZgeometry is null || YZgeometry is null)
            {
                //Print and return
                Console.WriteLine("!!! Geometry Generation Error !!!");
                return EXIT_FACE_FROM_PIXEL_ERROR;
            }

            //Write Geometry to File
            if(!OBJFileIO.WriteGeometryToObj(XYgeometry, YZgeometry, XZgeometry, args[1]))
            {
                Console.WriteLine("!!! Error Writing Geometry To File !!!");
                return EXIT_WRITE_GEOMETRY_ERROR;
            }

            //Success
            Console.WriteLine("--- Generation Complete ---");
            return EXIT_SUCCESS;
        }
    }
}
