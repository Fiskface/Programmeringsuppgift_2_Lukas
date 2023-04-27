using System;
using UnityEditor;
using UnityEngine;
using Vectors;

[ExecuteAlways]
[RequireComponent(typeof(VectorRenderer))]
public class Example : MonoBehaviour {
    
    //Timestamp video Emil: 1:05:50
    
    [NonSerialized] 
    private VectorRenderer vectors;

    public Vector3 Target = Vector3.forward;
    [Range(0, 1)] public float Time = 0.0f;

    internal Matrix4x4 A;
    
    void OnEnable() {
        vectors = GetComponent<VectorRenderer>();
    }

    void Update()
    {
        var aPos = new Vector3(A.m03, A.m13, A.m23);
        var pos = (1.0f - Time) * aPos + Time * Target;
        using (vectors.Begin()) {
            vectors.Draw(pos, pos + transform.up, Color.green);
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
        var newTarget = Handles.PositionHandle(demo.Target, demo.transform.rotation);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(demo, "Moved target");
            demo.Target = newTarget;
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

        var result = Matrix4x4.identity;
        for (var i = 0; i < 4; i++)
        {
            EditorGUILayout.BeginHorizontal();
            for (var j = 0; j < 4; j++)
            {
                result[i, j] = EditorGUILayout.FloatField(demo.A[i, j]);
            }
            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(demo, "Change matrix");
            demo.A = result;
            EditorUtility.SetDirty(demo);
        }
    }
}
