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
    public Vector3 aScale = Vector3.zero;
    public Vector3 bScale = Vector3.zero;
    
    void OnEnable() {
        vectors = GetComponent<VectorRenderer>();
    }

    void Update()
    {
        Vector4 posInter = (1.0f - Time) * Math.getColumnFromMatrix(A, 3) + Time * Math.getColumnFromMatrix(B, 3);
        if(ShowTranslation)
            C = Math.setColumnInMatrix(C, posInter, 3);

        if (ShowScale)
        {
            /*aScale = new Vector3(Math.GetMagnitude(Math.getColumnFromMatrix(A, 0)),
                Math.GetMagnitude(Math.getColumnFromMatrix(A, 1)),
                Math.GetMagnitude(Math.getColumnFromMatrix(A, 2)));
            */
            float ScaleAx = Math.GetMagnitude(Math.getColumnFromMatrix(A, 0));
            float ScaleAy = Math.GetMagnitude(Math.getColumnFromMatrix(A, 1));
            float ScaleAz = Math.GetMagnitude(Math.getColumnFromMatrix(A, 2));

            float ScaleBx = Math.GetMagnitude(Math.getColumnFromMatrix(B, 0));
            float ScaleBy = Math.GetMagnitude(Math.getColumnFromMatrix(B, 1));
            float ScaleBz = Math.GetMagnitude(Math.getColumnFromMatrix(B, 2));

            C.m00 = (1.0f - Time) * ScaleAx + Time * ScaleBx;
            C.m11 = (1.0f - Time) * ScaleAy + Time * ScaleBy;
            C.m22 = (1.0f - Time) * ScaleAz + Time * ScaleBz;
        }

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
        var newTargetScaleB = demo.bScale;

        if (Tools.current == Tool.Move)
        {
            newTargetPosA = Handles.PositionHandle(aPos, demo.aRotation);
            newTargetPosB = Handles.PositionHandle(bPos, demo.aRotation);
        }
        else if (Tools.current == Tool.Rotate)
        {
            newTargetRotA = Handles.RotationHandle(demo.aRotation, aPos);
            newTargetRotB = Handles.RotationHandle(demo.aRotation, bPos);
        }
        else if (Tools.current == Tool.Scale)
        {
            newTargetScaleA = Handles.ScaleHandle(newTargetScaleA, aPos, demo.aRotation);
            newTargetScaleB = Handles.ScaleHandle(demo.aScale, bPos, demo.aRotation);
        }

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(demo, "Moved target");
            var copy = demo.A;
            copy.m03 = newTargetPosA.x;
            copy.m13 = newTargetPosA.y;
            copy.m23 = newTargetPosA.z;
            demo.A = copy;

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
