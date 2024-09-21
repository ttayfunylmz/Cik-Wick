using UnityEngine;

namespace OccaSoftware.OutlineObjects.Runtime
{
    [RequireComponent(typeof(MeshFilter))]
    [AddComponentMenu("OccaSoftware/Outline Objects/Smooth Normals")]
    [ExecuteAlways]
    public class SmoothNormals : MonoBehaviour
    {
        MeshFilter meshFilter = null;
        Mesh meshSmooth = null;
        Mesh meshCached = null;

        private void OnEnable()
        {
            meshFilter = GetComponent<MeshFilter>();
            GenerateNormals generateNormals = new GenerateNormals();
            meshCached = meshFilter.sharedMesh;
            meshSmooth = Instantiate(meshFilter.sharedMesh);
            meshSmooth.name = meshFilter.sharedMesh.name + "_smoothed";
            generateNormals.GenerateSmoothNormals(meshSmooth);
            meshFilter.sharedMesh = meshSmooth;
        }

        private void OnDisable()
        {
            if (meshSmooth != null)
            {
                if (Application.isPlaying)
                {
                    meshFilter.sharedMesh = meshCached;
                    Destroy(meshSmooth);
                }
                else
                {
                    meshFilter.sharedMesh = meshCached;
                    DestroyImmediate(meshSmooth);
                }
            }
        }
    }
}
