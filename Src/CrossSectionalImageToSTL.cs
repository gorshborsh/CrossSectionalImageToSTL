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

        //Arguments
        //arg 1: Name of Images
        //arg 2: Name of Output .OBJ File
        static int Main(string[] args)
        {
            //Make sure all arguments are present
            if (args.Length < 2)
            {
                Console.WriteLine("Not enough arguments!");
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
                XYcrossSection = new Bitmap(args[1] + "1.bmp");
                XZcrossSection = new Bitmap(args[1] + "2.bmp");
                YZcrossSection = new Bitmap(args[1] + "3.bmp");
            }
            catch( FileNotFoundException )
            {
                //Print and return
                Console.WriteLine("!!! File(s) not found !!!");
                return EXIT_FILE_NOT_FOUND;
            }

            //Obtain geometric representation of bitmaps
            List<Face> XYgeometry = FaceStitcher.PixelsToFaces(XYcrossSection, Plane.XY, false);
            List<Face> XZgeometry = FaceStitcher.PixelsToFaces(XZcrossSection, Plane.XZ, false);
            List<Face> YZgeometry = FaceStitcher.PixelsToFaces(YZcrossSection, Plane.YZ, false);

            //NULL Check
            if (XYgeometry is null || XZgeometry is null|| YZgeometry is null) return EXIT_FACE_FROM_PIXEL_ERROR;
            

            //Success
            Console.WriteLine("---Generated model successfully---");
            return 0;
        }
    }
}
