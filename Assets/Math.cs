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
    
    //Calculates determinant of the matrix, which in this case is the volume of the cube since we're working with cubes
    public static float determinant(Matrix4x4 m)
    {
        return m[0, 0] * (m[1, 1] * m[2, 2] - m[1, 2] * m[2, 1])
            +m[0, 1] * (m[1, 2] * m[2, 0] - m[1, 0] * m[2, 2])
            +m[0, 2] * (m[1, 0] * m[2, 1] - m[1, 1] * m[2, 0]);
    }

    //Returns the dotproduct of 2 quaternions
    public static float QuatDot(Quaternion q1, Quaternion q2)
    {
        return (q1.x * q2.x + q1.y * q2.y + q1.z * q2.z + q1.w * q2.w);
    }

    //Used gamemath figure 8.20 for this
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

    //Used 
    //Takes out a quaternion as representation of the rotation from a matrix.
    public static Quaternion QuatFromMatrix(Matrix4x4 mat)
    {
        //Takes out vectors from the right columns
        Vector3 x = Normalize(GetColumnFromMatrix(mat, 0));
        Vector3 y = Normalize(GetColumnFromMatrix(mat, 1));
        Vector3 z = Normalize(GetColumnFromMatrix(mat, 2));
        //Inspiration from 8.7.4
        Quaternion quat = new Quaternion();
        quat.x = Mathf.Sqrt(Mathf.Max(0, 1 + x.x - y.y - z.z))/2;
        quat.y = Mathf.Sqrt(Mathf.Max(0, 1 - x.x + y.y - z.z))/2;
        quat.z = Mathf.Sqrt(Mathf.Max(0, 1 - x.x - y.y + z.z))/2;
        quat.w = Mathf.Sqrt(Mathf.Max(0, 1 + x.x + y.y + z.z))/2;
        //Checks if they're positive or negative
        quat.x *= Mathf.Sign(quat.x * (y.z - z.y));
        quat.y *= Mathf.Sign(quat.y * (z.x - x.z));
        quat.z *= Mathf.Sign(quat.z * (y.z - z.y));
        return quat;
    }

    //Information taken from gamemath Chapter 8.5.12
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
        
        //*Sometimes* Makes sure it takes shortest path around rotation axis
        float cosAngle = QuatDot(qA, qB);
        if (cosAngle < 0)
        {
            qB = new Quaternion(-qB.x, -qB.y, -qB.z, -qB.w);
            cosAngle = -cosAngle;
        }
        
        //Stops divide by zero error. 
        if (cosAngle > 0.9999f)
            return Matrix4x4.identity;
        
        float sinAngle = Mathf.Sqrt(1.0f - cosAngle * cosAngle);

        float angle = Mathf.Atan2(sinAngle, cosAngle);

        //Helps us only make the division once
        float oneOverSinAngle = 1 / sinAngle;

        //Gets interpolation values
        float t0 = Mathf.Sin((1 - time) * angle) * oneOverSinAngle;
        float t1 = Mathf.Sin(angle * time) * oneOverSinAngle;

        Quaternion interQuat = new Quaternion(qA.x * t0 + qB.x * t1,
            qA.y * t0 + qB.y * t1,
            qA.z * t0 + qB.z * t1,
            qA.w * t0 + qB.w * t1);

        return RotationMatrixFromQuaternion(interQuat);
    }
}
