using UnityEngine;

namespace GameCreator.Runtime.Common
{
    public static partial class GizmosExtension
	{
		// PUBLIC METHODS: ------------------------------------------------------------------------
		
		public static void Arc(Vector3 position, Quaternion rotation, float angle, float minRadius, float maxRadius)
		{
			Mesh mesh = GetArcMesh(angle, minRadius, maxRadius);

			Color color = Gizmos.color;
			Gizmos.DrawMesh(mesh, position, rotation, Vector3.one);
			Gizmos.color = color;
		}
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private static Mesh GetArcMesh(float angle, float minRadius, float maxRadius)
        {
	        int segments = Mathf.FloorToInt(angle / 10f);
	        Mesh mesh = new Mesh
	        {
		        vertices = new Vector3[4 * segments],
		        triangles = new int[3 * 2 * segments]
	        };

	        Vector3[] normals = new Vector3[4 * segments];
	        Vector2[] uv = new Vector2[4 * segments];
	        
	        Vector3[] vertices = new Vector3[4 * segments];
	        int[] triangles = new int[3 * 2 * segments];

	        for (int i = 0; i < uv.Length; i++)
	        {
		        uv[i] = new Vector2(0, 0);
	        }

	        for (int i = 0; i < normals.Length; i++)
	        {
		        normals[i] = new Vector3(0, 1, 0);
	        }
		    
		    mesh.uv = uv;
		    mesh.normals = normals;
		    
		    float angle1 = - angle / 2;
		    float angle2 = + angle / 2;
		    float delta = (angle2 - angle1) / segments;

		    float angleCurr = angle1;
		    float angleNext = angle1 + delta;
		    
		    Vector3 pos_curr_min = Vector3.zero;
		    Vector3 pos_curr_max = Vector3.zero;

		    Vector3 pos_next_min = Vector3.zero;
		    Vector3 pos_next_max = Vector3.zero;
	        
	        for (int i = 0; i < segments; i++)
	        {
		       Vector3 sphere_curr = new Vector3(
			       Mathf.Sin(Mathf.Deg2Rad * angleCurr), 
			       0,
			       Mathf.Cos(Mathf.Deg2Rad * angleCurr)
			   );

		       Vector3 sphere_next = new Vector3(
			       Mathf.Sin(Mathf.Deg2Rad * angleNext), 
			       0,
			       Mathf.Cos(Mathf.Deg2Rad * angleNext)
			   );

		       pos_curr_min = sphere_curr * minRadius;
		       pos_curr_max = sphere_curr * maxRadius;

		       pos_next_min = sphere_next * minRadius;
		       pos_next_max = sphere_next * maxRadius;

		       int a = 4 * i;
		       int b = 4 * i + 1;
		       int c = 4 * i + 2;
		       int d = 4 * i + 3;

		       vertices[a] = pos_curr_min;
		       vertices[b] = pos_curr_max;
		       vertices[c] = pos_next_max;
		       vertices[d] = pos_next_min;

		       triangles[6 * i] = a;
		       triangles[6 * i + 1] = b;
		       triangles[6 * i + 2] = c;
		       
		       triangles[6 * i + 3] = c;
		       triangles[6 * i + 4] = d;
		       triangles[6 * i + 5] = a;

		       angleCurr += delta;
		       angleNext += delta;
	        }

	        mesh.vertices = vertices;
	        mesh.triangles = triangles;

	        return mesh;
        }
	}
}