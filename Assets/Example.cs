using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Vectors;

[ExecuteAlways]
[RequireComponent(typeof(VectorRenderer))]
public class Example : MonoBehaviour {
    
    //Timestamp video Emil: 1:05:50
    
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

    [NonSerialized] public Vector4 vector0 = new Vector4(0,0,0,1);
    [NonSerialized] public Vector4 vectorA = new Vector4(1,0,0,1);
    [NonSerialized] public Vector4 vectorB = new Vector4(0,1,0,1);
    [NonSerialized] public Vector4 vectorC = new Vector4(0,0,1,1);
    [NonSerialized] public Vector4 vectorAB = new Vector4(1,1,0,1);
    [NonSerialized] public Vector4 vectorAC = new Vector4(1,0,1,1);
    [NonSerialized] public Vector4 vectorBC = new Vector4(0,1,1,1);
    [NonSerialized] public Vector4 vectorABC = new Vector4(1,1,1,1);

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
        if(ShowTranslation)
            CTranslate = Matrix4x4.identity;
            CTranslate = Math.setColumnInMatrix(Matrix4x4.identity, posInter, 3);
            
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
            CRotate = Matrix4x4.identity;

            Quaternion lerpedRotation = Rotationweee(Time);



            //Rotationen, fast fuskvärde
            /*for (int i = 0; i < 3; i++)
            {
                CRotate = Math.setColumnInMatrix(CRotate,
                    QuatMultVector3(lerpedRotation, Math.getColumnFromMatrix(CRotate, i)), i);
            }*/
            
            CRotate = Matrix4x4.identity;

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
        }

        //Ordningen som är smart
        C =  CTranslate * CRotate * CScale;


        using (vectors.Begin()) {
            vectors.Draw(CalculatePos(vector0), CalculatePos(vectorA), Color.red);
            vectors.Draw(CalculatePos(vectorC), CalculatePos(vectorAC), Color.red);
            vectors.Draw(CalculatePos(vectorB), CalculatePos(vectorAB), Color.red);
            vectors.Draw(CalculatePos(vectorBC), CalculatePos(vectorABC), Color.red);
            
            vectors.Draw(CalculatePos(vector0), CalculatePos(vectorC), Color.blue);
            vectors.Draw(CalculatePos(vectorA), CalculatePos(vectorAC), Color.blue);
            vectors.Draw(CalculatePos(vectorB), CalculatePos(vectorBC), Color.blue);
            vectors.Draw(CalculatePos(vectorAB), CalculatePos(vectorABC), Color.blue);
            
            vectors.Draw(CalculatePos(vector0), CalculatePos(vectorB), Color.green);
            vectors.Draw(CalculatePos(vectorA), CalculatePos(vectorAB), Color.green);
            vectors.Draw(CalculatePos(vectorC), CalculatePos(vectorBC), Color.green);
            vectors.Draw(CalculatePos(vectorAC), CalculatePos(vectorABC), Color.green);
        }

        Vector4 CalculatePos(Vector4 vec)
        {
            return C*(vec);
        }

        Vector3 QuatMultVector3(Quaternion quat, Vector3 vec)
        {
            Quaternion temp = quat * new Quaternion(vec.x, vec.y, vec.z, 0) * Quaternion.Inverse(quat);
            return new Vector3(temp.x, temp.y, temp.z);
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
            
            float angle = Mathf.Acos(cosAngle)/2;
            
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
                return Math.Normalize(Math.getColumnFromMatrix(matrix4X4, i) -
                    Math.DotProduct(Math.getColumnFromMatrix(matrix4X4, i) , planeNormal) * planeNormal);
            }
        }
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
