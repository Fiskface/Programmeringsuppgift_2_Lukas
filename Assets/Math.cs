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

    public static Vector4 getPositionFromMatrix(Matrix4x4 mat)
    {
        return new Vector4(mat.m03, mat.m13, mat.m23, 0);
    }

    public static Matrix4x4 setPositionInMatrix(Matrix4x4 mat, Vector4 vec)
    {
        mat.m03 = vec.x;
        mat.m13 = vec.y;
        mat.m23 = vec.z;
        return mat;
    }
}
