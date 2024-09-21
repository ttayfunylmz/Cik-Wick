using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace OccaSoftware.OutlineObjects.Runtime
{
    /// <summary>
    /// Used to re-generate the normals for mesh objects.
    /// </summary>
    public class GenerateNormals
    {
        private List<Vector3> vertices = new List<Vector3>();
        private List<Vector3> normals = new List<Vector3>();
        private List<int> vertexIndices = new List<int>();
        private Dictionary<UniqueVertex, List<VertexData>> meshData = new Dictionary<UniqueVertex, List<VertexData>>();

        /// <summary>
        /// Generate smooth normals for the mesh.
        /// This method directly modifies the mesh input object.
        /// </summary>
        /// <param name="mesh"></param>
        public void GenerateSmoothNormals(Mesh mesh)
        {
            mesh.GetVertices(vertices);
            vertexIndices = mesh.GetTriangles(0).ToList();

            meshData.Clear();
            normals.Clear();

            const int VERTEX_COUNT = 3;
            int[] localIds = new int[VERTEX_COUNT];
            Vector3[] localVertices = new Vector3[VERTEX_COUNT];

            for (int i = 0; i < vertexIndices.Count; i += VERTEX_COUNT)
            {
                #region Set up Per-Vertex Normal Data
                System.Array.Clear(localIds, 0, VERTEX_COUNT);
                System.Array.Clear(localVertices, 0, VERTEX_COUNT);

                for (int a = 0; a < VERTEX_COUNT; a++)
                {
                    localIds[a] = vertexIndices[i + a];
                    localVertices[a] = vertices[localIds[a]];
                }

                Vector3 AB = localVertices[1] - localVertices[0];
                Vector3 AC = localVertices[2] - localVertices[0];
                Vector3 normal = Vector3.Cross(AB, AC);
                if (normal.magnitude > 0)
                {
                    normal /= normal.magnitude;
                }
                #endregion

                #region Assign Vertex Data to a Unique Vertex
                List<VertexData> vertexData;
                UniqueVertex uniqueVertex;

                for (int b = 0; b < VERTEX_COUNT; b++)
                {
                    uniqueVertex = new UniqueVertex(localVertices[b]);

                    if (!meshData.TryGetValue(uniqueVertex, out vertexData))
                    {
                        vertexData = new List<VertexData>();
                        meshData.Add(uniqueVertex, vertexData);
                    }

                    vertexData.Add(new VertexData(localIds[b], normal));
                }
                #endregion
            }
            #region Setup List of Normal by Vertex
            for (int i = 0; i < mesh.vertexCount; i++)
            {
                Vector3 vertexNormal = Vector3.zero;

                List<VertexData> vertexData = meshData[new UniqueVertex(vertices[i])];
                vertexData = vertexData.Distinct(new UniqueVertexDataComparer()).ToList();

                foreach (VertexData v in vertexData)
                {
                    vertexNormal += v.Normal.normalized;
                }

                vertexNormal.Normalize();

                normals.Add(vertexNormal);
            }
            #endregion

            mesh.SetUVs(3, normals);
        }

        private class UniqueVertexDataComparer : IEqualityComparer<VertexData>
        {
            public bool Equals(VertexData x, VertexData y)
            {
                return x.Index == y.Index;
            }

            public int GetHashCode(VertexData obj)
            {
                return obj.Index.GetHashCode();
            }
        }

        private class UniqueVertex
        {
            private const float Precision = 0.00001f;
            private const int Tolerance = (int)(1f / Precision);

            private long x;
            private long y;
            private long z;

            public UniqueVertex(Vector3 position)
            {
                x = (long)Mathf.Round(position.x * Tolerance);
                y = (long)Mathf.Round(position.y * Tolerance);
                z = (long)Mathf.Round(position.z * Tolerance);
            }

            public override bool Equals(object obj)
            {
                UniqueVertex key = (UniqueVertex)obj;
                return x == key.x && y == key.y && z == key.z;
            }

            public override int GetHashCode()
            {
                return System.Tuple.Create(x, y, z).GetHashCode();
            }
        }

        private class VertexData
        {
            public int Index;
            public Vector3 Normal;

            public VertexData(int vertexIndex, Vector3 normal)
            {
                Index = vertexIndex;
                Normal = normal;
            }
        }
    }
}
