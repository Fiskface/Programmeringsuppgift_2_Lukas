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

    public bool ShowPosition = true;
    public bool ShowScale = true;
    public bool ShowRotation = true;

    [Range(0, 1)] public float Time = 0.0f;

    [SerializeField, HideInInspector] internal Matrix4x4 A;
    [SerializeField, HideInInspector] internal Matrix4x4 B;
    [SerializeField, HideInInspector] internal Matrix4x4 C;

    [NonSerialized] public Vector4 vector0 = new Vector4(0,0,0,1);
    [NonSerialized] public Vector4 vectorA = new Vector4(1,0,0,1);
    [NonSerialized] public Vector4 vectorB = new Vector4(0,1,0,1);
    [NonSerialized] public Vector4 vectorC = new Vector4(0,0,1,1);
    [NonSerialized] public Vector4 vectorAB = new Vector4(1,1,0,1);
    [NonSerialized] public Vector4 vectorAC = new Vector4(1,0,1,1);
    [NonSerialized] public Vector4 vectorBC = new Vector4(0,1,1,1);
    [NonSerialized] public Vector4 vectorABC = new Vector4(1,1,1,1);
    
    
    
    void OnEnable() {
        vectors = GetComponent<VectorRenderer>();
    }

    void Update()
    {
        Vector4 posInter = (1.0f - Time) * Math.getPositionFromMatrix(A) + Time * Math.getPositionFromMatrix(B);
        
        if(ShowPosition)
            C = Math.setPositionInMatrix(C, posInter);
        

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
            return (vec + Math.getPositionFromMatrix(C));
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
        
        var aPos = new Vector3(demo.A.m03, demo.A.m13, demo.A.m23);
        var newTarget = Handles.PositionHandle(aPos, demo.transform.rotation);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(demo, "Moved target");
            var copy = demo.A;
            copy.m03 = newTarget.x;
            copy.m13 = newTarget.y;
            copy.m23 = newTarget.z;
            demo.A = copy;
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
