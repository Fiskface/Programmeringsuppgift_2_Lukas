using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Vectors;

[ExecuteAlways]
[RequireComponent(typeof(VectorRenderer))]
public class Example : MonoBehaviour {
    
    [NonSerialized] 
    private VectorRenderer vectors;

    public bool ShowTranslation = true;
    public bool ShowScale = true;
    public bool ShowRotation = true;
    public bool InputWithHandles = false;

    [Range(0, 1)] public float Time = 0.0f;

    [SerializeField, HideInInspector] internal Matrix4x4 A;
    [SerializeField, HideInInspector] internal Matrix4x4 B;
    [SerializeField, HideInInspector] internal Matrix4x4 C = Matrix4x4.identity;

    [NonSerialized] public Vector4 vector0 = new Vector4(-0.5f,-0.5f,-0.5f,1);
    [NonSerialized] public Vector4 vectorA = new Vector4(0.5f,-0.5f,-0.5f,1);
    [NonSerialized] public Vector4 vectorB = new Vector4(-0.5f,0.5f,-0.5f,1);
    [NonSerialized] public Vector4 vectorC = new Vector4(-0.5f,-0.5f,0.5f,1);
    [NonSerialized] public Vector4 vectorAB = new Vector4(0.5f,0.5f,-0.5f,1);
    [NonSerialized] public Vector4 vectorAC = new Vector4(0.5f,-0.5f,0.5f,1);
    [NonSerialized] public Vector4 vectorBC = new Vector4(-0.5f,0.5f,0.5f,1);
    [NonSerialized] public Vector4 vectorABC = new Vector4(0.5f,0.5f,0.5f,1);

    public Quaternion aRotation = Quaternion.identity;
    public Quaternion bRotation = Quaternion.identity;

    private Matrix4x4 CTranslate = Matrix4x4.identity;
    private Matrix4x4 CScale = Matrix4x4.identity;
    private Matrix4x4 CRotate = Matrix4x4.identity;

    void OnEnable() {
        vectors = GetComponent<VectorRenderer>();
    }

    void Update()
    {
        //Interpolera vinkeln i kvaternionen istället för att lerpa hela
        //Quaternion lerpedRotation = Quaternion.Lerp(aRotation, bRotation, Time);
        
        Vector4 posInter = (1.0f - Time) * Math.getColumnFromMatrix(A, 3) + Time * Math.getColumnFromMatrix(B, 3);
        if (ShowTranslation)
        {
            CTranslate = Math.setColumnInMatrix(Matrix4x4.identity, posInter, 3);
        }
            
            
        if (ShowScale)
        {
            CScale = Matrix4x4.identity;
            
            float ScaleAx = Math.GetMagnitude(Math.getColumnFromMatrix(A, 0));
            float ScaleAy = Math.GetMagnitude(Math.getColumnFromMatrix(A, 1));
            float ScaleAz = Math.GetMagnitude(Math.getColumnFromMatrix(A, 2));

            float ScaleBx = Math.GetMagnitude(Math.getColumnFromMatrix(B, 0));
            float ScaleBy = Math.GetMagnitude(Math.getColumnFromMatrix(B, 1));
            float ScaleBz = Math.GetMagnitude(Math.getColumnFromMatrix(B, 2));

            //Gammal kod
            /*
            C.m00 = (1.0f - Time) * ScaleAx + Time * ScaleBx;
            C.m11 = (1.0f - Time) * ScaleAy + Time * ScaleBy;
            C.m22 = (1.0f - Time) * ScaleAz + Time * ScaleBz;
            */
            
            //Scalematrisen
            CScale.m00 = (1.0f - Time) * ScaleAx + Time * ScaleBx;
            CScale.m11 = (1.0f - Time) * ScaleAy + Time * ScaleBy;
            CScale.m22 = (1.0f - Time) * ScaleAz + Time * ScaleBz;
            
        }
        

        if (ShowRotation)
        {
            /*
            Quaternion lerpedRotation = Rotationweee(Time);


            Quaternion quaternionA = weeewooo(A, Matrix4x4.identity);
            
            Quaternion quaternionB = weeewooo(B, Matrix4x4.identity);

            Quaternion invertedQuaternionA = new Quaternion(quaternionA.x, quaternionA.y, quaternionA.z, quaternionA.w);

            Quaternion CQuaternion1 = invertedQuaternionA;
            
            Quaternion CQuaternion3 = quaternionB * invertedQuaternionA * quaternionA;

            lerpedRotation = quaternionA; //GetInterpolatedQuat(quaternionA, quaternionB);
            
            Debug.Log($"A:{quaternionA}" +
                      $"B:{quaternionB}");
            
            //Rotationen, fast fuskvärde
            /*
            for (int i = 0; i < 3; i++)
            {
                CRotate = Math.setColumnInMatrix(CRotate,
                    QuatMultVector3(lerpedRotation, Math.getColumnFromMatrix(CRotate, i)), i);
            }
            */


            CRotate = Math.CalculateRotationMatrix(A, B, Time);

            /*
            Vector3 temp;

            temp = QuatMultVector3(lerpedRotation, new Vector3(1, 0, 0));
            CRotate.m00 = temp.x;
            CRotate.m10 = temp.y;
            CRotate.m20 = temp.z;
            
            temp = QuatMultVector3(lerpedRotation, new Vector3(0, 1, 0));
            CRotate.m01 = temp.x;
            CRotate.m11 = temp.y;
            CRotate.m21 = temp.z;
            
            temp = QuatMultVector3(lerpedRotation, new Vector3(0, 0, 1));
            CRotate.m02 = temp.x;
            CRotate.m12 = temp.y;
            CRotate.m22 = temp.z;
            */
        }

        //Ordningen som är smart
        C =  CTranslate * CRotate * CScale;
        


        using (vectors.Begin()) {
            
            DrawCube(A);
            DrawCube(B);
            DrawCube(C);
            
            
            void DrawCube(Matrix4x4 mat)
            {
                vectors.Draw(CalculatePos(vector0, mat), CalculatePos(vectorA, mat), Color.red);
                vectors.Draw(CalculatePos(vectorC, mat), CalculatePos(vectorAC, mat), Color.red);
                vectors.Draw(CalculatePos(vectorB, mat), CalculatePos(vectorAB, mat), Color.red);
                vectors.Draw(CalculatePos(vectorBC, mat), CalculatePos(vectorABC, mat), Color.red);
                
                vectors.Draw(CalculatePos(vector0, mat), CalculatePos(vectorC, mat), Color.blue);
                vectors.Draw(CalculatePos(vectorA, mat), CalculatePos(vectorAC, mat), Color.blue);
                vectors.Draw(CalculatePos(vectorB, mat), CalculatePos(vectorBC, mat), Color.blue);
                vectors.Draw(CalculatePos(vectorAB, mat), CalculatePos(vectorABC, mat), Color.blue);
                
                vectors.Draw(CalculatePos(vector0, mat), CalculatePos(vectorB, mat), Color.green);
                vectors.Draw(CalculatePos(vectorA, mat), CalculatePos(vectorAB, mat), Color.green);
                vectors.Draw(CalculatePos(vectorC, mat), CalculatePos(vectorBC, mat), Color.green);
                vectors.Draw(CalculatePos(vectorAC, mat), CalculatePos(vectorABC, mat), Color.green);
            }
        }

        Vector4 CalculatePos(Vector4 vec, Matrix4x4 mat)
        {
            return mat*(vec);
        }

        Vector3 QuatMultVector3(Quaternion quat, Vector3 vec)
        {
            Quaternion temp = quat * new Quaternion(vec.x, vec.y, vec.z, 0) * Quaternion.Inverse(quat);
            return new Vector3(temp.x, temp.y, temp.z);
        }

        Quaternion weeewooo(Matrix4x4 matrix1, Matrix4x4 matrix2)
        {
            var planeNormal = Vector3.zero;
            for (int i = 0; i < 3; i++)
            {
                planeNormal += Math.CrossProduct(Math.Normalize(Math.getColumnFromMatrix(matrix1, i)),
                    Math.Normalize(Math.getColumnFromMatrix(matrix2, i)));
            }
            if (planeNormal == Vector3.zero)
                return Quaternion.identity;

            planeNormal = Math.Normalize(planeNormal);
            
            float xDiff = Math.DotProduct(planeNormal, Math.getColumnFromMatrix(matrix1, 0));
            float yDiff = Math.DotProduct(planeNormal, Math.getColumnFromMatrix(matrix1, 1));
            float zDiff = Math.DotProduct(planeNormal, Math.getColumnFromMatrix(matrix1, 2));
            
            Vector3 matrix2proj;
            Vector3 matrix1proj;
            if (xDiff <= yDiff && xDiff <= zDiff)
            {
                matrix2proj = Proj(0, matrix2);

                matrix1proj = Proj(0, matrix1);
            }
            else if (yDiff <= xDiff && yDiff <= zDiff)
            {
                matrix2proj = Proj(1, matrix2);

                matrix1proj = Proj(1, matrix1);
            }
            else if (zDiff <= xDiff && zDiff <= yDiff)
            {
                matrix2proj = Proj(2, matrix2);

                matrix1proj = Proj(2, matrix1);
            }
            else
            {
                matrix2proj = Vector3.one;
                matrix1proj = Vector3.zero;
                Debug.Log("Grått gråt");
            }
            
            
            
            //Hjälp
            float cosAngle = Math.DotProduct(Math.Normalize(matrix2proj), Math.Normalize(matrix1proj));
            
            while (cosAngle<-1)
            {
                cosAngle += 1;
            }

            while (cosAngle>1)
            {
                cosAngle -= 1;
            }
            
            float angle = Mathf.Acos(cosAngle)/2;
            
            return  new Quaternion(Mathf.Sin(angle ) * planeNormal.x,
                Mathf.Sin(angle) * planeNormal.y,Mathf.Sin(angle) * planeNormal.z, Mathf.Cos(angle));
            
            
            //Slut på hjälp
            
            
            
            Vector3 Proj(int i, Matrix4x4 matrix4X4)
            {
                return Math.getColumnFromMatrix(matrix4X4, i) -
                       Math.DotProduct(Math.getColumnFromMatrix(matrix4X4, i) , planeNormal) * planeNormal;
            }
        }






        Quaternion GetInterpolatedQuat(Quaternion a, Quaternion b)
        {
            var aInverse = new Quaternion(a.x, a.y, a.z, -a.w);
            Quaternion c = b * aInverse;
            var oldAngle = Mathf.Acos(c.w);
            var newAngle = oldAngle * Time;
            var rotationAxis = new Vector3(c.x / Mathf.Sin(oldAngle), c.y / Mathf.Sin(oldAngle),
                c.z / Mathf.Sin(oldAngle));
            var newRotationAxis = rotationAxis * newAngle;
            var interpolatedQuat = new Quaternion(newRotationAxis.x, newRotationAxis.y, newRotationAxis.z, newAngle);
            return interpolatedQuat * a;
        }
        
        
        
        
        Quaternion Rotationweee(float t)
        {
            var planeNormal = Vector3.zero;
            for (int i = 0; i < 3; i++)
            {
                planeNormal += Math.CrossProduct(Math.getColumnFromMatrix(A, i),
                    Math.getColumnFromMatrix(B, i));
            }

            if (planeNormal == Vector3.zero)
                return Quaternion.identity;

            planeNormal = Math.Normalize(planeNormal);

            float xDiff = Math.DotProduct(planeNormal, Math.getColumnFromMatrix(A, 0));
            float yDiff = Math.DotProduct(planeNormal, Math.getColumnFromMatrix(A, 1));
            float zDiff = Math.DotProduct(planeNormal, Math.getColumnFromMatrix(A, 2));
            Vector3 Aproj;
            Vector3 Bproj;
            if (xDiff <= yDiff && xDiff <= zDiff)
            {
                Aproj = Proj(0, A);

                Bproj = Proj(0, B);
            }
            else if (yDiff <= xDiff && yDiff <= zDiff)
            {
                Aproj = Proj(1, A);

                Bproj = Proj(1, B);
            }
            else if (zDiff <= xDiff && zDiff <= yDiff)
            {
                Aproj = Proj(2, A);

                Bproj = Proj(2, B);
            }
            else
            {
                Aproj = Vector3.one;
                Bproj = Vector3.zero;
                Debug.Log("Grått gråt");
            }

            float cosAngle = Math.DotProduct(Aproj, Bproj);
            
            while (cosAngle<-1)
            {
                cosAngle += 1;
            }

            while (cosAngle>1)
            {
                cosAngle -= 1;
            }
            
            float angle = Mathf.Acos(cosAngle)/2*t;
            
            Quaternion aaxe =  new Quaternion(Mathf.Sin(angle ) * planeNormal.x,
                Mathf.Sin(angle) * planeNormal.y,Mathf.Sin(angle) * planeNormal.z, Mathf.Cos(angle));

            Matrix4x4 q = Matrix4x4.identity;

            for (int i = 0; i < 3; i++)
            {
                q = Math.setColumnInMatrix(q, QuatMultVector3(aaxe, Math.getColumnFromMatrix(A, i)), i);
            }

            Debug.Log($"Q:{q}");
            return aaxe;
        
            Vector3 Proj(int i, Matrix4x4 matrix4X4)
            {
                return Math.getColumnFromMatrix(matrix4X4, i) -
                       Math.DotProduct(Math.getColumnFromMatrix(matrix4X4, i) , planeNormal) * planeNormal;
            }
        }
    }
    
    
    
    public Quaternion QuaternionFromMatrix(Matrix4x4 m){
        Quaternion q = new Quaternion();
        q.w = Mathf.Sqrt( Mathf.Max( 0, 1 + m[0,0] + m[1,1] + m[2,2] ) ) / 2; 
        q.x = Mathf.Sqrt( Mathf.Max( 0, 1 + m[0,0] - m[1,1] - m[2,2] ) ) / 2; 
        q.y = Mathf.Sqrt( Mathf.Max( 0, 1 - m[0,0] + m[1,1] - m[2,2] ) ) / 2; 
        q.z = Mathf.Sqrt( Mathf.Max( 0, 1 - m[0,0] - m[1,1] + m[2,2] ) ) / 2; 
        q.x *= Mathf.Sign( q.x * ( m[2,1] - m[1,2] ) );
        q.y *= Mathf.Sign( q.y * ( m[0,2] - m[2,0] ) );
        q.z *= Mathf.Sign( q.z * ( m[1,0] - m[0,1] ) );
        return q;
    }
    
}







[CustomEditor(typeof(Example))]
public class DemoEditor : Editor
{
    private void OnSceneGUI()
    {
        var demo = target as Example;
        if (!demo) return;

        EditorGUI.BeginChangeCheck();
        
        var aPos = Math.getColumnFromMatrix(demo.A, 3);
        var bPos = Math.getColumnFromMatrix(demo.B, 3);
        var newTargetPosA = aPos;
        var newTargetPosB = bPos;
        var newTargetRotA = demo.aRotation;
        var newTargetRotB = demo.bRotation;
        var newTargetScaleA = new Vector3(demo.A.m00,demo.A.m11,demo.A.m22);
        var newTargetScaleB = new Vector3(demo.B.m00, demo.B.m11, demo.B.m22);

        if (Tools.current == Tool.Move)
        {
            newTargetPosA = Handles.PositionHandle(aPos, demo.aRotation);
            newTargetPosB = Handles.PositionHandle(bPos, demo.bRotation);
        }
        else if (Tools.current == Tool.Rotate)
        {
            newTargetRotA = Handles.RotationHandle(demo.aRotation, aPos);
            newTargetRotB = Handles.RotationHandle(demo.bRotation, bPos);
        }
        else if (Tools.current == Tool.Scale)
        {
            newTargetScaleA = Handles.ScaleHandle(newTargetScaleA, aPos, demo.aRotation);
            newTargetScaleB = Handles.ScaleHandle(newTargetScaleB, bPos, demo.bRotation);
        }

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(demo, "Moved target");
            var copyA = demo.A;
            copyA.m03 = newTargetPosA.x;
            copyA.m13 = newTargetPosA.y;
            copyA.m23 = newTargetPosA.z;
            demo.A = copyA;
            
            var copyB = demo.B;
            copyB.m03 = newTargetPosB.x;
            copyB.m13 = newTargetPosB.y;
            copyB.m23 = newTargetPosB.z;
            demo.B = copyB;

            demo.aRotation = newTargetRotA;
            demo.bRotation = newTargetRotB;
            
            demo.B.m00 = newTargetScaleB.x;
            demo.B.m11 = newTargetScaleB.y;
            demo.B.m22 = newTargetScaleB.z;
            
            demo.A.m00 = newTargetScaleA.x;
            demo.A.m11 = newTargetScaleA.y;
            demo.A.m22 = newTargetScaleA.z;
            EditorUtility.SetDirty(demo);
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var demo = target as Example;
        if (!demo) return;
        
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Matrix A");
        EditorGUILayout.BeginVertical();

        var resultA = Matrix4x4.identity;
        for (var i = 0; i < 4; i++)
        {
            EditorGUILayout.BeginHorizontal();
            for (var j = 0; j < 4; j++)
            {
                resultA[i, j] = EditorGUILayout.FloatField(demo.A[i, j]);
            }
            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        
        //Radmellanrum
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Matrix B");
        EditorGUILayout.BeginVertical();

        var resultB = Matrix4x4.identity;
        for (var i = 0; i < 4; i++)
        {
            EditorGUILayout.BeginHorizontal();
            for (var j = 0; j < 4; j++)
            {
                resultB[i, j] = EditorGUILayout.FloatField(demo.B[i, j]);
            }
            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        
        //Radmellanrum
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Matrix C");
        EditorGUILayout.BeginVertical();

        var resultC = Matrix4x4.identity;
        for (var i = 0; i < 4; i++)
        {
            EditorGUILayout.BeginHorizontal();
            for (var j = 0; j < 4; j++)
            {
                resultC[i, j] = EditorGUILayout.FloatField(demo.C[i, j]);
            }
            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(demo, "Change matrix");
            demo.A = resultA;
            demo.B = resultB;

            EditorUtility.SetDirty(demo);
        }
    }
}
