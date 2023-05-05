using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class Math
{
    public static float DotProduct(Vector3 vec1, Vector3 vec2)
    {
        return vec1.x * vec2.x + vec1.y * vec2.y + vec1.z + vec2.z;
    }

    public static Vector3 CrossProduct(Vector3 vec1, Vector3 vec2)
    {
        return new Vector3(vec1.y * vec2.z - vec1.z * vec2.y,
            vec1.z * vec2.x - vec1.x * vec2.z,
            vec1.x * vec2.y - vec1.y * vec2.x);
    }

    public static float GetMagnitude(Vector3 vec)
    {
        return Mathf.Sqrt(vec.x * vec.x + vec.y * vec.y + vec.z * vec.z);
    }

    public static Vector3 Normalize(Vector3 vec)
    {
        if (GetMagnitude(vec) == 0)
            return Vector3.zero;
        
        return (1 / GetMagnitude(vec)) * vec;
    }

    public static Vector3 getColumnFromMatrix(Matrix4x4 mat, int i)
    {
        return new Vector3(mat[0,i], mat[1,i], mat[2,i]);
    }

    public static Matrix4x4 setColumnInMatrix(Matrix4x4 mat, Vector3 vec, int i)
    {
        mat[0, i] = vec.x;
        mat[1, i] = vec.y;
        mat[2, i] = vec.z;
        
        return mat;
    }
}
