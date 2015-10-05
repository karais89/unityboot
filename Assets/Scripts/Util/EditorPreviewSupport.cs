using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EditorPreviewSupport : MonoBehaviour {
#if UNITY_EDITOR
    public IEnumerator Run(IEnumerator iterationResult) {
        return EditorCoroutineRunner.StartEditorCoroutine(iterationResult);
    }
#else
    public Coroutine Run(IEnumerator iterationResult) {
        return StartCoroutine(iterationResult);
    }
#endif

#if UNITY_EDITOR
    public IEnumerator Wait(float seconds) {
        return new EditorWaitForSeconds(seconds);
    }
#else
    public WaitForSeconds Wait(float seconds) {
        return new WaitForSeconds(seconds);
    }
#endif

    // you should override below
    //-------------------------------------------------------------------------
    public virtual IEnumerator Preview() {
        yield break;
    }

    public virtual void Reset() {
    }
}