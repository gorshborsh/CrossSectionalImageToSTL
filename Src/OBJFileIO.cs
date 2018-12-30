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

        //Helper function to write vertex list to .OBJ file
        private static void StreamWriteVertexList(List<Vector3Df> vertexList, StreamWriter stream, string groupName, bool stitchLastToFirst)
        {
            //Compute number of faces
            int faceCount = (vertexList.Count / 2) - 1;
            //Used to format .OBJ file vertices
            int vertexIdx = 0;

            //Create new group if necessary (groupName is NOT null, empty)
            if( !string.IsNullOrEmpty(groupName) )
            {
                stream.WriteLine("g {0}", groupName);
            }
            
            //Write all vertices, then stitch
            foreach (Vector3Df vertex in vertexList)
            {
                stream.WriteLine(string.Format("v {0:N4} {1:N4} {2:N4}", vertex.x, vertex.y, vertex.z));
            }
            //Generate faces
            for (int i = 0; i < faceCount; i++)
            {
                //Use this instead of 'i' for keeping track of vertex indices
                vertexIdx = 1 + (i * 2);

                stream.WriteLine(string.Format("f {0:F0} {1:F0} {2:F0} {3:F0}", vertexIdx + 1, vertexIdx + 0, vertexIdx + 2, vertexIdx + 3));
            }
            //Stitch last face if required
            if (stitchLastToFirst)
            {
                stream.WriteLine(string.Format("f {0:F0} {1:F0} 2 1", vertexIdx + 2, vertexIdx + 3));
            }
       
            //return
        }

        //Translate raw vertex data into .OBJ format for use
        //in modeling applications / pipelines
        public static bool WriteGeometryToObj(List<Vector3Df> XYvertices, List<Vector3Df> YZvertices, List<Vector3Df> XZvertices, string path)
        { 
            //Check if empty
            if(XYvertices.Count == 0 || YZvertices.Count == 0 || XZvertices.Count == 0)
            {
                return false;
            }

            //Ensure that the vertex lists are valid
            if((XYvertices.Count % 2) != 0 || (YZvertices.Count % 2) != 0 || (XZvertices.Count % 2) != 0)
            {
                return false;
            }

            //Create a filestream to write geometry
            using (StreamWriter fstream = new StreamWriter(path))
            {
                StreamWriteVertexList(XYvertices, fstream, "xy_cross_section", true);
                StreamWriteVertexList(YZvertices, fstream, "yz_cross_section", true);
                StreamWriteVertexList(XZvertices, fstream, "xz_cross_section", true);
            }

            //Success!!
            return true;
        }
    }
}
