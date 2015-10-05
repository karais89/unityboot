using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class TweenExtensions {

    public static IEnumerator MoveTo(this MonoBehaviour v, EaseType easeType, float duration, Vector3 to) {
        Vector3 from = v.transform.localPosition;

        var ease = new EaseRunner(easeType, duration);
        while (ease.IsPlaying()) {
            v.transform.localPosition = Vector3.Lerp(from, to, ease.Run());
            yield return new WaitForEndOfFrame();
        }
    }

    public static IEnumerator ScaleTo(this MonoBehaviour v, EaseType easeType, float duration, Vector3 to) {
        Vector3 from = v.transform.localScale;

        var ease = new EaseRunner(easeType, duration);
        while (ease.IsPlaying()) {
            v.transform.localScale = Vector3.Lerp(from, to, ease.Run());
            yield return new WaitForEndOfFrame();
        }
    }

    public static IEnumerator RotationTo(this MonoBehaviour v, EaseType easeType, float duration, Vector3 to) {
        Vector3 from = v.transform.localEulerAngles;

        var ease = new EaseRunner(easeType, duration);
        while (ease.IsPlaying()) {
            v.transform.localEulerAngles = Vector3.Lerp(from, to, ease.Run());
            yield return new WaitForEndOfFrame();
        }
    }

    public static IEnumerator MoveTo(this GameObject v, EaseType easeType, float duration, Vector3 to) {
        Vector3 from = v.transform.localPosition;

        var ease = new EaseRunner(easeType, duration);
        while (ease.IsPlaying()) {
            v.transform.localPosition = Vector3.Lerp(from, to, ease.Run());
            yield return new WaitForEndOfFrame();
        }
    }

    public static IEnumerator ScaleTo(this GameObject v, EaseType easeType, float duration, Vector3 to) {
        Vector3 from = v.transform.localScale;

        var ease = new EaseRunner(easeType, duration);
        while (ease.IsPlaying()) {
            v.transform.localScale = Vector3.Lerp(from, to, ease.Run());
            yield return new WaitForEndOfFrame();
        }
    }

    public static IEnumerator RotationTo(this GameObject v, EaseType easeType, float duration, Vector3 to) {
        Vector3 from = v.transform.localEulerAngles;

        var ease = new EaseRunner(easeType, duration);
        while (ease.IsPlaying()) {
            v.transform.localEulerAngles = Vector3.Lerp(from, to, ease.Run());
            yield return new WaitForEndOfFrame();
        }
    }

    public static IEnumerator CameraRectTo(this Camera v, EaseType easeType, float duration, Rect to) {
        Rect from = v.rect;

        var ease = new EaseRunner(easeType, duration);
        while (ease.IsPlaying()) {
            Rect rect = new Rect();
            float process = ease.Run();
            rect.xMin = Mathf.Lerp(from.xMin, to.xMin, process);
            rect.yMin = Mathf.Lerp(from.yMin, to.yMin, process);
            rect.xMax = Mathf.Lerp(from.xMax, to.xMax, process);
            rect.yMax = Mathf.Lerp(from.yMax, to.yMax, process);
            v.rect = rect;
            yield return new WaitForEndOfFrame();
        }
    }

#if BOOT_NGUI_SUPPORT 
    public static IEnumerator AlphaTo(this UIPanel v, EaseType easeType, float duration, float to) {
        float from = v.alpha;

        var ease = new EaseRunner(easeType, duration);
        while (ease.IsPlaying()) {
            v.alpha = Mathf.Lerp(from, to, ease.Run());
#if UNITY_EDITOR
            NGUITools.SetDirty(v);
#endif
            yield return new WaitForEndOfFrame();
        }
    }

    public static IEnumerator ColorTo(this UIWidget v, EaseType easeType, float duration, Color to) {
        Color from = v.color;

        var ease = new EaseRunner(easeType, duration);
        while (ease.IsPlaying()) {
            v.color = Color.Lerp(from, to, ease.Run());
#if UNITY_EDITOR
            NGUITools.SetDirty(v);
#endif
            yield return new WaitForEndOfFrame();
        }
    }

    public static IEnumerator AlphaTo(this UIWidget v, EaseType easeType, float duration, float to) {
        Color from = v.color;
        Color toClolor = v.color;
        toClolor.a = to;

        var ease = new EaseRunner(easeType, duration);
        while (ease.IsPlaying()) {
            v.color = Color.Lerp(from, toClolor, ease.Run());
#if UNITY_EDITOR
            NGUITools.SetDirty(v);
#endif
            yield return new WaitForEndOfFrame();
        }
    }
#endif
}