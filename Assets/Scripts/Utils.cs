using System.Collections;
using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;

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
        public static HashSet<int>[] AdjacencyMap(Mesh mesh)
        {
            HashSet<int>[] vertexToAdjacent = Enumerable.Range(0, mesh.vertexCount).Select(_ => new HashSet<int>()).ToArray();
            for (int i = 0; i < mesh.triangles.Length / 3; i++)
            {
                for (int j = i * 3; j < i * 3 + 3; ++j)
                {
                    for (int k = j + 1; k < i * 3 + 3; ++k)
                    {
                        vertexToAdjacent[mesh.triangles[j]].Add(mesh.triangles[k]);
                        vertexToAdjacent[mesh.triangles[k]].Add(mesh.triangles[j]);
                    }
                }
            }
            return vertexToAdjacent;
        }
        public static byte[] GetHash(string inputString)
        {
            using (HashAlgorithm algorithm = SHA256.Create())
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        public static string GetHashString(string inputString, int length = 64)
        {
            if (length > 64)
                throw new ArgumentOutOfRangeException(String.Format("SHA-256 itself is 64 characters. So it can not be truncated to {0} characters", length));
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString().Substring(0, length);
        }
    }
    
}
