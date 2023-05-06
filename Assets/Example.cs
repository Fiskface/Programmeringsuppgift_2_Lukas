using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Vectors;

[ExecuteAlways]
[RequireComponent(typeof(VectorRenderer))]
public class Example : MonoBehaviour
{

    [NonSerialized] private VectorRenderer vectors;

    public bool ShowTranslation = true;
    public bool ShowScale = true;
    public bool ShowRotation = true;
    public bool InputWithHandles = false;

    [Range(0, 1)] public float Time = 0.0f;

    [SerializeField, HideInInspector] internal Matrix4x4 A;
    [SerializeField, HideInInspector] internal Matrix4x4 B;
    [SerializeField, HideInInspector] internal Matrix4x4 C = Matrix4x4.identity;

    [NonSerialized] public Vector4 vector0 = new Vector4(-0.5f, -0.5f, -0.5f, 1);
    [NonSerialized] public Vector4 vectorA = new Vector4(0.5f, -0.5f, -0.5f, 1);
    [NonSerialized] public Vector4 vectorB = new Vector4(-0.5f, 0.5f, -0.5f, 1);
    [NonSerialized] public Vector4 vectorC = new Vector4(-0.5f, -0.5f, 0.5f, 1);
    [NonSerialized] public Vector4 vectorAB = new Vector4(0.5f, 0.5f, -0.5f, 1);
    [NonSerialized] public Vector4 vectorAC = new Vector4(0.5f, -0.5f, 0.5f, 1);
    [NonSerialized] public Vector4 vectorBC = new Vector4(-0.5f, 0.5f, 0.5f, 1);
    [NonSerialized] public Vector4 vectorABC = new Vector4(0.5f, 0.5f, 0.5f, 1);

    public Quaternion aRotation = Quaternion.identity;
    public Quaternion bRotation = Quaternion.identity;

    private Matrix4x4 CTranslate = Matrix4x4.identity;
    private Matrix4x4 CScale = Matrix4x4.identity;
    private Matrix4x4 CRotate = Matrix4x4.identity;

    void OnEnable()
    {
        vectors = GetComponent<VectorRenderer>();
    }

    void Update()
    {
        if (ShowTranslation)
        {
            Vector4 posInter = (1.0f - Time) * Math.getColumnFromMatrix(A, 3) + Time * Math.getColumnFromMatrix(B, 3);
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

            //Scalematrisen
            CScale.m00 = (1.0f - Time) * ScaleAx + Time * ScaleBx;
            CScale.m11 = (1.0f - Time) * ScaleAy + Time * ScaleBy;
            CScale.m22 = (1.0f - Time) * ScaleAz + Time * ScaleBz;
        }
        
        if (ShowRotation)
        {
            CRotate = Math.CalculateRotationMatrix(A, B, Time);
        }
        
        C = CTranslate * CRotate * CScale;

        
        using (vectors.Begin())
        {

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
    }
    
    Vector4 CalculatePos(Vector4 vec, Matrix4x4 mat)
    {
        return mat * (vec);
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
