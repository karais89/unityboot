using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(EditorPreviewSupport), true)]
[CanEditMultipleObjects]
public class EditorPreviewEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
         
        if(GUILayout.Button("Preview")) {
            Play();
        }

        if(GUILayout.Button("Reset")) {
            Reset();
        }
    }

    public void Play() {
        EditorPreviewSupport alert = target as EditorPreviewSupport;
        EditorCoroutineRunner.StartEditorCoroutine(alert.Preview());
    }

    public void Reset() {
        EditorPreviewSupport alert = target as EditorPreviewSupport;
        alert.Reset();
    }
}
