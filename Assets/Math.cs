using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public static class Math
{
    //Lämnar här trots inte använt, behövs för omexamination 1 och potentiellt, men kanske inte, för uppgift 3
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

    //Returns the length of a vector3
    public static float GetMagnitude(Vector3 vec)
    {
        return Mathf.Sqrt(vec.x * vec.x + vec.y * vec.y + vec.z * vec.z);
    }

    //Normalizes a vector3
    public static Vector3 Normalize(Vector3 vec)
    {
        if (GetMagnitude(vec) == 0)
            return Vector3.zero;
        
        return (1 / GetMagnitude(vec)) * vec;
    }

    //Returns a vector that is a column from a matrix
    public static Vector3 GetColumnFromMatrix(Matrix4x4 mat, int col)
    {
        return new Vector3(mat[0,col], mat[1,col], mat[2,col]);
    }

    //Returns a matrix where specified column is changed to the vector
    public static Matrix4x4 SetColumnInMatrix(Matrix4x4 mat, Vector3 vec, int col)
    {
        mat[0, col] = vec.x;
        mat[1, col] = vec.y;
        mat[2, col] = vec.z;
        
        return mat;
    }

    //Returns the dotproduct of 2 quaternions
    public static float QuatDot(Quaternion q1, Quaternion q2)
    {
        return (q1.x * q2.x + q1.y * q2.y + q1.z * q2.z + q1.w * q2.w);
    }

    //Uses a quaternion to get a rotationmatrix
    public static Matrix4x4 RotationMatrixFromQuaternion(Quaternion q)
    {
        var x = q.x;
        var y = q.y;
        var z = q.z;
        var w = q.w;
        var c1 = new Vector4(1 - 2*y*y - 2*z*z,2*x*y + 2*w*z,2*x*z - 2*w*y,0);
        var c2 = new Vector4(2*x*y - 2*w*z,1 - 2*x*x - 2*z*z,2*y*z + 2*w*x, 0);
        var c3 = new Vector4(2*x*z + 2*w*y,2*y*z - 2*w*x,1 - 2*x*x - 2*y*y,0);
        var c4 = new Vector4(0,0,0,1);

        return new Matrix4x4(c1,c2,c3,c4);
    }

    //Takes out a quaternion as representation of the rotation from a matrix.
    public static Quaternion QuatFromMatrix(Matrix4x4 mat)
    {
        Vector3 x = Normalize(GetColumnFromMatrix(mat, 0));
        Vector3 y = Normalize(GetColumnFromMatrix(mat, 1));
        Vector3 z = Normalize(GetColumnFromMatrix(mat, 2));
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

    //With the help of QuatFromMatrix and RotationMatrixFromQuaternion functions
    //this function takes two matrices as parameters and a float between 0 and 1 to get an 
    //interpolated rotationmatrix between the two matrices
    public static Matrix4x4 CalculateInterpolatedRotationMatrix(Matrix4x4 A, Matrix4x4 B, float time)
    {
        Quaternion qA = QuatFromMatrix(A);
        Quaternion qB = QuatFromMatrix(B);
        
        //If both are rotated, but rotated exactly the same, it interpolates weirdly, now it doesn't.
        if (qA == qB)
        {
            return RotationMatrixFromQuaternion(qA);
        }
        
        qA.w = -qA.w;

        //Makes sure it takes shortest path around rotation axis
        float dot = QuatDot(qA, qB);
        if (dot < 0)
            qB = new Quaternion(-qB.x, -qB.y, -qB.z, -qB.w);
        
        //Stops divide by zero error. 
        if (dot < -0.9999)
            return Matrix4x4.identity;
        
        Quaternion qC = qB * qA;
        qA.w = -qA.w;
        
        float angle = Mathf.Acos(qC.w);
        float newAngle = angle * time;
        qC.w = Mathf.Cos(newAngle);
        float mult = Mathf.Sin(newAngle) / Mathf.Sin(angle);

        qC.x *= mult;
        qC.y *= mult;
        qC.z *= mult;
        qC *= qA;
        
        return RotationMatrixFromQuaternion(qC);
    }
}
