using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public delegate void SetDistanceTo(int x, double d);
public delegate void Log(string s);
[StructLayout(LayoutKind.Sequential)]
public struct UnsignedArray
{
    public uint[] ar;
    public int size;
}

[StructLayout(LayoutKind.Sequential)]
public struct DoubleArray
{
    public double[] ar;
    public int size;
}
public static class NativeMethods
{
    [DllImport("GeodesicAlgorithmLibrary", EntryPoint = "geodesicExactCompute", CallingConvention = CallingConvention.Cdecl)]
    public static extern void _geodesicExactCompute(
        DoubleArray vertices, UnsignedArray faces,
        UnsignedArray sources,
        SetDistanceTo setDistanceTo, Log log
    );
    [DllImport("GeodesicAlgorithmLibrary", EntryPoint = "testStuff", CallingConvention = CallingConvention.Cdecl)]
    public static extern void _testStuff(
        Log log, string s
    );
}
public class GeodesicAlgorithmIntegration
{
    public static void testStuff()
    {
        Log log = delegate (string s) { Debug.Log(s); } ;
        NativeMethods._testStuff(log, "Wowie");
    }
    private static DoubleArray doubleArrayConvertFromLocal(double[] ar)
    {
        DoubleArray _ar;
        _ar.ar = ar;
        _ar.size = ar.Length;
        return _ar;
    }
    private static UnsignedArray unsignedArrayConvertFromLocal(uint[] ar)
    {
        UnsignedArray _ar;
        _ar.ar = ar;
        _ar.size = ar.Length;
        return _ar;
    }

    public static double[] geodesicExactCompute(double[] vertices, uint[] faces, uint[] sources)
    {
        int vertexCount = vertices.Length / 3;
        double[] distances = new double[vertexCount];
        SetDistanceTo setDistanceTo = delegate (int x, double d) { distances[x] = d; };
        Log log = delegate (string s) { Debug.Log(s); } ;
        NativeMethods._geodesicExactCompute(
            doubleArrayConvertFromLocal(vertices), unsignedArrayConvertFromLocal(faces),
            unsignedArrayConvertFromLocal(sources),
            setDistanceTo, log
        );

        return distances;
    }

    public static double[,] geodesicExactComputeMatrix(double[] vertices, uint[] faces)
    {
        int vertexCount = vertices.Length / 3;
        double[,] matrix = new double[vertexCount, vertexCount];
        for (int i = 0; i < vertexCount; ++i)
        {
            uint[] sources = new uint[1];
            sources[0] = (uint)i;
            double[] distances = geodesicExactCompute(vertices, faces, sources);
            for (int t = 0; t < vertexCount; ++t)
                matrix[i, t] = distances[t];
        }
        return matrix;
    }
}
