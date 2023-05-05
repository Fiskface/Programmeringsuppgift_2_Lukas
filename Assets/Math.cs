using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    public static Matrix4x4 RotationMatrixFromQuaternion(Quaternion q)
    {
        var x = q.x;
        var y = q.y;
        var z = q.z;
        var w = q.w;
        var c1 = new Vector4(1-2*y*y-2*z*z,2*x*y+2*w*z,2*x*z-2*w*y,0);
        var c2 = new Vector4(2*x*y-2*w*z,1-2*x*x-2*z*z,2*y*z+2*w*x, 0);
        var c3 = new Vector4(2*x*z+2*w*y,2*y*z-2*w*x,1-2*x*x-2*y*y,0);
        var c4 = new Vector4(0,0,0,1);

        return new Matrix4x4(c1,c2,c3,c4);
    }

    public static Quaternion QuatFromMatrix(Matrix4x4 matrix4X4)
    {
        Vector3 x = new Vector3(matrix4X4.m00, matrix4X4.m10, matrix4X4.m20).normalized;
        Vector3 y = new Vector3(matrix4X4.m01, matrix4X4.m11, matrix4X4.m21).normalized;
        Vector3 z = new Vector3(matrix4X4.m02, matrix4X4.m12, matrix4X4.m22).normalized;
        Quaternion quat = new Quaternion();
        quat.w = Mathf.Sqrt(Mathf.Max(0, 1 + x.x + y.y + z.z))/2;
        quat.x = Mathf.Sqrt(Mathf.Max(0, 1 + x.x - y.y - z.z))/2;
        quat.y = Mathf.Sqrt(Mathf.Max(0, 1 - x.x + y.y - z.z))/2;
        quat.z = Mathf.Sqrt(Mathf.Max(0, 1 - x.x - y.y + z.z))/2;
        quat.x *= Mathf.Sign(quat.x * (y.z - z.y));
        quat.y *= Mathf.Sign(quat.y * (z.x - x.z));
        quat.z *= Mathf.Sign(quat.z * (y.z - z.y));
        return quat;
    }

    public static Matrix4x4 CalculateRotationMatrix(Matrix4x4 A, Matrix4x4 B, float time)
    {
        Quaternion q1 = QuatFromMatrix(A);
        Quaternion q2 = QuatFromMatrix(B);
        q1.w = -q1.w;
        
        q2.w = -q2.w;
        Quaternion q3 = q2 * q1;
        

        q1.w = -q1.w;
        float alpha = Mathf.Acos(q3.w);
        float newAlpha = alpha * time;
        q3.w = Mathf.Cos(newAlpha);
        float mult = Mathf.Sin(newAlpha) / Mathf.Sin(alpha);

        q3.x *= mult;
        q3.y *= mult;
        q3.z *= mult;
        q3 *= q1;

        return RotationMatrixFromQuaternion(q3);
    }
}
