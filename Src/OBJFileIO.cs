using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CrossSectionalImageToSTL.Src
{
    //Handle conversion of raw geometry to .OBJ format
    static class OBJFileIO
    {
        //Check if an .OBJ file with the same path exists in 
        //return 1 -> .OBJ exists
        //return 0 -> .OBJ does not exist in current directory
        public static int CheckOBJExists(string path)
        {

            return 1;
        }

        //Translate raw vertex data into .OBJ format for use
        //in modeling applications / pipelines
        public static void WriteGeometryToObj(List<Face> XYfaces, List<Face> YZfaces, List<Face> XZfaces, string path, FileMode accessMode)
        { 
            //Create a filestream to write geometry
            using (StreamWriter fstream = new StreamWriter(path))
            {
                //Iterate over faces in each plane
                //XY face
                //New group
                foreach(Face face in XYfaces)
                {

                }

                //New group

                //YZ faces
                foreach (Face face in XYfaces)
                {

                }

                //New group

                //XZ faces
                foreach (Face face in XYfaces)
                {

                }
            }
        }


    }
}
