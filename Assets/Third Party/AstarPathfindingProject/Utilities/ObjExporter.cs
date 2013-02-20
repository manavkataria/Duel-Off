using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;

namespace Pathfinding.Serialize {
	
	/** Obj mesh exporter.
	 * This class is a modified version of the one found at the UnifyCommunity wiki.
	 * It provides utilities for exporting a mesh to a .obj file
	 * \author KeliHlodversson (see http://unifycommunity.com/wiki/index.php?title=ObjExporter)
	 */
	public class ObjExporter {
	
		/** Generates an obj file from supplied Mesh object */
		public static string MeshToString(Mesh m) {
			
			StringBuilder sb = new StringBuilder();
			
			sb.Append("g ").Append(m.name).Append("\n");
			foreach(Vector3 v in m.vertices) {
				sb.Append(string.Format("v {0} {1} {2}\n",v.x,v.y,v.z));
			}
			sb.Append("\n");
			foreach(Vector3 v in m.normals) {
				sb.Append(string.Format("vn {0} {1} {2}\n",v.x,v.y,v.z));
			}
			sb.Append("\n");
			foreach(Vector3 v in m.uv) {
				sb.Append(string.Format("vt {0} {1}\n",v.x,v.y));
			}
			for (int material=0; material < m.subMeshCount; material ++) {
				sb.Append("\n");
					
				int[] triangles = m.GetTriangles(material);
				for (int i=0;i<triangles.Length;i+=3) {
					sb.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n", 
						triangles[i]+1, triangles[i+1]+1, triangles[i+2]+1));
				}
			}
			return sb.ToString();
		}
		
		/** Generates an obj file from supplied vertices and triangles arrays */
		public static string MeshToString(Vector3[] vertices, int[] triangles) {
			
			StringBuilder sb = new StringBuilder();
			
			for (int i=0;i<vertices.Length;i++) {
				Vector3 v = vertices[i];
				sb.Append(string.Format("v {0} {1} {2}\n",v.x,v.y,v.z));
			}
			
			for (int i=0;i<triangles.Length;i+=3) {
				sb.Append(string.Format("f {0} {1} {2}\n", 
					triangles[i]+1, triangles[i+1]+1, triangles[i+2]+1));
			}
			return sb.ToString();
		}
		
		/** Saves a Mesh to file as an .obj file */
		public static void MeshToFile(Mesh m, string filename) {
			using (StreamWriter sw = new StreamWriter(filename)) 
			{
				sw.Write(MeshToString(m));
			}
		}
		
		/** Saves mesh data to file as an .obj file */
		public static void MeshToFile(Vector3[] vertices, int[] triangles, string filename) {
			using (StreamWriter sw = new StreamWriter(filename)) 
			{
				sw.Write(MeshToString(vertices, triangles));
			}
		}
	}
}