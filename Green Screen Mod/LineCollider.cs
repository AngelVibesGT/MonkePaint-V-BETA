
using UnityEngine;

namespace Green_Screen_Mod
{
    //all this does is add a collider to the line renderer
    public class LineCollider : MonoBehaviour
    {
        
        public static void GenerateCollider(LineRenderer lineRenderer)
        {
            MeshCollider collider = lineRenderer.gameObject.GetComponent<MeshCollider>();

            if (collider == null) 
            {
                collider = lineRenderer.gameObject.AddComponent<MeshCollider>();
            }
            Mesh mesh = new Mesh();
            lineRenderer.BakeMesh(mesh, true);
            collider.sharedMesh = mesh;
            collider.convex = true;
            collider.isTrigger = true;
        }

    }
}
