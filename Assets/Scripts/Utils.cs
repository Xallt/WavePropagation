using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WavePropagation
{
    class Utils
    {
        public static Mesh CompressMesh(Mesh mesh)
        {
            /// <summary>
            /// Return identical mesh with duplicate vertices converted into a single one
            /// </summary>
         
            Dictionary<Vector3, int> vector_map = new Dictionary<Vector3, int>();
            int vector_count = 0;
            int[] new_faces = new int[mesh.triangles.Length];
            foreach (Vector3 v in mesh.vertices)
            {
                if (!vector_map.ContainsKey(v))
                    vector_map[v] = vector_count++;
            }
            Vector3[] new_vertices = new Vector3[vector_count];
            foreach (var item in vector_map)
            {
                new_vertices[item.Value] = item.Key;
            }
            for (int i = 0; i < mesh.triangles.Length; ++i)
            {
                new_faces[i] = vector_map[mesh.vertices[mesh.triangles[i]]];
            }
            Mesh new_mesh = new Mesh();
            new_mesh.vertices = new_vertices;
            new_mesh.triangles = new_faces;
            return new_mesh;
        }
        public static (Vector3, Vector3) BoundingBox(Mesh mesh)
        {
            Vector3 min = mesh.vertices[0], max = mesh.vertices[0];
            for (int i = 1; i < mesh.vertices.Length; ++i)
            {
                min = Vector3.Min(min, mesh.vertices[i]);
                max = Vector3.Max(max, mesh.vertices[i]);
            }
            return (min, max);
        }

        public static float MaxDimension(Mesh mesh)
        {
            var (min, max) = BoundingBox(mesh);
            var shape = max - min;
            return Mathf.Max(shape.x, shape.y, shape.z);
        }
        public static Vector3 Center(Mesh mesh)
        {
            var (min, max) = BoundingBox(mesh);
            return (min + max) / 2;
        }
    }
    
}
